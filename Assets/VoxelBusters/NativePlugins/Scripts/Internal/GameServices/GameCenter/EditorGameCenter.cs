using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.IO;
using VoxelBusters.Utility;
using VoxelBusters.DebugPRO;
using UnityEngine.SocialPlatforms;

namespace VoxelBusters.NativePlugins.Internal
{
	[Serializable]
	internal partial class EditorGameCenter : AdvancedScriptableObject <EditorGameCenter>
	{
		#region Fields

		// Related to User
		[SerializeField]
		private					List<EGCUser>						m_registeredUsers				= new List<EGCUser>();
		
		[NonSerialized]
		private					EGCLocalUser						m_localUser;

		// Related to Leaderboard
		[SerializeField]
		private					List<EGCLeaderboard>				m_leaderboardList				= new List<EGCLeaderboard>();

		// Related to Achievement
		[SerializeField]
		private					List<EGCAchievement>				m_achievementsList				= new List<EGCAchievement>();
		
		[SerializeField]
		private					List<EGCAchievementDescription>		m_achievementDescriptionList	= new List<EGCAchievementDescription>();

		// Related to UI
		[NonSerialized]
		private					EditorGameCenterUI					m_gameCenterUI					= null;

		[NonSerialized]
		private					bool								m_canShowAchievementBanner		= true;

		#endregion

		#region Methods

		public string[] GetLeaderboardIDList ()
		{
			string[] 	_idList		= new string[m_leaderboardList.Count];
			int 		_iter		= 0;

			foreach (EGCLeaderboard _gcLeaderboard in m_leaderboardList)
				_idList[_iter++]	= _gcLeaderboard.Identifier;

			return _idList;
		}

		public string[] GetAchievementIDList ()
		{
			string[] 	_idList		= new string[m_achievementDescriptionList.Count];
			int 		_iter		= 0;
			
			foreach (EGCAchievementDescription _gcAchievement in m_achievementDescriptionList)
				_idList[_iter++]	= _gcAchievement.Identifier;
			
			return _idList;
		}

		#endregion

		#region Accounts Methods

		public void Authenticate (Action<EditorLocalUser> _onCompletion)		
		{
			// Check if user has already authenticated
			if (m_localUser != null && m_localUser.IsAuthenticated)
			{
				if (_onCompletion != null)
					_onCompletion(m_localUser.GetEditorFormatData());

				return;
			}

			// As user isnt logged in, show login prompt
			NPBinding.UI.ShowLoginPromptDialog("Editor Game Center", "Login to start using Game Center.", "user identifier", "password", new string[] { "Log in", "Cancel"}, (string _button, string _loginID, string _password)=>{

				bool _failedToLogin	= false;

				if (_button.Equals("Cancel"))
				{
					DebugPRO.Console.LogWarning(Constants.kDebugTag, "[GameServices] User cancelled login prompt.");
					_failedToLogin	= true;
				}
				else if (string.IsNullOrEmpty(_loginID))
				{
					DebugPRO.Console.LogWarning(Constants.kDebugTag, "[GameServices] Login identifier is null/empty.");
					_failedToLogin	= true;
				}
				else
				{
					EGCUser 	_regUserInfo		= GetUserWithID(_loginID);
					
					// Copy details of logged in user
					if (_regUserInfo	== null)
					{
						_regUserInfo				= new EGCUser(_loginID);
						
						// Add it to registered user list
						m_registeredUsers.Add(_regUserInfo);
					}
					
					// Update local user info
					m_localUser						= new EGCLocalUser(_regUserInfo, true);		
				}

				if (_failedToLogin)
				{
					if (_onCompletion != null)
						_onCompletion(null);

					return;
				}
				else
				{
					if (_onCompletion != null)
						_onCompletion(m_localUser.GetEditorFormatData());
					
					return;
				}
			});
		} 

		public void SignOut ()
		{
			if (!VerifyUser())
				return;

			// Signing out
			m_localUser.IsAuthenticated		= false;
		}

		public void LoadFriends (Action<EditorUser[]> _onCompletion)
		{
			// Check if user has logged in
			if (!VerifyUser())
			{
				if (_onCompletion != null)
					_onCompletion(null);
				
				return;
			}

			// Load all friends info
			LoadUsers(m_localUser.Info.Friends, _onCompletion);
		}

