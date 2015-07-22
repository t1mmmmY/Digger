using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;

public class AdvertisingController : BaseSingleton<AdvertisingController>
{
    [SerializeField] int adsReward = 50;
    string GAME_ID = "56600";
    string REWARD_VIDEO_ID = "rewardedVideoZone";

    void Start()
    {
        Advertisement.Initialize(GAME_ID);
    }

    public void ShowAdvertisement()
    {
        if (Advertisement.IsReady())
        {
            Advertisement.Show(REWARD_VIDEO_ID, new ShowOptions
            {
                resultCallback = result =>
                {
                    BankController.AddCoins(adsReward);
                }
            });
        }
    }
}
