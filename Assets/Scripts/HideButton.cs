using UnityEngine;
using UnityEngine.EventSystems;
public class HideButton : MonoBehaviour, IPointerExitHandler
{
	[HideInInspector]
	public bool notInPiece = false;
	public GameObject button;
	[HideInInspector]
	public Cell mountainCell = null;
	private void FixedUpdate()
	{
		if (Input.GetMouseButtonDown(0) && notInPiece)
		{
			if (button.transform.childCount > 1)
			{
				for (int i = 0; i < button.transform.childCount; i++)
				{
					button.transform.GetChild(i).gameObject.SetActive(false);
				}
			}
			else if (button.activeSelf)
			{
				button.SetActive(false);
			}
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		notInPiece = true;
	}
}
