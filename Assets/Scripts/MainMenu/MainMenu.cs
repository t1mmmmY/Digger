using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour 
{

	void Start()
	{
		MultiplayerController.Instance.SignInAndStartMPGame();
	}

	public void StartSingleGame()
	{
		LevelLoader.Instance.LoadLevel(1);
	}

	public void StartMultiplayerGame()
	{
		MultiplayerController.Instance.StartMatchMakingRealTime();
//		LevelLoader.Instance.LoadLevel(1);
	}

	public void StartTurnBasedMultiplayerGame()
	{
		MultiplayerController.Instance.StartMatchMakingTurnBased();
		//		LevelLoader.Instance.LoadLevel(1);
	}

	public void SignInToGoogle()
	{
		MultiplayerController.Instance.SignInAndStartMPGame();
	}

	public void ShowLeaderboard()
	{
		MultiplayerController.Instance.ShowLeaderboard();
	}

}
