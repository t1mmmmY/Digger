using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins.Internal
{
	public sealed class iOSNotificationPayload : CrossPlatformNotification 
	{
		#region Constant

		private const string 		kAPS			= "aps";
		private const string 		kAlert			= "alert";
		private const string 		kBody			= "body";
		private const string 		kAction			= "action-loc-key";
		private const string 		kLaunchImage	= "launch-image";
		private const string 		kFireDate		= "fire-date";
		private const string 		kBadge			= "badge";
		private const string 		kSound			= "sound";

		#endregion

		#region Constructor

		public iOSNotificationPayload (IDictionary _payloadDict)
		{
			IDictionary _apsDict	= _payloadDict[kAPS] as IDictionary;
			iOSProperties			= new iOSSpecificProperties();

			// Alert
			if (_apsDict.Contains(kAlert))
			{
				object _alertUnknownType	= _apsDict[kAlert] as object;
			
				if (_alertUnknownType != null)
				{
					// String type
					if ((_alertUnknownType as string) != null)
					{
						AlertBody				= _alertUnknownType as string;
					}
					// Dictionary type
					else
					{
						IDictionary _alertDict	= _alertUnknownType as IDictionary;

						// Set alert body
						AlertBody				= _alertDict.GetIfAvailable<string>(kBody);

						// Set alert action
						string _alertAction		= _alertDict.GetIfAvailable<string>(kAction);

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

			// Userinfo
			string _userInfoKey			= NPSettings.Notification.iOS.UserInfoKey;
			
			if (_apsDict.Contains(_userInfoKey))
				UserInfo	= _apsDict[_userInfoKey] as IDictionary;
			
			// Fire date
			string _fireDateStr			= _apsDict.GetIfAvailable<string>(kFireDate);

			if (!string.IsNullOrEmpty(_fireDateStr))
				FireDate	= _fireDateStr.ToDateTimeLocalUsingZuluFormat();

			// Sound, Badge
			iOSProperties.SoundName		=  _apsDict.GetIfAvailable<string>(kSound);
			iOSProperties.BadgeCount	=  _apsDict.GetIfAvailable<int>(kBadge);
		}

		#endregion

		#region Static Methods

		public static IDictionary CreateNotificationPayload (CrossPlatformNotification _notification)
		{
			IDictionary _payloadDict				= new Dictionary<string, object>();
			IDictionary _apsDict					= new Dictionary<string, object>();
			iOSSpecificProperties _iosProperties	= _notification.iOSProperties;

			// Alert
			IDictionary _alertDict		= new Dictionary<string, string>();
			_alertDict[kBody]			= _notification.AlertBody;
			_alertDict[kAction]			= _iosProperties.AlertAction;
			_alertDict[kLaunchImage]	= _iosProperties.LaunchImage;
			_apsDict[kAlert]			= _alertDict;
			
			// User info, fire date
			string _keyForUserInfo		= NPSettings.Notification.iOS.UserInfoKey;
			_apsDict[_keyForUserInfo]	= _notification.UserInfo;
			_apsDict[kFireDate]			= _notification.FireDate.ToStringUsingZuluFormat();
		
			// Sound, badge
			_apsDict[kBadge]			= _iosProperties.BadgeCount;
			_apsDict[kSound]			= _iosProperties.SoundName;

			// Add aps dictionary to payload
			_payloadDict[kAPS]			= _apsDict;

			return _payloadDict;
		}

		#endregion
	}
}
