using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VoxelBusters.Utility;
using VoxelBusters.NativePlugins;
using VoxelBusters.AssetStoreProductUtility.Demo;

namespace VoxelBusters.NativePlugins.Demo
{
	public class NotificationDemo : DemoSubMenu 
	{
		#region Properties

		[SerializeField, EnumMaskField(typeof(NotificationType))]
		private NotificationType	m_notificationType;

		private ArrayList 			m_scheduledNotificationIDList	= new ArrayList();
			
		#endregion

		#region API Calls

		private void RegisterNotificationTypes(NotificationType _notificationTypes)
		{
			NPBinding.NotificationService.RegisterNotificationTypes(_notificationTypes);
		}
		
		private void RegisterForRemoteNotifications()
		{
			NPBinding.NotificationService.RegisterForRemoteNotifications();
		}

		private void UnregisterForRemoteNotifications()
		{
			NPBinding.NotificationService.UnregisterForRemoteNotifications();
		}

		private string ScheduleLocalNotification(CrossPlatformNotification _notification)
		{
			return NPBinding.NotificationService.ScheduleLocalNotification(_notification);
		}

		private void CancelLocalNotification(string _notificationID)
		{
			NPBinding.NotificationService.CancelLocalNotification(_notificationID);
		}
		
		private void CancelAllLocalNotifications()
		{
			NPBinding.NotificationService.CancelAllLocalNotification();
		}
		
		private void ClearNotifications()
		{
			NPBinding.NotificationService.ClearNotifications();
		}

		#endregion


		#region API Callbacks
		
		private void DidLaunchWithLocalNotificationEvent (CrossPlatformNotification _notification)
		{
			AddNewResult("Received DidLaunchWithLocalNotificationEvent");
			AppendNotificationResult(_notification);
		}
		
		private void DidLaunchWithRemoteNotificationEvent (CrossPlatformNotification _notification)
		{
			AddNewResult("Received DidLaunchWithRemoteNotificationEvent");
			AppendNotificationResult(_notification);
		}

		private void DidReceiveLocalNotificationEvent (CrossPlatformNotification _notification)
		{
			AddNewResult("Received DidReceiveLocalNotificationEvent");
			AppendNotificationResult(_notification);
		}
		
		private void DidReceiveRemoteNotificationEvent (CrossPlatformNotification _notification)
		{
			AddNewResult("Received DidReceiveRemoteNotificationEvent");
			AppendNotificationResult(_notification);
		}
		
		private void DidFinishRegisterForRemoteNotificationEvent (string _deviceToken, string _error)
		{
			AddNewResult("Received DidFinishRegisterForRemoteNotificationEvent");
			AppendResult("DeviceToken = " + _deviceToken);
			
			if(!string.IsNullOrEmpty(_error))
			{
				AppendResult("Error = " + _error);
			}
			
		}
		
		#region Helpers for Callbacks
		
		void AppendNotificationResult (CrossPlatformNotification _notification)
		{
			string _alert 					= _notification.AlertBody;

#pragma warning disable
			// Exists only for local notifications which will be useful if we need to cancel a local notification
			string _notificationIdentifier 	= _notification.GetNotificationID();
#pragma warning restore
			
			//Get UserInfo details
			IDictionary _userInfo 			= _notification.UserInfo;
			
			//Can get specific details of a notification based on platform
			/*
					//For Android
					_notification.AndroidProperties.ContentTitle
					_notification.AndroidProperties.TickerText

					//For iOS
					_notification.iOSProperties.AlertAction;
					_notification.iOSProperties.BadgeCount;
				*/
			
			// Append to result list
			AppendResult("Alert = " + _alert);

			// Append user info
			string _userInfoDetails = null;

			if (_userInfo != null)
			{
				// Initialize and iterate through the list
				_userInfoDetails	= string.Empty;

				foreach (string _key in _userInfo.Keys)
				{
					_userInfoDetails	+= _key + " : " + _userInfo[_key] + "\n";
				}
			}
			else
			{
				_userInfoDetails	= "NULL";
			}

			AppendResult("UserInfo = " + _userInfoDetails);	
		}
		
