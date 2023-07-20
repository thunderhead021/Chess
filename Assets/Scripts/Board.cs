using UnityEngine;
using UnityEngine.UI;

public enum CellState
{
	None,
	Friend,
	Enemy,
	Free,
	OutOfBound,
	Mountain
}
public class Board : MonoBehaviour
{
	public GameObject cellPrefab;
	public GameManager GameManager;
	[HideInInspector]
	public Cell[,] AllCells = new Cell[8, 8];
	public void CreateBoard()
	{
		for (int x = 0; x < 8; x++)
		{
			for (int y = 0; y < 8; y++)
			{
				//Create cell
				GameObject newCell = Instantiate(cellPrefab, transform);
				newCell.name = "[" + x + "," + y + "]";
				//Add Pos to Cell
				RectTransform rectTransform = newCell.GetComponent<RectTransform>();
				rectTransform.anchoredPosition = new Vector2((x * 100) + 50, (y * 100) + 50);
				//Set Cell to Board
				AllCells[x, y] = newCell.GetComponent<Cell>();
				AllCells[x, y].Setup(new Vector2Int(x, y), this);
			}
		}

		for (int x = 0; x < 8; x += 2)
		{
			for (int y = 0; y < 8; y++)
			{
				//Create offset
				int offset = (y % 2 != 0) ? 0 : 1;
				int xPos = x + offset;

				//Add color
				AllCells[xPos, y].GetComponent<Image>().color = new Color32(230, 220, 187, 255);
			}
		}
	}
	public CellState ValidateCell(int targetX, int targetY, BasePiece checkPiece)
	{
		//out of bound
		if (targetX < 0 || targetX > 7 || targetY < 0 || targetY > 7)
			return CellState.OutOfBound;

		Cell target = AllCells[targetX, targetY];

		//wall detect
		if (target.wall != null && checkPiece.color != target.color)
		{
			if (target.curPiece != null && target.curPiece is Rook rook)
			{
				if (rook.mark)
					return CellState.Enemy;
			}
			else
				return CellState.OutOfBound;
		}

		//friend cell or enemy cell
		if (target.curPiece != null)
		{
			if (checkPiece.color == target.curPiece.color)
				return CellState.Friend;
			else
			{
				//if target is in bush
				if (target.curPiece.transform.GetChild(3).gameObject.activeSelf)
					return CellState.OutOfBound;
				return CellState.Enemy;
			}
		}

		//if there a mountain
		if (target.transform.GetChild(2).gameObject.activeSelf)
		{
			//check piece race equal Dwarf
			if (checkPiece.race.Equals("Dwarf"))
			{
				return CellState.Free;
			}
			else
				return CellState.Mountain;
		}
		//else is free
		return CellState.Free;
	}

	public void ResetCells()
	{
		foreach (Cell cell in AllCells)
		{
			cell.transform.GetChild(1).gameObject.SetActive(false);
			cell.transform.GetChild(2).gameObject.SetActive(false);
		}
	}
}
