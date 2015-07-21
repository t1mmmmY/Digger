using UnityEngine;
using System.Collections;
using System;

public class BonusController : BaseSingleton<BonusController> 
{
    [SerializeField] double bonusFrequency = 10;
    string timeKey = "TIME_LAST_BONUS";

    private bool _isBonusActive = false;
    //public bool isBonusActive
    //{
    //    get { return _isBonusActive; }
    //}

    void Start()
    {   
        IsBonusReady();
    }

    public bool IsBonusReady()
    {
        DateTime currentTime = DateTime.Now;
        //Debug.Log(currentTime.ToString());

        if (!PlayerPrefs.HasKey(timeKey))
        {
            PlayerPrefs.SetString(timeKey, currentTime.ToString());
        }
        string lastBonusTimeString = PlayerPrefs.GetString(timeKey);
        //Debug.Log(lastBonusTimeString);

        DateTime lastBonusTime = Convert.ToDateTime(lastBonusTimeString);

        double deltaSeconds = (currentTime - lastBonusTime).TotalMinutes;
        Debug.Log(deltaSeconds);

        if (deltaSeconds > bonusFrequency)
        {
            //Time bonus
            _isBonusActive = true;
            return true;
        }
        else
        {
            _isBonusActive = false;
            return false;
        }
    }

    public bool GrabBonus()
    {
        if (!_isBonusActive)
        {
            return false;
        }

        DateTime currentTime = DateTime.Now;
        PlayerPrefs.SetString(timeKey, currentTime.ToString());
        
        _isBonusActive = false;

        return true;
    }
}