		private bool VerifyUser ()
		{
			if (m_localUser == null || !m_localUser.IsAuthenticated)
			{
				DebugPRO.Console.LogError(Constants.kDebugTag, "[GameServices] User needs to authenticate before using services.");
				return false;
			}

			return true;
		}

		#endregion

		#region User Methods
		
		public void LoadUsers (string[] _userIDList, Action<EditorUser[]> _onCompletion)
		{
			if (_onCompletion == null)
				return;

			// Check if user is authenticated
			if (!VerifyUser())
			{
				_onCompletion(null);
				return;
			}

			// Check if input is valid
			if (_userIDList == null)
			{
				DebugPRO.Console.LogError(Constants.kDebugTag, "[GameServices] Failed to user info.");
				_onCompletion(null);
				return;
			}

			// Fetch user info
			List<EGCUser> 	_userList	 	= new List<EGCUser>();
			
			foreach (string _curUserID in _userIDList)
			{
				EGCUser 	_curUserInfo	= GetUserWithID(_curUserID);
				
				if (_curUserInfo != null)
					_userList.Add(_curUserInfo);
			}
			
			_onCompletion(EGCUser.ConvertToEditorUserList(_userList.ToArray()));
		}

		public EGCUser GetUserWithID (string _id)
		{
			return m_registeredUsers.FirstOrDefault(_regUser => _regUser.Identifier.Equals(_id));
		}

		#endregion

		#region Leaderboard Methods

		public void LoadTopScores (Leaderboard _leaderboard, Action<EditorScore[], EditorScore> _onCompletion)
		{
			// Check if user has logged in
			if (!VerifyUser())
			{
				if (_onCompletion != null)
					_onCompletion(null, null);
				
				return;
			}

			// Get leaderboard
			EGCLeaderboard	_gcLeaderboard	= GetLeaderboardWithID(_leaderboard.Identifier);

			if (_gcLeaderboard == null)
			{
				if (_onCompletion != null)
					_onCompletion(null, null);
				
				return;
			}

			// Set score fetch range
			_gcLeaderboard.Range			= new Range(1, _leaderboard.MaxResults);

			// Load scores
			LoadScores(_gcLeaderboard, _leaderboard.TimeScope, _leaderboard.UserScope, _onCompletion);
		}

		public void LoadPlayerCenteredScores (Leaderboard _leaderboard, Action<EditorScore[], EditorScore> _onCompletion)
		{
			// Check if user has logged in
			if (!VerifyUser())
			{
				if (_onCompletion != null)
					_onCompletion(null, null);
				
				return;
			}

			// Get leaderboard
			EGCLeaderboard	_gcLeaderboard	= GetLeaderboardWithID(_leaderboard.Identifier);
			
			if (_gcLeaderboard == null)
			{
				if (_onCompletion != null)
					_onCompletion(null, null);
				
				return;
			}

			// Compute range based on player rank
			Score	_localUserScore			= _gcLeaderboard.GetScoreWithUserID(m_localUser.Info.Identifier);

			if (_localUserScore == null)
			{
				DebugPRO.Console.LogError(Constants.kDebugTag, "[GameServices] Local user score not found.");
				
				if (_onCompletion != null)
					_onCompletion(null, null);
				
				return;
			}

			int 	_localPlayerRank		= _localUserScore.Rank;
			int		_maxResults				= _leaderboard.MaxResults;
			int 	_loadFrom				= Mathf.Max(1, _localPlayerRank - Mathf.FloorToInt(_maxResults * 0.5f));
			_gcLeaderboard.Range			= new Range(_loadFrom, _maxResults);
			
			// Load scores
			LoadScores(_gcLeaderboard, _leaderboard.TimeScope, _leaderboard.UserScope, _onCompletion);
		}

