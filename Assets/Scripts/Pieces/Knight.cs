using UnityEngine;
using UnityEngine.UI;

public class Knight : BasePiece
{
	public override void Setup(Color teamColor, Color32 newSpriteColor, PieceManager pieceManager, string race)
	{
		base.Setup(teamColor, newSpriteColor, pieceManager, race);


		GetComponent<Image>().sprite = Resources.Load<Sprite>("Knight");
	}

	private void CreatePath(int flip)
	{
		int curX = curCell.boardPos.x;
		int curY = curCell.boardPos.y;

		//Left
		MatchesState(curX - 2, curY + flip);

		//Upper left
		MatchesState(curX - 1, curY + 2 * flip);

		//Right
		MatchesState(curX + 2, curY + flip);

		//Upper right
		MatchesState(curX + 1, curY + 2 * flip);
	}

	public override void CheckPath()
	{
		CreatePath(1);

		CreatePath(-1);
	}
	private void MatchesState(int targetX, int targetY)
	{
		CellState cellState = curCell.board.ValidateCell(targetX, targetY, this);

		if (cellState != CellState.Friend && cellState != CellState.OutOfBound && cellState != CellState.Mountain) 
		{
			if(!curCell.board.AllCells[targetX, targetY].transform.GetChild(2).gameObject.activeSelf)
				highlightCells.Add(curCell.board.AllCells[targetX, targetY]);
		}
			
	}
}
