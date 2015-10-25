using UnityEngine;
using System.Collections;

#if USES_NOTIFICATION_SERVICE 
using VoxelBusters.Utility;
using VoxelBusters.DebugPRO;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	public partial class NotificationService : MonoBehaviour 
	{
		#region Delegates

		/// <summary>
		/// Use this delegate type to get callback when app finishes registering with Push Notification service.
		/// </summary>
		/// <param name="_deviceToken">The device token received after registering successfully. This will be nil if register fails with error.</param>
		/// <param name="_error">An error that details why registration failed.</param>
		public delegate void RegisterForRemoteNotificationCompletion (string _deviceToken, string _error);

		/// <summary>
		/// Use this delegate type to get callback when app receives a notification.
		/// </summary>
		/// <param name="_notification">Received notification.</param>
		public delegate void ReceivedNotificationResponse (CrossPlatformNotification _notification);

		#endregion

		#region Events

		/// <summary>
		/// Occurs when app finishes registering with Push Notification service.
		/// </summary>
		/// <description>
		/// After you call the <see cref="RegisterForRemoteNotifications"/> method, the app calls this method when device registration completes successfully or when there is an error in the registration process.
		/// When registration completes successfully, connect with your push notification server and give the device token to it. 
		/// Push notification server pushes notifications only to the device represented by the token.
		/// </description>
		public static event RegisterForRemoteNotificationCompletion 	DidFinishRegisterForRemoteNotificationEvent;

		/// <summary>
		/// Occurs when app was launched by opening local notification.
		/// </summary>
		public static event ReceivedNotificationResponse 				DidLaunchWithLocalNotificationEvent;
		
		/// <summary>
		/// Occurs when app was launched by opening remote notification.
		/// </summary>
		public static event ReceivedNotificationResponse 				DidLaunchWithRemoteNotificationEvent;

		/// <summary>
		/// Occurs when app receives a local notification.
		/// </summary>
		public static event ReceivedNotificationResponse 				DidReceiveLocalNotificationEvent;

		/// <summary>
		/// Occurs when app receives a remote notification.
		/// </summary>
		public static event ReceivedNotificationResponse 				DidReceiveRemoteNotificationEvent;
		
		#endregion

		#region Launch Callback Methods

		private void DidReceiveAppLaunchInfo (string _launchData)
		{
			CrossPlatformNotification	_launchLocalNotification;
			CrossPlatformNotification	_launchRemoteNotification;

			// Parse and handle launch data
			ParseAppLaunchInfo(_launchData, out _launchLocalNotification, out _launchRemoteNotification);
			DidReceiveAppLaunchInfo(_launchLocalNotification, _launchRemoteNotification);
		}

		private void DidReceiveAppLaunchInfo (CrossPlatformNotification _launchLocalNotification, CrossPlatformNotification _launchRemoteNotification)
		{
			m_receivedAppLaunchInfo		= true;
			m_launchLocalNotification	= _launchLocalNotification;
			m_launchRemoteNotification	= _launchRemoteNotification;
		}

		#endregion

		#region Local Notification Callback Methods

		private void DidReceiveLocalNotification (string _notificationPayload)
		{
			CrossPlatformNotification _notification;
			
			// Parse received data
			ParseNotificationPayloadData(_notificationPayload, out _notification);

			// Triggers event
			DidReceiveLocalNotification(_notification);
		}

		private void DidReceiveLocalNotification (CrossPlatformNotification _notification)
		{
			Console.Log(Constants.kDebugTag, "[NotificationService] Received new local notification");
			
			if (DidReceiveLocalNotificationEvent != null)
				DidReceiveLocalNotificationEvent(_notification);
		}

		#endregion

		#region Remote Notification Callback Methods

		private void DidRegisterRemoteNotification (string _deviceToken)
		{
			Console.Log(Constants.kDebugTag, "[NotificationService] Remote notification registration finished, DeviceToken=" + _deviceToken);

			// Trigger event 
			if (DidFinishRegisterForRemoteNotificationEvent != null)
				DidFinishRegisterForRemoteNotificationEvent(_deviceToken, null);
		}

		private void DidFailToRegisterRemoteNotifications (string _errorDescription)
		{			
			Console.Log(Constants.kDebugTag, "[NotificationService] Remote notification registration failed, Error=" + _errorDescription);

			// Trigger event 
			if (DidFinishRegisterForRemoteNotificationEvent != null)
				DidFinishRegisterForRemoteNotificationEvent(null, _errorDescription);
		}

		private void DidReceiveRemoteNotification (string _notificationPayload)
		{
			CrossPlatformNotification _notification;
			
			// Parse received data
			ParseNotificationPayloadData(_notificationPayload, out _notification);
			
			// Triggers event 
			DidReceiveRemoteNotification(_notification);
		}

		private void DidReceiveRemoteNotification (CrossPlatformNotification _notification)
		{
			Console.Log(Constants.kDebugTag, "[NotificationService] Received new remote notification");

			if (DidReceiveRemoteNotificationEvent != null)
				DidReceiveRemoteNotificationEvent(_notification);
		}

		#endregion

		#region Parse Methods

		protected virtual void ParseAppLaunchInfo (string _launchData, out CrossPlatformNotification _launchLocalNotification, out CrossPlatformNotification _launchRemoteNotification)
		{
			_launchLocalNotification	= null;
			_launchRemoteNotification	= null;
		}

		protected virtual void ParseNotificationPayloadData (string _payload, out CrossPlatformNotification _notification)
		{
			_notification				= CrossPlatformNotification.CreateNotificationFromPayload(_payload);
		}

		#endregion
	}
}
#endif