using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PieceManager : MonoBehaviour
{

	public GameObject piecePrefab;
	public GameObject cellHitBox;
	public GameObject promotionSelection;
	public GameManager gameManager;
	public AI ai;
	[HideInInspector]
	public bool isGameContinue = true;
	[HideInInspector]
	public List<BasePiece> whitePieces = null;
	[HideInInspector]
	public List<BasePiece> blackPieces = null;
	private List<BasePiece> whitePawns = new List<BasePiece>();
	private List<BasePiece> blackPawns = new List<BasePiece>();
	[HideInInspector]
	public List<BasePiece> promotedPieces = new List<BasePiece>();
	[HideInInspector]
	public List<BasePiece> angelBuffBlackPieces = new List<BasePiece>();
	[HideInInspector]
	public List<BasePiece> angelBuffWhitePieces = new List<BasePiece>();
	[HideInInspector]
	public List<Cell> obstacleCells = new List<Cell>();
	[HideInInspector]
	public List<Cell> buffCells = new List<Cell>();
	private List<GameObject> allCellHitBox = new List<GameObject>();
	private int goblinWhiteSideSkillCooldown = 3;
	private int goblinBlackSideSkillCooldown = 3;
	private int giantWhiteSideSkillCooldown = 5;
	private int giantBlackSideSkillCooldown = 5;
	private int orcWhiteSideSkillCooldown = 0;
	private int orcBlackSideSkillCooldown = 0;
	private BasePiece blackKing;
	private BasePiece whiteKing;
	[HideInInspector]
	public List<Cell> legalCells = new List<Cell>();
	[HideInInspector]
	public List<CaptureCouple> captureCouples = new List<CaptureCouple>();
	private string[] pieceOrder = new string[16]
	{
		"P", "P", "P", "P", "P", "P", "P", "P",
		"R", "KN", "B", "Q", "K", "B", "KN", "R"
	};
	private string obstacleSelected;
	public GameObject obstacleSelection;
	private Dictionary<string, Type> pieceLib = new Dictionary<string, Type>()
	{
		{"P", typeof(Pawn)},
		{"R", typeof(Rook)},
		{"KN", typeof(Knight)},
		{"B", typeof(Bishop)},
		{"K", typeof(King)},
		{"Q", typeof(Queen)}
	};
	private Board board;
	public void Setup(Board board, Color playerSide, Color enemySide)
	{
		//Create player pieces
		whitePieces = CreatePieces(Color.white, playerSide);

		//Create enemy pieces
		blackPieces = CreatePieces(Color.black, enemySide);

		//Place pieces
		PlacePieces(1, 0, whitePieces, board);
		PlacePieces(6, 7, blackPieces, board);
		this.board = board;
		SelectObstacle();
		AddCellHitBox();
	}


	#region Pieces
	public void DisableBtn(Color side)
	{
		if (side == Color.white)
		{
			foreach (BasePiece piece in whitePieces)
			{
				piece.DisableAllBtn();
			}
			return;
		}

		if (side == Color.black)
		{
			foreach (BasePiece piece in blackPieces)
			{
				piece.DisableAllBtn();
			}
			return;
		}
	}

	public void SwitchSide(Color color)
	{
		if (!isGameContinue)
		{
			ResetPieces();

			isGameContinue = true;

			color = Color.black;
		}
		if(color != Color.red)
			FindLegalMove(color == Color.white ? Color.black : Color.white);
		if (color == Color.red)
		{
			gameManager.activeSkillTimer.transform.GetChild(2).gameObject.SetActive(false);
			gameManager.activeSkillTimer.transform.GetChild(3).gameObject.SetActive(false);
			SetInteractive(whitePieces, false);
			SetInteractive(blackPieces, false);
			return;
		}
		else /*if (color == Color.black)*/
		{		
			SwitchSideHelper(color);
		}
		//else
		//{
		//	ai.AIMove();
		//}
	}

	private void FindLegalMove(Color color) 
	{
		captureCouples.Clear();
		BasePiece king = color == Color.white ? whiteKing : blackKing;
		FindLegalMoveHelper(king);
	}
	private void FindLegalMoveHelper(BasePiece king) 
	{
		KnightCheck(king.curCell, king.color);
		PawnsCheck(king.curCell, king.color);
		//up
		UpLegalMove(king.curCell, king.color);
		//down
		DownLegalMove(king.curCell, king.color);
		//left
		LeftLegalMove(king.curCell, king.color);
		//right
		RightLegalMove(king.curCell, king.color);
		//upperleft
		UpperLeftLegalMove(king.curCell, king.color);
		//upperright
		UpperRightLegalMove(king.curCell, king.color);
		//lowerleft
		LowerLeftLegalMove(king.curCell, king.color);
		//lowerright
		LowerRightLegalMove(king.curCell, king.color);
	}
	private void PawnsCheck(Cell cell, Color color) 
	{
		int y = cell.boardPos.y;
		int x = cell.boardPos.x;
		_ = color == Color.white ?  y += 1 : y -= 1;
		if (x - 1 >= 0) 
		{
			if (y >= 0 && y <= 7 && cell.board.AllCells[x - 1, y].curPiece != null && cell.board.AllCells[x - 1, y].curPiece.color != color && cell.board.AllCells[x - 1, y].curPiece is Pawn) 
			{
				legalCells.Add(cell.board.AllCells[x - 1, y]);
			}
		}
		if (x + 1 <= 7)
		{
			if (y >= 0 && y <= 7 && cell.board.AllCells[x + 1, y].curPiece != null && cell.board.AllCells[x + 1, y].curPiece.color != color && cell.board.AllCells[x + 1, y].curPiece is Pawn)
			{
				legalCells.Add(cell.board.AllCells[x + 1, y]);
			}
		}
	}
	private void LowerLeftLegalMove(Cell cell, Color color)
	{
		BasePiece defPiece = null;
		List<Cell> attackDir = new List<Cell>();
		int y = cell.boardPos.y;
		int x = cell.boardPos.x;
		for (int i = 1; i >= y; i++)
		{
			if (x - i >= 0 && y - i >= 0)
			{
				Cell tmp = cell.board.AllCells[x - i, y - i];
				attackDir.Add(tmp);
				if (tmp.curPiece != null)
				{
					if (tmp.curPiece.color == color)
					{
						if (defPiece == null)
							defPiece = tmp.curPiece;
						else
						{
							break;
						}
					}
					else
					{
						if (tmp.curPiece is Bishop || tmp.curPiece is Queen)
						{
							if (defPiece == null)
							{
								legalCells = legalCells.Concat(attackDir).ToList();
								break;
							}
							else
							{
								captureCouples.Add(new CaptureCouple(tmp.curPiece, defPiece, attackDir));
								break;
							}
						}
					}
				}
			}
			else 
			{
				break;
			}
		}
	}
	private void LowerRightLegalMove(Cell cell, Color color)
	{
		BasePiece defPiece = null;
		List<Cell> attackDir = new List<Cell>();
		int y = cell.boardPos.y;
		int x = cell.boardPos.x;
		for (int i = 1; i >= y; i++)
		{
			if (x + i <= 7 && y - i >= 0)
			{
				Cell tmp = cell.board.AllCells[x + i, y - i];
				attackDir.Add(tmp);
				if (tmp.curPiece != null)
				{
					if (tmp.curPiece.color == color)
					{
						if (defPiece == null)
							defPiece = tmp.curPiece;
						else
						{
							break;
						}
					}
					else
					{
						if (tmp.curPiece is Bishop || tmp.curPiece is Queen)
						{
							if (defPiece == null)
							{
								legalCells = legalCells.Concat(attackDir).ToList();
								break;
							}
							else
							{
								captureCouples.Add(new CaptureCouple(tmp.curPiece, defPiece, attackDir));
								break;
							}
						}
					}
				}
			}
			else
			{
				break;
			}
		}
	}
	private void UpperRightLegalMove(Cell cell, Color color)
	{
		BasePiece defPiece = null;
		List<Cell> attackDir = new List<Cell>();
		int y = cell.boardPos.y;
		int x = cell.boardPos.x;
		for (int i = 1; i <= 7 - y; i++)
		{
			if (x + i <= 7)
			{
				Cell tmp = cell.board.AllCells[x + i, y + i];
				attackDir.Add(tmp);
				if (tmp.curPiece != null)
				{
					if (tmp.curPiece.color == color)
					{
						if (defPiece == null)
							defPiece = tmp.curPiece;
						else
						{
							break;
						}
					}
					else
					{
						if (tmp.curPiece is Bishop || tmp.curPiece is Queen)
						{
							if (defPiece == null)
							{
								legalCells = legalCells.Concat(attackDir).ToList();
								
								break;
							}
							else
							{
								captureCouples.Add(new CaptureCouple(tmp.curPiece, defPiece, attackDir));
								
								break;
							}
						}
					}
				}
			}
		}
	}
	private void UpperLeftLegalMove(Cell cell, Color color)
	{
		BasePiece defPiece = null;
		List<Cell> attackDir = new List<Cell>();
		int y = cell.boardPos.y;
		int x = cell.boardPos.x;
		for (int i = 1; i <= 7 - y; i++) 
		{
			if (x - i >= 0) 
			{
				Cell tmp = cell.board.AllCells[x - i, y + i];
				attackDir.Add(tmp);
				if (tmp.curPiece != null)
				{
					if (tmp.curPiece.color == color)
					{
						if (defPiece == null)
							defPiece = tmp.curPiece;
						else
						{
							break;
						}
					}
					else
					{
						if (tmp.curPiece is Bishop || tmp.curPiece is Queen)
						{
							if (defPiece == null)
							{
								legalCells = legalCells.Concat(attackDir).ToList();
								break;
							}
							else
							{
								captureCouples.Add(new CaptureCouple(tmp.curPiece, defPiece, attackDir));
								break;
							}
						}
					}
				}
			}
		}
	}
	private void RightLegalMove(Cell cell, Color color)
	{
		BasePiece defPiece = null;
		List<Cell> attackDir = new List<Cell>();
		int y = cell.boardPos.y;
		int x = cell.boardPos.x + 1;
		for (int i = x; i <= 7; i++)
		{
			Cell tmp = cell.board.AllCells[i, y];
			attackDir.Add(tmp);
			if (tmp.curPiece != null)
			{
				if (tmp.curPiece.color == color)
				{
					if (defPiece == null)
						defPiece = tmp.curPiece;
					else
					{
						
						break;
					}
				}
				else
				{
					if (tmp.curPiece is Rook || tmp.curPiece is Queen)
					{
						if (defPiece == null)
						{
							legalCells = legalCells.Concat(attackDir).ToList();
							
							break;
						}
						else
						{
							captureCouples.Add(new CaptureCouple(tmp.curPiece, defPiece, attackDir));
							
							break;
						}
					}
				}
			}
		}
	}
	private void LeftLegalMove(Cell cell, Color color)
	{
		BasePiece defPiece = null;
		List<Cell> attackDir = new List<Cell>();
		int y = cell.boardPos.y;
		int x = cell.boardPos.x - 1;
		for (int i = x; i >= 0; i--)
		{
			Cell tmp = cell.board.AllCells[i, y];
			attackDir.Add(tmp);
			if (tmp.curPiece != null)
			{
				if (tmp.curPiece.color == color)
				{
					if (defPiece == null)
						defPiece = tmp.curPiece;
					else
					{
						
						break;
					}
				}
				else
				{
					if (tmp.curPiece is Rook || tmp.curPiece is Queen)
					{
						if (defPiece == null)
						{
							legalCells = legalCells.Concat(attackDir).ToList();
							
							break;
						}
						else
						{
							captureCouples.Add(new CaptureCouple(tmp.curPiece, defPiece, attackDir));
							
							break;
						}
					}
				}
			}
		}
	}
	private void DownLegalMove(Cell cell, Color color)
	{
		BasePiece defPiece = null;
		List<Cell> attackDir = new List<Cell>();
		int y = cell.boardPos.y - 1;
		int x = cell.boardPos.x;
		for (int i = y; i >= 0; i--)
		{
			Cell tmp = cell.board.AllCells[x, i];
			attackDir.Add(tmp);
			if (tmp.curPiece != null)
			{
				if (tmp.curPiece.color == color)
				{
					if (defPiece == null)
						defPiece = tmp.curPiece;
					else 
					{
						
						break;
					}		
				}
				else
				{
					if (tmp.curPiece is Rook || tmp.curPiece is Queen)
					{
						if (defPiece == null)
						{
							legalCells = legalCells.Concat(attackDir).ToList();
							
							break;
						}
						else
						{
							captureCouples.Add(new CaptureCouple(tmp.curPiece, defPiece, attackDir));
							
							break;
						}
					}
				}
			}
		}
	}
	private void UpLegalMove(Cell cell, Color color) 
	{
		BasePiece defPiece = null;
		List<Cell> attackDir = new List<Cell>();
		int y = cell.boardPos.y + 1;
		int x = cell.boardPos.x;
		for (int i = y; i <= 7; i++) 
		{
			Cell tmp = cell.board.AllCells[x, i];
			attackDir.Add(tmp);
			if (tmp.curPiece != null) 
			{
				if (tmp.curPiece.color == color)
				{
					if (defPiece == null)
						defPiece = tmp.curPiece;
					else
					{
						
						break;
					}
				}
				else 
				{
					if (tmp.curPiece is Rook || tmp.curPiece is Queen) 
					{
						if (defPiece == null)
						{
							legalCells = legalCells.Concat(attackDir).ToList();
							
							break;
						}
						else 
						{
							captureCouples.Add(new CaptureCouple(tmp.curPiece, defPiece, attackDir));
							
							break;
						}
					}
				}
			}
		}
	}
	private void KnightCheck(Cell cell, Color color) 
	{
		Color opoColor = color == Color.white ? Color.black : Color.white;
		int x = cell.boardPos.x;
		int y = cell.boardPos.y;
		if (y + 1 <= 7) 
		{
			if (x + 2 <= 7 && cell.board.AllCells[x + 2, y + 1].curPiece != null && cell.board.AllCells[x + 2, y + 1].curPiece is Knight && cell.board.AllCells[x + 2, y + 1].curPiece.color == opoColor) 
			{
				legalCells.Add(cell.board.AllCells[x + 2, y + 1]);
			}
			if (x - 2 >= 0 && cell.board.AllCells[x - 2, y + 1].curPiece != null && cell.board.AllCells[x - 2, y + 1].curPiece is Knight && cell.board.AllCells[x - 2, y + 1].curPiece.color == opoColor)
			{
				legalCells.Add(cell.board.AllCells[x - 2, y + 1]);
			}
		}
		if (y + 2 <= 7)
		{
			if (x + 1 <= 7 && cell.board.AllCells[x + 1, y + 2].curPiece != null && cell.board.AllCells[x + 1, y + 2].curPiece is Knight && cell.board.AllCells[x + 1, y + 2].curPiece.color == opoColor)
			{
				legalCells.Add(cell.board.AllCells[x + 1, y + 2]);
			}
			if (x - 1 >= 0 && cell.board.AllCells[x - 1, y + 2].curPiece != null && cell.board.AllCells[x - 1, y + 2].curPiece is Knight && cell.board.AllCells[x - 1, y + 2].curPiece.color == opoColor)
			{
				legalCells.Add(cell.board.AllCells[x - 1, y + 2]);
			}
		}
		if (y - 1 >= 0)
		{
			if (x + 2 <= 7 && cell.board.AllCells[x + 2, y - 1].curPiece != null && cell.board.AllCells[x + 2, y - 1].curPiece is Knight && cell.board.AllCells[x + 2, y - 1].curPiece.color == opoColor)
			{
				legalCells.Add(cell.board.AllCells[x + 2, y - 1]);
			}
			if (x - 2 >= 0 && cell.board.AllCells[x - 2, y - 1].curPiece != null && cell.board.AllCells[x - 2, y - 1].curPiece is Knight && cell.board.AllCells[x - 2, y - 1].curPiece.color == opoColor)
			{
				legalCells.Add(cell.board.AllCells[x - 2, y - 1]);
			}
		}
		if (y - 2 >= 0)
		{
			if (x + 1 <= 7 && cell.board.AllCells[x + 1, y - 2].curPiece != null && cell.board.AllCells[x + 1, y - 2].curPiece is Knight && cell.board.AllCells[x + 1, y - 2].curPiece.color == opoColor)
			{
				legalCells.Add(cell.board.AllCells[x + 1, y - 2]);
			}
			if (x - 1 >= 0 && cell.board.AllCells[x - 1, y - 2].curPiece != null && cell.board.AllCells[x - 1, y - 2].curPiece is Knight && cell.board.AllCells[x - 1, y - 2].curPiece.color == opoColor)
			{
				legalCells.Add(cell.board.AllCells[x - 1, y - 2]);
			}
		}
	}
	private void SwitchSideHelper(Color color)
	{
		bool isBlackTurn = color == Color.white;
		gameManager.activeSkillTimer.transform.GetChild(isBlackTurn ? 3 : 2).gameObject.SetActive(true);
		gameManager.activeSkillTimer.transform.GetChild(isBlackTurn ? 2 : 3).gameObject.SetActive(false);
		SetInteractive(whitePieces, !isBlackTurn);
		SetInteractive(blackPieces, isBlackTurn);
		SkillCooldown(color);	
		foreach (BasePiece piece in promotedPieces)
		{
			piece.enabled = piece.color == Color.black ? isBlackTurn : !isBlackTurn;
		}
		CheckObstacleOnTurn(color == Color.white ? Color.black : Color.white);
		DisableBtn(color == Color.white ? Color.black : Color.white);
	}

	public void ResetPieces()
	{
		foreach (BasePiece piece in promotedPieces)
		{
			piece.Kill();

			Destroy(piece.gameObject);
		}
		promotedPieces.Clear();

		foreach (BasePiece piece in whitePieces)
			piece.Reset();

		foreach (BasePiece piece in blackPieces)
			piece.Reset();

		foreach (GameObject hitbox in allCellHitBox)
			Destroy(hitbox);
		foreach (Cell cell in buffCells)
		{
			cell.transform.GetChild(1).gameObject.SetActive(false);
		}
		buffCells.Clear();
		whitePawns.Clear();
		blackPawns.Clear();
		angelBuffBlackPieces.Clear();
		allCellHitBox.Clear();
		board.ResetCells();
		//AddBackMountains();
	}

	public void SetInteractive(List<BasePiece> allPieces, bool value)
	{
		foreach (BasePiece piece in allPieces)
		{
			piece.enabled = value;
		}
	}

	private void PlacePieces(int pawnRow, int royaltyRow, List<BasePiece> pieces, Board board)
	{
		for (int x = 0; x < 8; x++)
		{
			//add pawns
			pieces[x].Place(board.AllCells[x, pawnRow]);

			//add royalty
			pieces[x + 8].Place(board.AllCells[x, royaltyRow]);
		}
	}

	private string GetRace(Color32 race)
	{
		if (race.Equals(new Color32(104, 101, 118, 255)))
			return "Human";
		if (race.Equals(new Color32(138, 200, 116, 255)))
			return "Elf";
		if (race.Equals(new Color32(103, 88, 49, 255)))
			return "Orc";
		if (race.Equals(new Color32(232, 182, 80, 255)))
			return "Goblin";
		if (race.Equals(new Color32(115, 63, 23, 255)))
			return "Dwarf";
		if (race.Equals(new Color32(178, 102, 68, 255)))
			return "Giant";
		if (race.Equals(new Color32(4, 239, 249, 255)))
			return "Spirit";
		if (race.Equals(new Color32(107, 132, 138, 255)))
			return "Golem";
		if (race.Equals(new Color32(255, 255, 255, 255)))
			return "Angel";
		if (race.Equals(new Color32(231, 26, 35, 255)))
			return "Devil";
		return null;
	}

	private List<BasePiece> CreatePieces(Color teamColor, Color32 spriteColor)
	{
		List<BasePiece> newPieces = new List<BasePiece>();
		string race = GetRace(spriteColor);
		for (int i = 0; i < pieceOrder.Length; i++)
		{
			//Set type
			string key = pieceOrder[i];
			Type pieceType = pieceLib[key];
			BasePiece piece = CreatePiece(pieceType);
			piece.gameObject.name = i.ToString() + " " + (teamColor == Color.white ? "white" : "black");
			newPieces.Add(piece);
			piece.Setup(teamColor, spriteColor, this, race);
			if (piece is King) 
			{
				_ = piece.color == Color.white ? whiteKing = piece : blackKing = piece;
				continue;
			}
			if (!(piece is Pawn))
				continue;
			if (teamColor == Color.white)
			{
				whitePawns.Add(piece);
			}
			else
			{
				blackPawns.Add(piece);
			}
		}

		return newPieces;
	}

	private BasePiece CreatePiece(Type pieceType)
	{
		//Create piece
		GameObject newPiece = Instantiate(piecePrefab);
		newPiece.transform.SetParent(transform);
		newPiece.transform.localScale = new Vector3(0.7f, 0.7f, 1);
		newPiece.transform.localRotation = Quaternion.identity;

		BasePiece piece = (BasePiece)newPiece.AddComponent(pieceType);
		return piece;
	}
	#endregion
	#region Obstacle
	public void CheckObstacleAtStart()
	{
		obstacleSelection.SetActive(false);
		foreach (BasePiece piece in whitePieces)
		{
			if (!piece.race.Equals("Elf"))
				break;
			piece.ElfSkill();
		}

		foreach (BasePiece piece in blackPieces)
		{
			if (!piece.race.Equals("Elf"))
				break;
			piece.ElfSkill();
		}
	}

	private void CreateObstacle(Cell cell, GameObject btn)
	{
		GameObject obstacle;
		switch (obstacleSelected) 
		{
			case "Mountain":
				obstacle = cell.transform.GetChild(2).gameObject;
				break;
			case "Poison":
				obstacle = cell.transform.GetChild(3).gameObject;
				break;
			case "Fog":
				obstacle = cell.transform.GetChild(4).gameObject;
				break;
			case "Teleport":
				obstacle = cell.transform.GetChild(5).gameObject;
				break;
			case "Bomb":
				obstacle = cell.transform.GetChild(6).gameObject;
				break; 
			case "Root":
				obstacle = cell.transform.GetChild(7).gameObject;
				break;
			case "Slow":
				obstacle = cell.transform.GetChild(8).gameObject;
				break;
			default:
				obstacle = cell.transform.GetChild(2).gameObject;
				break;
		}
		if (!obstacle.activeSelf)
		{
			obstacle.SetActive(true);
			btn.GetComponentInChildren<Text>().text = "Remove";
			obstacleCells.Add(cell);
		}
		else
		{
			obstacle.SetActive(false);
			btn.GetComponentInChildren<Text>().text = "Add";
			obstacleCells.Remove(cell);
		}
	}
	//need fix
	private void AddBackMountains()
	{
		foreach (Cell cell in obstacleCells)
		{
			if (!cell.transform.GetChild(2).gameObject.activeSelf)
				cell.transform.GetChild(2).gameObject.SetActive(true);
		}
	}
	private void DestroyMountainBtn(GameObject hitbox)
	{
		if (gameManager.activeSkillTimer.transform.GetChild(2).gameObject.activeSelf && CheckOrcSkill(0, hitbox))
		{
			hitbox.GetComponent<HideButton>().notInPiece = false;
			hitbox.transform.GetChild(1).gameObject.SetActive(true);
			hitbox.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => UseOrcSkill(hitbox, 0));
			return;
		}

		if (gameManager.activeSkillTimer.transform.GetChild(3).gameObject.activeSelf && CheckOrcSkill(1, hitbox))
		{
			hitbox.GetComponent<HideButton>().notInPiece = false;
			hitbox.transform.GetChild(1).gameObject.SetActive(true);
			hitbox.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => UseOrcSkill(hitbox, 1));
			return;
		}
	}

	public void CreateCellHitBox(int posX, int posY, Color color, List<GameObject> cellList, Color team)
	{
		Cell cell = board.AllCells[posX, posY];
		GameObject hitbox = Instantiate(cellHitBox);
		hitbox.transform.SetParent(cell.transform);
		hitbox.transform.localRotation = Quaternion.identity;
		hitbox.transform.position = cell.transform.position;
		hitbox.transform.localScale = new Vector3(0.9f, 0.9f, 1);
		hitbox.transform.GetChild(0).gameObject.SetActive(false);
		hitbox.GetComponent<Image>().color = color;
		cell.wall = hitbox;
		cell.color = team;
		if (cellList != null)
			cellList.Add(hitbox);
	}
	public void RemoveCellHitBox()
	{
		foreach (GameObject hitbox in allCellHitBox)
		{
			Destroy(hitbox);
		}
		foreach (Cell cell in board.AllCells)
		{
			if (cell.transform.GetChild(2).gameObject.activeSelf)
			{
				GameObject hitbox = Instantiate(cellHitBox);
				hitbox.transform.SetParent(transform);
				hitbox.transform.localRotation = Quaternion.identity;
				hitbox.transform.position = cell.transform.position;
				hitbox.transform.localScale = new Vector3(1, 1, 1);
				hitbox.transform.GetChild(0).gameObject.SetActive(false);
				hitbox.GetComponent<HideButton>().mountainCell = cell;
				hitbox.GetComponent<Button>().onClick.AddListener(() => DestroyMountainBtn(hitbox));
				allCellHitBox.Add(hitbox);
			}
		}
	}
	public void ResetObstacleCells() 
	{
		foreach(Cell cell in obstacleCells)
		{
			cell.transform.GetChild(2).gameObject.SetActive(false);
			cell.transform.GetChild(3).gameObject.SetActive(false);
			cell.transform.GetChild(4).gameObject.SetActive(false);
			cell.transform.GetChild(5).gameObject.SetActive(false);
			cell.transform.GetChild(6).gameObject.SetActive(false);
			cell.transform.GetChild(7).gameObject.SetActive(false);
			cell.transform.GetChild(8).gameObject.SetActive(false);
		}
	}
	private void AddCellHitBox()
	{
		foreach (Cell cell in board.AllCells)
		{
			if (cell.curPiece == null)
			{
				GameObject hitbox = Instantiate(cellHitBox);
				hitbox.transform.SetParent(transform);
				hitbox.transform.localRotation = Quaternion.identity;
				hitbox.transform.position = cell.transform.position;
				hitbox.transform.localScale = new Vector3(0.9f, 0.9f, 1);
				hitbox.GetComponent<Button>().onClick.AddListener(() => CreateObstacle(cell, hitbox));
				allCellHitBox.Add(hitbox);
			}
		}
	}

	private void CheckObstacleOnTurn(Color color) 
	{
		foreach (Cell cell in obstacleCells) 
		{
			if (cell.curPiece != null && cell.curPiece.color == color) 
			{
				PoisonCell(cell);
				FogCell(cell);
				TeleportCell(cell);
				BombCell(cell);
				RootCell(cell);
				SlowCell(cell);
			}
		}
	}
	private void PoisonCell(Cell cell) 
	{
		if (!cell.transform.GetChild(3).gameObject.activeSelf)
			return;
		BasePiece piece = cell.curPiece;
		if (!piece.transform.GetChild(4).gameObject.activeSelf) 
		{
			piece.transform.GetChild(4).gameObject.SetActive(true);
			return;
		}
		if (piece.transform.GetChild(4).transform.GetChild(0).gameObject.activeSelf) 
		{
			piece.transform.GetChild(4).transform.GetChild(0).gameObject.SetActive(false);
			return;
		}
		piece.Kill();
	}
	private void SlowCell(Cell cell)
	{
		if (!cell.transform.GetChild(8).gameObject.activeSelf)
			return;
		if (!cell.curPiece.transform.GetChild(5).gameObject.activeSelf)
		{
			cell.curPiece.tmpMovement = cell.curPiece.movement;
			cell.curPiece.Slowed(1);
			cell.curPiece.transform.GetChild(5).gameObject.SetActive(true);
		}
	}
	private void RootCell(Cell cell)
	{
		if (!cell.transform.GetChild(7).gameObject.activeSelf)
			return;

		if (cell.curPiece.root > 0 && cell.curPiece.transform.GetChild(6).gameObject.activeSelf) 
		{
			cell.curPiece.root -= 1;
			return;
		}	

		if(cell.curPiece.root == 0) 
		{
			cell.curPiece.root = 2;
			cell.curPiece.transform.GetChild(6).gameObject.SetActive(false);
			cell.curPiece.movement = cell.curPiece.tmpMovement;
			return;
		}

		cell.curPiece.tmpMovement = cell.curPiece.movement;
		cell.curPiece.Slowed(0);
		cell.curPiece.transform.GetChild(6).gameObject.SetActive(true);
	}
	private void FogCell(Cell cell)
	{
		if (!cell.transform.GetChild(4).gameObject.activeSelf)
			return;
		cell.curPiece.transform.GetChild(3).gameObject.SetActive(true);
		cell.curPiece.transform.GetChild(3).GetComponent<Image>().sprite = Resources.Load<Sprite>("Fog");
	}
	
	private void TeleportCell(Cell cell)
	{
		if (!cell.transform.GetChild(5).gameObject.activeSelf)
			return;
		List<Cell> cells = TeleportCellList(cell);
		if (cells.Count > 0)
		{
			cell.curPiece.canTeleport = true;
			cell.curPiece.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
			cell.curPiece.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(() => TeleportPiece(cells, cell.curPiece));
		}
		else 
		{
			cell.curPiece.canTeleport = false;
		}
	}
	private void TeleportPiece(List<Cell> cells, BasePiece piece) 
	{
		int rInt = Roll(cells.Count - 1);
		piece.targetCell = cells[rInt];
		piece.MoveHelper();
		SwitchSide(piece.color);
	}
	private int Roll(int max)
	{
		float random = Random.Range(0f, (float)max);
		return (int)Math.Round(random);
	}
	private List<Cell> TeleportCellList(Cell curcell) 
	{
		List<Cell> tpList = new List<Cell>();
		foreach (Cell cell in obstacleCells) 
		{
			if (cell.transform.GetChild(5).gameObject.activeSelf && (cell != curcell && cell.curPiece == null))
				tpList.Add(cell);
		}
		return tpList;
	}
	private void BombCell(Cell cell)
	{
		if (!cell.transform.GetChild(6).gameObject.activeSelf)
			return;
		if (cell.curPiece.transform.GetChild(7).gameObject.activeSelf)
			return;
		cell.curPiece.transform.GetChild(7).gameObject.SetActive(true);
		cell.transform.GetChild(6).gameObject.SetActive(false);
	}
	
	private void SelectObstacle() 
	{
		obstacleSelection.SetActive(true);
		foreach (Transform child in obstacleSelection.transform)
		{
			Button btn = child.gameObject.GetComponent<Button>();
			btn.onClick.AddListener(() => GetObstacle(btn));
		}
	}
	private void GetObstacle(Button btn) 
	{
		obstacleSelected = btn.gameObject.name;
	}
	#endregion
	#region Skills
	private void UseOrcSkill(GameObject btn, int side)
	{
		GameObject cell = btn.GetComponent<HideButton>().mountainCell.gameObject;
		cell.transform.GetChild(2).gameObject.SetActive(false);
		_ = side == 0 ? orcWhiteSideSkillCooldown = 4 : orcBlackSideSkillCooldown = 4;
		Destroy(btn);
		ShowActiveSkillLableHelper(4, side);
		SwitchSide(side == 0 ? Color.white : Color.black);
	}
	private bool CheckOrcSkill(int side, GameObject btn)
	{
		if (side == 0)
		{
			return orcWhiteSideSkillCooldown == 0 && whitePieces[0].race.Equals("Orc") && btn.GetComponent<HideButton>().mountainCell.FindFriend(whitePieces[0]);
		}
		return orcBlackSideSkillCooldown == 0 && blackPieces[0].race.Equals("Orc") && btn.GetComponent<HideButton>().mountainCell.FindFriend(blackPieces[0]);
	}
	public void SkillCooldown(Color color)
	{
		if (color == Color.black)
		{
			BasePiece tmp = whitePieces[0];
			SkillCoolDownHelper(tmp, 0);
		}
		else
		{
			BasePiece tmp = blackPieces[0];
			SkillCoolDownHelper(tmp, 1);
		}
	}

	private void SkillCoolDownHelper(BasePiece piece, int side)
	{
		string race = piece.race;
		if (!race.Equals("Goblin") && !race.Equals("Giant") && !race.Equals("Orc"))
		{
			return;
		}
		switch (race)
		{
			case "Goblin":
				GoblinSkillCD(side);
				break;
			case "Giant":
				GiantSkillCD(side);
				break;
			case "Orc":
				OrcSkillCD(side);
				break;
			default:
				break;
		}
		if (!gameManager.activeSkillTimer.transform.GetChild(side).gameObject.activeSelf)
		{
			gameManager.activeSkillTimer.transform.GetChild(side).gameObject.SetActive(true);
		}
	}

	public int GetGiantSide(Color side)
	{
		if (side == Color.white)
			return giantWhiteSideSkillCooldown;
		return giantBlackSideSkillCooldown;
	}

	public void SetGiantSide(Color side, int newValue)
	{
		if (side == Color.white)
			giantWhiteSideSkillCooldown = newValue;
		else
			giantBlackSideSkillCooldown = newValue;
	}

	private void GiantSkillCD(int side)
	{
		int cooldown = GetGiantSide(side == 0 ? Color.white : Color.black);
		cooldown = ShowActiveSkillLable(cooldown, side);
		SetGiantSide(side == 0 ? Color.white : Color.black, cooldown);
	}
	private void OrcSkillCD(int side)
	{
		_ = side == 0 ? orcWhiteSideSkillCooldown = ShowActiveSkillLable(orcWhiteSideSkillCooldown, side)
			: orcBlackSideSkillCooldown = ShowActiveSkillLable(orcBlackSideSkillCooldown, side);
	}
	private int ShowActiveSkillLable(int cooldown, int side)
	{
		if (!gameManager.activeSkillTimer.transform.GetChild(side).gameObject.activeSelf)
		{
			return ShowActiveSkillLableHelper(cooldown, side);
		}
		cooldown--;
		return ShowActiveSkillLableHelper(cooldown, side);
	}

	private int ShowActiveSkillLableHelper(int cooldown, int side)
	{
		if (cooldown > 0)
		{
			gameManager.activeSkillTimer.transform.GetChild(side).GetComponent<Text>().text = cooldown.ToString() + " turns left until your skill refresh";
			return cooldown;
		}
		gameManager.activeSkillTimer.transform.GetChild(side).GetComponent<Text>().text = "Skill activated";
		return 0;
	}
	//Passive Skill
	private void GoblinSkillCD(int side)
	{
		int goblinCD = side == 0 ? goblinWhiteSideSkillCooldown : goblinBlackSideSkillCooldown;
		if (goblinCD > 0)
		{
			gameManager.activeSkillTimer.transform.GetChild(side).GetComponent<Text>().text = goblinCD.ToString() + " turns left until your skill refresh";
		}
		else
		{
			gameManager.activeSkillTimer.transform.GetChild(side).GetComponent<Text>().text = "Pawns can now move 2 tiles forward";
			_ = side == 0 ? goblinWhiteSideSkillCooldown = 4 : goblinBlackSideSkillCooldown = 4;
			if (side == 0)
			{
				foreach (BasePiece pawn in whitePawns)
				{
					pawn.firstMove = true;
				}
			}
			else
			{
				foreach (BasePiece pawn in blackPawns)
				{
					pawn.firstMove = true;
				}
			}
		}
		_ = side == 0 ? goblinWhiteSideSkillCooldown-- : goblinBlackSideSkillCooldown--;
	}
	#endregion
	#region Pawn
	public void PromotePiece(Pawn pawn, Cell cell, Color teamColor, Color spriteColor, Type promoteType, bool devilPromote)
	{
		//remove pawn
		pawn.Kill();

		//Create piece
		BasePiece newPiece = CreatePiece(promoteType);
		newPiece.Setup(teamColor, spriteColor, this, pawn.race);


		//Place piece
		newPiece.Place(cell);
		//Add to list
		promotedPieces.Add(newPiece);
		SwitchSide(teamColor == Color.white ? Color.white : Color.black);
		//Devil trigger
		if (devilPromote)
		{
			if (newPiece is Queen)
			{
				gameManager.SubNumOfPieceTaken(9);
				return;
			}
			if (newPiece is Rook)
			{
				gameManager.SubNumOfPieceTaken(5);
				return;
			}
			if (newPiece is Knight || newPiece is Bishop)
			{
				gameManager.SubNumOfPieceTaken(3);
				return;
			}
		}
		//Golen trigger
		if (!pawn.race.Equals("Golem"))
			return;

		if (teamColor == Color.white)
		{
			PromoteHelper(whitePieces, teamColor, spriteColor, promoteType);
		}
		else
		{
			PromoteHelper(blackPieces, teamColor, spriteColor, promoteType);
		}
	}
	private void PromoteHelper(List<BasePiece> Pieces, Color teamColor, Color spriteColor, Type promoteType)
	{
		foreach (BasePiece piece in Pieces)
		{
			if (piece is Pawn && piece.gameObject.activeSelf)
				PromotedPawn((Pawn)piece, piece.curCell, teamColor, spriteColor, promoteType);
		}
	}
	private BasePiece PromotedPawn(Pawn pawn, Cell cell, Color teamColor, Color spriteColor, Type promoteType)
	{
		//remove pawn
		pawn.Kill();

		//Create piece
		BasePiece newPiece = CreatePiece(promoteType);
		newPiece.Setup(teamColor, spriteColor, this, pawn.race);


		//Place piece
		newPiece.Place(cell);

		//Add to list
		promotedPieces.Add(newPiece);

		return newPiece;
	}
	#endregion
}
