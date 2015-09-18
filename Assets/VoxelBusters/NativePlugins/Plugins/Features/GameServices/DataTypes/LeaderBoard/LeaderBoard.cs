using UnityEngine;
using System.Collections;
using System;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins
{	
	using Internal;

	/// <summary>
	/// <see cref="VoxelBusters.NativePlugins.Leaderboard"/> object is used to read data from a leaderboard stored on game service servers i.e,  Game Center for iOS, Google Play Services for Android.
	/// </summary>
 	public abstract class Leaderboard
	{
		#region Constants

		protected	const	int 	kLoadScoresMinResults		= 1;

#if UNITY_ANDROID
		protected	const	int 	kLoadScoresMaxResults		= 25; //On android  max of 25 results can be loaded
#else
		protected	const	int 	kLoadScoresMaxResults		= 100;
#endif

		#endregion

		#region Fields

		private				int		m_maxResults;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the identifier of <see cref="VoxelBusters.NativePlugins.Leaderboard"/>.
		/// </summary>
		/// <value>The identifier of <see cref="VoxelBusters.NativePlugins.Leaderboard"/>.</value>
		public abstract string Identifier
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the title for the <see cref="VoxelBusters.NativePlugins.Leaderboard"/>.
		/// </summary>
		/// <value>The title for the <see cref="VoxelBusters.NativePlugins.Leaderboard"/>.</value>
		public abstract string Title
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets or sets the value of user based filter mode.
		/// </summary>
		/// <value>A filter used to restrict the search to a subset of the users.</value>
		public abstract eLeaderboardUserScope UserScope
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the value of time based filter mode.
		/// </summary>
		/// <value>A filter used to restrict the search to scores that were posted within a specific period of time.</value>
		public abstract eLeaderboardTimeScope TimeScope
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the max <see cref="VoxelBusters.NativePlugins.Score"/> results that has to be fetched from search.
		/// </summary>
		/// <value>The max <see cref="VoxelBusters.NativePlugins.Score"/> results that has to be fetched from search.</value>
		public int MaxResults
		{
			get
			{
				return Mathf.Clamp(m_maxResults, kLoadScoresMinResults, kLoadScoresMaxResults);
			}

			set
			{
				m_maxResults	= value;
			}
		}

		/// <summary>
		/// Gets the array of <see cref="VoxelBusters.NativePlugins.Score"/> returned by search.
		/// </summary>
		/// <value>The array of <see cref="VoxelBusters.NativePlugins.Score"/> returned by search.</value>
		public abstract Score[] Scores
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the <see cref="VoxelBusters.NativePlugins.Score"/> earned by the <see cref="VoxelBusters.NativePlugins.LocalUser"/>.
		/// </summary>
		/// <value>The <see cref="VoxelBusters.NativePlugins.Score"/> earned by the <see cref="VoxelBusters.NativePlugins.LocalUser"/>.</value>
		public abstract Score LocalUserScore
		{
			get;
			protected set;
		}

		#endregion	

		#region delegate

		/// <summary>
		/// Gets the local user score.
		/// </summary>
		public delegate void LoadScoreCompletion (Score[] _scores, Score _localUserScore);

		#endregion

		#region Constructors

		protected Leaderboard ()
		{}

		protected Leaderboard (string _identifier, string _title = null) : this (_identifier, _title, eLeaderboardUserScope.GLOBAL, eLeaderboardTimeScope.ALL_TIME, kLoadScoresMaxResults, null, null)
		{}

		protected Leaderboard (string _identifier, string _title, eLeaderboardUserScope _userScope, eLeaderboardTimeScope _timeScope, int _maxResults, Score[] _scores, Score _localUserScore)
		{
			// Initialize properties
			Identifier			= _identifier;
			Title				= _title;
			UserScope			= _userScope;
			TimeScope			= _timeScope;
			MaxResults			= _maxResults;
			Scores				= _scores;
			LocalUserScore		= _localUserScore;
		}

		#endregion

		#region Abstract Methods

		/// <summary>
		/// Asynchronously loads the top scores.
		/// </summary>
		/// <param name="_onCompletion">Callback to be called when score results are retrieved from the game service server.</param>
		public abstract	void LoadTopScores (LoadScoreCompletion _onCompletion);

		/// <summary>
		/// Asynchronously loads the player-centered scores.
		/// </summary>
		/// <param name="_onCompletion">Callback to be called when score results are retrieved from the game service server.</param>
		public abstract	void LoadPlayerCenteredScores (LoadScoreCompletion _onCompletion);

		/// <summary>
		/// Asynchronously loads an additional score data.
		/// </summary>
		/// <param name="_pageDirection">The direction of pagination.</param>
		/// <param name="_onCompletion">Callback to be called when score results are retrieved from the game service server.</param>
		public abstract	void LoadMoreScores (eLeaderboardPageDirection _pageDirection, LoadScoreCompletion _onCompletion);

		#endregion

		#region Override Methods

		public override string ToString ()
		{
			return string.Format("[Leaderboard: Identifier={0}, UserScope={1}, TimeScope={2}]", Identifier, UserScope, TimeScope);
		}

		#endregion
	}
}