using UnityEngine;
using System.Collections;

//Add these Namespaces
using VoxelBusters.NativePlugins;
using VoxelBusters.Utility;

public class RemoteNotificationTest : MonoBehaviour 
{
	[SerializeField, EnumMaskField(typeof(NotificationType))]
	private NotificationType	m_notificationType;


	void Start()
	{
		NPBinding.NotificationService.RegisterNotificationTypes(m_notificationType);
	}
	void OnEnable ()
	{
		// Register RemoteNotificated related callbacks
		NotificationService.DidFinishRegisterForRemoteNotificationEvent	+= DidFinishRegisterForRemoteNotificationEvent;
		NotificationService.DidReceiveRemoteNotificationEvent			+= DidReceiveRemoteNotificationEvent;

		//Add below for local notification
		//NotificationService.DidReceiveLocalNotificationEvent 			+= DidReceiveLocalNotificationEvent;
		
	}
	
	void OnDisable ()
	{
		// Un-Register from callbacks
		NotificationService.DidFinishRegisterForRemoteNotificationEvent	-= DidFinishRegisterForRemoteNotificationEvent;
		NotificationService.DidReceiveRemoteNotificationEvent			-= DidReceiveRemoteNotificationEvent;
		
		//Add below for local notification
		//NotificationService.DidReceiveLocalNotificationEvent 			-= DidReceiveLocalNotificationEvent;

	}
	

	
	// Update is called once per frame
	void OnGUI () 
	{
		if(GUILayout.Button("Register for Remote Notifications", GUILayout.Width(Screen.width/2f),  GUILayout.Height(Screen.height * 0.2f)))
		{
			NPBinding.NotificationService.RegisterForRemoteNotifications(); //This triggers a event. so capture it by registering to that event.
		}

	}


	#region API Callbacks
	
	private void DidReceiveLocalNotificationEvent (CrossPlatformNotification _notification)
	{
		Debug.Log("Received DidReceiveLocalNotificationEvent : " + _notification.ToString());
	}
	
	private void DidReceiveRemoteNotificationEvent (CrossPlatformNotification _notification)
	{
		Debug.Log("Received DidReceiveRemoteNotificationEvent : " + _notification.ToString());
	}
	
	private void DidFinishRegisterForRemoteNotificationEvent (string _deviceToken, string _error)
	{
		if(string.IsNullOrEmpty(_error))
		{
			Debug.Log("Device Token : " + _deviceToken);
		}
		else
		{
			Debug.Log("Error in registering for remote notifications : " + _deviceToken);
		}
	}

	#endregion
}