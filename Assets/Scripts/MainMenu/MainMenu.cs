using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour 
{
	public Button[] allButtons;
	public uint timeout = 20;

	public static MainMenu Instance;

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		Application.targetFrameRate = 30;
		MultiplayerController.Instance.SignIn();
		SetActiveAllButtons(true);
	}

	public void StartSingleGame()
	{
		SetActiveAllButtons(false);
		LevelLoader.Instance.LoadLevel(1);
	}

	public void StartMultiplayerGame()
	{
		SetActiveAllButtons(false);
		MultiplayerController.Instance.StartMatchMakingRealTime();
	}

	public void StartFastMultiplayerGame()
	{
		SetActiveAllButtons(false);
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
	 

	public void StartTimerTimeout()
	{
		Debug.LogWarning("TimerStart");
		StartCoroutine(TimeoutCoroutine(EndTimerTimeout));
	}
	
	void EndTimerTimeout()
	{
		Debug.LogWarning("TimerEnd");
		SetActiveAllButtons(true);
	}
	
	IEnumerator TimeoutCoroutine(System.Action callback)
	{
		yield return new WaitForSeconds(timeout);
		
		if (callback != null)
		{
			callback();
		}
	}

}
