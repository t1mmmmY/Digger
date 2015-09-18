using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VoxelBusters.Utility;
#if UNITY_IOS && UNITY_5
using CalendarUnit = UnityEngine.iOS.CalendarUnit;
#endif

#if UNITY_IOS
namespace VoxelBusters.NativePlugins.Internal
{
	public sealed class iOSNotificationPayload : CrossPlatformNotification 
	{
		#region Constant

		private 	const 	string 		kAPS				= "aps";
		private 	const 	string 		kAlert				= "alert";
		private 	const 	string 		kBody				= "body";
		private 	const 	string 		kAction				= "action-loc-key";
		private 	const 	string 		kLaunchImage		= "launch-image";
		private 	const 	string 		kFireDate			= "fire-date";
		private 	const 	string 		kRepeatIntervalKey	= "repeat-interval";
		private 	const 	string 		kBadge				= "badge";
		private 	const 	string 		kSound				= "sound";

		#endregion

		#region Constructor

		public iOSNotificationPayload (IDictionary _payloadDict)
		{
			IDictionary _apsDict			= _payloadDict[kAPS] as IDictionary;
			string 		_userInfoKey		= NPSettings.Notification.iOS.UserInfoKey;
			iOSProperties					= new iOSSpecificProperties();

			// Read alert info
			if (_apsDict.Contains(kAlert))
			{
				object 	_alertUnknownType	= _apsDict[kAlert] as object;
			
				if (_alertUnknownType != null)
				{
					// String format
					if ((_alertUnknownType as string) != null)
					{
						AlertBody						= _alertUnknownType as string;
					}
					// Dictionary format
					else
					{
						IDictionary _alertDict			= _alertUnknownType as IDictionary;
						AlertBody						= _alertDict.GetIfAvailable<string>(kBody);
						string 		_alertAction		= _alertDict.GetIfAvailable<string>(kAction);

						if (string.IsNullOrEmpty(_alertAction))
						{
							iOSProperties.AlertAction	= null;
							iOSProperties.HasAction		= false;
						}
						else
						{
							iOSProperties.AlertAction	= _alertAction;
							iOSProperties.HasAction		= true;
						}

						// Launch image
						iOSProperties.LaunchImage		= _alertDict.GetIfAvailable<string>(kLaunchImage);
					}
				}
			}

			// Read sound, badge info
			iOSProperties.SoundName		=  _apsDict.GetIfAvailable<string>(kSound);
			iOSProperties.BadgeCount	=  _apsDict.GetIfAvailable<int>(kBadge);

			// Read user info
			if (_apsDict.Contains(_userInfoKey))
				UserInfo				= _payloadDict[_userInfoKey] as IDictionary;
			
			// Read fire date, repeat interval
			string 	_fireDateStr		= _payloadDict.GetIfAvailable<string>(kFireDate);

			if (!string.IsNullOrEmpty(_fireDateStr))
				FireDate				= _fireDateStr.ToDateTimeLocalUsingZuluFormat();

			RepeatInterval				= ConvertToRepeatInterval(_payloadDict.GetIfAvailable<CalendarUnit>(kRepeatIntervalKey));
		}

		#endregion

		#region Static Methods

		public static IDictionary CreateNotificationPayload (CrossPlatformNotification _notification)
		{
			IDictionary 			_payloadDict	= new Dictionary<string, object>();
			IDictionary 			_apsDict		= new Dictionary<string, object>();
			iOSSpecificProperties 	_iosProperties	= _notification.iOSProperties;
			string 					_keyForUserInfo	= NPSettings.Notification.iOS.UserInfoKey;

			// Add alert info
			IDictionary 			_alertDict		= new Dictionary<string, string>();
			_alertDict[kBody]						= _notification.AlertBody;
			_alertDict[kAction]						= _iosProperties.AlertAction;
			_alertDict[kLaunchImage]				= _iosProperties.LaunchImage;

			// Add alert, badge, sound to "aps" dictionary
			_apsDict[kAlert]						= _alertDict;
			_apsDict[kBadge]						= _iosProperties.BadgeCount;
			_apsDict[kSound]						= _iosProperties.SoundName;

			// Add aps, user info, fire date, repeat interval to "payload" dictionary
			_payloadDict[kAPS]						= _apsDict;
			_payloadDict[_keyForUserInfo]			= _notification.UserInfo;
			_payloadDict[kFireDate]					= _notification.FireDate.ToStringUsingZuluFormat();
			_payloadDict[kRepeatIntervalKey]		= (int)ConvertToCalendarUnit(_notification.RepeatInterval);

			return _payloadDict;
		}

		public static eNotificationRepeatInterval ConvertToRepeatInterval (CalendarUnit _unit)
		{
			switch (_unit)
			{
			case CalendarUnit.Minute:
				return eNotificationRepeatInterval.MINUTE;

			case CalendarUnit.Day:
				return eNotificationRepeatInterval.DAY;

			case CalendarUnit.Week:
				return eNotificationRepeatInterval.WEEK;

			case CalendarUnit.Month:
				return eNotificationRepeatInterval.MONTH;

			case CalendarUnit.Year:
				return eNotificationRepeatInterval.YEAR;

			default:
				return eNotificationRepeatInterval.NONE;
			}
		}

		public static CalendarUnit ConvertToCalendarUnit (eNotificationRepeatInterval _repeatInterval)
		{
			switch (_repeatInterval)
			{
			case eNotificationRepeatInterval.MINUTE:
				return CalendarUnit.Minute;
				
			case eNotificationRepeatInterval.DAY:
				return CalendarUnit.Day;
				
			case eNotificationRepeatInterval.WEEK:
				return CalendarUnit.Week;
				
			case eNotificationRepeatInterval.MONTH:
				return CalendarUnit.Month;
				
			case eNotificationRepeatInterval.YEAR:
				return CalendarUnit.Year;
				
			default:
				return 0;
			}
		}

		#endregion
	}
}
#endif