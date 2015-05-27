using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour 
{
	public Button[] allButtons;
	public uint timeout = 20;
	[SerializeField] Animator mainMenuAnimator;

	public static MainMenu Instance;


	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		SetActiveAllButtons(true);
	}

	public void StartSingleGame()
	{
		SetActiveAllButtons(false);
		LevelLoader.Instance.LoadLevel(Scene.SinglePlayer);
	}

	public void StartMultiplayerGame()
	{
		SetActiveAllButtons(false);
		mainMenuAnimator.SetBool("ButtonsVisible", false);
		MultiplayerController.Instance.StartMatchMakingRealTime();
	}

	public void StartFastMultiplayerGame()
	{
		SetActiveAllButtons(false);
		mainMenuAnimator.SetBool("ButtonsVisible", false);
		MultiplayerController.Instance.StartMatchMakingRealTimeFast();
	}

	public void SignInToGoogle()
	{
		MultiplayerController.Instance.SignIn();
	}

	public void ShowLeaderboard()
	{
		MultiplayerController.Instance.ShowLeaderboard();
	}


	public void SetActiveAllButtons(bool isActive)
	{
		foreach (Button button in allButtons)
		{
			button.interactable = isActive;
		}
	}
	 

	public void StopTimer()
	{
		mainMenuAnimator.SetBool("ButtonsVisible", true);
		SetActiveAllButtons(true);
	}

	public void MoveToShop()
	{
//		SetActiveAllButtons(false);
//		mainMenuAnimator.SetBool("InShop", true);
	}

	public void ReturnToMenu()
	{
//		SetActiveAllButtons(true);
//		mainMenuAnimator.SetBool("InShop", false);
	}


}
