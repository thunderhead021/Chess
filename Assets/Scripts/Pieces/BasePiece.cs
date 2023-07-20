using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class BasePiece : EventTrigger
{
	[HideInInspector]
	public Color color = Color.clear;
	public string race;
	protected Cell originCell = null;
	public Cell curCell = null;
	public Cell targetCell = null;
	protected RectTransform rect = null;
	public PieceManager pieceManager;
	public Vector3Int movement = Vector3Int.one;
	public Vector3Int tmpMovement = Vector3Int.one;
	public List<Cell> highlightCells = new List<Cell>();
	public bool firstMove = true;
	[HideInInspector]
	public bool canTeleport = false;
	private bool angleBuff = false;
	public int root = 2;
	public virtual void Setup(Color teamColor, Color32 newSpriteColor, PieceManager pieceManager, string race)
	{
		this.pieceManager = pieceManager;
		GetComponent<Image>().color = newSpriteColor;
		rect = GetComponent<RectTransform>();
		color = teamColor;
		this.race = race;
	}

	public virtual void Place(Cell newCell)
	{
		//change cell
		curCell = newCell;
		originCell = newCell;
		curCell.curPiece = this;

		transform.position = newCell.transform.position;
		gameObject.SetActive(true);
	}

	public void Reset()
	{
		Kill();
		Place(originCell);
		firstMove = true;
		for (int i = 0; i < transform.childCount; i++) 
		{
			transform.GetChild(i).gameObject.SetActive(false);
		}
	}
	public virtual void Kill()
	{
		curCell.curPiece = null;
		gameObject.SetActive(false);
		transform.GetChild(4).gameObject.SetActive(false);
		transform.GetChild(4).transform.GetChild(0).gameObject.SetActive(true);
		transform.GetChild(3).GetComponent<Image>().sprite = Resources.Load<Sprite>("Bush");
	}
	#region Movement
	protected virtual void Move()
	{
		targetCell.RemovePiece();
		curCell.curPiece = null;

		curCell = targetCell;
		curCell.curPiece = this;

		transform.position = curCell.transform.position;
		targetCell = null;

		firstMove = false;
		angleBuff = false;
		transform.GetChild(2).gameObject.SetActive(false);
		pieceManager.gameManager.DeactiveSkipBtn();
		CheckNearbyEnemies();
		if (curCell.transform.GetChild(1).gameObject.activeSelf)
		{
			angleBuff = true;
			transform.GetChild(2).gameObject.SetActive(true);
			curCell.transform.GetChild(1).gameObject.SetActive(false);
			if (color == Color.black)
			{
				pieceManager.angelBuffBlackPieces.Add(this);
				pieceManager.gameManager.button.transform.GetChild(1).gameObject.SetActive(true);
			}
			else
			{
				pieceManager.angelBuffWhitePieces.Add(this);
				pieceManager.gameManager.button.transform.GetChild(0).gameObject.SetActive(true);
			}
		}
		if (!curCell.transform.GetChild(8).gameObject.activeSelf && transform.GetChild(5).gameObject.activeSelf) 
		{
			movement = tmpMovement;
			transform.GetChild(5).gameObject.SetActive(false);
			return;
		}
		if (!curCell.transform.GetChild(5).gameObject.activeSelf && canTeleport)
		{
			canTeleport = false;
			transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.SetActive(false); 
			return;
		}
	}
	public virtual void Slowed(int num) 
	{
		if (num == 0)
			movement = Vector3Int.zero;
	}
	private void CreatePath(int xDir, int yDir, int movement)
	{
		if (curCell == null)
			return;
		//target Pos
		int curX = curCell.boardPos.x;
		int curY = curCell.boardPos.y;

		for (int i = 1; i <= movement; i++)
		{
			curX += xDir;
			curY += yDir;

			CellState cellState = curCell.board.ValidateCell(curX, curY, this);

			if (cellState == CellState.Enemy)
			{
				highlightCells.Add(curCell.board.AllCells[curX, curY]);
				break;
			}

			if (cellState != CellState.Free)
				break;

			if (!curCell.board.AllCells[curX, curY].transform.GetChild(2).gameObject.activeSelf)
				highlightCells.Add(curCell.board.AllCells[curX, curY]);
		}
	}

	public virtual void CheckPath()
	{
		//Horizontal
		CreatePath(1, 0, movement.x);
		CreatePath(-1, 0, movement.x);

		//Vertical
		CreatePath(0, 1, movement.y);
		CreatePath(0, -1, movement.y);

		//Upper Diagonal
		CreatePath(1, 1, movement.z);
		CreatePath(-1, 1, movement.z);

		//Lower Diagonal
		CreatePath(-1, -1, movement.z);
		CreatePath(1, -1, movement.z);

		//remove illegal move
	}

	protected void ShowCells()
	{
		foreach (Cell cell in highlightCells)
			cell.outlineImage.enabled = true;
	}

	public void CleanCells()
	{
		foreach (Cell cell in highlightCells)
			cell.outlineImage.enabled = false;
		highlightCells.Clear();
	}
	#endregion

	#region Event
	public override void OnBeginDrag(PointerEventData eventData)
	{
		base.OnBeginDrag(eventData);
		PathHelper();
		ShowCells();
	}
	public void PathHelper() 
	{
		CheckPath();
		RemoveHighlightCell();
	}
	private void RemoveHighlightCell() 
	{
		List<Cell> tmp = new List<Cell>();
		if (pieceManager.legalCells.Count > 0)
		{
			tmp = new List<Cell>(pieceManager.legalCells);
		}
		if (pieceManager.captureCouples.Count > 0)
		{
			tmp = GetMoveableList();
		}
		if (tmp.Count > 0)
		{
			if (!(this is King) && tmp.Contains(curCell))
			{
				if (!IsAnyOtherHighlightCellInList(tmp)) 
				{
					highlightCells.Clear();
					return;
				}	
			}
			else
			{
				List<Cell> x = new List<Cell>(highlightCells);
				foreach (Cell cell in x)
				{
					if (this is King)
					{
						if(tmp.Contains(cell) && cell.curPiece == null)
							highlightCells.Remove(cell);
					}
					else 
					{
						if (!tmp.Contains(cell))
							highlightCells.Remove(cell);
					}
				}
			}	
		}
	}
	private bool IsAnyOtherHighlightCellInList(List<Cell> check) 
	{
		foreach (Cell cell in highlightCells) 
		{
			if (check.Contains(cell))
				return true;
		}
		return false;
	}
	private List<Cell> GetMoveableList() 
	{
		foreach (CaptureCouple couple in pieceManager.captureCouples) 
		{		
			if (couple.IsThisPieceTheAttackedPiece(this))
			{
				return couple.attackDir;
			}
		}
		return new List<Cell>();
	}
	public override void OnDrag(PointerEventData eventData)
	{
		base.OnDrag(eventData);

		transform.position += (Vector3)eventData.delta;

		foreach (Cell cell in highlightCells)
		{
			if (RectTransformUtility.RectangleContainsScreenPoint(cell.rectTransform, Input.mousePosition))
			{
				targetCell = cell;
				break;
			}

			targetCell = null;
		}
	}

	public override void OnEndDrag(PointerEventData eventData)
	{
		base.OnEndDrag(eventData);
		CleanCells();
		if (!targetCell)
		{
			transform.position = curCell.gameObject.transform.position;
			return;
		}

		MoveHelper();
		if (!angleBuff)
			pieceManager.SwitchSide(color);
		else
		{
			pieceManager.SwitchSide(Color.red);
			if (color == Color.black)
				pieceManager.SetInteractive(pieceManager.angelBuffBlackPieces, true);
			else
				pieceManager.SetInteractive(pieceManager.angelBuffWhitePieces, true);
		}
	}

	public void MoveHelper() 
	{
		Move();
		//Elf passive active here
		if (race.Equals("Elf"))
			ElfSkill();
		CheckNearbyEnemies();
	}
	public void DisableAllBtn() 
	{
		transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(false);
		transform.GetChild(1).transform.GetChild(1).gameObject.SetActive(false);
		transform.GetChild(1).transform.GetChild(2).gameObject.SetActive(false);
	}
	public override void OnPointerClick(PointerEventData eventData)
	{
		base.OnPointerClick(eventData);
		GetComponent<HideButton>().notInPiece = false;
		pieceManager.DisableBtn(color);
		GameObject btn1 = transform.GetChild(1).gameObject.transform.GetChild(0).gameObject;
		GameObject btn2 = transform.GetChild(1).gameObject.transform.GetChild(1).gameObject;
		GameObject btn3 = transform.GetChild(1).gameObject.transform.GetChild(2).gameObject;
		if (btn1.activeSelf || btn2.activeSelf || btn3.activeSelf)
		{
			DisableAllBtn();
			return;
		}
		if (race.Equals("Devil") && pieceManager.gameManager.numberOfPieceTaken >= 3 && this is Pawn)
		{
			btn1.SetActive(true);
			btn1.GetComponent<Button>().onClick.AddListener(() => GetComponent<Pawn>().Promote(true));
		}
		if (race.Equals("Giant"))
		{
			btn1.GetComponentInChildren<Text>().text = "Active Skill";
			if (pieceManager.GetGiantSide(color) == 0)
			{
				btn1.SetActive(true);
				btn1.GetComponent<Button>().onClick.AddListener(() => GiantActive());
			}
		}
		if (race.Equals("Spirit") && !(this is King))
		{
			btn1.GetComponentInChildren<Text>().text = "Active Skill";
			btn1.SetActive(true);
			btn1.GetComponent<Button>().onClick.AddListener(() => SpiritSkill(this));
		}
		if (canTeleport) 
		{
			btn2.SetActive(true);
		}
		if (transform.GetChild(7).gameObject.activeSelf) 
		{
			btn3.SetActive(true);
			btn3.GetComponent<Button>().onClick.AddListener(() => Suicide());
		}
	}
	#endregion

	#region Skill
	private void Suicide() 
	{
		Kill();
		GiantActive();
		transform.GetChild(7).gameObject.SetActive(false);
	}
	private void CheckSurrounding(int xDir, int yDir)
	{
		int curX = curCell.boardPos.x;
		int curY = curCell.boardPos.y;

		curX += xDir;
		curY += yDir;

		CellState cellState = curCell.board.ValidateCell(curX, curY, this);

		if (cellState == CellState.Mountain)
		{
			if (race.Equals("Elf"))
			{
				transform.GetChild(3).gameObject.SetActive(true);
				return;
			}
			if (race.Equals("Orc"))
			{
				curCell.board.AllCells[curX, curY].transform.GetChild(2).gameObject.SetActive(false);
				return;
			}
		}

		if (cellState == CellState.Enemy)
		{
			BasePiece enemy = curCell.board.AllCells[curX, curY].curPiece;
			if (race.Equals("Giant") || transform.GetChild(7).gameObject.activeSelf)
			{
				enemy.Kill();
				return;
			}
		}	
	}

	private void EnemyNearby(int xDir, int yDir) 
	{
		int curX = curCell.boardPos.x;
		int curY = curCell.boardPos.y;

		curX += xDir;
		curY += yDir;
		if (curX > 7 || curY > 7 || curX < 0 || curY < 0 || curCell.board.AllCells[curX, curY].curPiece == null)
			return;

		BasePiece enemy = curCell.board.AllCells[curX, curY].curPiece;
		CellState cellState = curCell.board.ValidateCell(curX, curY, this);
		if (enemy != null && (cellState == CellState.Enemy || cellState == CellState.OutOfBound))
		{	
			if (enemy.transform.GetChild(3).gameObject.activeSelf)
			{
				enemy.transform.GetChild(3).gameObject.SetActive(false);
				return;
			}
			else if(transform.GetChild(3).gameObject.activeSelf)
			{
				transform.GetChild(3).gameObject.SetActive(false);
				return;
			}
		}
	}

	internal void CheckNearbyEnemies() 
	{
		//Horizontal
		EnemyNearby(1, 0);
		EnemyNearby(-1, 0);

		//Vertical
		EnemyNearby(0, 1);
		EnemyNearby(0, -1);

		//Upper Diagonal
		EnemyNearby(1, 1);
		EnemyNearby(-1, 1);

		//Lower Diagonal
		EnemyNearby(-1, -1);
		EnemyNearby(1, -1);
	}

	private void SpiritSkill(BasePiece piece)
	{
		piece.curCell.transform.GetChild(2).gameObject.SetActive(true);
		piece.Kill();
		piece.pieceManager.SwitchSide(piece.color == Color.white ? Color.white : Color.black);
	}

	public void ElfSkill()
	{
		if (!transform.GetChild(3).GetComponent<Image>().sprite.name.Equals("Bush"))
			return;
		transform.GetChild(3).gameObject.SetActive(false);
		ElfSKillHelper();
	}

	private void ElfSKillHelper() 
	{
		//Horizontal
		CheckSurrounding(1, 0);
		if (transform.GetChild(3).gameObject.activeSelf)
			return;

		CheckSurrounding(-1, 0);
		if (transform.GetChild(3).gameObject.activeSelf)
			return;

		//Vertical
		CheckSurrounding(0, 1);
		if (transform.GetChild(3).gameObject.activeSelf)
			return;

		CheckSurrounding(0, -1);
		if (transform.GetChild(3).gameObject.activeSelf)
			return;

		//Upper Diagonal
		CheckSurrounding(1, 1);
		if (transform.GetChild(3).gameObject.activeSelf)
			return;

		CheckSurrounding(-1, 1);
		if (transform.GetChild(3).gameObject.activeSelf)
			return;

		//Lower Diagonal
		CheckSurrounding(-1, -1);
		if (transform.GetChild(3).gameObject.activeSelf)
			return;
		CheckSurrounding(1, -1);
		if (transform.GetChild(3).gameObject.activeSelf)
			return;
	}

	private void GiantActive()
	{
		//Horizontal
		CheckSurrounding(1, 0);
		CheckSurrounding(-1, 0);

		//Vertical
		CheckSurrounding(0, 1);
		CheckSurrounding(0, -1);

		//Upper Diagonal
		CheckSurrounding(1, 1);
		CheckSurrounding(-1, 1);

		//Lower Diagonal
		CheckSurrounding(-1, -1);
		CheckSurrounding(1, -1);

		pieceManager.SetGiantSide(color, 6);
		pieceManager.SkillCooldown(color == Color.white ? Color.black : Color.white);
		GameObject btn = transform.GetChild(1).gameObject;
		btn.SetActive(false);
		pieceManager.SwitchSide(color);
	}
	#endregion
}
