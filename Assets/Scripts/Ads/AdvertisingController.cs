using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;

public class AdvertisingController : BaseSingleton<AdvertisingController>
{
    [SerializeField] int adsReward = 50;
    [Range(1, 10)]
    [SerializeField] int showAdsFrequency = 3;
    string GAME_ID = "56600";
    string REWARD_VIDEO_ID = "rewardedVideoZone";
    int countGames = 0;
    bool chestReady = false;

    public static System.Action<int> onShowAdvertising;


    void Start()
    {
        countGames = showAdsFrequency - 1;
        Advertisement.Initialize(GAME_ID);
    }

    void OnEnable()
    {
        SingleplayerGameManager.OnStartSinglePlayerGame += OnStartSinglePlayerGame;
    }

    void OnDisable()
    {
        SingleplayerGameManager.OnStartSinglePlayerGame -= OnStartSinglePlayerGame;
    }

    void OnStartSinglePlayerGame()
    {
        countGames++;
        //Debug.Log("StartGame");
    }

    public void ProposeAdvertisement()
    {
        countGames = 0;
    }

    public void ShowAdvertisement()
    {
        if (Advertisement.IsReady())
        {
            chestReady = false;
            Advertisement.Show(REWARD_VIDEO_ID, new ShowOptions
            {
                resultCallback = result =>
                {
                    if (onShowAdvertising != null)
                    {
                        onShowAdvertising(adsReward);
                    }
                    BankController.AddCoins(adsReward);
                }
            });
        }
    }

    public bool NeedToShowChestInMainMenu()
    {
        if (!Advertisement.IsReady())
        {
            return false;
        }

        return chestReady;
    }

    public bool NeedToShowChestInGame()
    {
        if (!Advertisement.IsReady())
        {
            return false;
        }

        if (countGames > showAdsFrequency)
        {
            chestReady = true;
            return true;
        }
        else
        {

            return false;
        }
    }

}
