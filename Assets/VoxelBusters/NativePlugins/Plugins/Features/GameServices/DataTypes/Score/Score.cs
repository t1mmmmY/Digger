using UnityEngine;
using System.Collections;
using System;
using System.Globalization;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	/// <summary>
	/// <see cref="VoxelBusters.NativePlugins.Score"/> class holds information for a score that was earned by the <see cref="VoxelBusters.NativePlugins.User"/>.
	/// </summary>
	public abstract class Score : NPObject
	{
		#region Properties

		/// <summary>
		/// Gets the unique identifier of <see cref="VoxelBusters.NativePlugins.Leaderboard"/> which is common for all supported platforms.
		/// </summary>
		/// <value>A string used to uniquely identify <see cref="VoxelBusters.NativePlugins.Leaderboard"/> across all supported platforms.</value>
		public string LeaderboardGlobalID
		{
			get;
			protected set;
		}
	
		/// <summary>
		/// Gets the identifier of <see cref="VoxelBusters.NativePlugins.Leaderboard"/> specific to current platform.
		/// </summary>
		/// <value>A string used to uniquely identify <see cref="VoxelBusters.NativePlugins.Leaderboard"/> specific to current platform.</value>
		public abstract string LeaderboardID
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the <see cref="VoxelBusters.NativePlugins.User"/> that earned the score.
		/// </summary>
		/// <value>The <see cref="VoxelBusters.NativePlugins.User"/> that earned the score.</value>
		public abstract User User
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the score earned by <see cref="VoxelBusters.NativePlugins.User"/>.
		/// </summary>
		/// <value>The score earned by <see cref="VoxelBusters.NativePlugins.User"/>.</value>
		public abstract long Value
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the date and time when score was earned.
		/// </summary>
		/// <value>The date and time when score was earned.</value>
		public abstract DateTime Date
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the player’s score as a formatted string.
		/// </summary>
		/// <value>Returns the player’s score as a formatted string.</value>
		public virtual string FormattedValue
		{
			get
			{
				return Value.ToString("0,0", CultureInfo.InvariantCulture);
			}
		}

		/// <summary>
		/// Gets the position of the score in <see cref="VoxelBusters.NativePlugins.Leaderboard"/>.
		/// </summary>
		/// <value>The position of the score in <see cref="VoxelBusters.NativePlugins.Leaderboard"/>.</value>
		public abstract int Rank
		{
			get;
			protected set;
		}

		#endregion

		#region Delegates

		/// <summary>
		/// The callback delegate used when report see cref="VoxelBusters.NativePlugins.Score"/> request completes.
		/// </summary>
		/// <param name="_success">The operation completion status.</param>
		/// <param name="_error">If the operation was successful, this value is nil; otherwise, this parameter holds the description of the problem that occurred.</param>
		public delegate void ReportScoreCompletion (bool _success, string _error);

		#endregion

		#region Events

		protected event ReportScoreCompletion ReportScoreFinishedEvent;

		#endregion

		#region Constructor
		
		protected Score () : base (NPObjectManager.eCollectionType.GAME_SERVICES)
		{}

		protected Score (string _leaderboardGlobalID, string _leaderboardID, User _user, long _scoreValue) : base (NPObjectManager.eCollectionType.GAME_SERVICES)
		{
			// Initialize properties
			LeaderboardGlobalID	= _leaderboardGlobalID;
			LeaderboardID		= _leaderboardID;
			User				= _user;
			Value				= _scoreValue;
			Date				= DateTime.Now;
			Rank				= 0;
		}	
		
		#endregion
		
		#region Methods

		/// <summary>
		/// Reports the score to game service server.
		/// </summary>
		/// <param name="_onCompletion">Callback to be called when operation is completed.</param>
		public virtual void ReportScore (ReportScoreCompletion _onCompletion)
		{
			// Cache event
			ReportScoreFinishedEvent = _onCompletion;
		}
		
		public override string ToString ()
		{
			return string.Format("[Score: Rank={0}, UserName={1}, Value={2}]", 
			                     Rank, User.Name, Value);
		}

		#endregion

		#region Event Callback Methods

		protected virtual void ReportScoreFinished (IDictionary _dataDict)
		{}
		
		protected void ReportScoreFinished (bool _success, string _error)
		{
			if (ReportScoreFinishedEvent != null)
				ReportScoreFinishedEvent(_success, _error);
		}

		#endregion
	}
}