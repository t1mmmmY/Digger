using UnityEngine;
using System.Collections;
using System;
using VoxelBusters.Utility;
using System.Collections.Generic;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	/// <summary>
	/// <see cref="VoxelBusters.NativePlugins.Achievement"/> object provides interface to communicate with game server about <see cref="VoxelBusters.NativePlugins.LocalUser"/> progress towards completing achievement.
	/// </summary>
	public abstract class Achievement : NPObject
	{
		#region Properties

		/// <summary>
		/// Gets the unique identifier of <see cref="VoxelBusters.NativePlugins.Achievement"/> which is common for all supported platforms.
		/// </summary>
		/// <value>A string used to uniquely identify <see cref="VoxelBusters.NativePlugins.Achievement"/> across all supported platforms.</value>
		public string GlobalIdentifier
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the identifier of <see cref="VoxelBusters.NativePlugins.Achievement"/> specific to current platform.
		/// </summary>
		/// <value>A string used to uniquely identify <see cref="VoxelBusters.NativePlugins.Achievement"/> specific to current platform.</value>
		public abstract string Identifier
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets or set the value indicating points scored for <see cref="VoxelBusters.NativePlugins.Achievement"/>.
		/// </summary>
		/// <value>Value indicates how far the player has progressed on this <see cref="VoxelBusters.NativePlugins.Achievement"/>.</value>
		public abstract int PointsScored
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the value indicating <see cref="VoxelBusters.NativePlugins.Achievement"/> progress in terms of percentage.
		/// </summary>
		/// <value>Value indicates how far the player has progressed on this <see cref="VoxelBusters.NativePlugins.Achievement"/> in terms of percentage.</value>
		public double PercentageCompleted
		{
			get
			{
				if (Description == null)
					return 0;

				double _percentageCompleted	= ((double)PointsScored * 100 / (double)Description.MaximumPoints);

				return System.Math.Min(100.0, _percentageCompleted);
			}
		}

		/// <summary>
		/// Gets the value indicating whether this <see cref="VoxelBusters.NativePlugins.Achievement"/> is completed.
		/// </summary>
		/// <value>States whether the player has completed the <see cref="VoxelBusters.NativePlugins.Achievement"/>.</value>
		public abstract bool Completed
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the last reported date of this <see cref="VoxelBusters.NativePlugins.Achievement"/>.
		/// </summary>
		/// <value>The last time that progress on the <see cref="VoxelBusters.NativePlugins.Achievement"/> was successfully reported.</value>
		public abstract DateTime LastReportedDate
		{
			get;
			protected set;
		}

		protected AchievementDescription Description
		{
			get
			{
				return AchievementHandler.GetAchievementDescription(Identifier);
			}
		}

		#endregion

		#region Delegates

		/// <summary>
		/// The callback delegate used when load <see cref="VoxelBusters.NativePlugins.Achievement"/> request completes.
		/// </summary>
		/// <param name="_achievements">An array of <see cref="VoxelBusters.NativePlugins.Achievement"/> objects that represents all progress reported to Server for the local user.</param>
		/// <param name="_error">If the operation was successful, this value is nil; otherwise, this parameter holds the description of the problem that occurred.</param>
		public delegate void LoadAchievementsCompletion (Achievement[] _achievements, string _error);

		/// <summary>
		/// The callback delegate used when report see cref="VoxelBusters.NativePlugins.Achievement"/> progress request completes.
		/// </summary>
		/// <param name="_success">The operation completion status.</param>
		/// <param name="_error">If the operation was successful, this value is nil; otherwise, this parameter holds the description of the problem that occurred.</param>
		public delegate void ReportProgressCompletion (bool _success, string _error);

		#endregion

		#region Events

		private event ReportProgressCompletion ReportProgressFinishedEvent;

		#endregion

		#region Constructor

		protected Achievement () : base (NPObjectManager.eCollectionType.GAME_SERVICES)
		{}

		protected Achievement (string _globalIdentifier, string _identifier, int _pointsScored, bool _completed, DateTime _reportedDate) 
			: base (NPObjectManager.eCollectionType.GAME_SERVICES)
		{	
			// Initialize properties
			GlobalIdentifier	= _globalIdentifier;
			Identifier			= _identifier;
			PointsScored		= _pointsScored;
			Completed			= _completed;
			LastReportedDate	= _reportedDate;
		}

		protected Achievement (string _globalIdentifier, string _identifier, int _pointsScored = 0) 
			: this (_globalIdentifier, _identifier, _pointsScored, false, DateTime.Now)
		{}

		#endregion

		#region Methods

		/// <summary>
		/// Reports the player’s progress of this <see cref="VoxelBusters.NativePlugins.Achievement"/>.
		/// </summary>
		/// <param name="_onCompletion">Callback to be called when operation is completed.</param>
		public virtual void ReportProgress (ReportProgressCompletion _onCompletion)
		{
			// Cache callback
			ReportProgressFinishedEvent	= _onCompletion;

			// Check if its valid request
			if (Description == null)
			{
				DebugPRO.Console.LogError(Constants.kDebugTag, string.Format("[GameServices] Report progress failed. Achievement description with id={0} couldnt be found.", Identifier)); 
				ReportProgressFinished(false, "The requested operation could not be completed because description for this achievement was not found.");
				return;
			}
		}
		
		public override string ToString ()
		{
			return string.Format("[Achievement: Identifier={0}, PointsScored={1}, Completed={2}, LastReportedDate={3}]",
			                     Identifier, PointsScored, Completed, LastReportedDate);
		}
		
		#endregion

		#region Event Callback Methods

		protected virtual void ReportProgressFinished (IDictionary _dataDict)
		{}

		protected void ReportProgressFinished (bool _success, string _error)
		{
			if (ReportProgressFinishedEvent != null)
				ReportProgressFinishedEvent(_success, _error);
		}

		#endregion
	}
}