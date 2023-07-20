using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Queen : BasePiece
{
	public override void Setup(Color teamColor, Color32 newSpriteColor, PieceManager pieceManager, string race)
	{
		base.Setup(teamColor, newSpriteColor, pieceManager, race);

		movement = new Vector3Int(7, 7, 7);
		GetComponent<Image>().sprite = Resources.Load<Sprite>("Queen");
	}
	public override void Slowed(int num)
	{
		base.Slowed(num);
		movement = new Vector3Int(num, num, num);
	}
}
