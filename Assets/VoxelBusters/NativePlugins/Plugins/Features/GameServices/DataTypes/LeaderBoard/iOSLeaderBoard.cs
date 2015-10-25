using UnityEngine;
using System.Collections;

#if USES_GAME_SERVICES && UNITY_IOS
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins.Internal
{
	public sealed class iOSLeaderboard : Leaderboard 
	{
		private struct Range 
		{
			public 	int 	from;
			public 	int 	count;

			public Range (int _from, int _count) : this ()
			{
				this.from	= _from;
				this.count	= _count;
			}
		}

		#region Constants

		private 	const 	string 		kLeaderboardInfoKey	= "leaderboard-info";
		private 	const 	string 		kPlayerScopeKey		= "player-scope";
		private 	const 	string 		kRangeFromKey		= "range-from";
		private 	const 	string 		kRangeLengthKey		= "range-length";
		private 	const 	string 		kTimeScopeKey		= "time-scope";
		private 	const 	string 		kIdentifierKey		= "id";
		private 	const 	string 		kTitleKey			= "title";
		private 	const 	string 		kScoresKey			= "scores";
		private 	const 	string 		kLocalScoreKey		= "local-score";

		#endregion

		#region Fields

		private		Range	m_range;
	
		#endregion

		#region Properties
		
		public override string Identifier
		{
			get;
			protected set;
		}
		
		public override string Title
		{
			get;
			protected set;
		}
		
		public override eLeaderboardUserScope UserScope
		{
			get;
			set;
		}
		
		public override eLeaderboardTimeScope TimeScope
		{
			get;
			set;
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

		public iOSLeaderboard (string _globalIdentifier, string _identifier) : base (_globalIdentifier, _identifier)
		{
			// Initialize properties
			m_range				= new Range(0, 0);
		}

		public iOSLeaderboard (IDictionary _dataDict) : base ()
		{
			// Parse data dictionary values
			Identifier			= _dataDict.GetIfAvailable<string>(kIdentifierKey);
			Title				= _dataDict.GetIfAvailable<string>(kTitleKey);
			UserScope			= (eLeaderboardUserScope)_dataDict.GetIfAvailable<int>(kPlayerScopeKey);
			TimeScope			= (eLeaderboardTimeScope)_dataDict.GetIfAvailable<int>(kTimeScopeKey);
			Scores				= null;
			LocalUserScore		= null;
			m_range				= new Range(0, 0);

			// Set global identifier
			GlobalIdentifier	= GameServicesIDHandler.GetLeaderboardGID(Identifier);
		}			
		
		#endregion
		
		#region External Methods
		
		[DllImport("__Internal")]
		private static extern void loadScores(string _leaderboardInfoJSON);
		
		#endregion
		
		#region Methods

		public override	void LoadTopScores (LoadScoreCompletion _onCompletion)
		{
			// Request for top scores
			Range 		_range		= new Range(1, MaxResults);
			
			LoadScores(_range, _onCompletion);
		}
		
		public override	void LoadPlayerCenteredScores (LoadScoreCompletion _onCompletion)
		{
			Range 		_range		= new Range(1, 1);

			// First get local player score
			LoadScores(_range, (Score[] _scores, Score _localUserScore, string _error)=>{

				// Check if data is valid
				if (_localUserScore == null)
				{
					LoadScoresFinished(null, null, _error);
				}
				else
				{
					int 	_localPlayerRank		= LocalUserScore.Rank;
					int		_maxResults				= MaxResults;
					int 	_loadFrom				= Mathf.Max(1, _localPlayerRank - Mathf.FloorToInt(_maxResults * 0.5f));
					Range	_playerCenteredRange	= new Range(_loadFrom, _maxResults);
					
					// Load scores player centered scores
					LoadScores(_playerCenteredRange, _onCompletion);
					return;
				}
			});
		}
		
		public override	void LoadMoreScores (eLeaderboardPageDirection _pageDirection, LoadScoreCompletion _onCompletion)
		{
			// Check if user has requested for initial score info
			Range		_curRange		= m_range;

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
					LoadScoresFinished(null, null, "The operation could not be completed because there are no entries available to load.");
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
			// Cache properties
			m_range					= _range;
			SetLoadScoreFinishedEvent(_onCompletion);

			// Load scores
			loadScores(GetLeaderboardInfoJSONObject().ToJSON());
		}

		private void SetScores (IList _scoresJSONList, IDictionary _localScoreInfoDict)
		{
			// Set default values
			Scores			= null;
			LocalUserScore	= null;
			
			// Set scores list
			if (_scoresJSONList != null)
			{
				int	_count	= _scoresJSONList.Count;
				Scores		= new iOSScore[_count];
				
				for (int _iter = 0; _iter < _count; _iter++)
					Scores[_iter]	= new iOSScore((IDictionary)_scoresJSONList[_iter]);
			}
			
			// Set local score info
			if (_localScoreInfoDict != null)
				LocalUserScore		= new iOSScore(_localScoreInfoDict);
		}
		
		public IDictionary GetLeaderboardInfoJSONObject ()
		{
			IDictionary		_JSONDict		= new Dictionary<string, object>();
			_JSONDict[kIdentifierKey]		= Identifier;
			_JSONDict[kRangeFromKey]		= m_range.from;
			_JSONDict[kRangeLengthKey]		= m_range.count;
			_JSONDict[kTimeScopeKey]		= (int)TimeScope;
			_JSONDict[kPlayerScopeKey]		= (int)UserScope;
			_JSONDict[GameServicesIOS.kObjectInstanceIDKey]	= GetInstanceID();

			return _JSONDict;
		}
		
		#endregion

		#region Event Callback Methods
		
		protected override void LoadScoresFinished (IDictionary _dataDict)
		{
			string		_error		= _dataDict.GetIfAvailable<string>(GameServicesIOS.kNativeMessageErrorKey);
			IDictionary _infoDict	= _dataDict.GetIfAvailable<IDictionary>(kLeaderboardInfoKey);

			if (_infoDict != null)
			{
				// Update title
				Title							= _infoDict.GetIfAvailable<string>(kTitleKey);
				
				// Update scores
				IList		_scoresJSONList		= _infoDict.GetIfAvailable<IList>(kScoresKey);
				IDictionary	_localScoreInfoDict	= _infoDict.GetIfAvailable<IDictionary>(kLocalScoreKey);
				
				SetScores(_scoresJSONList, _localScoreInfoDict);
			}

			// Call finish handler
			LoadScoresFinished(Scores, LocalUserScore, _error);
		}

		#endregion
	}
}
#endif