using UnityEngine;
using System.Collections;
using VoxelBusters.AssetStoreProductUtility.Demo;

namespace VoxelBusters.NativePlugins.Demo
{
	using Internal;

	public class GameServicesDemo : DemoSubMenu 
	{
		#region Fields

		// Related to leaderboard
		[Header("Leaderboard Properties")]
		[SerializeField]
		private						eLeaderboardTimeScope		m_timeScope;

#pragma warning disable		
		[SerializeField]
		private						string[]					m_iOSLeaderboardIDList		= new string[0];

		[SerializeField]
		private						string[]					m_androidLeaderboardIDList	= new string[0];

		private						string[]					m_editorLeaderboardIDList;
#pragma warning restore

		[SerializeField]
		private						int							m_maxScoreResults			= 20;
		
		private						Leaderboard					m_curLeaderboard;

		private						string						m_curLeaderboardID;
		
		private						int							m_curLeaderboardIDIndex		= -1;

		// Related to achievements
#pragma warning disable
		[Header("Achievement Properties")]
		[SerializeField]
		private						string[]					m_iOSAchievementIDList		= new string[0];
		
		[SerializeField]
		private						string[]					m_androidAchievementIDList	= new string[0];

		private						string[]					m_editorAchievementIDList;
#pragma warning restore

		private						string						m_curAchievementID;
		
		private						int							m_curAchievementIDIndex		= -1;

		#endregion

		#region Properties

		public string[] LeaderboardIDList
		{
			get
			{
#if !UNITY_EDITOR && UNITY_ANDROID
				return m_androidLeaderboardIDList;
#elif !UNITY_EDITOR && UNITY_IOS
				return m_iOSLeaderboardIDList;
#else
				return m_editorLeaderboardIDList;
#endif
			}
		}

		public string[] AchievementIDList
		{
			get
			{
#if !UNITY_EDITOR && UNITY_ANDROID
				return m_androidAchievementIDList;
#elif !UNITY_EDITOR && UNITY_IOS
				return m_iOSAchievementIDList;
#else
				return m_editorAchievementIDList;
#endif
			}
		}
	
		#endregion

		#region Menu Methods
		
		protected override void OnEnable ()
		{
			base.OnEnable ();

#if UNITY_EDITOR
			AddNewResult("Using Game Service simulation.");

			// Set editor properties
			m_editorLeaderboardIDList	= EditorGameCenter.Instance.GetLeaderboardIDList();
			m_editorAchievementIDList	= EditorGameCenter.Instance.GetAchievementIDList();
#elif UNITY_ANDROID
			AddNewResult("Game Service will use Google Play Service.");
#elif UNITY_IOS
			AddNewResult("Game Service will use Game Center.");
#else 
			AddNewResult("Not supported.");
#endif

			// Leaderboard
			if (LeaderboardIDList == null || LeaderboardIDList.Length == 0)
				AppendResult("Leaderboard identifiers not configured in inspector.");
			else if (m_curLeaderboardIDIndex == -1)
				ChangeLeaderboardID(true);

			// Achievement
			if (AchievementIDList == null || AchievementIDList.Length == 0)
				AppendResult("Achievement identifiers not configured in inspector.");
			else if (m_curAchievementIDIndex == -1)
				ChangeAchievementID(true);
		}
		
		protected override void OnGUIWindow()
		{		
			base.OnGUIWindow();
			
			RootScrollView.BeginScrollView();
			{
				if(NPSettings.Application.SupportedFeatures.UsesGameServices)
				{
					if (GUILayout.Button("Is Game Service Available"))
					{
						IsAvailable();
					}
	
					if (GUILayout.Button("Is User Authenticated"))
					{
						if (IsAuthenticated())
							AddNewResult("Local user is authenticated.");
						else
							AddNewResult("Local user is not authenticated.");
					}
	
					if (IsAuthenticated())
					{
						DrawUsersInformationSection();
						DrawLeaderboardSection();
						DrawAchievementSection();
						DrawUISection();
					}
					else
					{
						if (GUILayout.Button("Authenticate User"))
							AuthenticateUser();
					}
				}
				else
				{
					AddNewResult("Enable Game services feature in NPSettings.");
				}
			}
			RootScrollView.EndScrollView();
			
			DrawResults();
			DrawPopButton();
		}