		public void LoadMoreScores (Leaderboard _leaderboard, eLeaderboardPageDirection _pageDirection, Action<EditorScore[], EditorScore> _onCompletion)
		{
			// Check if user has logged in
			if (!VerifyUser())
			{
				if (_onCompletion != null)
					_onCompletion(null, null);
				
				return;
			}

			// Get leaderboard
			EGCLeaderboard	_gcLeaderboard	= GetLeaderboardWithID(_leaderboard.Identifier);
			
			if (_gcLeaderboard == null)
			{
				if (_onCompletion != null)
					_onCompletion(null, null);
				
				return;
			}

			// Compute score fetch range
			Range		_curRange			= _gcLeaderboard.Range;
			
			if (_curRange.from == 0)
			{
				LoadTopScores(_leaderboard, _onCompletion);
				return;
			}
			
			// Based on page direction, compute range of score to be loaded
			int			_maxResults			= _leaderboard.MaxResults;
			Range		_newRange			= new Range(0, _maxResults);
			
			if (_pageDirection == eLeaderboardPageDirection.PREVIOUS)
			{
				if (_curRange.from == 1)
				{
					if (_onCompletion != null)
						_onCompletion(null, null);
					
					return;
				}
				
				_newRange.from		= Mathf.Max(1, _curRange.from - _maxResults);
			}
			else if (_pageDirection == eLeaderboardPageDirection.NEXT)
			{
				_newRange.from		= _curRange.from + _maxResults;

				if (_newRange.from > _gcLeaderboard.Scores.Count)
				{
					if (_onCompletion != null)
						_onCompletion(null, null);

					return;
				}
			}

			// Set score fetch range
			_gcLeaderboard.Range	= _newRange;
			
			// Load scores
			LoadScores(_gcLeaderboard, _leaderboard.TimeScope, _leaderboard.UserScope, _onCompletion);
		}

		private void LoadScores (EGCLeaderboard _gcLeaderboard, eLeaderboardTimeScope _timeScope, eLeaderboardUserScope _userScope, Action<EditorScore[], EditorScore> _onCompletion)
		{
			EGCScore[] 			_filteredScoreList			= GetFilteredScoreList(_gcLeaderboard, _timeScope, _userScope);
			EGCScore 			_gcLocalUserScore			= _gcLeaderboard.GetScoreWithUserID(m_localUser.Info.Identifier);

			// Now get final list
			EditorScore[]		_formattedScoreList			= EGCScore.ConvertToEditorScoreList(_filteredScoreList);
			EditorScore			_formattedLocalUserScore	= _gcLocalUserScore == null ? null : _gcLocalUserScore.GetEditorFormatData();

			// Invoke on finished action
			if (_onCompletion != null)
				_onCompletion(_formattedScoreList, _formattedLocalUserScore);
		}
		
		public void LoadScoresWithID (Leaderboard _leaderboard, string[] _userIDs, Action<EditorScore[]> _onCompletion)
		{
			// Check if user has logged in
			if (!VerifyUser())
			{
				if (_onCompletion != null)
					_onCompletion(null);
				
				return;
			}

			// Check if user id's are valid
			if (_userIDs == null)
			{
				DebugPRO.Console.LogError(Constants.kDebugTag, "[GameServices] UserID list is null.");
				
				if (_onCompletion != null)
					_onCompletion(null);
				
				return;
			}

			// Get leaderboard
			EGCLeaderboard	_gcLeaderboard		= GetLeaderboardWithID(_leaderboard.Identifier);

			if (_gcLeaderboard == null)
			{
				if (_onCompletion != null)
					_onCompletion(null);
				
				return;
			}

			// Gather score info
			List<EGCScore>	_finalScoreList		= new List<EGCScore>();

			foreach (string _userID in _userIDs)
			{
				EGCScore   	_userScore			= _gcLeaderboard.Scores.First(_score => _score.User.Identifier.Equals(_userID));
				
				if (_userScore == null)
				{
					DebugPRO.Console.LogWarning(Constants.kDebugTag, string.Format("[GameServices] Couldnt find score of User with ID={0}.", _userID));
				}
				else
				{
					_finalScoreList.Add(_userScore);
				}
			}
			
			if (_onCompletion != null)
				_onCompletion(EGCScore.ConvertToEditorScoreList(_finalScoreList.ToArray()));
		}

