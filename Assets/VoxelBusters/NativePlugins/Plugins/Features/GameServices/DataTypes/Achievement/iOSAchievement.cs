using UnityEngine;
using System.Collections;

#if USES_GAME_SERVICES && UNITY_IOS
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins.Internal
{
	public sealed class iOSAchievement : Achievement 
	{
		#region Constants

		private		const 	string		kAchievementInfoKey		= "achievement-info";
		private 	const 	string 		kIdentifierKey			= "id";
		private 	const 	string 		kPercentCompleteKey		= "percent-complete";
		private 	const 	string 		kCompletedKey			= "completed";
		private 	const 	string 		kLastReportedDateKey	= "last-reported-date";

		#endregion

		#region Fields

		private				int			m_pointsScored;
		private 			double		m_percentComplete;

		#endregion

		#region Properties

		public override	string Identifier
		{
			get;
			protected set;
		}

		public override	int PointsScored
		{
			get
			{
				return m_pointsScored;
			}

			set
			{
				if (Description == null || value <= 0)
				{
					m_pointsScored		= 0;
					m_percentComplete	= 0f;
				}
				else
				{
					// Set points scored
					int		_maxPoints	= Description.MaximumPoints;
					m_pointsScored		= Mathf.Min(value, _maxPoints);

					// Update percent completed
					m_percentComplete	= ((double)m_pointsScored / (double)_maxPoints) * 100;
				}
			}
		}
		
		public override	bool Completed
		{
			get;
			protected set;
		}
		
		public override	DateTime LastReportedDate
		{
			get;
			protected set;
		}
		
		#endregion
		
		#region Constructors

		private iOSAchievement ()
		{}

		public iOSAchievement (string _globalIdentifier, string _identifier, int _pointsScored = 0) 
			: base (_globalIdentifier, _identifier, _pointsScored)
		{}
		                     
		public iOSAchievement (IDictionary _dataDict)
		{
			// Parse data dictionary values
			Identifier			= _dataDict.GetIfAvailable<string>(kIdentifierKey);
			Completed			= _dataDict.GetIfAvailable<bool>(kCompletedKey);
			LastReportedDate	= _dataDict.GetIfAvailable<string>(kLastReportedDateKey).ToZuluFormatDateTimeLocal();

			SetPercentComplete(_dataDict.GetIfAvailable<double>(kPercentCompleteKey));

			// Set global identifier			
			GlobalIdentifier	= GameServicesIDHandler.GetAchievementGID(Identifier);
		}
		
		#endregion
		
		#region External Methods
		
		[DllImport("__Internal")]
		private static extern void reportProgress (string _achievementInfoJSON, double _percentComplete);
		
		#endregion

		#region Static Methods

		public static iOSAchievement[] ConvertAchievementsList (IList _achievementsJSONList)
		{
			if (_achievementsJSONList == null)
				return null;
			
			int 				_count				= _achievementsJSONList.Count;
			iOSAchievement[]	_achievementsList	= new iOSAchievement[_count];
			
			for (int _iter = 0; _iter < _count; _iter++)
				_achievementsList[_iter]			= new iOSAchievement((IDictionary)_achievementsJSONList[_iter]);

			return _achievementsList;
		}

		#endregion

		#region Methods

		public override void ReportProgress (ReportProgressCompletion _onCompletion)
		{
			base.ReportProgress (_onCompletion);

			if (Description == null)
				return;

			// Native method call 
			reportProgress(GetAchievementInfoJSONObject().ToJSON(), m_percentComplete);
		}

		public IDictionary GetAchievementInfoJSONObject ()
		{
			IDictionary		_JSONDict		= new Dictionary<string, object>();
			_JSONDict[kIdentifierKey]		= Identifier;
			_JSONDict[GameServicesIOS.kObjectInstanceIDKey]	= GetInstanceID();

			return _JSONDict;
		}
		
		private void SetPercentComplete (double	_newValue)
		{
			if (Description == null)
			{
				m_percentComplete	= 0;
				m_pointsScored		= 0;
				return;
			}
			else
			{
				m_percentComplete	= _newValue;
				m_pointsScored		= Mathf.RoundToInt((float)m_percentComplete * 0.01f * Description.MaximumPoints);
				return;
			}
		}

		#endregion

		#region Event Callback Methods

		protected override void ReportProgressFinished (IDictionary _dataDict)
		{
			string		_error		= _dataDict.GetIfAvailable<string>(GameServicesIOS.kNativeMessageErrorKey);
			IDictionary _infoDict	= _dataDict.GetIfAvailable<IDictionary>(kAchievementInfoKey);

			if (_infoDict != null)
			{
				// Update properties
				Completed			= _infoDict.GetIfAvailable<bool>(kCompletedKey);
				LastReportedDate	= _infoDict.GetIfAvailable<string>(kLastReportedDateKey).ToZuluFormatDateTimeLocal();
				
				SetPercentComplete(_infoDict.GetIfAvailable<double>(kPercentCompleteKey));
			}

			ReportProgressFinished(_error == null, _error);
		}

		#endregion
	}
}
#endif