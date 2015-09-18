using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SocialPlatforms;
using System.Collections.Generic;
using System.Linq;

#if UNITY_IOS
namespace VoxelBusters.NativePlugins.Internal
{
	internal sealed class iOSLeaderboard : Leaderboard 
	{
		#region Fields
		
		private		ILeaderboard	m_leaderboardData;
		
		#endregion
		
		#region Properties
		
		public override string Identifier
		{
			get
			{
				return m_leaderboardData.id;
			}
			
			protected set
			{
				m_leaderboardData.id	= value;
			}
		}
		
		public override string Title
		{
			get
			{
				return m_leaderboardData.title;
			}
			
			protected set
			{
				throw new Exception("[GameServices] Only getter is supported.");
			}
		}
		
		public override eLeaderboardUserScope UserScope
		{
			get
			{
				return (eLeaderboardUserScope)m_leaderboardData.userScope;
			}
			
			set
			{
				m_leaderboardData.userScope	= (UserScope)value;
			}
		}
		
		public override eLeaderboardTimeScope TimeScope
		{
			get
			{
				return (eLeaderboardTimeScope)m_leaderboardData.timeScope;
			}
			
			set
			{
				m_leaderboardData.timeScope	= (TimeScope)value;
			}
		}
		
		public override Score[] Scores
		{
			get;
			protected set;
		}
		
		public override Score LocalUserScore
		{
			get;
			protected set;
		}
		
		#endregion
		
		#region Constructors
		
		private iOSLeaderboard ()
		{}
		
		internal iOSLeaderboard (string _identifier)
		{
			// Initialize properties
			m_leaderboardData			= Social.CreateLeaderboard();
			Identifier					= _identifier;
			
			// Set initial properties
			m_leaderboardData.range		= new Range(0, 0);
		}
		
		#endregion
		
		#region Methods
		
		public override	void LoadTopScores (LoadScoreCompletion _onCompletion)
		{
			Range 		_range			= new Range(1, MaxResults);
			
			// Load scores
			LoadScores(_range, _onCompletion);
		}
		
		public override	void LoadPlayerCenteredScores (LoadScoreCompletion _onCompletion)
		{
			Range 		_range			= new Range(1, 1);
			
			// Set leaderboard properties
			m_leaderboardData.range		= _range;
			
			// Load local players scores
			m_leaderboardData.LoadScores((bool _status)=>{
				if (_status)
				{
					if (m_leaderboardData.localUserScore == null)
					{
						LoadTopScores(_onCompletion);
						return;
					}
					else
					{
						int 	_localPlayerRank		= m_leaderboardData.localUserScore.rank;
						int		_maxResults				= MaxResults;
						int 	_loadFrom				= Mathf.Max(1, _localPlayerRank - Mathf.FloorToInt(_maxResults * 0.5f));
						Range	_playerCenteredRange	= new Range(_loadFrom, _maxResults);
						
						// Load scores player centered scores
						LoadScores(_playerCenteredRange, _onCompletion);
						return;
					}
				}
				else
				{
					if (_onCompletion != null)
						_onCompletion(null, null);
				}
			});
		}
		
		public override	void LoadMoreScores (eLeaderboardPageDirection _pageDirection, LoadScoreCompletion _onCompletion)
		{
			Range		_curRange		= m_leaderboardData.range;

			if (_curRange.from == 0)
			{
				LoadTopScores(_onCompletion);
				return;
			}
			
			// Based on page direction, compute range of score to be loaded
			int			_maxResults		= MaxResults;
			Range		_newRange		= new Range(0, _maxResults);
			
			if (_pageDirection == eLeaderboardPageDirection.PREVIOUS)
			{
				if (_curRange.from == 1)
				{
					if (_onCompletion != null)
						_onCompletion(null, null);

					return;
				}
				
				// Set range start value
				_newRange.from	= Mathf.Max(1, _curRange.from - _maxResults);
			}
			else if (_pageDirection == eLeaderboardPageDirection.NEXT)
			{
				// Set range start value
				_newRange.from	= _curRange.from + _maxResults;
			}
			
			// Load scores
			LoadScores(_newRange, _onCompletion);
		} 
		
		private void LoadScores (Range _range, LoadScoreCompletion _onCompletion)
		{
			// Set Leaderboard properties
			m_leaderboardData.range	= _range;

			// Load scores
			m_leaderboardData.LoadScores((bool _success)=>{
				
				if (_success)
				{
					LoadUserInfo(_onCompletion);
					return;
				}
				else
				{
					if (_onCompletion != null)
						_onCompletion(null, null);
					
					return;
				}
			});
		}
		
		private void LoadUserInfo (LoadScoreCompletion _onCompletion)
		{
			IScore[]					_gcScoreList		= m_leaderboardData.scores;
			int 						_gcScoreCount		= _gcScoreList.Length;
			Dictionary<string, IScore>	_gcUserIDScoreMap	= new Dictionary<string, IScore>(_gcScoreCount);
			
			for (int _iter = 0; _iter < _gcScoreCount; _iter++)
			{
				IScore			_curScore		= _gcScoreList[_iter];
				
				// Add entries to user id score map
				_gcUserIDScoreMap.Add(_curScore.userID, _curScore);
			}
			
			// Now load users
			NPBinding.GameServices.LoadUsers(_gcUserIDScoreMap.Keys.ToArray(), (User[] _userList)=>{
				
				if (_userList == null)
				{
					if (_onCompletion != null)
						_onCompletion(null, null);

					return;
				}
				else
				{
					Score[]		_finalScoreList	= new iOSScore[_gcScoreCount];
					
					for (int _iter = 0; _iter < _gcScoreCount; _iter++)
					{
						User		_curUser	= _userList[_iter];
						IScore		_userScore	= _gcUserIDScoreMap[_curUser.Identifier];
						
						// Add score and user info
						_finalScoreList[_iter]	= new iOSScore(_userScore, _curUser);
					}
					
					// Set new values
					Scores 						= _finalScoreList;
					LocalUserScore				= new iOSScore(m_leaderboardData.localUserScore, NPBinding.GameServices.LocalUser);

					// Trigger callback
					if (_onCompletion != null)
						_onCompletion(Scores, LocalUserScore);

					return;
				}
			});
		}
		
		#endregion
	}
}
#endif