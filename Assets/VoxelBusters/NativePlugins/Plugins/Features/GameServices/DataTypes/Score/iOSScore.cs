using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SocialPlatforms;

#if UNITY_IOS
namespace VoxelBusters.NativePlugins.Internal
{
	internal sealed class iOSScore : Score 
	{
		#region Properties

		public override	string LeaderboardID
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

		private iOSScore ()
		{}

		internal iOSScore (IScore _scoreData, User _user)
		{
			// Initialize properties
			LeaderboardID	= _scoreData.leaderboardID;
			User			= _user;
			Value			= _scoreData.value;
			Date			= _scoreData.date;
			Rank			= _scoreData.rank;
		}

		internal iOSScore (string _leaderboardID, User _user, long _value = 0L) : base (_leaderboardID, _user, _value)
		{}

		#endregion

		#region Methods

		public override void ReportScore (Action<bool> _onCompletion)
		{
			Social.ReportScore(Value, LeaderboardID, _onCompletion);
		}

		#endregion
	}
}
#endif