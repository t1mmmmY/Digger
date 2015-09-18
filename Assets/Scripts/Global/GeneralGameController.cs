using UnityEngine;
using System.Collections;

public class GeneralGameController : BaseSingleton<GeneralGameController> 
{
	public AudioSource music;
//	Character currentCharacter;
	int currentCharacterNumber = 0;
    string muteAudioKey = "MUTE_AUDIO";

	public float goldMultiplier { get; private set; }
	
    //Object backgroundMusic;

	public int characterNumber
	{
		get { return currentCharacterNumber; }
	}

    public bool isMusicPlaying
    {
        get
        {
            if (!PlayerPrefs.HasKey(muteAudioKey))
            {
                PlayerPrefs.SetInt(muteAudioKey, 1);
            }
            return PlayerPrefs.GetInt(muteAudioKey) == 1 ? true : false;
        }
    }

    int targetMusicVolume = 0;

	public static System.Action onLoadLobby;
	public static System.Action<int> onSelectCharacter;

	void Start()
	{
		//Application.targetFrameRate = 30;
		BankController.Init();
		currentCharacterNumber = PlayerStatsController.Instance.GetCurrentPlayerNumber();
	}

	void OnEnable()
	{
		SplashScreen.OnLogoComplite += OnLogoComplite;
        
        int volume = 1;
        if (!PlayerPrefs.HasKey(muteAudioKey))
        {
            PlayerPrefs.SetInt(muteAudioKey, volume);
        }
        volume = PlayerPrefs.GetInt(muteAudioKey);
        SetVolume(volume);
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

	public void SetBonus(float multiplier)
	{
		goldMultiplier = multiplier;
	}

//	public BonusCharacter GetBonusCharacter()
//	{
//		return bonusCharacter;
//	}
//	
//	public void SetBonusCharacter(BonusCharacter bonusCharacter)
//	{
//		this.bonusCharacter = bonusCharacter;
//	}

    public void MuteAudio(bool isPlaying)
    {
        int volume = isPlaying == true ? 1 : 0;

        PlayerPrefs.SetInt(muteAudioKey, volume);
        targetMusicVolume = volume;
        SetVolumeAtSeconds(1.0f);
    }

    void SetVolumeAtSeconds(float seconds)
    {
        StopCoroutine("SetVolumeCoroutine");
        StartCoroutine("SetVolumeCoroutine", seconds);
    }

    IEnumerator SetVolumeCoroutine(float time)
    {
        float startVolume = AudioListener.volume;
        float elapsedTime = 0;
        do
        {
            AudioListener.volume = Mathf.Lerp(startVolume, targetMusicVolume, elapsedTime / time);
            
            yield return null;
            elapsedTime += Time.deltaTime;

        } while (AudioListener.volume != targetMusicVolume);

        //Debug.LogWarning("DONE");
    }

    void SetVolume(int volume)
    {
        AudioListener.volume = volume;
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

//		MultiplayerController.Instance.TrySilentSignIn();
	}


	void LoadMusicAndPlay()
	{
		StartCoroutine(LoadAsset(CONST.MUSIC_PATH, typeof(AudioClip), PlayMusic));
	}

	void PlayMusic(Object clip)
	{
		if (clip != null)
		{

			music.clip = (AudioClip)clip;
			if (music.clip != null)
			{
				music.Play();
			}
			else
			{
			}
		}
	}

	IEnumerator LoadAsset(string path, System.Type type, System.Action<Object> callback)
	{
		ResourceRequest request = Resources.LoadAsync(path, type);

		yield return request;

		if (request.isDone)
		{
            //backgroundMusic = request.asset;
			if (callback != null)
			{
				callback(request.asset);
			}
		}
	}

}
