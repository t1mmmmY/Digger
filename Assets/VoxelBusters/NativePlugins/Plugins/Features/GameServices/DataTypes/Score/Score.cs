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
	public abstract class Score
	{
		#region Properties
	
		/// <summary>
		/// Gets the identifier of the leaderboard that the score is being sent to.
		/// </summary>
		/// <value>Identifies the leaderboard that the score is being sent to.</value>
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
		
		#region Constructor
		
		protected Score ()
		{}

		protected Score (string _leaderboardID, User _user, long _scoreValue) 
		{
			// Initialize properties
			LeaderboardID			= _leaderboardID;
			User					= _user;
			Value					= _scoreValue;
			Date					= DateTime.Now;
			Rank					= -1;
		}	
		
		#endregion
		
		#region Abstract Methods

		/// <summary>
		/// Reports the score to game service server.
		/// </summary>
		/// <param name="_onCompletion">Callback to be called when operation is completed.</param>
		public abstract void ReportScore (Action<bool> _onCompletion);

		#endregion
		
		#region Override Methods
		
		public override string ToString ()
		{
			return string.Format("[Score: Rank={0}, UserName={1}, Value={2}]", Rank, User.Name, Value);
		}

		#endregion
	}
}