using UnityEngine;
using System.Collections;

#if USES_NOTIFICATION_SERVICE && UNITY_IOS
using System.Collections.Generic;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	public partial class NotificationServiceIOS : NotificationService
	{
		#region Constants

		private		const		string		 kAppLaunchLocalNotification		= "launch-local-notification";
		private		const		string		 kAppLaunchRemoteNotification		= "launch-remote-notification";

		#endregion

		#region Parse Methods

		protected override void ParseAppLaunchInfo (string _launchData, out CrossPlatformNotification _launchLocalNotification, out CrossPlatformNotification _launchRemoteNotification)
		{
			Dictionary<string, object> 	_launchDataDict		= JSONUtility.FromJSON(_launchData) as Dictionary<string, object>;
			object						_payloadDict		= null;

			// Launched with local notification
			if (_launchDataDict.TryGetValue(kAppLaunchLocalNotification, out _payloadDict))
			{
				_launchLocalNotification	= new iOSNotificationPayload((IDictionary)_payloadDict);
				_launchRemoteNotification	= null;
			}
			// Launched with remote notification
			else if (_launchDataDict.TryGetValue(kAppLaunchRemoteNotification, out _payloadDict))
			{
				_launchLocalNotification	= null;
				_launchRemoteNotification	= new iOSNotificationPayload((IDictionary)_payloadDict);
			}
			// Normal launch
			else
			{
				_launchLocalNotification	= null;
				_launchRemoteNotification	= null;
			}
		}

		#endregion
	}
}
#endif