		public void ReportScore (Score _userScore, Action<EditorScore> _onCompletion)
		{
			// Couldnt verify user
			if (!VerifyUser())
			{
				// Send callback
				if (_onCompletion != null)
					_onCompletion(null);

				return;
			}

			// Get leaderboard info
			EGCLeaderboard	_gcLeaderboard		= GetLeaderboardWithID(_userScore.LeaderboardID);

			if (_gcLeaderboard == null)
			{
				if (_onCompletion != null)
					_onCompletion(null);

				return;
			}

			// Add this new score info
			string			_reportedUserID		= _userScore.User.Identifier;
			List<EGCScore> 	_scoreHistory		= _gcLeaderboard.Scores;
			int 			_existingScoreEntry	= _scoreHistory.FindIndex(_score => _score.User.Identifier.Equals(_reportedUserID));
			EGCScore		_newScoreEntry		= new EGCScore(_userScore.LeaderboardID, _reportedUserID, _userScore.Value);
			
			// Dint find any score record for this user
			if (_existingScoreEntry != -1)
				_scoreHistory.RemoveAt(_existingScoreEntry);
			
			// Add new entry and sort the list
			_scoreHistory.Add(_newScoreEntry);
			_scoreHistory.Sort(CompareScore);
			
			// Update Ranks
			for (int _iter = 0; _iter < _scoreHistory.Count; _iter++)
				(_scoreHistory[_iter] as EGCScore).SetRank(_iter + 1);
			
			// Complete action on finishing task
			if (_onCompletion != null)
				_onCompletion(_newScoreEntry.GetEditorFormatData());
		}

		private EGCScore[] GetFilteredScoreList (EGCLeaderboard _gcLeaderboard, eLeaderboardTimeScope _timeScope, eLeaderboardUserScope _userScope)
		{	
			List<EGCScore> 	_gcScores			= _gcLeaderboard.Scores;

			// User scope based filtering
			List<EGCScore>	_usScoreList		= new List<EGCScore>();

			if (_userScope == eLeaderboardUserScope.GLOBAL)
			{
				_usScoreList.AddRange(_gcScores);
			}
			else
			{
				string[]		_friendIDList	= m_localUser.Info.Friends;

				foreach (EGCScore _curScore in _gcScores)
				{
					string		_curUserID		= _curScore.User.Identifier;
					
					if (_friendIDList.Any(_curFriendID => _curFriendID.Equals(_curUserID)))
						_usScoreList.Add(_curScore);
				}
			}

			// Time scope based filtering
			List<EGCScore>	_tsScoreList		= new List<EGCScore>();
			
			if (_timeScope == eLeaderboardTimeScope.ALL_TIME)
			{
				_tsScoreList.AddRange(_usScoreList);
			}
			else
			{
				TimeSpan 	_timespan;
				
				if (_timeScope == eLeaderboardTimeScope.TODAY)
					_timespan					= TimeSpan.FromDays(1);
				else
					_timespan					= TimeSpan.FromDays(7);

				long		_intervalStartTick	= DateTime.Now.Subtract(_timespan).Ticks;
				long		_intervalEndTick	= DateTime.Now.Ticks;
				
				foreach (EGCScore _curScore in _usScoreList)
				{
					long	_curScoreTicks		= _curScore.Date.Ticks;
					
					if (_curScoreTicks >= _intervalStartTick && _curScoreTicks <= _intervalEndTick)
						_tsScoreList.Add(_curScore);
				}
			}

			// Now get elements based on range
			Range			_range				= _gcLeaderboard.Range;
			List<EGCScore>	_finalScore			= new List<EGCScore>();
			int				_startIndex			= _range.from - 1;
			int				_endIndex			= _startIndex + _range.count;

			for (int _iter = _startIndex; (_iter < _tsScoreList.Count && _iter < _endIndex); _iter++)
				_finalScore.Add(_tsScoreList[_iter]);

			return _finalScore.ToArray();
		}
		
