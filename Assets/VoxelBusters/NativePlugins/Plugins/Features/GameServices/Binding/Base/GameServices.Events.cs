using UnityEngine;
using System.Collections;
using System;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins
{
	public partial class GameServices : MonoBehaviour 
	{
		#region Delegates

		public delegate void GameServiceViewClosed ();

		#endregion

		#region Events
		
		private 		Action<AchievementDescription[]> 	m_loadAchievementDescriptionCallback;
		protected		GameServiceViewClosed				m_showLeaderboardViewFinished;
		protected		GameServiceViewClosed				m_showAchievementViewFinished;

		#endregion

		#region Callback Methods

		protected void ShowLeaderboardViewFinished (string _msg)
		{
			// Resume unity player
			this.ResumeUnity();

			// Send callback
			if (m_showLeaderboardViewFinished != null)
				m_showLeaderboardViewFinished();
		}

		protected void ShowAchievementViewFinished (string _msg)
		{
			// Resume unity player
			this.ResumeUnity();
			
			// Send callback
			if (m_showAchievementViewFinished != null)
				m_showAchievementViewFinished();
		}

		#endregion
	}
}