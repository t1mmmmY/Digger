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
	public abstract class Achievement
	{
		#region Fields
		
		private static Dictionary<string,Action<bool>> m_reportProgressCallbackList = new Dictionary<string,Action<bool>>();
		
		#endregion

		#region Properties

		/// <summary>
		/// Gets the identifier of <see cref="VoxelBusters.NativePlugins.Achievement"/>.
		/// </summary>
		/// <value>A string used to uniquely identify the specific <see cref="VoxelBusters.NativePlugins.Achievement"/> object refers to.</value>
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
		public int PercentageCompleted
		{
			get
			{
				int _percentageCompleted	= (int)((float)PointsScored * 100 / (float)Description.MaximumPoints);

				return Mathf.Min(100, _percentageCompleted);
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

		#region Constructor

		protected Achievement ()
		{}

		protected Achievement (string _identifier) : this (_identifier, 0, false, DateTime.Now)
		{}

		protected Achievement (string _identifier, int _pointsScored, bool _completed, DateTime _reportedDate)
		{
			// Initialize properties
			Identifier			= _identifier;
			PointsScored		= _pointsScored;
			Completed			= _completed;
			LastReportedDate	= _reportedDate;
		}

		#endregion

		#region Abstract Methods

		/// <summary>
		/// Reports the player’s progress of this <see cref="VoxelBusters.NativePlugins.Achievement"/>.
		/// </summary>
		/// <param name="_onCompletion">Callback to be called when operation is completed.</param>
		public virtual void ReportProgress (Action<bool> _onCompletion)
		{
			if (Description == null)
			{
				DebugPRO.Console.LogError(Constants.kDebugTag, string.Format("[GameServices] Report progress failed. Achievement description with id={0} couldnt be found.", Identifier)); 

				if (_onCompletion != null)
					_onCompletion(false);

				return;
			}

			m_reportProgressCallbackList.Remove(Identifier);
			m_reportProgressCallbackList.Add(Identifier, _onCompletion);
		}

		#endregion
		
		#region Override Methods
		
		public override string ToString ()
		{
			return string.Format("[Achievement: Identifier={0}, PointsScored={1}, Completed={2}, LastReportedDate={3}]", Identifier, PointsScored, Completed, LastReportedDate);
		}
		
		#endregion

		#region Callback Methods

		protected void OnReportProgressFinished (bool _status)
		{
			Action<bool> 	_requiredCallback;
			m_reportProgressCallbackList.TryGetValue(Identifier, out _requiredCallback);

			// Completion handler is called
			if(_requiredCallback != null)
			{
				m_reportProgressCallbackList.Remove(Identifier);
				_requiredCallback(_status);
			}
		}

		#endregion
	}
}