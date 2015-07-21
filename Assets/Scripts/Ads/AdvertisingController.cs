using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;

public class AdvertisingController : BaseSingleton<AdvertisingController>
{
    string GAME_ID = "56600";

    void Start()
    {
        Advertisement.Initialize(GAME_ID);
    }

    public void ShowAdvertisement()
    {
        if (Advertisement.IsReady())
        {
            Advertisement.Show();
        }
    }
}