		#endregion
		
		#endregion

		#region Enabling/Disabling Events

		protected override void OnEnable ()
		{
			base.OnEnable ();

			// Note for developers
			AddNewResult("Callbacks" +
			             "\nDidFinishRegisterForRemoteNotificationEvent: Triggered when registering for remote notification finished." +
			             "\nDidLaunchWithLocalNotificationEvent: Triggered when application is launched from local notification." +
			             "\nDidLaunchWithRemoteNotificationEvent: Triggered when application is launched from remote notification." +
			             "\nDidReceiveLocalNotificationEvent: Triggered when local notification is received." +
			             "\nDidReceiveRemoteNotificationEvent: Triggered when remote notification is received.");

			// Register for callbacks
			NotificationService.DidFinishRegisterForRemoteNotificationEvent	+= DidFinishRegisterForRemoteNotificationEvent;
			NotificationService.DidLaunchWithLocalNotificationEvent			+= DidLaunchWithLocalNotificationEvent;
			NotificationService.DidLaunchWithRemoteNotificationEvent		+= DidLaunchWithRemoteNotificationEvent;
			NotificationService.DidReceiveLocalNotificationEvent 			+= DidReceiveLocalNotificationEvent;
			NotificationService.DidReceiveRemoteNotificationEvent			+= DidReceiveRemoteNotificationEvent;
		}

		protected override void OnDisable ()
		{
			base.OnDisable ();

			// Un-Register from callbacks
			NotificationService.DidFinishRegisterForRemoteNotificationEvent	-= DidFinishRegisterForRemoteNotificationEvent;
			NotificationService.DidLaunchWithLocalNotificationEvent 		-= DidLaunchWithLocalNotificationEvent;
			NotificationService.DidLaunchWithRemoteNotificationEvent 		-= DidLaunchWithRemoteNotificationEvent;
			NotificationService.DidReceiveLocalNotificationEvent 			-= DidReceiveLocalNotificationEvent;
			NotificationService.DidReceiveRemoteNotificationEvent			-= DidReceiveRemoteNotificationEvent;
		}

		#endregion

		#region UI

		protected override void OnGUIWindow ()
		{
			base.OnGUIWindow();

			RootScrollView.BeginScrollView();
			{
				DrawRegisterAPI();
				DrawScheduleNotificationAPI();
				DrawCancelNotificationAPI();
			}
			RootScrollView.EndScrollView();
						
			DrawResults();
			DrawPopButton();
		}

		private void DrawRegisterAPI ()
		{
			GUILayout.Label("Register/Unregister", kSubTitleStyle);
			
			if (GUILayout.Button("Register Notification Types [None, Alert, Badge and Sound]"))
			{
				RegisterNotificationTypes(m_notificationType);
				AddNewResult("Registered Types : " + m_notificationType.GetValue());
			}
			
			if (GUILayout.Button("Register For Remote Notifications"))
			{
				RegisterForRemoteNotifications();
			}
			
			if (GUILayout.Button("Unregister For Remote Notifications"))
			{
				UnregisterForRemoteNotifications();
				AddNewResult("Unregistered for remote notifications");
			}
		}

