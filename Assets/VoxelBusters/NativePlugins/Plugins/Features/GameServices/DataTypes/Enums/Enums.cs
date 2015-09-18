using UnityEngine;
using System.Collections;
using System;

namespace VoxelBusters.NativePlugins
{
	/// <summary>
	/// The scope of <see cref="VoxelBusters.NativePlugins.User"/> to be searched for scores.
	/// </summary>
	[Serializable]
	public enum eLeaderboardUserScope
	{
		/// <summary> All the user's are considered for search. </summary>
		GLOBAL,
		/// <summary> Only friends of <see cref="VoxelBusters.NativePlugins.LocalUser"/> are considered for search. </summary>
		FRIENDS_ONLY
	}

	/// <summary>
	/// The period of time to which user's best <see cref="VoxelBusters.NativePlugins.Score"/> are restricted.
	/// </summary>
	[Serializable]
	public enum eLeaderboardTimeScope
	{
		/// <summary> Best score of all user's recorded in past 24hrs is returned. </summary>
		TODAY,
		/// <summary> Best score of all user's recorded in past week is returned. </summary>
		WEEK,
		/// <summary> Best score of all user's recorded is returned. </summary>
		ALL_TIME
	}

	/// <summary>
	/// Direction constants for pagination over data sets.
	/// </summary>
	[Serializable]
	public enum eLeaderboardPageDirection
	{
		/// <summary> Direction advancing toward the end of the data. </summary>
		NEXT,
		/// <summary> Direction advancing toward the beginning of the data. </summary>
		PREVIOUS
	}
}