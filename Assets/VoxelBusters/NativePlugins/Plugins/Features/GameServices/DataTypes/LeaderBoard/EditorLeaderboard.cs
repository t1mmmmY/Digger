using UnityEngine;
using System.Collections;
using System;

#if UNITY_EDITOR
using System.Runtime.Serialization;

namespace VoxelBusters.NativePlugins.Internal
{
	internal sealed class EditorLeaderboard : Leaderboard
	{
		#region Fields

		private		LoadScoreCompletion		m_loadScoreCompletionCallback;

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

		private EditorLeaderboard ()
		{}

		internal EditorLeaderboard (string _identifier) : base (_identifier)
		{}

		#endregion

		#region Methods

		public override	void LoadTopScores (LoadScoreCompletion _onCompletion)
		{
			// Cache callback
			m_loadScoreCompletionCallback	= _onCompletion;

			// Load scores
			EditorGameCenter.Instance.LoadTopScores(this, LoadScoreRequestFinished);
		}
		
		public override	void LoadPlayerCenteredScores (LoadScoreCompletion _onCompletion)
		{
			// Cache callback
			m_loadScoreCompletionCallback	= _onCompletion;
			
			// Load scores
			EditorGameCenter.Instance.LoadPlayerCenteredScores(this, LoadScoreRequestFinished);
		}
		
		public override	void LoadMoreScores (eLeaderboardPageDirection _pageDirection, LoadScoreCompletion _onCompletion)
		{
			// Cache callback
			m_loadScoreCompletionCallback	= _onCompletion;
			
			// Load scores
			EditorGameCenter.Instance.LoadMoreScores(this, _pageDirection, LoadScoreRequestFinished);
		}

		private void LoadScoreRequestFinished (Score[] _scoreList, Score _localUserScore)
		{
			// Update values
			this.Scores			= _scoreList;
			this.LocalUserScore	= _localUserScore;
			
			// Call back
			if (m_loadScoreCompletionCallback != null)
				m_loadScoreCompletionCallback(_scoreList, _localUserScore);
		}

		#endregion
	}
}
#endif