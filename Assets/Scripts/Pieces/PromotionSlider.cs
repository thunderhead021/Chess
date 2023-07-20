using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PromotionSlider : MonoBehaviour
{
	public List<Image> allChild;
	private Pawn pawn;
	private RectTransform rect;
	public void Setup(Color32 SpriteColor, Pawn promotePawn)
	{
		pawn = promotePawn;
		rect = gameObject.transform.GetComponent<RectTransform>();
		rect.anchoredPosition = new Vector2(0, 0);
		foreach (Image child in allChild)
		{
			child.color = SpriteColor;
		}
	}

	public void CancleBtn()
	{
		rect.anchoredPosition = new Vector2(-1000, 0);
		gameObject.SetActive(false);
		pawn.pieceManager.SwitchSide(pawn.color == Color.white ? Color.black : Color.white);
		TurnOnAllChild();
	}

	public void OnPointerClick()
	{
		var type = EventSystem.current.currentSelectedGameObject.name;
		pawn.pieceManager.PromotePiece(pawn, pawn.curCell, pawn.color, pawn.GetComponent<Image>().color, getType(type), transform.GetChild(4).gameObject.activeSelf);
		rect.anchoredPosition = new Vector2(-1000, 0);
		this.gameObject.SetActive(false);
		TurnOnAllChild();
	}

	private void TurnOnAllChild() 
	{
		for (int i = 0; i < 4; i++) 
		{
			transform.GetChild(i).gameObject.SetActive(true);
		}
	}

	private Type getType(string type)
	{
		Type result = null;
		switch (type)
		{
			case "Bishop":
				result = typeof(Bishop);
				break;
			case "Knight":
				result = typeof(Knight);
				break;
			case "Queen":
				result = typeof(Queen);
				break;
			case "Rook":
				result = typeof(Rook);
				break;
			default:
				result = null;
				break;

		}
		return result;
	}
}
