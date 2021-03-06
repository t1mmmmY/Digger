﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EndGamePanel : MonoBehaviour 
{
    [SerializeField] GameObject tavernKeeperGameObject;
    [SerializeField] GameObject chestGameObject;
	[SerializeField] Animator animator;
	[SerializeField] Text[] titleTexts;
	[SerializeField] Text cheaterText;
    [SerializeField] Text chestText;
	[SerializeField] int[] scoreLevels;
	[SerializeField] Text coinsCountText;
	[SerializeField] Text levelText;
    [SerializeField] Button[] adsButton;

    [SerializeField] Button[] buyRandomCharacterButtons;
    [SerializeField] Text buyRandomCharacterText;
    [SerializeField] Text buyRandomCharacterCost;

	[SerializeField] Button[] buyCharacterButtons;
	[SerializeField] GameObject buyCharacterContainer;
	[SerializeField] Transform buyCharacterAnchor;
	[SerializeField] Text buyCharacterText;
	[SerializeField] Text buyCharacterCost;
	int buyCharacterNumber = 0;


	int showPanelAnimationHash = Animator.StringToHash("ShowPanel");
    int hideChestHash = Animator.StringToHash("HideChest");

    void Start()
    {
    }

    public void ShowAds()
    {
        AdvertisingController.onShowAdvertising += OnShowAdvertising;
        AdvertisingController.Instance.ShowAdvertisement();
    }

    public void BuyRandomCharacter()
    {
        ShopInGame.Instance.BuyRandomCharacter();
        BankController.RemoveCoins(CONST.RANDOM_CHARACTER_COST);
        SingleplayerGameManager.Instance.RestartGame();
        //Debug.Log("Buy");
    }

	public void BuyCharacter()
	{
////		InGameStore.Instance.BuyProduct(buyCharacterNumber, );
		ShopInGame.Instance.BuyCharacter(buyCharacterNumber, OnTransacionFinished);
////		BankController.RemoveCoins(CONST.RANDOM_CHARACTER_COST);
//		SingleplayerGameManager.Instance.RestartGame();
		//Debug.Log("Buy");
	}

	void OnTransacionFinished(bool isSuccess)
	{
		if (isSuccess)
		{
			SingleplayerGameManager.Instance.RestartGame();
			if (GeneralGameController.Instance != null)
			{
				GeneralGameController.Instance.SelectCharacter(buyCharacterNumber);
			}
		}
		else
		{
		}
	}

    void OnShowAdvertising(int reward)
    {
        //StartCoroutine(AddCoins(reward, System.Convert.ToInt32(coinsCountText.text)));
        //coinsCountText.text = (System.Convert.ToInt32(coinsCountText.text) + reward).ToString();
        AdvertisingController.onShowAdvertising -= OnShowAdvertising;
        animator.SetTrigger(hideChestHash);
        ShowEndGamePanel(false, reward, System.Convert.ToInt32(levelText.text), false);
    }

    IEnumerator AddCoins(int coins, int oldPlayerCoins)
    {
        float coinsAddAnimationTime = 0.5f;
        float coinsAddDelay = coinsAddAnimationTime / coins;
        float elapsedTime = 0;

        do
        {
            yield return null;
            elapsedTime += Time.deltaTime;

            if (elapsedTime > coinsAddAnimationTime)
            {
                elapsedTime = 1;
            }

            coinsCountText.text = (Mathf.RoundToInt(oldPlayerCoins + coins * elapsedTime)).ToString();

        } while (elapsedTime < coinsAddAnimationTime);

        //		playerCoins += coins;

        coinsCountText.text = (oldPlayerCoins + coins).ToString();

    }

	public void ShowEndGamePanel(bool isRecord, int coinsCount, int level, bool animate = true)
	{
		if (isRecord)
		{
            //It is a new record
            ShowRecord(animate);
		}
		else
		{
            if (ShopInGame.Instance.NeedToShowProposalInGame(BankController.coins - coinsCount, false))
            {
                //Show proposal to buy a random character
                ShowProposalToBuyRandomCharacter(animate);
            }
            else
            {
				if (ShopInGame.Instance.NeedToShowProposalInGame(BankController.coins - coinsCount, true))
				{
					//Show proposal to buy a character for real money
					buyCharacterNumber = ShopInGame.Instance.GetRandomCharacterNumber(true);
					if (!ShowProposalToBuyCharacterForRealMoney(buyCharacterNumber, animate))
					{
						ShowSomeLabel(level, animate);
						Debug.Log("Show default label because of not connection");
					}
				}
				else
				{
					if (AdvertisingController.Instance != null && AdvertisingController.Instance.NeedToShowChestInGame())
	                {
	                    //Show advertising
	                    ShowAdvertising(animate);
	                }
	                else
	                {
	                    //Show some label with status
	                    ShowSomeLabel(level, animate);
	                }
				}
            }
		}

        StartCoroutine(AddCoins(coinsCount, System.Convert.ToInt32(coinsCountText.text)));
        //coinsCountText.text = coinsCount.ToString();
		levelText.text = level.ToString();

        if (animate)
        {
            animator.SetTrigger(showPanelAnimationHash);
        }
	}

    void ShowRecord(bool animate = true)
    {
        if (animate)
        {
            SetActiveChest(false);
            SetActiveAdsButtons(false);
            SetActiveTavernKeeper(true);
        }
        else
        {
            SetActiveAdsButtons(false);
            SetActiveChest(true);
            SetActiveTavernKeeper(true);
        }

        cheaterText.enabled = false;
        for (int i = 0; i < titleTexts.Length - 1; i++)
        {
            titleTexts[i].enabled = false;
        }
        titleTexts[titleTexts.Length - 1].enabled = true;
    }

    void ShowProposalToBuyRandomCharacter(bool animate = true)
    {
        if (animate)
        {
            SetActiveChest(false);
            SetActiveAdsButtons(false);
            SetActiveTavernKeeper(true);
        }
        else
        {
            SetActiveAdsButtons(false);
            SetActiveChest(true);
            SetActiveTavernKeeper(true);
        }

        cheaterText.enabled = false;
        for (int i = 0; i < titleTexts.Length; i++)
        {
            titleTexts[i].enabled = false;
        }

        foreach (Button button in buyRandomCharacterButtons)
        {
            button.gameObject.SetActive(true);
        }
        buyRandomCharacterText.enabled = true;
        buyRandomCharacterCost.enabled = true;
        buyRandomCharacterCost.text = CONST.RANDOM_CHARACTER_COST + " coins";
    }

	bool ShowProposalToBuyCharacterForRealMoney(int characterNumber, bool animate = true)
	{
		if (animate)
		{
			SetActiveChest(false);
			SetActiveAdsButtons(false);
			SetActiveTavernKeeper(true);
		}
		else
		{
			SetActiveAdsButtons(false);
			SetActiveChest(true);
			SetActiveTavernKeeper(true);
		}
		
		cheaterText.enabled = false;
		for (int i = 0; i < titleTexts.Length; i++)
		{
			titleTexts[i].enabled = false;
		}
		
		foreach (Button button in buyCharacterButtons)
		{
			button.gameObject.SetActive(true);
		}
		buyCharacterText.enabled = true;
		buyCharacterCost.enabled = true;

		float price = InGameStore.Instance.GetProductPrice(characterNumber);// "USD " + CONST.CHARACTER_COSTS[positionNumber].ToString();
		string currency = InGameStore.Instance.GetProductCurrency(characterNumber);
		if (price == 0)
		{
			buyCharacterCost.text = "X";
			return false;
		}
		else
		{
			buyCharacterCost.text = string.Format("{0} {1}", currency, price);
		}

		buyCharacterContainer.SetActive(true);

		Object obj = Resources.Load(string.Format("{0}{1}{2}", CONST.TAVERN_PLAYERS_PATH, CONST.PLAYER_NAMES[characterNumber], CONST.TAVERN_PREFIX));
		if (obj != null)
		{
			GameObject characterGO = GameObject.Instantiate<GameObject>((GameObject)obj);

			characterGO.transform.parent = buyCharacterAnchor;
			characterGO.transform.localPosition = Vector3.zero;


			characterGO.layer = 10;
			foreach (Transform child in characterGO.GetComponentsInChildren<Transform>(true))
			{
				child.gameObject.layer = 10;
			}
			characterGO.transform.localScale = new Vector3(2, 2, 1);
			//characterGO.GetComponent<Animator>().SetTrigger("Idle");

			return true;
		}
		else
		{
			Debug.LogError("Cannot load character!");
			return false;
		}


	}

    void ShowAdvertising(bool animate = true)
    {
        if (animate)
        {
            SetActiveAdsButtons(true);
            SetActiveChest(true);
            SetActiveTavernKeeper(false);
        }
        else
        {
            SetActiveAdsButtons(false);
            SetActiveChest(true);
            SetActiveTavernKeeper(true);
        }

        cheaterText.enabled = false;
        for (int i = 0; i < titleTexts.Length - 1; i++)
        {
            titleTexts[i].enabled = false;
        }
        titleTexts[titleTexts.Length - 1].enabled = false;
        AdvertisingController.Instance.ProposeAdvertisement();
    }

    void ShowSomeLabel(int level, bool animate = true)
    {
        if (animate)
        {
            SetActiveAdsButtons(false);
            SetActiveChest(false);
            SetActiveTavernKeeper(true);
        }
        else
        {
            SetActiveAdsButtons(false);
            SetActiveChest(true);
            SetActiveTavernKeeper(true);
        }

        titleTexts[titleTexts.Length - 1].enabled = false;
        bool findTitle = false;
        for (int i = 0; i < titleTexts.Length - 1; i++)
        {
            if (level >= scoreLevels[i] && level < scoreLevels[i + 1])
            {
                titleTexts[i].enabled = true;
                findTitle = true;
            }
            else
            {
                titleTexts[i].enabled = false;
            }
        }
        cheaterText.enabled = !findTitle;
    }

    void SetActiveChest(bool isActive)
    {
        chestText.enabled = isActive;
        chestGameObject.SetActive(isActive);
    }

    void SetActiveAdsButtons(bool isActive)
    {
        foreach (Button button in adsButton)
        {
            button.interactable = isActive;
        }
    }

    void SetActiveTavernKeeper(bool isActive)
    {
        tavernKeeperGameObject.SetActive(isActive);
    }
}
