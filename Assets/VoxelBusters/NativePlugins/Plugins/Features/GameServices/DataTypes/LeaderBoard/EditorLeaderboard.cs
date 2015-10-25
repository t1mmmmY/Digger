using UnityEngine;
using System.Collections;

#if USES_GAME_SERVICES && UNITY_EDITOR
using System;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins.Internal
{
	public sealed class EditorLeaderboard : Leaderboard
	{
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

		private EditorLeaderboard ()
		{}

		public EditorLeaderboard (string _globalIdentifier, string _identifier) : base (_globalIdentifier, _identifier)
		{}

		public EditorLeaderboard (EGCLeaderboard _gcLeaderboardInfo)
		{
			// Set properties from info object
			Identifier			= _gcLeaderboardInfo.Identifier;
			Title 				= _gcLeaderboardInfo.Title;
			Scores				= null;
			LocalUserScore		= null;

			// Set global identifier
			GlobalIdentifier	= GameServicesIDHandler.GetLeaderboardGID(Identifier);
		}

		#endregion

		#region Methods

		public override	void LoadTopScores (LoadScoreCompletion _onCompletion)
		{
			base.LoadTopScores(_onCompletion);

			// Load scores
			EditorGameCenter.Instance.LoadTopScores(this);
		}
		
		public override	void LoadPlayerCenteredScores (LoadScoreCompletion _onCompletion)
		{
			base.LoadPlayerCenteredScores(_onCompletion);

			// Load scores
			EditorGameCenter.Instance.LoadPlayerCenteredScores(this);
		}
		
		public override	void LoadMoreScores (eLeaderboardPageDirection _pageDirection, LoadScoreCompletion _onCompletion)
		{
			base.LoadMoreScores(_pageDirection, _onCompletion);

			// Load scores
			EditorGameCenter.Instance.LoadMoreScores(this, _pageDirection);
		}

		private void SetScores (EGCScore[] _scoresList, EGCScore _localScore)
		{
			// Set default values
			Scores			= null;
			LocalUserScore	= null;
			
			// Set scores list
			if (_scoresList != null)
			{
				int	_count	= _scoresList.Length;
				Scores		= new EditorScore[_count];
				
				for (int _iter = 0; _iter < _count; _iter++)
					Scores[_iter]	= new EditorScore(_scoresList[_iter]);
			}
			
			// Set local score info
			if (_localScore != null)
				LocalUserScore		= new EditorScore(_localScore);
		}

		#endregion

		#region Event Callback Methods
		
		protected override void LoadScoresFinished (IDictionary _dataDict)
		{
			string			_error				= _dataDict.GetIfAvailable<string>(EditorGameCenter.kErrorKey);
			EGCLeaderboard 	_leaderboardInfo	= _dataDict.GetIfAvailable<EGCLeaderboard>(EditorGameCenter.kLeaderboardInfoKey);
			
			if (_leaderboardInfo != null)
			{
				// Update title
				Title							= _leaderboardInfo.Title;

				// Update scores
				SetScores(_leaderboardInfo.GetLastQueryResults(), _leaderboardInfo.LocalUserScore);
			}
			
			// Call finish handler
			LoadScoresFinished(Scores, LocalUserScore, _error);
		}
		
		#endregion
	}
}
#endif