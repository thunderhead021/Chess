using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public Board board;
	public PieceManager pieceManager;
	public GameObject playerSideUI;
	public GameObject enemySideUI;
	public GameObject playerSelectText;
	public GameObject enemySelectText;
	public GameObject selectGUI;
	public GameObject devil;
	public GameObject activeSkillTimer;
	public GameObject button;
	public GameObject createBoardBtn;
	[HideInInspector]
	public int numberOfPieceTaken = 0;
	public Text describ1;
	public Text describ2;

	private Color playerSide = new Color32(80, 124, 159, 255);
	private Color enemySide = new Color32(210, 95, 64, 255);
	// Start is called before the first frame update
	void Start()
	{
		AddClickEvent();
		board.CreateBoard();
	}

	public void AddNumOfPieceTaken()
	{
		numberOfPieceTaken++;
		//3 bishop and knight 5 rook 9 queen
		if (numberOfPieceTaken < 3)
			return;

		if (playerSelectText.GetComponent<Text>().text.Contains("Devil"))
		{
			GameObject child = devil.transform.GetChild(0).gameObject;
			DevilHelper(child);
		}

		if (enemySelectText.GetComponent<Text>().text.Contains("Devil"))
		{
			GameObject child = devil.transform.GetChild(1).gameObject;
			DevilHelper(child);
		}
	}

	public void SubNumOfPieceTaken(int num)
	{
		numberOfPieceTaken -= num;
		print(numberOfPieceTaken);
		//3 bishop and knight 5 rook 9 queen
		if (playerSelectText.GetComponent<Text>().text.Contains("Devil"))
		{
			DevilHelper2(devil.transform.GetChild(0).gameObject);

		}

		if (enemySelectText.GetComponent<Text>().text.Contains("Devil"))
		{
			DevilHelper2(devil.transform.GetChild(1).gameObject);

		}
	}

	private void DevilHelper2(GameObject devilSide)
	{
		if (numberOfPieceTaken < 3)
		{
			devilSide.SetActive(false);
			return;
		}
		if (numberOfPieceTaken < 9)
			devilSide.transform.GetChild(0).transform.GetChild(3).gameObject.SetActive(false);
		if (numberOfPieceTaken < 5)
			devilSide.transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(false);
	}

	private void DevilHelper(GameObject devilSide)
	{
		devilSide.SetActive(true);
		if (numberOfPieceTaken == 5)
		{
			devilSide.transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(true);
			return;
		}

		if (numberOfPieceTaken == 9)
		{
			devilSide.transform.GetChild(0).transform.GetChild(3).gameObject.SetActive(true);
			return;
		}
	}

	void AddClickEvent()
	{
		foreach (Transform child in playerSideUI.transform)
		{
			Button btn = child.gameObject.GetComponent<Button>();
			btn.onClick.AddListener(() => SelectType(btn, "Player"));
		}

		foreach (Transform child in enemySideUI.transform)
		{
			Button btn = child.gameObject.GetComponent<Button>();
			btn.onClick.AddListener(() => SelectType(btn, "Enemy"));
		}

		Button returnPlayer = playerSelectText.GetComponentInChildren<Button>();
		returnPlayer.onClick.AddListener(() => ReturnBtn("Player"));

		Button returnEnemy = enemySelectText.GetComponentInChildren<Button>();
		returnEnemy.onClick.AddListener(() => ReturnBtn("Enemy"));
		button.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => SkipTurnBtn(Color.white));
		button.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => SkipTurnBtn(Color.black));
	}

	public void DeactiveSkipBtn()
	{
		button.transform.GetChild(0).gameObject.SetActive(false);
		button.transform.GetChild(1).gameObject.SetActive(false);
	}

	private void SkipTurnBtn(Color side)
	{
		pieceManager.SwitchSide(side);
		DeactiveSkipBtn();
	}

	void SelectType(Button buttonClicked, string side)
	{
		if (side.Equals("Player"))
		{
			playerSide = buttonClicked.GetComponent<Image>().color;
			playerSideUI.SetActive(false);
			playerSelectText.SetActive(true);
			playerSelectText.GetComponent<Text>().text = buttonClicked.name + " selected";
			if (BothSideReady())
				createBoardBtn.SetActive(true);
		}
		if (side.Equals("Enemy"))
		{
			enemySide = buttonClicked.GetComponent<Image>().color;
			enemySideUI.SetActive(false);
			enemySelectText.SetActive(true);
			enemySelectText.GetComponent<Text>().text = buttonClicked.name + " selected";
			if (BothSideReady())
				createBoardBtn.SetActive(true);
		}
	}
	private bool BothSideReady()
	{
		return playerSelectText.gameObject.activeSelf && enemySelectText.gameObject.activeSelf;
	}
	public void CreateBoardBtn()
	{
		pieceManager.Setup(board, playerSide, enemySide);
		selectGUI.SetActive(false);
		pieceManager.SwitchSide(Color.red);
		button.transform.GetChild(2).gameObject.SetActive(true);
	}

	public void RestartBoard()
	{
		pieceManager.isGameContinue = false;
		pieceManager.SwitchSide(Color.black);
	}

	public void NewBoard()
	{
		for (int i = 0; i < pieceManager.transform.childCount; i++)
		{
			Destroy(pieceManager.transform.GetChild(i).gameObject);
		}
		for (int i = 0; i < activeSkillTimer.transform.childCount; i++)
		{
			activeSkillTimer.transform.GetChild(i).gameObject.SetActive(false);
		}
		for (int i = 0; i < devil.transform.childCount; i++)
		{
			devil.transform.GetChild(i).gameObject.SetActive(false);
		}
		playerSelectText.SetActive(false);
		selectGUI.transform.GetChild(2).gameObject.SetActive(false);
		enemySelectText.SetActive(false);
		playerSideUI.SetActive(true);
		enemySideUI.SetActive(true);
		selectGUI.SetActive(true);
		numberOfPieceTaken = 0;
		foreach (Cell cell in pieceManager.obstacleCells) 
		{
			pieceManager.ResetObstacleCells();
		}
		foreach (Cell cell in pieceManager.buffCells)
		{
			cell.transform.GetChild(1).gameObject.SetActive(false);
		}
		pieceManager.buffCells.Clear();
		pieceManager.obstacleCells.Clear();
		describ1.text = "";
		describ2.text = "";
		for (int i = 0; i < button.transform.childCount - 1; i++)
		{
			button.transform.GetChild(i).gameObject.SetActive(false);
		}
	}

	public void QuitGame()
	{
		Application.Quit();
	}

	public void StartBtn()
	{
		pieceManager.CheckObstacleAtStart();
		pieceManager.SwitchSide(Color.black);
		pieceManager.RemoveCellHitBox();
		button.transform.GetChild(2).gameObject.SetActive(false);
		button.transform.GetChild(3).gameObject.SetActive(true);
		button.transform.GetChild(4).gameObject.SetActive(true);
	}
	public void ReturnBtn(string side)
	{
		if (side.Equals("Player"))
		{
			playerSideUI.SetActive(true);
			playerSelectText.SetActive(false);

		}
		if (side.Equals("Enemy"))
		{
			enemySideUI.SetActive(true);
			enemySelectText.SetActive(false);
		}
		createBoardBtn.SetActive(false);
	}
}
