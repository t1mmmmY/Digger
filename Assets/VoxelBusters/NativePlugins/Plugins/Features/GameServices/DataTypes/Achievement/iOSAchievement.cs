using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SocialPlatforms;

#if UNITY_IOS
namespace VoxelBusters.NativePlugins.Internal
{
	internal sealed class iOSAchievement : Achievement 
	{
		#region Properties
		
		private		IAchievement		m_achievementData;

		#endregion

		#region Properties

		public override	string Identifier
		{
			get
			{
				return m_achievementData.id;
			}
			
			protected set
			{
				m_achievementData.id	= value;
			}
		}

		public override	int PointsScored
		{
			get
			{
				if (Description == null)
					return 0;

				return Mathf.RoundToInt((float)m_achievementData.percentCompleted * 0.01f * Description.MaximumPoints);
			}

			set
			{
				int		_newScoreValue		= value;
				double	_percentageValue;

				if (Description == null || _newScoreValue < 0)
				{
					_percentageValue		= 0f;
				}
				else
				{
					int		_maxPoints		= Description.MaximumPoints;
					_newScoreValue			= Mathf.Min(_newScoreValue, _maxPoints);
					_percentageValue		= ((float)_newScoreValue / (float)_maxPoints) * 100f;
				}

				// Set new value
				m_achievementData.percentCompleted	= _percentageValue;
			}
		}
		
		public override	bool Completed
		{
			get
			{
				return m_achievementData.completed;
			}
			
			protected set
			{
				throw new Exception("[GameServices] Only getter is supported.");
			}
		}
		
		public override	DateTime LastReportedDate
		{
			get
			{
				return m_achievementData.lastReportedDate;
			}

			protected set
			{
				throw new Exception("[GameServices] Only getter is supported.");
			}
		}
		
		#endregion
		
		#region Constructors

		private iOSAchievement ()
		{}

		internal iOSAchievement (string _identifier)
		{
			m_achievementData	= Social.CreateAchievement();
			Identifier			= _identifier;
		}
		
		internal iOSAchievement (IAchievement _achievementData)
		{
			m_achievementData	= _achievementData;
		}
		
		#endregion

		#region Static Methods

		internal static iOSAchievement[] ConvertAchievementList (IAchievement[] _achievementList)
		{
			if (_achievementList == null)
				return null;

			int 				_count				= _achievementList.Length;
			iOSAchievement[]	_iOSAchievementList	= new iOSAchievement[_count];
			
			for (int _iter = 0; _iter < _count; _iter++)
				_iOSAchievementList[_iter]			= new iOSAchievement(_achievementList[_iter]);

			return _iOSAchievementList;
		}

		#endregion

		#region Methods

		public override void ReportProgress (Action<bool> _onCompletion)
		{
			base.ReportProgress(_onCompletion);

			if (Description == null)
				return;

			// Report progress to GameCenter
			m_achievementData.ReportProgress(OnReportProgressFinished);
		}

		#endregion
	}
}
#endif