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
	public abstract class Leaderboard : NPObject
	{
		#region Constants

		protected	const	int 		kLoadScoresMinResults		= 1;

#if UNITY_ANDROID
		protected	const	int 		kLoadScoresMaxResults		= 25; //On android  max of 25 results can be loaded
#else	
		protected	const	int 		kLoadScoresMaxResults		= 100;
#endif

		#endregion

		#region Fields

		private				int			m_maxResults;

		#endregion

		#region Properties
		
		/// <summary>
		/// Gets the unique identifier of <see cref="VoxelBusters.NativePlugins.Leaderboard"/> which is common for all supported platforms.
		/// </summary>
		/// <value>A string used to uniquely identify <see cref="VoxelBusters.NativePlugins.Leaderboard"/> across all supported platforms.</value>
		public string GlobalIdentifier
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the identifier of <see cref="VoxelBusters.NativePlugins.Leaderboard"/> specific to current platform.
		/// </summary>
		/// <value>A string used to uniquely identify <see cref="VoxelBusters.NativePlugins.Leaderboard"/> specific to current platform.</value>
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

		#region Delegates

		/// <summary>
		/// The callback delegate used when load <see cref="VoxelBusters.NativePlugins.Score"/> request completes.
		/// </summary>
		/// <param name="_scores">An array of <see cref="VoxelBusters.NativePlugins.Score"/> objects that holds the requested scores.</param>
		/// <param name="_localUserScore">The score earned by the local user.</param>
		/// <param name="_error">If the operation was successful, this value is nil; otherwise, this parameter holds the description of the problem that occurred.</param>
		public delegate void LoadScoreCompletion (Score[] _scores, Score _localUserScore, string _error);

		#endregion
		
		#region Events

		private event LoadScoreCompletion LoadScoreFinishedEvent;
		
		#endregion

		#region Constructors

		protected Leaderboard () : base (NPObjectManager.eCollectionType.GAME_SERVICES)
		{}

		protected Leaderboard (string _globalIdentifer, string _identifier, string _title = null, eLeaderboardUserScope _userScope = eLeaderboardUserScope.GLOBAL, eLeaderboardTimeScope _timeScope = eLeaderboardTimeScope.ALL_TIME, int _maxResults = 20, Score[] _scores = null, Score _localUserScore = null)
			: base (NPObjectManager.eCollectionType.GAME_SERVICES)
		{
			// Initialize properties
			GlobalIdentifier	= _globalIdentifer;
			Identifier			= _identifier;
			Title				= _title;
			UserScope			= _userScope;
			TimeScope			= _timeScope;
			MaxResults			= _maxResults;
			Scores				= _scores;
			LocalUserScore		= _localUserScore;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Asynchronously loads the top scores.
		/// </summary>
		/// <param name="_onCompletion">Callback to be called when score results are retrieved from the game service server.</param>
		public virtual void LoadTopScores (LoadScoreCompletion _onCompletion)
		{
			// Cache callback information
			LoadScoreFinishedEvent	= _onCompletion;
		}

		/// <summary>
		/// Asynchronously loads the player-centered scores.
		/// </summary>
		/// <param name="_onCompletion">Callback to be called when score results are retrieved from the game service server.</param>
		public virtual	void LoadPlayerCenteredScores (LoadScoreCompletion _onCompletion)
		{
			// Cache callback information
			LoadScoreFinishedEvent	= _onCompletion;
		}

		/// <summary>
		/// Asynchronously loads an additional score data.
		/// </summary>
		/// <param name="_pageDirection">The direction of pagination.</param>
		/// <param name="_onCompletion">Callback to be called when score results are retrieved from the game service server.</param>
		public virtual void LoadMoreScores (eLeaderboardPageDirection _pageDirection, LoadScoreCompletion _onCompletion)
		{
			// Cache callback information
			LoadScoreFinishedEvent	= _onCompletion;
		}
		
		public override string ToString ()
		{
			return string.Format("[Leaderboard: Identifier={0}, UserScope={1}, TimeScope={2}]", Identifier, UserScope, TimeScope);
		}

		protected void SetLoadScoreFinishedEvent (LoadScoreCompletion _onCompletion)
		{
			LoadScoreFinishedEvent	= _onCompletion;
		}

		#endregion

		#region Event Callback Methods

		protected virtual void LoadScoresFinished (IDictionary _dataDict)
		{}

		protected void LoadScoresFinished (Score[] _scores, Score _localUserScore, string _error)
		{
			// Set properties
			Scores			= _scores;
			LocalUserScore	= _localUserScore;

			// Send event
			if (LoadScoreFinishedEvent != null)
				LoadScoreFinishedEvent(_scores, _localUserScore, _error);
		}

		#endregion
	}
}