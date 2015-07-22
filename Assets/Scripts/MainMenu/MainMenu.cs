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

    [SerializeField] int randomCharacterCost = 100;
    string coinsText = " coins";
    [SerializeField] Text costLabel;
    [SerializeField] GameObject treasureChest;

    GameObject currentCharacterGameObject;

	public static MainMenu Instance;


	void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		SetActiveAllButtons(true);
        LoadCharacter();

        costLabel.text = randomCharacterCost.ToString() + coinsText;

        if (!HaveAvailibleCharacters())
        {
            NothingToBuy();
        }

        treasureChest.SetActive(AdvertisingController.Instance.NeedToShowChestInMainMenu());
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
            //follower.SetTarget(characterGO.transform);
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
			if (status == PlayerStatus.NotBought)
			{
				canBuy.Add(i);
			}
		}

		if (canBuy.Count == 0)
		{
            NothingToBuy();
            return;
		}

        if (BankController.coins < randomCharacterCost)
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

        BankController.RemoveCoins(randomCharacterCost);

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

}
