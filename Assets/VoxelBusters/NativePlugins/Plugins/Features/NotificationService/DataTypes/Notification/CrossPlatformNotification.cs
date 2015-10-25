using UnityEngine;
using System.Collections;

#if USES_NOTIFICATION_SERVICE 
using System.Collections.Generic;
using VoxelBusters.Utility;
using VoxelBusters.DebugPRO;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	/// <summary>
	/// Is a wrapper class to represent notifications that an application can schedule, handle and present.
	/// </summary>
	public partial class CrossPlatformNotification
	{	
		#region Properties

		/// <summary>
		/// Gets or sets the message displayed in the notification alert.
		/// </summary>
		/// <value>The alert body.</value>
		public string AlertBody
		{ 
			get; 
			set; 
		}
		
		/// <summary>
		/// Gets or sets the date and time when the system should deliver the notification.
		/// </summary>
		/// <value>The fire date.</value>
		public System.DateTime FireDate
		{ 
			get; 
			set; 
		}

		/// <summary>
		/// Gets or sets the interval at which to reschedule the notification.
		/// </summary>
		/// <value>The intervalinterval at which to reschedule the notification.</value>
		public eNotificationRepeatInterval RepeatInterval
		{ 
			get; 
			set; 
		}

		/// <summary>
		/// Gets or sets the dictionary for passing custom information to the notified application.
		/// </summary>
		/// <value>The user info.</value>
		public IDictionary UserInfo
		{
			get; 
			set; 
		}
		
		/// <summary>
		/// Gets or sets the name of the sound file to play when an alert is displayed.
		/// </summary>
		/// <value>The name of the sound.</value>
		public string SoundName
		{
			get; 
			set;
		}

		/// <summary>
		/// Gets or sets the iOS platform specific notification properties
		/// </summary>
		/// <value>The iOS properties.</value>
		public iOSSpecificProperties iOSProperties
		{
			get; 
			set; 
		}

		/// <summary>
		/// Gets or sets the android platform specific notification properties.
		/// </summary>
		/// <value>The android properties.</value>
		public AndroidSpecificProperties AndroidProperties
		{
			get; 
			set; 
		}
		
		#endregion

		#region Constants	

		// Related to JSON representation
		private 	const 	string 		kAlertBodyKey			= "alert-body";
		private 	const 	string 		kFireDateKey			= "fire-date";
		private 	const 	string 		kRepeatIntervalKey		= "repeat-interval";
		private 	const 	string 		kUserInfoKey			= "user-info";
		private 	const 	string 		kSoundNameKey			= "sound-name";
		private 	const 	string 		kiOSPropertiesKey		= "ios-properties";
		private 	const	string 		kAndroidPropertiesKey	= "android-properties";

		internal 	const 	string 		kNotificationID			= "np-notification-identifier";

		#endregion

		#region Constructors
		
		public CrossPlatformNotification ()
		{
			AlertBody			= null;
			UserInfo			= null;
			RepeatInterval		= eNotificationRepeatInterval.NONE;
			SoundName			= null;
			iOSProperties		= null;
			AndroidProperties	= null;
		}

		internal CrossPlatformNotification (IDictionary _jsonDict)
		{
			// Get alert body
			AlertBody			= _jsonDict.GetIfAvailable<string>(kAlertBodyKey);

			// Get fire date, repeat interval
			string _fireDateStr	= _jsonDict.GetIfAvailable<string>(kFireDateKey);
			
			if (!string.IsNullOrEmpty(_fireDateStr))
				FireDate		= _fireDateStr.ToZuluFormatDateTimeLocal();

			RepeatInterval		= _jsonDict.GetIfAvailable<eNotificationRepeatInterval>(kRepeatIntervalKey);

			// Get user info
			UserInfo			= _jsonDict[kUserInfoKey] as IDictionary;

			// Get sound name
			SoundName			= _jsonDict.GetIfAvailable<string>(kSoundNameKey);

			// Get iOS specific properties, if included
			if (_jsonDict.Contains(kiOSPropertiesKey))
				iOSProperties		= new iOSSpecificProperties(_jsonDict[kiOSPropertiesKey] as IDictionary);

			// Get Android specific properties, if included
			if (_jsonDict.Contains(kAndroidPropertiesKey))
				AndroidProperties	= new AndroidSpecificProperties(_jsonDict[kAndroidPropertiesKey] as IDictionary);
		}
		
		#endregion
		
		#region Methods
		
		internal string GetNotificationID ()
		{
			if (UserInfo != null && UserInfo.Contains(kNotificationID))
				return UserInfo[kNotificationID] as string;
			
			return null;
		}
		
		internal string GenerateNotificationID ()
		{
			string _notificationID		= NPBinding.Utility.GetUUID();
			
			if (UserInfo == null)
				UserInfo	= new Dictionary<string, string>();
			
			UserInfo[kNotificationID]	= _notificationID;
			
			return _notificationID;
		}

		internal IDictionary JSONObject ()
		{
			Dictionary<string, object> _jsonDict	= new Dictionary<string, object>();
			_jsonDict[kAlertBodyKey]				= AlertBody;
			_jsonDict[kFireDateKey]					= FireDate.ToStringUsingZuluFormat();
			_jsonDict[kRepeatIntervalKey]			= (int)RepeatInterval;
			_jsonDict[kUserInfoKey]					= UserInfo;
			_jsonDict[kSoundNameKey]				= SoundName;

			if (iOSProperties != null)
				_jsonDict[kiOSPropertiesKey]		= iOSProperties.JSONObject();

			if (AndroidProperties != null)
				_jsonDict[kAndroidPropertiesKey]	= AndroidProperties.JSONObject();

			return _jsonDict;
		}
		
		#endregion
		
		#region Static Methods
		
		public static CrossPlatformNotification CreateNotificationFromPayload (string _notificationPayload)
		{
			IDictionary _payloadDict	= JSONUtility.FromJSON(_notificationPayload) as IDictionary;

			if (_payloadDict == null)
			{
				Console.LogError(Constants.kDebugTag, "[CrossPlatformNotification] Failed to create notification.");
				return null;
			}

#if UNITY_ANDROID
			return new AndroidNotificationPayload(_payloadDict);
#elif UNITY_IOS
			return new iOSNotificationPayload(_payloadDict);
#else
			return null;
#endif
		}
		
		#endregion
		
		#region Overriden Method

		/// <summary>
		/// String representation of <see cref="CrossPlatformNotification"/>.
		/// </summary>
		public override string ToString ()
		{
			return string.Format ("CrossPlatformNotification: AlertBody={0}, FireDate={1}, RepeatInterval={2}", 
			                      AlertBody, FireDate, RepeatInterval);
		}
		
		#endregion
	}
}
#endif