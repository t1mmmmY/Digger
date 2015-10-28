using UnityEngine;
using System.Collections;

#if USES_GAME_SERVICES && UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace VoxelBusters.NativePlugins.Internal
{
	[Serializable]
	public sealed class EGCAchievement
	{
		#region Fields
		
		[SerializeField]
		private				string				m_identifier;
		
		[SerializeField]
		private				int					m_pointsScored;
		
		[SerializeField]
		private				float				m_percentageCompleted;
		
		[SerializeField]
		private				bool				m_completed;
		
		[SerializeField]
		private				string				m_lastReportedDate;
		
		#endregion
		
		#region Properties
		
		public string Identifier
		{
			get
			{
				return m_identifier;
			}
			
			private set
			{
				m_identifier	= value;
			}
		}
		
		public int PointsScored
		{
			get
			{
				return m_pointsScored;
			}
			
			private set
			{
				m_pointsScored	= value;
			}
		}

		public float PercentageCompleted
		{
			get
			{
				return m_percentageCompleted;
			}

			private set
			{
				m_percentageCompleted	= value;
			}
		}
		
		public bool Completed
		{
			get
			{
				return m_completed;
			}
			
			private set
			{
				m_completed		= value;
			}
		}
		
		public DateTime LastReportedDate
		{
			get
			{
				if (string.IsNullOrEmpty(m_lastReportedDate))
					return new DateTime();
				
				return DateTime.Parse(m_lastReportedDate);
			}
			
			private set
			{
				m_lastReportedDate	= value.ToString();
			}
		}
		
		#endregion
		
		#region Constructor
		
		public EGCAchievement (string _identifier, int _pointsScored, int _maxPoints, bool _completed)
		{
			// Initialize
			Identifier			= _identifier;

			// Set progress
			UpdateProgress(_pointsScored, _maxPoints, _completed);
		}
		
		#endregion

		#region Methods

		public void UpdateProgress (int _pointsScored, int _maxPoints, bool _completed)
		{
			PointsScored		= _pointsScored;
			Completed			= _completed;
			LastReportedDate	= DateTime.Now;
			PercentageCompleted	= Mathf.Min(100.0f, ((float)_pointsScored * 100 / (float)_maxPoints));
		}

		#endregion
	}
}
#endif