		private EGCLeaderboard GetLeaderboardWithID (string _leaderboardID)
		{
			if (string.IsNullOrEmpty(_leaderboardID))
			{
				DebugPRO.Console.LogError(Constants.kDebugTag, "[GameServices] Leaderboard id is null/empty.");
				return null;
			}
			
			// Find leaderboard with given id
			EGCLeaderboard	_gcLeaderboard	= m_leaderboardList.FirstOrDefault(_curLeaderboard => _curLeaderboard.Identifier.Equals(_leaderboardID));
			
			if (_gcLeaderboard == null)
			{
				DebugPRO.Console.LogError(Constants.kDebugTag, "[GameServices] Couldnt find leaderboard.");
				return null;
			}

			return _gcLeaderboard;
		}

		private int CompareScore (EGCScore _score1, EGCScore _score2)
		{
			if (_score1.Value > _score2.Value)
				return -1;
			else if (_score2.Value > _score1.Value)
				return 0;
			else
				return _score1.User.Name.CompareTo(_score2.User.Name);
		}
			
		#endregion

		#region Achievement Methods

		public void LoadAchievements (Action<EditorAchievement[]> _onCompletion)
		{
			// Verify authentication state
			if (!VerifyUser())
			{
				_onCompletion(null);
				return;
			}

			// Send achievement list using completion handler
			if (_onCompletion != null)
				_onCompletion(EGCAchievement.ConvertToEditorAchievementList(m_achievementsList.ToArray()));
		}

		public void LoadAchievementDescriptions (Action<EditorAchievementDescription[]> _onCompletion)
		{
			// Verify authentication state
			if (!VerifyUser())
			{
				_onCompletion(null);
				return;
			}
			
			// Send achievement description list using completion handler
			if (_onCompletion != null)
				_onCompletion(EGCAchievementDescription.ConvertToEditorAchievementDescriptionList(m_achievementDescriptionList.ToArray()));
		}
			
		public void ReportProgress (Achievement _reportedAchievement, Action<EditorAchievement> _onCompletion)
		{
			if (!VerifyUser())
			{
				OnReportProgressFinished(null, _onCompletion);
				return;
			}

			AchievementDescription	_reportedAchievementDescription	= GetAchievementDescription(_reportedAchievement.Identifier);

			if (_reportedAchievementDescription == null)
			{
				OnReportProgressFinished(null, _onCompletion);
				return;
			}

			bool			_isAchivementCompleted		= _reportedAchievement.PointsScored >= _reportedAchievementDescription.MaximumPoints;
			DateTime		_currentTime				= DateTime.Now;

			// Iterate and update gamecenter data copy
			foreach (EGCAchievement _curAchievement in m_achievementsList)
			{
				if (_curAchievement.Identifier.Equals(_reportedAchievement.Identifier))
				{
					// Update gamecenter copy of achievement
					_curAchievement.SetCompleted(_isAchivementCompleted);
					_curAchievement.SetLastReportedDate(_currentTime);

					// Action on finishing report
					OnReportProgressFinished(_curAchievement.GetEditorFormatData(), _onCompletion);
					return;
				}
			}

			// Check if new achievement was reported
			foreach (EGCAchievementDescription _curAchievementDescription in m_achievementDescriptionList)
			{
				if (_curAchievementDescription.Identifier.Equals(_reportedAchievement.Identifier))
				{
					// Current achivement is set as visible
					_curAchievementDescription.SetIsHidden(false);
					
					// Create new achievement entry for game center
					EGCAchievement 	_newAchievement		= new EGCAchievement(_reportedAchievement.Identifier, _reportedAchievement.PointsScored, _isAchivementCompleted, _currentTime);

					// Add it to the list
					m_achievementsList.Add(_newAchievement);

					// Action on finishing report
					OnReportProgressFinished(_newAchievement.GetEditorFormatData(), _onCompletion);
					return;
				}
			}

			// Failed to achievement info
			OnReportProgressFinished(null, _onCompletion);
			DebugPRO.Console.LogError(Constants.kDebugTag, string.Format("[GameServices] Failed to report achievement progress. Couldnt find achievement with ID = {0}", _reportedAchievement.Identifier));
		}

		private void OnReportProgressFinished (EditorAchievement _achievement, Action<EditorAchievement> _onCompletion)
		{
			// Show achivement banner if required
			if (_achievement != null)
			{
				if (_achievement.Completed && m_canShowAchievementBanner)
					ShowAchievementBanner(GetAchievementDescription(_achievement.Identifier));
			}

			// Send callback
			if (_onCompletion != null)
				_onCompletion(_achievement);
		}

