using UnityEngine;
using System.Collections;
using System;
using System.Runtime.InteropServices;
using UnityEngine.SocialPlatforms.GameCenter;
using UnityEngine.SocialPlatforms;
using VoxelBusters.DebugPRO;

#if UNITY_IOS
namespace VoxelBusters.NativePlugins
{
	using Internal;

	public sealed class GameServicesIOS : GameServices 
	{
		#region Fields

		private		iOSLocalUser		m_localUser;

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
				m_localUser = value as iOSLocalUser;
			}
		}

		#endregion
		
		#region External Methods

		[DllImport("__Internal")]
		private static extern bool isGameCenterAvailable ();

		[DllImport("__Internal")]
		private static extern void showLeaderboardView (string _leaderboardID, int _timeScope);

		[DllImport("__Internal")]
		private static extern void showAchievementView ();

		#endregion 

		#region Unity Methods

		protected override void Awake ()
		{
			base.Awake ();

			// Initialize
			LocalUser		= new iOSLocalUser();
		}

		#endregion

		#region Methods

		public override bool IsAvailable ()
		{
			return isGameCenterAvailable();
		}

		#endregion
		
		#region Leaderboard Methods

		public override Leaderboard CreateLeaderboard (string _leaderboardID)
		{
			// Verify user authentication state before proceeding
			if (VerifyUser())
				return new iOSLeaderboard(_leaderboardID);

			return null;
		}

		#endregion

		#region Achievement Methods

		public override Achievement CreateAchievement (string _achievementID)
		{
			// Verify user authentication state before proceeding
			if (VerifyUser())
				return new iOSAchievement(_achievementID);

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
			Social.LoadAchievementDescriptions((IAchievementDescription[] _achievementDescriptionList)=>{

				// Call handle method
				OnLoadAchievementDescriptionsFinished(iOSAchievementDescription.ConvertAchievementDescriptionList(_achievementDescriptionList));
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
			Social.LoadAchievements((IAchievement[] _achievements)=>{
				
				// Send callback
				if (_onCompletion != null)
				{
					_onCompletion(iOSAchievement.ConvertAchievementList(_achievements));
				}
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

			// Create new achievement instance
			iOSAchievement	_achievement	= new iOSAchievement(_achievementID);
			
			// Set new score and report
			_achievement.PointsScored		= _pointsScored;
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
			Social.LoadUsers(_userIDs, (IUserProfile[] _userProfileList)=>{

				if (_onCompletion != null)
				{
					_onCompletion(iOSUser.ConvertToUserList(_userProfileList));
				}
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
			iOSScore		_newScore		= new iOSScore(_leaderboardID, LocalUser, _score);

			// Report score
			_newScore.ReportScore(_onCompletion);
		}

		#endregion

		#region UI Methods

		public override void ShowDefaultAchievementCompletionBanner (bool _canShow)
		{
			GameCenterPlatform.ShowDefaultAchievementCompletionBanner(_canShow);
		}

		public override void ShowAchievementsUI (GameServiceViewClosed _onCompletion)
		{
			base.ShowAchievementsUI(_onCompletion);

			if (!VerifyUser())
				return;

			// Native call
			showAchievementView();
		}
		
		public override void ShowLeaderboardUI (string _leaderboardID, eLeaderboardTimeScope _timeScope, GameServiceViewClosed _onCompletion)
		{
			base.ShowLeaderboardUI(_leaderboardID, _timeScope, _onCompletion);
			
			if (string.IsNullOrEmpty(_leaderboardID))
				return;
			
			if (!VerifyUser())
				return;

			// Native call
			showLeaderboardView(_leaderboardID, (int)_timeScope);
		}

		#endregion
	}
}
#endif