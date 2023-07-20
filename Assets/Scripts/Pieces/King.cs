using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class King : BasePiece
{
	private Rook leftRook = null;
	private Rook rightRook = null;

	public override void Setup(Color teamColor, Color32 newSpriteColor, PieceManager pieceManager, string race)
	{
		base.Setup(teamColor, newSpriteColor, pieceManager, race);

		movement = new Vector3Int(1, 1, 1);
		GetComponent<Image>().sprite = Resources.Load<Sprite>("King");
	}

	public override void Kill()
	{
		base.Kill();

		pieceManager.isGameContinue = false;
	}

	public override void CheckPath()
	{
		base.CheckPath();

		rightRook = GetRook(1, 3);

		leftRook = GetRook(-1, 4);

		List<Cell> tmp = new List<Cell>(highlightCells);
		foreach (Cell cell in tmp)
		{
			if (!CanBeCheckThere(cell, color))
			{
				highlightCells.Remove(cell);
			}
		}
	}

	private bool CanBeCheckThere(Cell cell, Color color) 
	{
		return PawnsCheck(cell, color) && LowerDiagLegalMove(cell, color) && UpperDiagLegalMove(cell, color) && RightLegalMove(cell, color) && LeftLegalMove(cell, color) && DownLegalMove(cell, color) && UpLegalMove(cell, color) && KnightCheck(cell, color);
	}
	private bool PawnsCheck(Cell cell, Color color)
	{
		int y = cell.boardPos.y;
		int x = cell.boardPos.x;
		_ = color == Color.white ? y += 1 : y -= 1;
		if (x - 1 >= 0)
		{
			if (y >= 0 && y <= 7 && cell.board.AllCells[x - 1, y].curPiece != null && cell.board.AllCells[x - 1, y].curPiece.color != color && cell.board.AllCells[x - 1, y].curPiece is Pawn)
			{
				return false;
			}
		}
		if (x + 1 <= 7)
		{
			if (y >= 0 && y <= 7 && cell.board.AllCells[x + 1, y].curPiece != null && cell.board.AllCells[x + 1, y].curPiece.color != color && cell.board.AllCells[x + 1, y].curPiece is Pawn)
			{
				return false;
			}
		}
		return true;
	}
	private bool LowerDiagLegalMove(Cell cell, Color color)
	{
		BasePiece defPiece = null;
		BasePiece defPiece1 = null;
		int y = cell.boardPos.y;
		int x = cell.boardPos.x;
		for (int i = 1; i >= y; i++)
		{
			if (x - i >= 0 && y - i >= 0)
			{
				Cell tmp = cell.board.AllCells[x - i, y - i];
				if (tmp.curPiece != null)
				{
					if (tmp.curPiece.color == color)
					{
						if (defPiece == null)
							defPiece = tmp.curPiece;
					}
					else
					{
						if (tmp.curPiece is Bishop || tmp.curPiece is Queen)
						{
							if (defPiece == null)
							{
								return false;
							}
						}
					}
				}
			}
			if (x + i <= 7 && y - i >= 0)
			{
				Cell tmp = cell.board.AllCells[x + i, y - i];
				if (tmp.curPiece != null)
				{
					if (tmp.curPiece.color == color)
					{
						if (defPiece1 == null)
							defPiece1 = tmp.curPiece;
					}
					else
					{
						if (tmp.curPiece is Bishop || tmp.curPiece is Queen)
						{
							if (defPiece1 == null)
							{
								return false;
							}
						}
					}
				}
			}
			else
				break;
		}
		return true;
	}
	private bool UpperDiagLegalMove(Cell cell, Color color)
	{
		BasePiece defPiece = null;
		BasePiece defPiece1 = null;
		int y = cell.boardPos.y;
		int x = cell.boardPos.x;
		for (int i = 1; i <= 7 - y; i++)
		{
			if (x + i <= 7)
			{
				Cell tmp = cell.board.AllCells[x + i, y + i];
				if (tmp.curPiece != null)
				{
					if (tmp.curPiece.color == color)
					{
						if (defPiece == null)
							defPiece = tmp.curPiece;
					}
					else
					{
						if (tmp.curPiece is Bishop || tmp.curPiece is Queen)
						{
							if (defPiece == null)
							{
								return false;
							}
						}
					}
				}
			}
			if (x - i >= 0)
			{
				Cell tmp = cell.board.AllCells[x - i, y + i];
				if (tmp.curPiece != null)
				{
					if (tmp.curPiece.color == color)
					{
						if (defPiece1 == null)
							defPiece1 = tmp.curPiece;
					}
					else
					{
						if (tmp.curPiece is Bishop || tmp.curPiece is Queen)
						{
							if (defPiece == null)
							{
								return false;
							}
						}
					}
				}
			}
		}
		return true;
	}
	private bool RightLegalMove(Cell cell, Color color)
	{
		BasePiece defPiece = null;
		int y = cell.boardPos.y;
		int x = cell.boardPos.x + 1;
		for (int i = x; i <= 7; i++)
		{
			Cell tmp = cell.board.AllCells[i, y];
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
							return false;
						}
						else
						{
							break;
						}
					}
				}
			}
		}
		return true;
	}
	private bool LeftLegalMove(Cell cell, Color color)
	{
		BasePiece defPiece = null;
		int y = cell.boardPos.y;
		int x = cell.boardPos.x - 1;
		for (int i = x; i >= 0; i--)
		{
			Cell tmp = cell.board.AllCells[i, y];
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
							return false;
						}
						else
						{
							break;
						}
					}
				}
			}
		}
		return true;
	}
	private bool DownLegalMove(Cell cell, Color color)
	{
		BasePiece defPiece = null;
		int y = cell.boardPos.y - 1;
		int x = cell.boardPos.x;
		for (int i = y; i >= 0; i--)
		{
			Cell tmp = cell.board.AllCells[x, i];
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
							return false;
						}
						else
						{
							break;
						}
					}
				}
			}
		}
		return true;
	}
	private bool UpLegalMove(Cell cell, Color color)
	{
		BasePiece defPiece = null;
		int y = cell.boardPos.y + 1;
		int x = cell.boardPos.x;
		for (int i = y; i <= 7; i++)
		{
			Cell tmp = cell.board.AllCells[x, i];
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
							return false;
						}
						else
						{
							break;
						}
					}
				}
			}
		}
		return true;
	}
	private bool KnightCheck(Cell cell, Color color)
	{
		Color opoColor = color == Color.white ? Color.black : Color.white;
		int x = cell.boardPos.x;
		int y = cell.boardPos.y;
		if (y + 1 <= 7)
		{
			if (x + 2 <= 7 && cell.board.AllCells[x + 2, y + 1].curPiece != null && cell.board.AllCells[x + 2, y + 1].curPiece is Knight && cell.board.AllCells[x + 2, y + 1].curPiece.color == opoColor)
			{
				return false;
			}
			if (x - 2 >= 0 && cell.board.AllCells[x - 2, y + 1].curPiece != null && cell.board.AllCells[x - 2, y + 1].curPiece is Knight && cell.board.AllCells[x - 2, y + 1].curPiece.color == opoColor)
			{
				return false;
			}
		}
		if (y + 2 <= 7)
		{
			if (x + 1 <= 7 && cell.board.AllCells[x + 1, y + 2].curPiece != null && cell.board.AllCells[x + 1, y + 2].curPiece is Knight && cell.board.AllCells[x + 1, y + 2].curPiece.color == opoColor)
			{
				return false;
			}
			if (x - 1 >= 0 && cell.board.AllCells[x - 1, y + 2].curPiece != null && cell.board.AllCells[x - 1, y + 2].curPiece is Knight && cell.board.AllCells[x - 1, y + 2].curPiece.color == opoColor)
			{
				return false;
			}
		}
		if (y - 1 >= 0)
		{
			if (x + 2 <= 7 && cell.board.AllCells[x + 2, y - 1].curPiece != null && cell.board.AllCells[x + 2, y - 1].curPiece is Knight && cell.board.AllCells[x + 2, y - 1].curPiece.color == opoColor)
			{
				return false;
			}
			if (x - 2 >= 0 && cell.board.AllCells[x - 2, y - 1].curPiece != null && cell.board.AllCells[x - 2, y - 1].curPiece is Knight && cell.board.AllCells[x - 2, y - 1].curPiece.color == opoColor)
			{
				return false;
			}
		}
		if (y - 2 >= 0)
		{
			if (x + 1 <= 7 && cell.board.AllCells[x + 1, y - 2].curPiece != null && cell.board.AllCells[x + 1, y - 2].curPiece is Knight && cell.board.AllCells[x + 1, y - 2].curPiece.color == opoColor)
			{
				return false;
			}
			if (x - 1 >= 0 && cell.board.AllCells[x - 1, y - 2].curPiece != null && cell.board.AllCells[x - 1, y - 2].curPiece is Knight && cell.board.AllCells[x - 1, y - 2].curPiece.color == opoColor)
			{
				return false;
			}
		}
		return true; 
	}

	protected override void Move()
	{
		base.Move();
		if (CanCastle(leftRook))
			leftRook.Castle();

		if (CanCastle(rightRook))
			rightRook.Castle();
	}

	internal Cell CanCastle() 
	{
		foreach (Cell cell in highlightCells) 
		{
			if (leftRook != null && cell == leftRook.castleTriggerCell)
				return cell;
			if (rightRook != null && cell == rightRook.castleTriggerCell)
				return cell;
		}
		return null;
	}
	private bool CanCastle(Rook rook)
	{
		if (rook == null)
			return false;

		if (rook.castleTriggerCell != curCell)
			return false;
		return true;
	}

	private Rook GetRook(int dir, int count)
	{
		if (!firstMove)
			return null;

		int curX = curCell.boardPos.x;
		int curY = curCell.boardPos.y;

		for (int i = 1; i < count; i++)
		{
			int offsetX = curX + i * dir;
			CellState state = curCell.board.ValidateCell(offsetX, curY, this);

			if (state != CellState.Free)
				return null;
		}
		if(curX + count * dir < 0 || curX + count * dir > 7 || curY > 7 || curY < 0)
			return null;
		Cell rookCell = curCell.board.AllCells[curX + count * dir, curY];
		Rook rook = null;

		if (rookCell.curPiece == null)
			return null;

		if (rookCell.curPiece is Rook)
			rook = (Rook)rookCell.curPiece;
		if (rook == null)
			return null;

		if (rook.color != color || !rook.firstMove)
			return null;

		if (rook != null)
			highlightCells.Add(rook.castleTriggerCell);
		return rook;
	}
}
