using UnityEngine;
using System.Collections;

public class GeneralGameController : MonoBehaviour 
{
	public AudioSource music;

	void Start()
	{
		DontDestroyOnLoad(this.gameObject);
		Application.targetFrameRate = 30;
	}

	void OnEnable()
	{
		SplashScreen.OnLogoComplite += OnLogoComplite;
	}

	void OnDisable()
	{
		SplashScreen.OnLogoComplite -= OnLogoComplite;
	}

	void OnLogoComplite()
	{
		LevelLoader.Instance.LoadLevel(Scene.Lobby, OnLoadLobby);
	}

	void OnLoadLobby()
	{
		music.Play();
		MultiplayerController.Instance.SignIn();
	}
}