		private void DrawUsersInformationSection()
		{
			GUILayout.Label("Users Information", kSubTitleStyle);
			
			if (GUILayout.Button("Load Local Player Friends"))
			{
				LoadLocalUserFriends();
			}
			if(GUILayout.Button("Load Users"))
			{
				string[] _userIDList = new string[]{NPBinding.GameServices.LocalUser.Identifier};
				LoadUsers(_userIDList);
			}
		}

		private void DrawLeaderboardSection ()
		{
			if (LeaderboardIDList.Length == 0)
			{
				GUILayout.Box("Couldnt find Leaderboard Identifiers in inspector. Please configure if you want to use Leaderboard feature.");
			}
			else
			{
				GUILayout.Label("Leaderboard", kSubTitleStyle);
				
				GUILayout.BeginHorizontal();
				{
					if (GUILayout.Button("Goto Next Leaderboard ID"))
						ChangeLeaderboardID(true);
					
					if (GUILayout.Button("Goto Previous Leaderboard ID"))
						ChangeLeaderboardID(false);
				}
				GUILayout.EndHorizontal();
				
				if (GUILayout.Button("Create Leaderboard"))
				{
					Leaderboard _leaderboard = CreateLeaderboard(m_curLeaderboardID);
					AddNewResult("Created a platform specific instance of Leaderboard" + "[" + _leaderboard.Identifier +"].");	
				}
				
				if (GUILayout.Button("Load Top Scores"))
				{
					LoadTopScores(m_curLeaderboardID);
				}
				
				if (GUILayout.Button("Load Player Centered Scores"))
				{
					LoadPlayerCenteredScores(m_curLeaderboardID);
				}
				
				GUILayout.BeginHorizontal();
				{
					if (GUILayout.Button("Load Next Scores"))
					{
						LoadMoreScores(m_curLeaderboardID, eLeaderboardPageDirection.NEXT);
					}

					if (GUILayout.Button("Load Previous Scores"))
					{
						LoadMoreScores(m_curLeaderboardID, eLeaderboardPageDirection.PREVIOUS);
					}
				}
				GUILayout.EndHorizontal();

				if (GUILayout.Button("Report Score"))
				{
					ReportScore(m_curLeaderboardID);
				}
			}
		}

		private void DrawAchievementSection ()
		{
			if (AchievementIDList.Length == 0)
			{
				GUILayout.Box("Couldnt find Achievement configuration in GameServices. Please configure if you want to use Achievement feature.");
			}
			else
			{
				GUILayout.Label("Achievement", kSubTitleStyle);
				
				GUILayout.BeginHorizontal();
				{
					if (GUILayout.Button("Goto Next Achievement ID"))
						ChangeAchievementID(true);
					
					if (GUILayout.Button("Goto Previous Achievement ID"))
						ChangeAchievementID(false);
				}
				GUILayout.EndHorizontal();
				
				if (GUILayout.Button("Load Achievement Descriptions"))
				{
					LoadAchievementDescriptions();
				}
				
				if (GUILayout.Button("Load Achievements"))
				{
					LoadAchievements();
				}
				
				if (GUILayout.Button("Create Achievement"))
				{
					Achievement _achievement = CreateAchievement(m_curAchievementID);
					AddNewResult("Created a platform specific instance of Achievement" + "[" + _achievement.Identifier +"].");	
				}
				
				if (GUILayout.Button("Report Progress"))
				{
					ReportProgress(m_curAchievementID);
				}
			}
		}

		private void DrawUISection ()
		{
			bool 	_canShowLeaderboarAPI	= (LeaderboardIDList.Length != 0);
			bool	_canShowAchievementAPI	= (AchievementIDList.Length != 0);

			if (_canShowLeaderboarAPI || _canShowAchievementAPI)
				GUILayout.Label("UI", kSubTitleStyle);
			
			if (_canShowLeaderboarAPI && GUILayout.Button("Show Leaderboard UI"))
			{
				ShowLeaderboardUI();
			}
			
			if (_canShowAchievementAPI && GUILayout.Button("Show Achievements UI"))
			{
				ShowAchievementsUI();
			}
		}
		
		#endregion
		
		#region Service Methods
		
		private void IsAvailable ()
		{
			AddNewResult(string.Format("GameService is available={0}.", NPBinding.GameServices.IsAvailable()));
		}

		#endregion

