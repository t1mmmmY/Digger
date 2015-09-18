using UnityEngine;
using System.Collections;
using System;

#if UNITY_EDITOR
namespace VoxelBusters.NativePlugins.Internal
{
	internal sealed class EditorScore : Score
	{
		#region Properties

		public override string LeaderboardID
		{
			get;
			protected set;
		}
		
		public override User User
		{
			get;
			protected set;
		}
		
		public override long Value
		{
			get;
			set;
		}
		
		public override DateTime Date
		{
			get;
			protected set;
		}
		
		public override int Rank
		{
			get;
			protected set;
		}
		
		#endregion
		
		#region Constructors

		internal EditorScore ()
		{}

		internal EditorScore (string _leaderboardID, User _user, long _scoreValue = 0L) : base (_leaderboardID, _user, _scoreValue)
		{}

		internal EditorScore (EditorGameCenter.EGCScore _scoreInfo)
		{
			LeaderboardID	= _scoreInfo.LeaderboardID;
			User			= _scoreInfo.User;
			Value			= _scoreInfo.Value;
			Date			= _scoreInfo.Date;
			Rank			= _scoreInfo.Rank;
		}

		#endregion

		#region Methods

		public override void ReportScore (Action<bool> _onCompletion)
		{
			EditorGameCenter.Instance.ReportScore(this, (EditorScore _newScoreInfo)=>{

				if (_newScoreInfo == null)
				{
					if (_onCompletion != null)
						_onCompletion(false);
				}
				else
				{
					// Update properties
					this.Rank	= _newScoreInfo.Rank;

					if (_onCompletion != null)
						_onCompletion(true);
				}
			});
		}

		#endregion
	}
}
#endif