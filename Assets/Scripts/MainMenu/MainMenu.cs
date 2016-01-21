using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour 
{
	[SerializeField] Button[] allButtons;
	public uint timeout = 20;
	[SerializeField] Animator mainMenuAnimator;
    [SerializeField] Transform characterStartPosition;

    [SerializeField] Animator tavernAnimator;
    [SerializeField] Transform tavernStartPosition;

    
    string coinsText = " coins";
    [SerializeField] Text costLabel;
    [SerializeField] GameObject treasureChest;
    [SerializeField] Text bestScoreLabel;
    [SerializeField] Grammophone grammophone;
	[SerializeField] GameObject loginButton;
	
    GameObject currentCharacterGameObject;
    bool isMusicPlaying = true;


	public static MainMenu Instance;


	void Awake()
	{
		Instance = this;
	}

	void OnEnable()
	{
		GeneralGameController.onLogin += OnSignInCallback;
	}

	void OnDisable()
	{
		GeneralGameController.onLogin -= OnSignInCallback;
	}


	void Start()
	{
		if (GeneralGameController.Instance != null)
		{
        	isMusicPlaying = GeneralGameController.Instance.isMusicPlaying;
		}
		SetActiveAllButtons(true);
        LoadCharacter();

        costLabel.text = CONST.RANDOM_CHARACTER_COST.ToString() + coinsText;

        if (!HaveAvailibleCharacters())
        {
            NothingToBuy();
        }

		if (AdvertisingController.Instance != null)
		{
        	treasureChest.SetActive(AdvertisingController.Instance.NeedToShowChestInMainMenu());
		}
        bestScoreLabel.text = PlayerPrefs.GetInt("BestLevel", 0).ToString();

		if (GeneralGameController.Instance.isLoginSuccess)
		{
			HideLoginButton();
		}
	}

    private void LoadCharacter()
    {
        int characterNumber = 0;
        if (GeneralGameController.Instance != null)
        {
            characterNumber = GeneralGameController.Instance.characterNumber;
        }

        Object obj = Resources.Load(string.Format("{0}{1}", CONST.PLAYABLE_PLAYERS_PATH, CONST.PLAYER_NAMES[characterNumber]));
        if (obj != null)
        {
            currentCharacterGameObject = GameObject.Instantiate<GameObject>((GameObject)obj);
            currentCharacterGameObject.transform.parent = characterStartPosition;
            currentCharacterGameObject.transform.localPosition = Vector3.zero;

            currentCharacterGameObject.GetComponent<Rigidbody>().useGravity = false;
        }
        else
        {
            Debug.LogError("Cannot load character!");
        }
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

#if UNITY_ANDROID
//		MultiplayerController.Instance.StartMatchMakingRealTime();
#elif UNITY_IOS

#endif
	}

	public void StartFastMultiplayerGame()
	{
		SetActiveAllButtons(false);
		mainMenuAnimator.SetBool("ButtonsVisible", false);
#if UNITY_ANDROID
//		MultiplayerController.Instance.StartMatchMakingRealTimeFast();
#elif UNITY_IOS

#endif
	}

	public void SignInToGoogle()
	{
#if UNITY_ANDROID
		MultiplayerController.Instance.SignIn(OnSignInCallback);
#elif UNITY_IOS
		MultiplayerController.Instance.SignIn(OnSignInCallback);
#endif
	}

	private void OnSignInCallback(bool isSuccess)
	{
		if (true)
		{
			HideLoginButton();
		}
		else
		{
			loginButton.SetActive(true);
		}
	}

	private void HideLoginButton()
	{
		loginButton.SetActive(false);
	}


	public void ShowLeaderboard()
	{
#if UNITY_ANDROID
		MultiplayerController.Instance.ShowLeaderboard();
#elif UNITY_IOS
		MultiplayerController.Instance.ShowLeaderboard();
#endif
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
	}

	public void ReturnToMenu()
	{
	}

	public void EnterTavern()
	{
		LevelLoader.Instance.LoadLevel(Scene.Tavern);
	}
	
	public void BuyRandomCharacter()
	{
        if (PlayerStatsController.Instance == null)
        {
            NothingToBuy();
            return;
        }

		List<int> canBuy = new List<int>();
		for (int i = 0; i < CONST.PLAYER_KEYS.Length; i++)
		{
			PlayerStatus status = PlayerStatsController.Instance.GetStatus(i);
			if (status == PlayerStatus.NotBought && !CONST.IsSpecialCharacter(i))
			{
				canBuy.Add(i);
			}
		}

		if (canBuy.Count == 0)
		{
            NothingToBuy();
            return;
		}

        if (BankController.coins < CONST.RANDOM_CHARACTER_COST)
        {
            NotEnoughMoney();
            return;
        }

		int randomNumber = Random.Range(0, canBuy.Count);
		int randomCharacterNumber = canBuy[randomNumber];

		if (GeneralGameController.Instance != null)
		{
			GeneralGameController.Instance.SelectCharacter(randomCharacterNumber);
		}


        Destroy(currentCharacterGameObject);
        LoadCharacter();
        currentCharacterGameObject.transform.parent = tavernStartPosition;
        currentCharacterGameObject.transform.position = Vector3.zero;
        tavernAnimator.SetTrigger("ShowNewUser");

        mainMenuAnimator.SetTrigger("BuyRandomCharacter");

        BankController.RemoveCoins(CONST.RANDOM_CHARACTER_COST);

        TavernAnimationScript.onEndAnimation += OnEndShowAnimation;
	}

    public void ShowAdvertisment()
    {
        AdvertisingController.onShowAdvertising += OnShowAdvertising;
        AdvertisingController.Instance.ShowAdvertisement();
    }

    void OnShowAdvertising(int reward)
    {
        AdvertisingController.onShowAdvertising -= OnShowAdvertising;
        treasureChest.SetActive(false);
    }

    bool HaveAvailibleCharacters()
    {
        if (PlayerStatsController.Instance == null)
        {
            return false;
        }

        List<int> canBuy = new List<int>();
        for (int i = 0; i < CONST.PLAYER_KEYS.Length; i++)
        {
            PlayerStatus status = PlayerStatsController.Instance.GetStatus(i);
            if (status == PlayerStatus.NotBought)
            {
                canBuy.Add(i);
            }
        }

        if (canBuy.Count == 0)
        {
            return false;
        }

        return true;
    }

    void OnEndShowAnimation()
    {
        TavernAnimationScript.onEndAnimation -= OnEndShowAnimation;
        currentCharacterGameObject.transform.parent = characterStartPosition;
        currentCharacterGameObject.transform.position = Vector3.zero;
        currentCharacterGameObject.transform.localRotation = Quaternion.identity;
    }

    void NothingToBuy()
    {
        Debug.LogWarning("Nothing to buy!");
        mainMenuAnimator.SetTrigger("NothingToBuy");
    }

    void NotEnoughMoney()
    {
        Debug.LogWarning("Not enough money!");
        mainMenuAnimator.SetTrigger("NotEnoughMoney");
    }

    public void MuteAudio()
    {
        isMusicPlaying = !isMusicPlaying;
        GeneralGameController.Instance.MuteAudio(isMusicPlaying);
        grammophone.MuteAudio(isMusicPlaying);
    }
}