		#endregion

		#region Achievements Methods

		public void ResetAllAchievements (Action<bool> _onCompletion)
		{
			// Reset
			m_achievementsList.Clear();

			// Send callback
			if (_onCompletion != null)
				_onCompletion(true);
		}

		private AchievementDescription GetAchievementDescription (string _achievementID)
		{
			return m_achievementDescriptionList.FirstOrDefault(_curDescription=>_curDescription.Identifier.Equals(_achievementID));
		}
		
		#endregion

		#region UI Methods

		private void CreateGameCenterUIInstance ()
		{
			GameObject 			_gameObject		= new GameObject();
			_gameObject.hideFlags				= HideFlags.HideInHierarchy;

			// UI component
			m_gameCenterUI						= _gameObject.AddComponent<EditorGameCenterUI>();
		}
		
		public void ShowLeaderboardUI (string _leaderboardID, eLeaderboardTimeScope _timeScope, Action _onCompletion)
		{
			// Check if user has logged in
			if (!VerifyUser())
			{
				if (_onCompletion != null)
					_onCompletion();
					
				return;
			}
			
			// Application needs to be in play mode
			if (!Application.isPlaying)
			{				
				DebugPRO.Console.LogError(Constants.kDebugTag, "[GameServices] Works in play mode only.");

				if (_onCompletion != null)
					_onCompletion();
				
				return;
			}
			
			// Get leaderboard info
			EGCLeaderboard	_gcLeaderboard	= GetLeaderboardWithID(_leaderboardID);

			if (_gcLeaderboard == null)
			{
				if (_onCompletion != null)
					_onCompletion();
				
				return;
			}

			// Set leaderboard score fetch range
			Range			_oldRange		= _gcLeaderboard.Range;
			Range			_newRange		= new Range(1, int.MaxValue);
			_gcLeaderboard.Range			= _newRange;

			// Fetch scores from leaderboard
			EGCScore[]		_scoreList		= GetFilteredScoreList(_gcLeaderboard, _timeScope, eLeaderboardUserScope.GLOBAL);

			// Reset range to old value
			_gcLeaderboard.Range			= _oldRange;

			// Show UI
			if (m_gameCenterUI == null)
				CreateGameCenterUIInstance();

			m_gameCenterUI.ShowLeaderboardUI(_scoreList, _onCompletion);
		}
		
		public void ShowAchievementsUI (Action _onCompletion)
		{
			// Check if user has logged in
			if (!VerifyUser()) 
			{
				if (_onCompletion != null)
					_onCompletion();
				
				return;
			}
			
			// Application needs to be in play mode
			if (!Application.isPlaying)
			{				
				DebugPRO.Console.LogError(Constants.kDebugTag, "[GameServices] Available in play mode only.");
			
				if (_onCompletion != null)
					_onCompletion();
				
				return;
			}
			
			// Gather data required to display properties
			Dictionary<EGCAchievementDescription, EGCAchievement> 	_gcAchievementMap	= new Dictionary<EGCAchievementDescription, EGCAchievement>();

			foreach (EGCAchievementDescription _gcAchievementDesc in m_achievementDescriptionList)
			{
				string			_gcDescriptionID	= _gcAchievementDesc.Identifier;
				EGCAchievement	_gcAchievement		= m_achievementsList.FirstOrDefault(_curAchievement => _curAchievement.Identifier.Equals(_gcDescriptionID));

				// Add each entry
				_gcAchievementMap.Add(_gcAchievementDesc, _gcAchievement);
			}

			// Show UI
			if (m_gameCenterUI == null)
				CreateGameCenterUIInstance();

			m_gameCenterUI.ShowAchievementUI(_gcAchievementMap, _onCompletion);
		}

		public void ShowDefaultAchievementCompletionBanner (bool _canShow)
		{
			m_canShowAchievementBanner	= _canShow;
		}

		private void ShowAchievementBanner (AchievementDescription _description)
		{
			if (m_gameCenterUI == null)
				CreateGameCenterUIInstance();
			
			m_gameCenterUI.ShowAchievementBanner(_description);
		}

		#endregion
	}
}
#endif