		#region Auth Methods

		private bool IsAuthenticated ()
		{
			return NPBinding.GameServices.LocalUser.IsAuthenticated;
		}
		
		private void AuthenticateUser ()
		{
			NPBinding.GameServices.LocalUser.Authenticate((bool _success)=>{
				
				if (_success)
				{
					AddNewResult("Sign-In Successfully");
					AppendResult("Local User Details : " + NPBinding.GameServices.LocalUser.ToString());
				}
				else
				{
					AddNewResult("Sign-In Failed");
				}
			});
		}

		#endregion

		#region User Methods

		private void LoadLocalUserFriends()
		{
			NPBinding.GameServices.LocalUser.LoadFriends((User[] _friendsList) =>{
				if(_friendsList != null)
				{
					AddNewResult("Succesfully loaded user friends.");
					foreach(User _eachFriend in _friendsList)
					{
						AppendResult(string.Format("Name : {0}, Id : {1}", _eachFriend.Name, _eachFriend.Identifier));
					}
				}
				else
				{
					AddNewResult("Failed to load user friends.");
				}
		
			}); 
		}

		private void LoadUsers(string[] _userIDList)
		{
			NPBinding.GameServices.LoadUsers(_userIDList, (User[] _users) =>{
				if(_users != null)
				{
					AddNewResult("Succesfully loaded users info.");
					foreach(User _eachUser in _users)
					{
						AppendResult(string.Format("Name : {0}, Id : {1}", _eachUser.Name, _eachUser.Identifier));
					}
				}
				else
				{
					AddNewResult("Failed to load users info.");
				}
	
			});
		}

		#endregion

		#region Leaderboard Methods

		private Leaderboard CreateLeaderboard (string _leaderboardID)
		{
			m_curLeaderboard			= NPBinding.GameServices.CreateLeaderboard(_leaderboardID);
			m_curLeaderboard.MaxResults	= m_maxScoreResults;

			return m_curLeaderboard;
		}

		private void LoadPlayerCenteredScores(string _leaderboardID)
		{
			if (m_curLeaderboard == null)
			{
				AddNewResult("Failed to load scores. Create leaderboard instance before requesting score load.");
				return;
			}
			
			// Load scores
			m_curLeaderboard.LoadPlayerCenteredScores(OnLoadScoresFinished);
		}
		
		private void LoadTopScores(string _leaderboardID)
		{
			if (m_curLeaderboard == null)
			{
				AddNewResult("Failed to load scores. Create leaderboard instance before requesting score load.");
				return;
			}
			
			// Load scores
			m_curLeaderboard.LoadTopScores(OnLoadScoresFinished);
		}
		
		private void LoadMoreScores(string _leaderboardID, eLeaderboardPageDirection _direction)
		{
			if (m_curLeaderboard == null)
			{
				AddNewResult("Failed to load scores. Create leaderboard instance before requesting score load.");
				return;
			}
			
			// Load scores
			m_curLeaderboard.LoadMoreScores(_direction, OnLoadScoresFinished);
		}
		
		private void OnLoadScoresFinished (Score[] _scores, Score _localUserScore)
		{
			if (_scores != null)
			{
				int		_scoresCount	= _scores.Length;
				
				AddNewResult(string.Format("Successfully loaded scores. Count={0}.", _scoresCount));
				AppendResult(string.Format("Local User: {0}.", _localUserScore));
				
				for (int _iter = 0; _iter < _scoresCount; _iter++)
				{
					AppendResult(string.Format("[Index {0}]: {1}.", _iter, _scores[_iter]));
				}
			}
			else
			{
				AddNewResult("Failed to load scores.");
			}
		}
		
		private void ReportScore (string _leaderboardID)
		{
			int			_randomScore	= Random.Range(0, 100);
			
			NPBinding.GameServices.ReportScore(_leaderboardID, _randomScore, (bool _status)=>{
				
				if (_status)
					AddNewResult(string.Format("Successfully reported score={0} to leaderboard with ID={1}.", _randomScore, m_curLeaderboardID));
				else
					AddNewResult(string.Format("Failed to report score to leaderboard with ID={0}.", m_curLeaderboardID));
			});
		}

		#endregion

		#region Achievement Methods

		private Achievement CreateAchievement (string _achievementID)
		{
			Achievement _newAchievement	= NPBinding.GameServices.CreateAchievement(_achievementID);
			return _newAchievement;
		}

