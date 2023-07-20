using UnityEngine;
using UnityEngine.UI;
public class Pawn : BasePiece
{

	public override void Setup(Color teamColor, Color32 newSpriteColor, PieceManager pieceManager, string race)
	{
		base.Setup(teamColor, newSpriteColor, pieceManager, race);
		movement = color == Color.white ? new Vector3Int(0, 1, 1) : new Vector3Int(0, -1, -1);
		GetComponent<Image>().sprite = Resources.Load<Sprite>("Pawn");
	}

	protected override void Move()
	{
		base.Move();

		CheckForPromotion();
	}

	public override void CheckPath()
	{
		if (curCell == null)
			return;
		int curX = curCell.boardPos.x;
		int curY = curCell.boardPos.y;

		//Top left
		MatchesState(curX - movement.z, curY + movement.z, CellState.Enemy);

		//Top right
		MatchesState(curX + movement.z, curY + movement.z, CellState.Enemy);

		//Forward
		if (MatchesState(curX, curY + movement.y, CellState.Free))
			if (firstMove)
				MatchesState(curX, curY + movement.y * 2, CellState.Free);

	}

	private bool MatchesState(int targetX, int targetY, CellState targetState)
	{
		CellState cellState = curCell.board.ValidateCell(targetX, targetY, this);

		if (cellState == targetState)
		{
			if (!curCell.board.AllCells[targetX, targetY].transform.GetChild(2).gameObject.activeSelf)
				highlightCells.Add(curCell.board.AllCells[targetX, targetY]);
			return true;
		}
		return false;
	}

	private void CheckForPromotion()
	{
		int curX = curCell.boardPos.x;
		int curY = curCell.boardPos.y;

		CellState cellState = curCell.board.ValidateCell(curX, curY + movement.y, this);

		if (cellState == CellState.OutOfBound)
		{
			//show promotion
			Promote(false);
		}
	}

	public void Promote(bool isDevil)
	{
		if (isDevil)
		{
			pieceManager.promotionSelection.transform.GetChild(4).gameObject.SetActive(true);
			if (pieceManager.gameManager.numberOfPieceTaken < 5)
				pieceManager.promotionSelection.transform.GetChild(2).gameObject.SetActive(false);
			if (pieceManager.gameManager.numberOfPieceTaken < 9)
				pieceManager.promotionSelection.transform.GetChild(3).gameObject.SetActive(false);
		}
		else
		{
			for (int i = 2; i < pieceManager.promotionSelection.transform.childCount; i++)
			{
				pieceManager.promotionSelection.transform.GetChild(i).gameObject.SetActive(true);
			}
			pieceManager.promotionSelection.transform.GetChild(4).gameObject.SetActive(false);
		}
		pieceManager.promotionSelection.SetActive(true);
		pieceManager.promotionSelection.GetComponent<PromotionSlider>().Setup(GetComponent<Image>().color, this);
		pieceManager.SwitchSide(Color.red);
	}
}
