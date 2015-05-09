using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour 
{
	public Button[] allButtons;
	public uint timeout = 20;
	[SerializeField] Animator mainMenuAnimator;

	public static MainMenu Instance;

//	bool _isStopLoading = false;
//	public bool isStopLoading
//	{
//		get { return _isStopLoading; }
//	}

	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
//		Application.targetFrameRate = 30;
//		MultiplayerController.Instance.SignIn();
		SetActiveAllButtons(true);
	}

	public void StartSingleGame()
	{
		SetActiveAllButtons(false);
		LevelLoader.Instance.LoadLevel(Scene.SinglePlayer);
	}

	public void StartMultiplayerGame()
	{
//		_isStopLoading = false;
		SetActiveAllButtons(false);
		mainMenuAnimator.SetBool("ButtonsVisible", false);
		MultiplayerController.Instance.StartMatchMakingRealTime();
	}

	public void StartFastMultiplayerGame()
	{
//		_isStopLoading = false;
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
	 

	public void StartTimerTimeout()
	{
//		Debug.LogWarning("TimerStart");
//		StartCoroutine(TimeoutCoroutine(EndTimerTimeout));
	}

	public void StopTimer()
	{
		mainMenuAnimator.SetBool("ButtonsVisible", true);
//		_isStopLoading = true;
		SetActiveAllButtons(true);
//		StopCoroutine(TimeoutCoroutine(EndTimerTimeout));
	}
	
//	void EndTimerTimeout()
//	{
//		Debug.LogWarning("TimerEnd");
//		SetActiveAllButtons(true);
//	}
	
//	IEnumerator TimeoutCoroutine(System.Action callback)
//	{
//		yield return new WaitForSeconds(timeout);
//
//		if (_isStopLoading)
//		{
//			yield break;
//		}
//		else
//		{
//			Debug.Log("CALLBACK");
//			if (callback != null)
//			{
//				callback();
//			}
//		}
//	}

}
