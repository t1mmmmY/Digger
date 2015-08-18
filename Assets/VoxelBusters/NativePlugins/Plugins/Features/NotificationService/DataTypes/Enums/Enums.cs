using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	/// <summary>
	/// Specifies the types of notifications the app supports.
	/// </summary>
	[System.Flags]
	public enum NotificationType
	{
		/// <summary>
		/// Notification is a badge on application's icon
		/// </summary>
		Badge	= 1, 

		/// <summary>
		/// Notification is an alert sound.
		/// </summary>
		Sound,

		/// <summary>
		/// Notification is an alert message.
		/// </summary>
		Alert 	= 4
	}
}
