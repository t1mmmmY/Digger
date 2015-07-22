using UnityEngine;
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
            //Hide chest
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
			titleTexts[titleTexts.Length-1].enabled = true;

		}
		else
		{
            if (AdvertisingController.Instance.NeedToShowChestInGame())
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
            else
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
		}

        StartCoroutine(AddCoins(coinsCount, System.Convert.ToInt32(coinsCountText.text)));
        //coinsCountText.text = coinsCount.ToString();
		levelText.text = level.ToString();

        if (animate)
        {
            animator.SetTrigger(showPanelAnimationHash);
        }
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