		private void DrawScheduleNotificationAPI ()
		{
			GUILayout.Label("Schedule Notifications", kSubTitleStyle);
			
			if (GUILayout.Button("Schedule Local Notification (After 1min, Repeat: Disabled)"))
			{
				// Schedules a local notification after 1 min
				string _nID = ScheduleLocalNotification(CreateNotification(60, eNotificationRepeatInterval.NONE));
				
				// Add notification id to list
				m_scheduledNotificationIDList.Add(_nID);
				
				// Update info
				AddNewResult("Newly scheduled notification ID = " + _nID);
			}
			
			if (GUILayout.Button("Schedule Local Notification (After 1min, Repeat: Every Minute)"))
			{
				// Schedules a local notification after 1 min and it keeps rescheduling for every minute
				string _nID = ScheduleLocalNotification(CreateNotification(60, eNotificationRepeatInterval.MINUTE));
				
				// Add notification id to list
				m_scheduledNotificationIDList.Add(_nID);
				
				// Update info
				AddNewResult("Newly scheduled notification ID = " + _nID);
			}
			
			if (GUILayout.Button("Schedule Local Notification (After 1min, Repeat: Every Hour)"))
			{
				// Schedules a local notification after 1 min and it keeps rescheduling for every hour
				string _nID = ScheduleLocalNotification(CreateNotification(60, eNotificationRepeatInterval.HOUR));
				
				// Add notification id to list
				m_scheduledNotificationIDList.Add(_nID);
				
				// Update info
				AddNewResult("Newly scheduled notification ID = " + _nID);
			}
		}

		private void DrawCancelNotificationAPI ()
		{
			GUILayout.Label("Cancel Notifications", kSubTitleStyle);
			
			if (GUILayout.Button("Cancel Local Notification"))
			{
				if (m_scheduledNotificationIDList.Count > 0)
				{
					string _nID		= m_scheduledNotificationIDList[0] as string;
					
					AddNewResult("Cancelling notification with ID=" + _nID);
					
					CancelLocalNotification(_nID);
					
					// Remove notification id
					m_scheduledNotificationIDList.RemoveAt(0);
				}
				else
				{
					AddNewResult("No Scheduled Local Notifications");
				}
			}
			
			if (GUILayout.Button("Cancel All Local Notifications"))
			{
				// Clearing list
				m_scheduledNotificationIDList.Clear();
				
				// Cancelling all notifications
				CancelAllLocalNotifications();
				
				// Update info
				AddNewResult("Scheduled notifications are invalidated");
			}
			
			if (GUILayout.Button("Clear Notifications"))
			{
				ClearNotifications();
				
				// Update info
				AddNewResult("Cleared notifications from notification bar.");
			}
		}

		#endregion

		#region Misc. Methods
 
		private CrossPlatformNotification CreateNotification (long _fireAfterSec, eNotificationRepeatInterval _repeatInterval)
		{
			// User info
			IDictionary _userInfo			= new Dictionary<string, string>();
			_userInfo["data"]				= "add what is required";
			
			CrossPlatformNotification.iOSSpecificProperties _iosProperties			= new CrossPlatformNotification.iOSSpecificProperties();
			_iosProperties.HasAction		= true;
			_iosProperties.AlertAction		= "alert action";
			
			CrossPlatformNotification.AndroidSpecificProperties _androidProperties	= new CrossPlatformNotification.AndroidSpecificProperties();
			_androidProperties.ContentTitle	= "content title";
			_androidProperties.TickerText	= "ticker ticks over here";
			_androidProperties.CustomSound	= "Notification.mp3"; //Keep the files in Assets/StreamingAssets/VoxelBusters/NativePlugins/Android folder.
			_androidProperties.LargeIcon	= "NativePlugins.png"; //Keep the files in Assets/StreamingAssets/VoxelBusters/NativePlugins/Android folder.
			
			CrossPlatformNotification _notification	= new CrossPlatformNotification();
			_notification.AlertBody			= "alert body"; //On Android, this is considered as ContentText
			_notification.FireDate			= System.DateTime.Now.AddSeconds(_fireAfterSec);
			_notification.RepeatInterval	= _repeatInterval;
			_notification.UserInfo			= _userInfo;
			_notification.iOSProperties		= _iosProperties;
			_notification.AndroidProperties	= _androidProperties;

			return _notification;
		}

		#endregion
	}
}