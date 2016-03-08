using UnityEngine;
using System.Collections;
using System;

public class BonusController : BaseSingleton<BonusController> 
{
    [SerializeField] double bonusFrequency = 720;
    string timeKey = "TIME_LAST_BONUS";
	string notificationNumberKey = "NOTIFICATION_NUMBER";

    private bool _isBonusActive = false;

//	float sleepUntil = 0;


	public Texture2D largeIcon;

	private string _contentTitle = "It's time";
	private string _contentText = "I have a good feelings!";
//	private bool _sticky = false;
	private int _number = 1;


    //public bool isBonusActive
    //{
    //    get { return _isBonusActive; }
    //} 

    void Start()
    {   
        IsBonusReady();

		double timeToNextBonus = bonusFrequency - GetDeltaMinutes();

		if (timeToNextBonus <= 1)
		{
			timeToNextBonus += bonusFrequency;
		}

		int notificationNumber = 1;
		if (!PlayerPrefs.HasKey(notificationNumberKey))
		{
			PlayerPrefs.SetInt(notificationNumberKey, 1);
		}

		notificationNumber = PlayerPrefs.GetInt(notificationNumberKey);

		Notification notification = PrepareNotification();

		NotificationManager.ShowNotification(notificationNumber, notification, (long)(timeToNextBonus * 60 * 1000));

		PlayerPrefs.SetInt(notificationNumberKey, notificationNumber + 1);

//		UnityEngine.iOS.LocalNotification.SendNotification(1, (long)(timeToNextBonus * 60), "It's time", "I have a good feelings", new Color32(0xff, 0x44, 0x44, 255));
//		sleepUntil = Time.time + 99999;

		// schedule notification to be delivered in 10 seconds
//		UnityEngine.iOS.LocalNotification notif = new UnityEngine.iOS.LocalNotification();
//		
//		double timeToNextBonus = bonusFrequency - GetDeltaMinutes();
//		
//		notif.fireDate = DateTime.Now.AddMinutes(timeToNextBonus);
//		notif.alertBody = "Hello!";
//		UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(notif);
    }

	public bool IsBonusReady()
	{
		//        DateTime currentTime = DateTime.Now;
		//        //Debug.Log(currentTime.ToString());
		//
		//        if (!PlayerPrefs.HasKey(timeKey))
		//        {
		//            PlayerPrefs.SetString(timeKey, currentTime.ToString());
		//			_isBonusActive = true;
		//			return true;
		//        }
		//        string lastBonusTimeString = PlayerPrefs.GetString(timeKey);
		//
		//        DateTime lastBonusTime = Convert.ToDateTime(lastBonusTimeString);
		//
		//        double deltaMinutes = (currentTime - lastBonusTime).TotalMinutes;
		
		double deltaMinutes = GetDeltaMinutes();
		
		if (deltaMinutes > bonusFrequency)
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

	double GetDeltaMinutes()
	{
		DateTime currentTime = DateTime.Now;
		//Debug.Log(currentTime.ToString());
		
		if (!PlayerPrefs.HasKey(timeKey))
		{
			PlayerPrefs.SetString(timeKey, currentTime.ToString());
			_isBonusActive = true;
			return bonusFrequency;
		}
		string lastBonusTimeString = PlayerPrefs.GetString(timeKey);
		
		DateTime lastBonusTime = Convert.ToDateTime(lastBonusTimeString);
		
		return (currentTime - lastBonusTime).TotalMinutes;
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


	private Notification PrepareNotification() {
		/*
		 * Unfortunately, from Unity 5.0, providing Android resources became obsolete.
		 * Before Unity 5.0, you can provide the name of the drawable in the folder Plugins/Android/res/drawable.
		 */

		Notification notification = new Notification(_contentTitle, _contentText);
//		Notification notification = new Notification("res/drawable/small_icon", _contentTitle, _contentText);
		notification.SetContentInfo("Gold Miner - Brain Work");
//		notification.EnableSound(true);
		/*
		 * Requires VIBRATE permission.
		 */ 
//		notification.SetVibrate(new long[] {200, 100, 100, 200, 200, 100});

		/*
		 * Lights or LED notification are only working when screen is off.
		 */
		notification.SetLights(new Color32(255, 255, 0, 255), 500, 500);

		/*
		 * If you pass a texture, it has to be readable. 
		 * Tick Read/Write Enabled option for the Texture in the inspector
		 */ 

		notification.SetLargeIcon(largeIcon);

		if(_number > 1)
		{
			notification.SetNumber(_number);
		}

//		notification.SetSticky(_sticky);

		return notification;
	}

}
