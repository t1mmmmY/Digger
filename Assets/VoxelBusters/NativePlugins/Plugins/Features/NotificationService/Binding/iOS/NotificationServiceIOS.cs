using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using VoxelBusters.Utility;
#if UNITY_IOS && UNITY_5
using LocalNotification = UnityEngine.iOS.LocalNotification;
using NotificationServices = UnityEngine.iOS.NotificationServices;
#endif

#if UNITY_IOS
namespace VoxelBusters.NativePlugins
{
	public partial class NotificationServiceIOS : NotificationService
	{
		#region Native Methods

		[DllImport("__Internal")]
		private static extern void initNotificationService (string _keyForUserInfo);

		[DllImport("__Internal")]
		private static extern void registerNotificationTypes (int notificationTypes);

		[DllImport("__Internal")]
		private static extern void registerForRemoteNotifications ();

		[DllImport("__Internal")]
		private static extern void unregisterForRemoteNotifications ();

		#endregion

		#region Initialise
		
		protected override void Initialise (NotificationServiceSettings _settings)
		{
			string _keyForUserInfo	= _settings.iOS.UserInfoKey;

			// Initialise native component
			initNotificationService(_keyForUserInfo);
		}
		
		#endregion

		#region Local Notification API'S

		public override void RegisterNotificationTypes (NotificationType _notificationTypes)
		{
			base.RegisterNotificationTypes(_notificationTypes);

			// Native call
			registerNotificationTypes((int)_notificationTypes);
		}

		public override string ScheduleLocalNotification (CrossPlatformNotification _notification)
		{
			// Append notification id to user info
			string _notificationID				= _notification.GenerateNotificationID();
			
			// Assign notification data
			LocalNotification _newNotification	= new LocalNotification();
			_newNotification.alertBody			= _notification.AlertBody;
			_newNotification.fireDate			= _notification.FireDate;
			_newNotification.userInfo			= _notification.UserInfo;
			
			// iOS Notification additional data
			CrossPlatformNotification.iOSSpecificProperties _iOSProperties	= _notification.iOSProperties;
			
			if (_iOSProperties != null)
			{
				_newNotification.hasAction					= _iOSProperties.HasAction;
				_newNotification.applicationIconBadgeNumber	= _iOSProperties.BadgeCount;
				
				if (!string.IsNullOrEmpty(_iOSProperties.AlertAction))
					_newNotification.alertAction		= _iOSProperties.AlertAction;
				
				if (!string.IsNullOrEmpty(_iOSProperties.LaunchImage))
					_newNotification.alertLaunchImage	= _iOSProperties.LaunchImage;
				
				if (!string.IsNullOrEmpty(_iOSProperties.SoundName))
					_newNotification.soundName			= _iOSProperties.SoundName;
			}

			// Schedule notification
			NotificationServices.ScheduleLocalNotification(_newNotification);
			return _notificationID;
		}
		
		public override void CancelLocalNotification (string _notificationID)
		{
			foreach (LocalNotification _scheduledNotification in NotificationServices.scheduledLocalNotifications)
			{
				IDictionary _scheduledNotificationUserInfo	= _scheduledNotification.userInfo;
				
				if (_scheduledNotificationUserInfo != null)
				{
					string _scheduledNotificationID	= _scheduledNotificationUserInfo.GetIfAvailable<string>(CrossPlatformNotification.kNotificationID);
					
					// Cancel notification
					if (!string.IsNullOrEmpty(_scheduledNotificationID) && _scheduledNotificationID.Equals(_notificationID))
					{
						NotificationServices.CancelLocalNotification(_scheduledNotification);
						break;
					}
				}
			}
		}
		
		public override void CancelAllLocalNotification ()
		{
			NotificationServices.CancelAllLocalNotifications();
		}

		public override void ClearNotifications ()
		{
			// Removing badge count
			NPBinding.Utility.SetApplicationIconBadgeNumber(0);

			// Clears notification
			NotificationServices.ClearLocalNotifications();
			NotificationServices.ClearRemoteNotifications();
		}

		#endregion
		
		#region Overriden Remote Notification API's

		public override void RegisterForRemoteNotifications ()
		{
			registerForRemoteNotifications();
		}
		
		public override void UnregisterForRemoteNotifications ()
		{
			unregisterForRemoteNotifications();
		}

		#endregion
	}
}
#endif