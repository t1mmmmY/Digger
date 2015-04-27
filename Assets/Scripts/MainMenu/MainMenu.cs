using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour 
{

	void Start()
	{
		MultiplayerController.Instance.TrySilentSignIn();
	}

	public void StartGame()
	{
		LevelLoader.Instance.LoadLevel(1);
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
