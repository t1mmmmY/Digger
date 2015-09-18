using UnityEngine;
using System.Collections;
using System;
using VoxelBusters.DebugPRO;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	/// <summary>
	/// <see cref="VoxelBusters.NativePlugins.GameServices"/> object provides interface to social game features like <see cref="VoxelBusters.NativePlugins.Leaderboard"/>, <see cref="VoxelBusters.NativePlugins.Achievement"/>. 
	/// It makes use of GameCenter for iOS and Google Play Services for Android.
	/// </summary>
	public partial class GameServices : MonoBehaviour 
	{
		#region Properties

		/// <summary>
		/// Gets or sets the shared instance of the local user.
		/// </summary>
		/// <value>Retrieves the shared instance of the local user.</value>
		public virtual LocalUser LocalUser
		{
			get
			{
				return null;
			}

			protected set
			{
				throw new Exception("[GameServices] Only getter is supported.");
			}
		}

		#endregion

		#region Unity Methods

		protected virtual void Awake ()
		{}

		#endregion

		#region Methods

		/// <summary>
		/// Determines whether <see cref="VoxelBusters.NativePlugins.GameServices"/> feature is supported in current platform.
		/// </summary>
		/// <returns><c>true</c> if this <see cref="VoxelBusters.NativePlugins.GameServices"/> is available; otherwise, <c>false</c>.</returns>
		public virtual bool IsAvailable ()
		{
			return false;
		}

		#endregion

		#region Leaderboard Methods

		/// <summary>
		/// Creates an instance of <see cref="VoxelBusters.NativePlugins.Leaderboard"/>.
		/// </summary>
		/// <returns>An initialized <see cref="VoxelBusters.NativePlugins.Leaderboard"/> instance.</returns>
		/// <param name="_leaderboardID">A string used to uniquely identify the specific <see cref="VoxelBusters.NativePlugins.Leaderboard"/> object refers to.</param>
		public virtual Leaderboard CreateLeaderboard (string _leaderboardID)
		{
			return null;
		}

		#endregion

		#region Achievement Methods

		/// <summary>
		/// Creates an instance of <see cref="VoxelBusters.NativePlugins.Achievement"/>.
		/// </summary>
		/// <returns>An initialized <see cref="VoxelBusters.NativePlugins.Achievement"/> instance.</returns>
		/// <param name="_achievementID">A string used to uniquely identify the specific <see cref="VoxelBusters.NativePlugins.Achievement"/> object refers to.</param>
		public virtual Achievement CreateAchievement (string _achievementID)
		{
			return null;
		}

		/// <summary>
		/// Loads the achievement descriptions from game service server.
		/// </summary>
		/// <param name="_onCompletion">Callback to be called when request completes.</param>
		public void LoadAchievementDescriptions (Action<AchievementDescription[]> _onCompletion)
		{
			LoadAchievementDescriptions(true, _onCompletion);
		}

		protected virtual void LoadAchievementDescriptions (bool _needsVerification, Action<AchievementDescription[]> _onCompletion)
		{
			// Cache callback
			m_loadAchievementDescriptionCallback	= _onCompletion;
		}
		
		protected void OnLoadAchievementDescriptionsFinished (AchievementDescription[] _descriptionList)
		{
			// Notify Achievement Handler about new description list
			AchievementHandler.SetAchievementDescriptionList(_descriptionList);
			
			// Completion handler is called
			if (m_loadAchievementDescriptionCallback != null)
				m_loadAchievementDescriptionCallback(_descriptionList);
		}

		/// <summary>
		/// Loads previously submitted achievement progress for the local player.
		/// </summary>
		/// <param name="_onCompletion">Callback to be called when the request is completed.</param>
		public virtual void LoadAchievements (Action<Achievement[]> _onCompletion)
		{}

		/// <summary>
		/// Reports the player’s progress for given <see cref="VoxelBusters.NativePlugins.Achievement"/> identifier.
		/// </summary>
		/// <param name="_achievementID">A string used to uniquely identify the specific <see cref="VoxelBusters.NativePlugins.Achievement"/> object refers to.</param>
		/// <param name="_pointsScored">Value indicates how far the player has progressed.</param>
		/// <param name="_onCompletion">Callback to be called when operation is completed.</param>
		public virtual void ReportProgress (string _achievementID, int _pointsScored, Action<bool> _onCompletion)
		{
			// Check if identifier is valid
			if (string.IsNullOrEmpty(_achievementID))
			{
				DebugPRO.Console.LogError(Constants.kDebugTag, "[GameServices] Achievement identifier cant be null/empty.");
				
				if (_onCompletion != null)
					_onCompletion(false);
				
				return;
			}
		}

		#endregion

		#region User Methods

		/// <summary>
		/// Loads the user info from game service server.
		/// </summary>
		/// <param name="_userIDs">Array of id's whose information needs to be loaded.</param>
		/// <param name="_onCompletion">Callback to be called when operation is completed.</param>
		public virtual void LoadUsers (string[] _userIDs, Action<User[]> _onCompletion)
		{
			// Check if user id's are valid
			if (_userIDs == null)
			{
				DebugPRO.Console.LogError(Constants.kDebugTag, "[GameServices] UserID list is null.");

				if (_onCompletion != null)
					_onCompletion(null);

				return;
			}
		}

		/// <summary>
		/// Reports the score to game service server.
		/// </summary>
		/// <param name="_leaderboardID">A string used to uniquely identify the specific <see cref="VoxelBusters.NativePlugins.Leaderboard"/> object refers to.</param>
		/// <param name="_score">The score earned by <see cref="VoxelBusters.NativePlugins.LocalUser"/></param>
		/// <param name="_onCompletion">Callback to be called when operation is completed.</param>
		public virtual void ReportScore (string _leaderboardID, long _score, Action<bool> _onCompletion)
		{
			// Check if leaderboard identifier is valid
			if (string.IsNullOrEmpty(_leaderboardID))
			{
				DebugPRO.Console.LogError(Constants.kDebugTag, "[GameServices] Leaderboard identifier cant be null/empty.");
				
				if (_onCompletion != null)
					_onCompletion(false);
				
				return;
			}
		}

		#endregion

		#region UI Methods

		/// <summary>
		/// Show the default banner when <see cref="VoxelBusters.NativePlugins.Achievement"/> is completed.
		/// </summary>
		/// <param name="_canShow">If set to <c>true</c> default banner is showed on achievement completion.</param>
		public virtual void ShowDefaultAchievementCompletionBanner (bool _canShow)
		{}

		/// <summary>
		/// Shows the default achievements UI.
		/// </summary>
		/// <param name="_onCompletion">Callback called when Achievements view is closed.</param>
		public virtual void ShowAchievementsUI (GameServiceViewClosed _onCompletion)
		{
			// Cache callback
			m_showAchievementViewFinished	= _onCompletion;
			
			// Pause unity
			this.PauseUnity();
			
			// Check if valid account
			if (!VerifyUser())
			{
				ShowAchievementViewFinished(null);
				return;
			}
		}

		/// <summary>
		/// Show the leaderboard UI with a specific leaderboard shown initially with a specific time scope.
		/// </summary>
		/// <param name="_leaderboardID">A string used to uniquely identify the specific <see cref="VoxelBusters.NativePlugins.Leaderboard"/> object refers to.</param>
		/// <param name="_timeScope">Time scope initially used while showing leaderboard UI.</param>
		/// <param name="_onCompletion">Callback called when Leaderboard view is closed.</param>
		public virtual void ShowLeaderboardUI (string _leaderboardID, eLeaderboardTimeScope _timeScope, GameServiceViewClosed _onCompletion)
		{
			// Cache callback
			m_showLeaderboardViewFinished	= _onCompletion;

			// Pause unity
			this.PauseUnity();

			// Check if learboard identifier is valid
			if (string.IsNullOrEmpty(_leaderboardID))
			{
				DebugPRO.Console.LogError(Constants.kDebugTag, "[GameServices] Leaderboard identifier cant be null/empty.");
				ShowLeaderboardViewFinished(null);
				return;
			}

			// Check if valid account
			if (!VerifyUser())
			{
				ShowLeaderboardViewFinished(null);
				return;
			}
		}

		#endregion

		#region Methods

		protected bool VerifyUser ()
		{
			if (LocalUser.IsAuthenticated)
				return true;

			DebugPRO.Console.LogError(Constants.kDebugTag, "[GameServices] User not authenticated.");
			return false;
		}

		#endregion
	}
}