		private void LoadAchievementDescriptions ()
		{
			NPBinding.GameServices.LoadAchievementDescriptions((AchievementDescription[] _descriptions)=>{

				if (_descriptions == null)
				{
					AddNewResult("Couldnt load achievement description list.");
					return;
				}

				int		_descriptionCount	= _descriptions.Length;
				AddNewResult(string.Format("Successfully loaded achievement description list. Count={0}.", _descriptionCount));

				for (int _iter = 0; _iter < _descriptionCount; _iter++)
				{
					AppendResult(string.Format("[Index {0}]: {1}", _iter, _descriptions[_iter]));
				}
			});
		}

		private void LoadAchievements ()
		{
			NPBinding.GameServices.LoadAchievements((Achievement[] _achievements)=>{
				
				if (_achievements == null)
				{
					AddNewResult("Couldnt load achievement list.");
					return;
				}
				
				int		_achievementCount	= _achievements.Length;
				AddNewResult(string.Format("Successfully loaded achievement list. Count={0}.", _achievementCount));
				
				for (int _iter = 0; _iter < _achievementCount; _iter++)
				{
					AppendResult(string.Format("[Index {0}]: {1}", _iter, _achievements[_iter]));
				}
			});
		}

		private void ReportProgress (string _achievementID)
		{
			//If its an incremental achievement, make sure you send a incremented cumulative value everytime you call this method
			int			_randomPoints	= Random.Range(0, 100);

			NPBinding.GameServices.ReportProgress(_achievementID, _randomPoints, (bool _status)=>{

				if (_status)
					AddNewResult(string.Format("Successfully reported points={0} to achievement with ID={1}.", _randomPoints, m_curAchievementID));
				else
					AddNewResult(string.Format("Failed to report progress of achievement with ID={0}.", m_curAchievementID));
			});
		}

		#endregion

		#region UI Methods

		private void ShowAchievementsUI ()
		{
			AddNewResult("Showing achievements UI.");
			NPBinding.GameServices.ShowAchievementsUI(()=>{
				AppendResult("Closed achievements UI.");
			});
		}

		private void ShowLeaderboardUI ()
		{
			AddNewResult("Showing leaderboard UI.");
			NPBinding.GameServices.ShowLeaderboardUI(m_curLeaderboardID, m_timeScope, ()=>{
				AppendResult("Closed leaderboard UI.");
			});
		}

		#endregion

		#region Misc Methods

		private void ChangeLeaderboardID (bool _gotoNext)
		{
			int		 _identifierCount	= LeaderboardIDList.Length;

			if (_gotoNext)
			{
				if ( _identifierCount == 0)
					return;
				
				// Move to next index
				m_curLeaderboardIDIndex++;
				
				if (m_curLeaderboardIDIndex >=  _identifierCount)
					m_curLeaderboardIDIndex	= 0;
			}
			else
			{
				if ( _identifierCount == 0)
					return;
				
				// Move to previous index
				m_curLeaderboardIDIndex--;
				
				if (m_curLeaderboardIDIndex < 0)
					m_curLeaderboardIDIndex	=  _identifierCount - 1;
			}

			// Set id
			m_curLeaderboardID			= LeaderboardIDList[m_curLeaderboardIDIndex];
			AddNewResult(string.Format("Current Leaderboard ID={0}", m_curLeaderboardID));
		}

		private void ChangeAchievementID (bool _gotoNext)
		{
			int		 _identifierCount	= AchievementIDList.Length;
			
			if (_gotoNext)
			{
				if ( _identifierCount == 0)
					return;
				
				// Move to next index
				m_curAchievementIDIndex++;
				
				if (m_curAchievementIDIndex >=  _identifierCount)
					m_curAchievementIDIndex	= 0;
			}
			else
			{
				if ( _identifierCount == 0)
					return;
				
				// Move to previous index
				m_curAchievementIDIndex--;
				
				if (m_curAchievementIDIndex < 0)
					m_curAchievementIDIndex	=  _identifierCount - 1;
			}
			
			// Set id
			m_curAchievementID	= AchievementIDList[m_curAchievementIDIndex];
			AddNewResult(string.Format("Current Achievement ID={0}", m_curAchievementID));
		}

		#endregion
	}
}