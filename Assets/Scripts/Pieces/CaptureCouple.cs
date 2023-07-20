using System.Collections.Generic;
public class CaptureCouple
{
    public BasePiece attackingPiece;
    public BasePiece attackedPiece;
    public List<Cell> attackDir = new List<Cell>();

    public CaptureCouple(BasePiece attackingPiece, BasePiece attackedPiece, List<Cell> attackDir)
	{
		this.attackingPiece = attackingPiece;
		this.attackedPiece = attackedPiece;
		this.attackDir = attackDir;
	}

	public bool IsThisPieceTheAttackedPiece(BasePiece piece) 
	{
		return attackedPiece.gameObject.name.Equals(piece.gameObject.name);
	}
}
