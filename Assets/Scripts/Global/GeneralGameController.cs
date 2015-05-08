using UnityEngine;
using System.Collections;

public class GeneralGameController : MonoBehaviour 
{
	public AudioSource music;

	private static GeneralGameController _instance;
	public static GeneralGameController Instance
	{
		get { return _instance; }
	}

	public static System.Action onLoadLobby;

	void Awake()
	{
		_instance = this;
	}

	void Start()
	{
		Application.targetFrameRate = 30;
		BankController.Init();
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
		Debug.Log("Object == null");
		if (clip != null)
		{
			Debug.Log("Load something");

			music.clip = (AudioClip)clip;
			if (music.clip != null)
			{
				Debug.Log("Everything okay");
				music.Play();
			}
			else
			{
				Debug.Log("music.clip == null");
			}
		}
	}

	IEnumerator LoadAsset(string path, System.Type type, System.Action<Object> callback)
	{
		Debug.Log("Start loading");
		ResourceRequest request = Resources.LoadAsync(path, type);

		yield return request;

		if (request.isDone)
		{
			Debug.Log("Done");
			if (callback != null)
			{
				callback(request.asset);
			}
		}
	}
}
