using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rook : BasePiece
{
	[HideInInspector]
	public Cell castleTriggerCell = null;
	[HideInInspector]
	public bool mark = false;
	private Cell castleCell = null;
	private List<GameObject> castleWall = new List<GameObject>();
	private List<Cell> castleWallCell = new List<Cell>();
	public override void Setup(Color teamColor, Color32 newSpriteColor, PieceManager pieceManager, string race)
	{
		base.Setup(teamColor, newSpriteColor, pieceManager, race);

		movement = new Vector3Int(7, 7, 0);
		GetComponent<Image>().sprite = Resources.Load<Sprite>("Rook");
	}

	public override void Slowed(int num)
	{
		base.Slowed(num);
		movement = new Vector3Int(num, num, 0);
	}
	public override void Kill()
	{
		base.Kill();
		if (mark)
		{
			foreach (GameObject wall in castleWall)
			{
				Destroy(wall);
			}
			foreach (Cell cell in castleWallCell)
			{
				cell.wall = null;
				cell.color = Color.clear;
			}
			mark = false;
			transform.GetChild(0).gameObject.SetActive(false);
		}
	}
	public override void Place(Cell newCell)
	{
		base.Place(newCell);

		int triggerOffset = curCell.boardPos.x < 4 ? 2 : -1;
		castleTriggerCell = SetCell(triggerOffset);

		int castlOffset = curCell.boardPos.x < 4 ? 3 : -2;
		castleCell = SetCell(castlOffset);

	}

	public void Castle() 
	{
		targetCell = castleCell;

		Move();
		if(race.Equals("Human"))
			BuildWall();
	}

	private void BuildWall() 
	{
		Color32 wall = new Color32(162, 115, 95, 255);
		if (castleCell.boardPos.x < 4)
			for (int i = castleCell.boardPos.x; i >= 0; i--) 
			{
				pieceManager.CreateCellHitBox(i, castleCell.boardPos.y, wall, castleWall, color);
				Cell cell = curCell.board.AllCells[i, castleCell.boardPos.y];
				castleWallCell.Add(cell);
			}
		else
			for (int i = castleCell.boardPos.x; i < 8; i++)
			{
				pieceManager.CreateCellHitBox(i, castleCell.boardPos.y, wall, castleWall, color);
				Cell cell = curCell.board.AllCells[i, castleCell.boardPos.y];
				castleWallCell.Add(cell);
			}
		mark = true;
		//mark gameobject is child 0
		transform.GetChild(0).gameObject.SetActive(true);
	}

	private Cell SetCell(int offset) 
	{
		Vector2Int newPos = curCell.boardPos;
		newPos.x += offset;

		return curCell.board.AllCells[newPos.x, newPos.y];
	}
}
