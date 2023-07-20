using UnityEngine;
using UnityEngine.UI;


public class Cell : MonoBehaviour
{
	public Image outlineImage;

	[HideInInspector]
	public Vector2Int boardPos = Vector2Int.zero;
	[HideInInspector]
	public Board board = null;
	internal BasePiece curPiece;
	internal GameObject wall;
	internal Color color = Color.clear;
	[HideInInspector]
	public RectTransform rectTransform = null;

	private bool CheckSurrounding(int xDir, int yDir, BasePiece piece)
	{
		int curX = boardPos.x;
		int curY = boardPos.y;

		curX += xDir;
		curY += yDir;

		CellState cellState = board.ValidateCell(curX, curY, piece);
		return cellState == CellState.Friend;
	}

	public bool FindFriend(BasePiece piece)
	{
		if (CheckSurrounding(1, 0, piece))
			return true;
		if (CheckSurrounding(-1, 0, piece))
			return true;
		if (CheckSurrounding(0, 1, piece))
			return true;
		if (CheckSurrounding(0, -1, piece))
			return true;
		if (CheckSurrounding(1, 1, piece))
			return true;
		if (CheckSurrounding(-1, 1, piece))
			return true;
		if (CheckSurrounding(-1, -1, piece))
			return true;
		if (CheckSurrounding(1, -1, piece))
			return true;

		return false;
	}
	public void Setup(Vector2Int boardPos, Board newBoard)
	{
		this.boardPos = boardPos;
		board = newBoard;
		outlineImage.enabled = false;
		rectTransform = GetComponent<RectTransform>();

	}

	public void RemovePiece()
	{
		if (curPiece != null)
		{
			curPiece.Kill();
			board.GameManager.AddNumOfPieceTaken();
		}
	}
}
