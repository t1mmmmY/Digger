using UnityEngine;
using System.Collections;
using VoxelBusters.Utility;
using System.Collections.Generic;
using VoxelBusters.DebugPRO;
using System;

namespace VoxelBusters.NativePlugins.Internal
{
	public sealed class AndroidNotificationPayload : CrossPlatformNotification 
	{
		#region Key Constants //The same keys are used  by Native code as well
		
		// Json object keys
		public  const string		kFireDate						= "fire-date";
		public  const string		kAlertBody						= "alert-body";
		public  const string		kCustomSound					= "custom-sound";
		public  const string		kLargeIcon						= "large-icon";
		public 	const string 		kRepeatIntervalKey				= "repeat-interval";
		public 	const string 		kIsRemoteNotification			= "is-remote-notification";
				

		public static string 		ContentTitleKey 
		{ 
			get
			{
				return NPSettings.Notification.Android.ContentTitleKey;
			}
		}

		public static string 		ContentTextKey
		{ 
			get
			{
				return NPSettings.Notification.Android.ContentTextKey;
			}
		}

		public static string 		TickerTextKey 
		{ 
			get
			{
				return NPSettings.Notification.Android.TickerTextKey;
			}
		}

		public static string 		TagKey
		{ 
			get
			{
				return NPSettings.Notification.Android.TagKey;
			}
		}

		public static string 		UserInfoKey 
		{ 
			get
			{
				return NPSettings.Notification.Android.UserInfoKey;
			}
		}
		
		#endregion

		#region Constructor
		
		public AndroidNotificationPayload (IDictionary _payloadDict)
		{
			AndroidProperties		= new AndroidSpecificProperties();
			
			// Alert
			if (_payloadDict.Contains(NPSettings.Notification.Android.ContentTextKey))
			{
				//Check here which key is being received.
				VoxelBusters.DebugPRO.Console.Log(Constants.kDebugTag, "[AndroidNotificationPayload] " + _payloadDict.ToJSON());//TODO
				object _alertUnknownType	= _payloadDict[ContentTextKey] as object;
				
				// String type
				if ((_alertUnknownType as string) != null)
				{
					AlertBody	= _alertUnknownType as string;
				}
			}
						
			if (_payloadDict.Contains(UserInfoKey))
				UserInfo		= _payloadDict[UserInfoKey] as IDictionary;
			
			// Fire date
			long _secsFromNow	= _payloadDict.GetIfAvailable<long>(kFireDate);
				
			FireDate			= _secsFromNow.ToDateTimeFromJavaTime();

			if (_payloadDict.Contains(kRepeatIntervalKey))
				RepeatInterval		= (eNotificationRepeatInterval)_payloadDict.GetIfAvailable<int>(kRepeatIntervalKey);

			// Sound, Badge
			AndroidProperties.ContentTitle		=  	_payloadDict.GetIfAvailable<string>(ContentTitleKey);
			AndroidProperties.TickerText		=  	_payloadDict.GetIfAvailable<string>(TickerTextKey);
			AndroidProperties.Tag				=  	_payloadDict.GetIfAvailable<string>(TagKey);

			//Added in 1.03
			AndroidProperties.CustomSound		= 	_payloadDict.GetIfAvailable<string>(kCustomSound);
			AndroidProperties.LargeIcon			= 	_payloadDict.GetIfAvailable<string>(kLargeIcon);
		}
		
		#endregion
		
		#region Static Methods
		
		public static IDictionary CreateNotificationPayload (CrossPlatformNotification _notification)
		{
			IDictionary _payloadDict						= new Dictionary<string, object>();
			AndroidSpecificProperties _androidProperties	= _notification.AndroidProperties;
			
			// Alert
			_payloadDict[ContentTextKey]		= _notification.AlertBody;
			
			// User info, fire date
			_payloadDict[UserInfoKey]			= _notification.UserInfo;
			_payloadDict[kFireDate]				= _notification.FireDate.ToJavaTimeFromDateTime();


			_payloadDict[kRepeatIntervalKey]	= (int)_notification.RepeatInterval;
			

			// ContentTitle, TickerText, Tag
			if(_androidProperties != null)
			{
				_payloadDict[ContentTitleKey]	= _androidProperties.ContentTitle;
				_payloadDict[TickerTextKey]		= _androidProperties.TickerText;
				_payloadDict[TagKey]			= _androidProperties.Tag;
				_payloadDict[kCustomSound]		= _androidProperties.CustomSound;
				_payloadDict[kLargeIcon]		= _androidProperties.LargeIcon;
			}
			
			return _payloadDict;
		}
		
		#endregion
	}
}