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

	/// <summary>
	/// The interval at which the notification has to be rescheduled.
	/// </summary>
	public enum eNotificationRepeatInterval
	{
		/// <summary>
		/// The system fires the notification once and then discards it.
		/// </summary>
		NONE	= 0, 

		/// <summary>
		/// The system reschedules the notification delivery for every minute.
		/// </summary>
		MINUTE,

		/// <summary>
		/// The system reschedules the notification delivery for every hour.
		/// </summary>
		HOUR,

		/// <summary>
		/// The system reschedules the notification delivery for every day.
		/// </summary>
		DAY,

		/// <summary>
		/// The system reschedules the notification delivery for every week.
		/// </summary>
		WEEK,

		/// <summary>
		/// The system reschedules the notification delivery for every month.
		/// </summary>
		MONTH,

		/// <summary>
		/// The system reschedules the notification delivery for every year.
		/// </summary>
		YEAR
	}
}