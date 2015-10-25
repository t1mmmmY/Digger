using UnityEngine;
using System.Collections;

#if USES_GAME_SERVICES
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
		{
			// Initialise
			SetLeaderboardIDCollection(NPSettings.GameServicesSettings.LeaderboardIDCollection);
			SetAchievementIDCollection(NPSettings.GameServicesSettings.AchievementIDCollection);
		}

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
		/// Sets the leaderboard identifier information of all the targetted platforms.
		/// </summary>
		/// <param name="_newCollection">The list of identifiers information of all the targetted platforms.</param>
		public void SetLeaderboardIDCollection (params IDContainer[] _newCollection)
		{
			GameServicesIDHandler.SetLeaderboardIDCollection(_newCollection);
		}

		/// <summary>
		/// Creates an instance of <see cref="VoxelBusters.NativePlugins.Leaderboard"/>.
		/// </summary>
		/// <returns>An initialized <see cref="VoxelBusters.NativePlugins.Leaderboard"/> instance.</returns>
		/// <param name="_leaderboardID">A string used to uniquely identify <see cref="VoxelBusters.NativePlugins.Leaderboard"/> specific to current platform.</param>
		public Leaderboard CreateLeaderboardWithID (string _leaderboardID)
		{
			string	_leaderboardGID	= GameServicesIDHandler.GetLeaderboardGID(_leaderboardID);

			return CreateLeaderboard(_leaderboardGID, _leaderboardID);
		}

		/// <summary>
		/// Creates an instance of <see cref="VoxelBusters.NativePlugins.Leaderboard"/>.
		/// </summary>
		/// <returns>An initialized <see cref="VoxelBusters.NativePlugins.Leaderboard"/> instance.</returns>
		/// <param name="_leaderboardGID">A string used to uniquely identify <see cref="VoxelBusters.NativePlugins.Leaderboard"/> across all supported platforms.</param>
		public Leaderboard CreateLeaderboardWithGlobalID (string _leaderboardGID)
		{
			string	_leaderboardID	= GameServicesIDHandler.GetLeaderboardID(_leaderboardGID);

			return CreateLeaderboard(_leaderboardGID, _leaderboardID);
		}

		protected virtual Leaderboard CreateLeaderboard (string _leaderboardGID, string _leaderboardID)
		{
			return null;
		}

		#endregion

		#region Achievement Description Methods

		/// <summary>
		/// Loads the achievement descriptions from game service server.
		/// </summary>
		/// <param name="_onCompletion">Callback to be called when request completes.</param>
		public void LoadAchievementDescriptions (AchievementDescription.LoadAchievementDescriptionsCompletion _onCompletion)
		{
			LoadAchievementDescriptions(true, _onCompletion);
		}

		protected virtual void LoadAchievementDescriptions (bool _needsVerification, AchievementDescription.LoadAchievementDescriptionsCompletion _onCompletion)
		{
			// Cache callback
			LoadAchievementDescriptionsFinishedEvent = _onCompletion;

			// Verify user authentication state before proceeding
			if (_needsVerification && !VerifyUser())
			{
				LoadAchievementDescriptionsFinished(null, Constants.kGameServicesUserAuthMissingError);
				return;
			}
		}

		#endregion

		#region Achievement Methods
		
		/// Sets the achievement identifier information of all the targetted platforms.
		/// </summary>
		/// <param name="_newCollection">The list of identifiers information of all the targetted platforms.</param>
		public void SetAchievementIDCollection (params IDContainer[] _newCollection)
		{
			GameServicesIDHandler.SetAchievementIDCollection(_newCollection);
		}
		
		/// <summary>
		/// Creates an instance of <see cref="VoxelBusters.NativePlugins.Achievement"/>.
		/// </summary>
		/// <returns>An initialized <see cref="VoxelBusters.NativePlugins.Achievement"/> instance.</returns>
		/// <param name="_achievementID">A string used to uniquely identify <see cref="VoxelBusters.NativePlugins.Achievement"/> specific to current platform.</param>
		public Achievement CreateAchievementWithID (string _achievementID)
		{
			string	_achievementGID	= GameServicesIDHandler.GetAchievementGID(_achievementID);
			
			return CreateAchievement(_achievementGID, _achievementID);
		}
		
		/// <summary>
		/// Creates an instance of <see cref="VoxelBusters.NativePlugins.Achievement"/>.
		/// </summary>
		/// <returns>An initialized <see cref="VoxelBusters.NativePlugins.Achievement"/> instance.</returns>
		/// <param name="_achievementGID">A string used to uniquely identify <see cref="VoxelBusters.NativePlugins.Achievement"/> across all supported platforms.</param>
		public Achievement CreateAchievementWithGlobalID (string _achievementGID)
		{
			string	_achievementID	= GameServicesIDHandler.GetAchievementID(_achievementGID);
			
			return CreateAchievement(_achievementGID, _achievementID);
		}

		protected virtual Achievement CreateAchievement (string _achievementGID, string _achievementID)
		{
			return null;
		}

		/// <summary>
		/// Loads previously submitted achievement progress for the local player.
		/// </summary>
		/// <param name="_onCompletion">Callback to be called when the request is completed.</param>
		public virtual void LoadAchievements (Achievement.LoadAchievementsCompletion _onCompletion)
		{
			// Cache callback
			LoadAchievementsFinishedEvent = _onCompletion;

			// Verify auth status
			if (!VerifyUser())
			{
				LoadAchievementsFinished(null, Constants.kGameServicesUserAuthMissingError);
				return;
			}
		}

		/// <summary>
		/// Reports the player’s progress for given <see cref="VoxelBusters.NativePlugins.Achievement"/> identifier.
		/// </summary>
		/// <param name="_achievementID">A string used to uniquely identify <see cref="VoxelBusters.NativePlugins.Achievement"/> specific to current platform.</param>
		/// <param name="_pointsScored">Value indicates how far the player has progressed.</param>
		/// <param name="_onCompletion">Callback to be called when operation is completed.</param>
		public void ReportProgressWithID (string _achievementID, int _pointsScored, Achievement.ReportProgressCompletion _onCompletion)
		{
			string	_achievementGID	= GameServicesIDHandler.GetAchievementGID(_achievementID);

			// Invoke handler
			ReportProgress(_achievementGID, _achievementID, _pointsScored, _onCompletion);
		}

		/// <summary>
		/// Reports the player’s progress for given <see cref="VoxelBusters.NativePlugins.Achievement"/> identifier.
		/// </summary>
		/// <param name="_achievementGID">A string used to uniquely identify <see cref="VoxelBusters.NativePlugins.Achievement"/> across all supported platforms.</param>
		/// <param name="_pointsScored">Value indicates how far the player has progressed.</param>
		/// <param name="_onCompletion">Callback to be called when operation is completed.</param>
		public void ReportProgressWithGlobalID (string _achievementGID, int _pointsScored, Achievement.ReportProgressCompletion _onCompletion)
		{
			string	_achievementID	= GameServicesIDHandler.GetAchievementID(_achievementGID);

			// Invoke handler
			ReportProgress(_achievementGID, _achievementID, _pointsScored, _onCompletion);
		}

		private void ReportProgress (string _achievementGID, string _achievementID, int _pointsScored, Achievement.ReportProgressCompletion _onCompletion)
		{
			Achievement	_newAchievement 	= CreateAchievement(_achievementGID, _achievementID);

			if (_newAchievement == null)
			{
				DebugPRO.Console.LogError(Constants.kDebugTag, "[GameServices] Failed to report progress.");
				
				if (_onCompletion != null)
					_onCompletion(false, "The requested operation could not be completed because Game Service failed to create Achievement object.");
				
				return;
			}

			// Set the points scored
			_newAchievement.PointsScored	= _pointsScored;

			// Report
			_newAchievement.ReportProgress(_onCompletion);
		}

		#endregion

		#region User Methods

		/// <summary>
		/// Loads the user info from game service server.
		/// </summary>
		/// <param name="_userIDs">Array of id's whose information needs to be loaded.</param>
		/// <param name="_onCompletion">Callback to be called when operation is completed.</param>
		public virtual void LoadUsers (string[] _userIDs, User.LoadUsersCompletion _onCompletion)
		{
			// Cache callback
			LoadUsersFinishedEvent = _onCompletion;

			// Verify auth status
			if (!VerifyUser())
			{
				LoadUsersFinished(null, Constants.kGameServicesUserAuthMissingError);
				return;
			}

			// Check if user id's are valid
			if (_userIDs == null)
			{
				DebugPRO.Console.LogError(Constants.kDebugTag, "[GameServices] UserID list is null.");
				LoadUsersFinished(null, "The requested operation could not be completed because user id list is null.");
				return;
			}
		}

		#endregion

		#region Score Methods

		protected virtual Score CreateScoreForLocalUser (string _leaderboardGID, string _leaderboardID)
		{
			return null;
		}

		/// <summary>
		/// Reports the score to game service server.
		/// </summary>
		/// <param name="_leaderboardID">A string used to uniquely identify <see cref="VoxelBusters.NativePlugins.Leaderboard"/> specific to current platform.</param>
		/// <param name="_score">The score earned by <see cref="VoxelBusters.NativePlugins.LocalUser"/></param>
		/// <param name="_onCompletion">Callback to be called when operation is completed.</param>
		public void ReportScoreWithID (string _leaderboardID, long _score, Score.ReportScoreCompletion _onCompletion)
		{
			string	_leaderboardGID	= GameServicesIDHandler.GetLeaderboardGID(_leaderboardID);

			// Invoke handler
			ReportScore(_leaderboardGID, _leaderboardID, _score, _onCompletion);
		}

		/// <summary>
		/// Reports the score to game service server.
		/// </summary>
		/// <param name="_leaderboardGID">A string used to uniquely identify <see cref="VoxelBusters.NativePlugins.Leaderboard"/> across all supported platforms.</param>
		/// <param name="_score">The score earned by <see cref="VoxelBusters.NativePlugins.LocalUser"/></param>
		/// <param name="_onCompletion">Callback to be called when operation is completed.</param>
		public void ReportScoreWithGlobalID (string _leaderboardGID, long _score, Score.ReportScoreCompletion _onCompletion)
		{
			string	_leaderboardID	= GameServicesIDHandler.GetLeaderboardID(_leaderboardGID);

			// Invoke handler
			ReportScore(_leaderboardGID, _leaderboardID, _score, _onCompletion);
		}

		private void ReportScore (string _leaderboardGID, string _leaderboardID, long _score, Score.ReportScoreCompletion _onCompletion)
		{
			Score	_newScore		= CreateScoreForLocalUser(_leaderboardGID, _leaderboardID);

			if (_newScore == null)
			{
				DebugPRO.Console.LogError(Constants.kDebugTag, "[GameServices] Failed to report score.");

				if (_onCompletion != null)
					_onCompletion(false, "The requested operation could not be completed because Game Service failed to create Score object.");
				
				return;
			}

			// Set the new score value
			_newScore.Value			= _score;

			// Report
			_newScore.ReportScore(_onCompletion);
		}

		#endregion

		#region UI Methods

		/// <summary>
		/// Shows the default achievements UI.
		/// </summary>
		/// <param name="_onCompletion">Callback called when Achievements view is closed.</param>
		public virtual void ShowAchievementsUI (GameServiceViewClosed _onCompletion)
		{
			// Cache callback
			ShowAchievementViewFinishedEvent	= _onCompletion;
			
			// Pause unity
			this.PauseUnity();
			
			// Check if valid account
			if (!VerifyUser())
			{
				ShowAchievementViewFinished(Constants.kGameServicesUserAuthMissingError);
				return;
			}
		}

		/// <summary>
		/// Show the leaderboard UI with a specific time scope.
		/// </summary>
		/// <param name="_leaderboardID">A string used to uniquely identify <see cref="VoxelBusters.NativePlugins.Leaderboard"/> specific to current platform.</param>
		/// <param name="_timeScope">Time scope initially used while showing leaderboard UI.</param>
		/// <param name="_onCompletion">Callback called when Leaderboard view is closed.</param>
		public virtual void ShowLeaderboardUIWithID (string _leaderboardID, eLeaderboardTimeScope _timeScope, GameServiceViewClosed _onCompletion)
		{
			// Cache callback
			ShowLeaderboardViewFinishedEvent	= _onCompletion;

			// Pause unity
			this.PauseUnity();

			// Check if valid account
			if (!VerifyUser())
			{
				ShowLeaderboardViewFinished(Constants.kGameServicesUserAuthMissingError);
				return;
			}
		}

		/// <summary>
		/// Show the leaderboard UI with a specific time scope.
		/// </summary>
		/// <param name="_leaderboardGID">A string used to uniquely identify <see cref="VoxelBusters.NativePlugins.Leaderboard"/> across all supported platforms.</param>
		/// <param name="_timeScope">Time scope initially used while showing leaderboard UI.</param>
		/// <param name="_onCompletion">Callback called when Leaderboard view is closed.</param>
		public void ShowLeaderboardUIWithGlobalID (string _leaderboardGID, eLeaderboardTimeScope _timeScope, GameServiceViewClosed _onCompletion)
		{
			string	_leaderboardID	= GameServicesIDHandler.GetLeaderboardID(_leaderboardGID);

			ShowLeaderboardUIWithID(_leaderboardID, _timeScope, _onCompletion);
		}

		#endregion

		#region Misc. Methods

		protected bool VerifyUser ()
		{
			if (LocalUser.IsAuthenticated)
				return true;

			DebugPRO.Console.LogError(Constants.kDebugTag, "[GameServices] User not authenticated.");
			return false;
		}

		#endregion

		#region Deprecated Methods

		[System.Obsolete("This method is deprecated. Instead use CreateLeaderboardWithID.")]
		public Leaderboard CreateLeaderboard (string _leaderboardID)
		{
			return CreateLeaderboardWithID(_leaderboardID);
		}

		[System.Obsolete("This method is deprecated. Instead use CreateAchievementWithID.")]
		public Achievement CreateAchievement (string _achievementID)
		{
			return CreateAchievementWithID(_achievementID);
		}

		[System.Obsolete("This method is deprecated. Instead use ReportProgressWithID.")]
		public void ReportProgress (string _achievementID, int _pointsScored, Achievement.ReportProgressCompletion _onCompletion)
		{
			ReportProgressWithID(_achievementID, _pointsScored, _onCompletion);
		}

		[System.Obsolete("This method is deprecated. Instead use ReportProgressWithID.")]
		public void ReportScore (string _leaderboardID, long _score, Score.ReportScoreCompletion _onCompletion)
		{
			ReportScoreWithID(_leaderboardID, _score, _onCompletion);
		}

		[System.Obsolete("This method is deprecated. Instead use ShowLeaderboardUIWithID.")]
		public void ShowLeaderboardUI (string _leaderboardID, eLeaderboardTimeScope _timeScope, GameServiceViewClosed _onCompletion)
		{
			ShowLeaderboardUIWithID(_leaderboardID, _timeScope, _onCompletion);
		}

		#endregion
	}
}
#endif