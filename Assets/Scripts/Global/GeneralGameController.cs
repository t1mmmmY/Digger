﻿using UnityEngine;
using System.Collections;

public class GeneralGameController : BaseSingleton<GeneralGameController> 
{
	public AudioSource music;
//	Character currentCharacter;
	int currentCharacterNumber = 0;

	public int characterNumber
	{
		get { return currentCharacterNumber; }
	}

	public static System.Action onLoadLobby;
	public static System.Action<int> onSelectCharacter;

	void Start()
	{
		Application.targetFrameRate = 30;
		BankController.Init();
		currentCharacterNumber = PlayerStatsController.Instance.GetCurrentPlayerNumber();
	}

	void OnEnable()
	{
		SplashScreen.OnLogoComplite += OnLogoComplite;
	}


	void OnDisable()
	{
		SplashScreen.OnLogoComplite -= OnLogoComplite;
	}


	public void SelectCharacter(int characterNumber)
	{
		PlayerStatsController.Instance.SetStatus(characterNumber, PlayerStatus.Bought);

//		currentCharacter = character;
		currentCharacterNumber = characterNumber;

		if (onSelectCharacter != null)
		{
			onSelectCharacter(characterNumber);
		}
	}


	void OnLogoComplite()
	{
		LevelLoader.Instance.LoadLevel(Scene.Lobby, OnLoadLobby);
	}

	void OnLoadLobby()
	{
		Debug.Log("OnLoadLobby");
		LoadMusicAndPlay();

		if (onLoadLobby != null)
		{
			onLoadLobby();
		}

		MultiplayerController.Instance.SignIn();
	}

	void LoadMusicAndPlay()
	{
		StartCoroutine(LoadAsset(CONST.MUSIC_PATH, typeof(AudioClip), PlayMusic));
	}

	void PlayMusic(Object clip)
	{
//		Debug.Log("Object == null");
		if (clip != null)
		{
//			Debug.Log("Load something");

			music.clip = (AudioClip)clip;
			if (music.clip != null)
			{
//				Debug.Log("Everything okay");
				music.Play();
			}
			else
			{
//				Debug.Log("music.clip == null");
			}
		}
	}

	IEnumerator LoadAsset(string path, System.Type type, System.Action<Object> callback)
	{
//		Debug.Log("Start loading");
		ResourceRequest request = Resources.LoadAsync(path, type);

		yield return request;

		if (request.isDone)
		{
//			Debug.Log("Done");
			if (callback != null)
			{
				callback(request.asset);
			}
		}
	}

}
