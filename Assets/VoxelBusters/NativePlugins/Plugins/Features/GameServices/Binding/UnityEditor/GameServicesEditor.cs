using UnityEngine;
using System.Collections;
using System;

#if UNITY_EDITOR
namespace VoxelBusters.NativePlugins
{
	using Internal;

	public sealed class GameServicesEditor : GameServices 
	{
		#region Fields
		
		private		EditorLocalUser		m_localUser;
		
		#endregion
		
		#region Properties
		
		public override LocalUser LocalUser 
		{
			get 
			{
				return m_localUser;
			}
			
			protected set 
			{
				m_localUser = value as EditorLocalUser;
			}
		}
		
		#endregion

		#region Unity Methods

		protected override void Awake ()
		{
			base.Awake ();

			// Initialize
			LocalUser	= new EditorLocalUser();
		}

		#endregion
		
		#region Methods
		
		public override bool IsAvailable ()
		{
			return true;
		}
		
		#endregion
		
		#region Leaderboard Methods

		public override Leaderboard CreateLeaderboard (string _leaderboardID)
		{
			// Verify user authentication state before proceeding
			if (VerifyUser())
				return new EditorLeaderboard(_leaderboardID);

			return null;
		}
		
		#endregion
		
		#region Achievement Methods
		
		public override Achievement CreateAchievement (string _achievementID)
		{
			// Verify user authentication state before proceeding
			if (VerifyUser())
				return new EditorAchievement(_achievementID);

			return null;
		}
		
		protected override void LoadAchievementDescriptions (bool _needsVerification, Action<AchievementDescription[]> _onCompletion)
		{
			base.LoadAchievementDescriptions(_needsVerification, _onCompletion);

			// Verify user authentication state before proceeding
			if (_needsVerification && !VerifyUser())
			{
				if (_onCompletion != null)
					_onCompletion(null);

				return;
			}

			// Load achievement descriptions
			EditorGameCenter.Instance.LoadAchievementDescriptions((EditorAchievementDescription[] _descriptionList)=>{
				
				// Call request finished method
				OnLoadAchievementDescriptionsFinished(_descriptionList);
			});
		}
		
		public override void LoadAchievements (Action<Achievement[]> _onCompletion)
		{
			// Verify user authentication state before proceeding
			if (!VerifyUser())
			{
				if (_onCompletion != null)
					_onCompletion(null);
				
				return;
			}

			// Load achievements
			EditorGameCenter.Instance.LoadAchievements((EditorAchievement[] _achievementList)=>{
				
				if (_onCompletion != null)
					_onCompletion(_achievementList);
			});
		}
		
		public override void ReportProgress (string _achievementID, int _pointsScored, Action<bool> _onCompletion)
		{
			base.ReportProgress(_achievementID, _pointsScored, _onCompletion);
			
			if (string.IsNullOrEmpty(_achievementID))
				return;
			
			// Verify user authentication state before proceeding
			if (!VerifyUser())
			{
				if (_onCompletion != null)
					_onCompletion(false);

				return;
			}

			// Create new instance
			EditorAchievement	_achievement	= new EditorAchievement(_achievementID);
			_achievement.PointsScored			= _pointsScored;
			
			// Report progress
			_achievement.ReportProgress(_onCompletion);
		}
		
		#endregion
		
		#region User Methods
		
		public override void LoadUsers (string[] _userIDs, Action<User[]> _onCompletion)
		{
			base.LoadUsers(_userIDs, _onCompletion);
			
			if (_userIDs == null)
				return;

			// Verify user authentication state before proceeding
			if (!VerifyUser())
			{
				if (_onCompletion != null)
					_onCompletion(null);
				
				return;
			}

			// Load users
			EditorGameCenter.Instance.LoadUsers(_userIDs, (EditorUser[] _userList)=>{

				// Trigger callback
				if (_onCompletion != null)
					_onCompletion(_userList);
			});
		}
		
		public override void ReportScore (string _leaderboardID, long _score, Action<bool> _onCompletion)
		{
			base.ReportScore(_leaderboardID, _score, _onCompletion);
			
			if (string.IsNullOrEmpty(_leaderboardID))
				return;

			// Verify user authentication state before proceeding
			if (!VerifyUser())
			{
				if (_onCompletion != null)
					_onCompletion(false);
				
				return;
			}

			// Create instance
			EditorScore			_newScore		= new EditorScore(_leaderboardID, LocalUser, _score);

			// Report new score
			_newScore.ReportScore(_onCompletion);
		}

		#endregion

		#region UI Methods

		public override void ShowDefaultAchievementCompletionBanner (bool _canShow)
		{
			EditorGameCenter.Instance.ShowDefaultAchievementCompletionBanner(_canShow);
		}
		
		public override void ShowAchievementsUI (GameServiceViewClosed _onCompletion)
		{
			base.ShowAchievementsUI(_onCompletion);

			if (!VerifyUser())
				return;

			// Request for view
			EditorGameCenter.Instance.ShowAchievementsUI(()=>{
				ShowAchievementViewFinished(null);
			});
		}
		
		public override void ShowLeaderboardUI (string _leaderboardID, eLeaderboardTimeScope _timeScope, GameServiceViewClosed _onCompletion)
		{
			base.ShowLeaderboardUI(_leaderboardID, _timeScope, _onCompletion);

			if (string.IsNullOrEmpty(_leaderboardID))
				return;

			if (!VerifyUser())
				return;
		
			// Request for view
			EditorGameCenter.Instance.ShowLeaderboardUI(_leaderboardID, _timeScope, ()=>{
				ShowLeaderboardViewFinished(null);
			});
		}
		
		#endregion
	}
}
#endif