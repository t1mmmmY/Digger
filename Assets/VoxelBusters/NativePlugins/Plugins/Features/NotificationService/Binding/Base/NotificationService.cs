using UnityEngine;
using System.Collections;

#if USES_NOTIFICATION_SERVICE 
using System.Collections.Generic;

namespace VoxelBusters.NativePlugins
{
	/// <summary>
	/// Used for scheduling, registering and handling both local and remote notifications.
	/// </summary>
	public partial class NotificationService : MonoBehaviour 
	{
		#region Static Fields

		public				string						notificationDefaultSoundName	= "default.mp3";

		#endregion

		#region properties

		// Received notifications
		private				CrossPlatformNotification	m_launchLocalNotification		= null;
		private				CrossPlatformNotification	m_launchRemoteNotification		= null;
		private				bool						m_receivedAppLaunchInfo			= false;

		#endregion

		#region Unity Methods

		private void Awake ()
		{
			if (NPSettings.Application.SupportedFeatures.UsesNotificationService)
			{
				// Initialise component
				Initialise(NPSettings.Notification);
			}
		}

		private void Start ()
		{
			StartCoroutine(ProcessAppLaunchInfo());
		}

		#endregion

		#region Initialise Methods

		protected virtual void Initialise (NotificationServiceSettings _settings)
		{}

		private IEnumerator ProcessAppLaunchInfo ()
		{
			yield return new WaitForEndOfFrame();
			
			while (!m_receivedAppLaunchInfo)
				yield return null;
			
			if (m_launchLocalNotification != null)
			{
				if (DidLaunchWithLocalNotificationEvent != null)
					DidLaunchWithLocalNotificationEvent(m_launchLocalNotification);
			}
			
			if (m_launchRemoteNotification != null)
			{
				if (DidLaunchWithRemoteNotificationEvent != null)
					DidLaunchWithRemoteNotificationEvent(m_launchRemoteNotification);
			}
		}

		#endregion

		#region Notification Methods

		/// <summary>
		/// Apps that use either local or remote notifications must register the types of notifications they intend to deliver.
		/// </summary>
		/// <param name="_notificationTypes">The notification types that your app supports.</param>
		/// <description>
		/// If your app displays alerts, play sounds, or badges its icon, you must call this method during your launch.
		/// </description>
		public virtual void RegisterNotificationTypes (NotificationType _notificationTypes)
		{}

		#endregion

		#region Local Notification Methods

		/// <summary>
		/// Schedules a local notification.
		/// </summary>
		/// <returns>Notification ID that can be used to uniquely identify every scheduled notification.</returns>
		/// <param name="_notification">Notification to be scheduled.</param>
		public virtual string ScheduleLocalNotification (CrossPlatformNotification _notification)
		{
			return null;
		}

		/// <summary>
		/// Cancels the delivery of the specified scheduled local notification.
		/// </summary>
		/// <param name="_notificationID">Identifier for the scheduled notification to be cancelled.</param>
		public virtual void CancelLocalNotification (string _notificationID)
		{}

		/// <summary>
		/// Cancels the delivery of all scheduled local notification.
		/// </summary>
		public virtual void CancelAllLocalNotification ()
		{}
			
		/// <summary>
		/// Discards all received notifications.
		/// </summary>
		public virtual void ClearNotifications ()
		{}
		
		#endregion

		#region Remote Notification Methods

		/// <summary>
		/// Register to receive remote notifications via Push Notification service.
		/// </summary>
		public virtual void RegisterForRemoteNotifications ()
		{}

		/// <summary>
		/// Unregister for all remote notifications received via Push Notification service.
		/// </summary>
		public virtual void UnregisterForRemoteNotifications ()
		{}

		#endregion
	}
}
#endif