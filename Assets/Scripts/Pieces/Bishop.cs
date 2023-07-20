using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bishop : BasePiece
{
	public override void Setup(Color teamColor, Color32 newSpriteColor, PieceManager pieceManager, string race)
	{
		base.Setup(teamColor, newSpriteColor, pieceManager, race);

		movement = new Vector3Int(0, 0, 7);
		GetComponent<Image>().sprite = Resources.Load<Sprite>("Bishop");
	}

	protected override void Move() 
	{
		base.Move();
		if (race.Equals("Angel"))
			AngelSkill();
	}
	public override void Slowed(int num)
	{
		base.Slowed(num);
		movement = new Vector3Int(0, 0, num);
	}

	private void AngelSkill() 
	{
		if (!curCell.transform.GetChild(1).gameObject.activeSelf)
		{
			curCell.transform.GetChild(1).gameObject.SetActive(true);
			pieceManager.buffCells.Add(curCell);
		}
	}
}
