using UnityEngine;
using System.Collections;

#if USES_GAME_SERVICES && UNITY_EDITOR
using System;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins.Internal
{
	public sealed class EditorAchievement : Achievement
	{
		#region Properties

		public override string Identifier
		{
			get;
			protected set;
		}
		
		public override int PointsScored
		{
			get;
			set;
		}
		
		public override bool Completed
		{
			get;
			protected set;
		}
		
		public override DateTime LastReportedDate
		{
			get;
			protected set;
		}

		#endregion

		#region Constructors

		private EditorAchievement ()
		{}

		public EditorAchievement (string _globalIdentifier, string _identifier, int _pointsScored = 0) 
			: base (_globalIdentifier, _identifier, _pointsScored)
		{}

		public EditorAchievement (EGCAchievement _gcAchievementInfo)
		{
			// Set properties from info object
			Identifier					= _gcAchievementInfo.Identifier;
			PointsScored				= _gcAchievementInfo.PointsScored;
			Completed					= _gcAchievementInfo.Completed;
			LastReportedDate			= _gcAchievementInfo.LastReportedDate;

			// Set global identifier			
			GlobalIdentifier			= GameServicesIDHandler.GetAchievementGID(Identifier);
		}
		
		#endregion

		#region Static Methods
		
		public static EditorAchievement[] ConvertAchievementsList (IList _gcAchievements)
		{
			if (_gcAchievements == null)
				return null;
			
			int 				_count				= _gcAchievements.Count;
			EditorAchievement[]	_achievementsList	= new EditorAchievement[_count];
			
			for (int _iter = 0; _iter < _count; _iter++)
				_achievementsList[_iter]			= new EditorAchievement((EGCAchievement)_gcAchievements[_iter]);
			
			return _achievementsList;
		}
		
		#endregion

		#region Methods

		public override void ReportProgress (ReportProgressCompletion _onCompletion)
		{
			base.ReportProgress (_onCompletion);
			
			if (Description == null)
				return;
			
			EditorGameCenter.Instance.ReportProgress(this);
		}

		#endregion

		#region Event Callback Methods
		
		protected override void ReportProgressFinished (IDictionary _dataDict)
		{
			string			_error				= _dataDict.GetIfAvailable<string>(EditorGameCenter.kErrorKey);
			EGCAchievement 	_gcAchievementInfo	= _dataDict.GetIfAvailable<EGCAchievement>(EditorGameCenter.kAchievementInfoKey);
			
			if (_gcAchievementInfo != null)
			{
				// Update properties
				PointsScored		= _gcAchievementInfo.PointsScored;
				Completed			= _gcAchievementInfo.Completed;
				LastReportedDate	= _gcAchievementInfo.LastReportedDate;
			}
			
			ReportProgressFinished(_error == null, _error);
		}
		
		#endregion
	}
}
#endif