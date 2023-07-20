using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Description : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public Text description;

	public void OnPointerEnter(PointerEventData pointerEventData)
	{
		description.text = GetDescription();
	}

	private string GetDescription()
	{
		switch (name)
		{
			case "Human":
				return "Until the castled rook died, castling rendered the tile beneath impassable";
			case "Devil":
				return "After a certain amount of pieces has been taken from both sides, you may be able to promote";
			case "Goblin":
				return "Your pawn can advance 2 tiles again after the third turn";
			case "Golem":
				return "When one pawn is promoted, all pawns are promoted as well";
			case "Giant":
				return "After your fifth turn, you can use your skill to remove all opponent pieces that one tile surrounds you";
			case "Angel":
				return "The tile under your bishop is blessed, and the piece that goes to it gains an extra move";
			case "Dwarf":
				return "Your pieces have the ability to pass across mountains";
			case "Elf":
				return "Your pieces are invisible around the mountain. Invisibility will be removed when an enemy is nearby";
			case "Orc":
				return "When your pieces are placed around a mountain, they can destroy it. Your ability has a four-turn cooldown";
			case "Spirit":
				return "You can make an obstacle by sacrificing a non-king piece";
			default:
				break;
		}
		return name;
	}
	public void OnPointerExit(PointerEventData pointerEventData)
	{
		description.text = "";
	}
}
