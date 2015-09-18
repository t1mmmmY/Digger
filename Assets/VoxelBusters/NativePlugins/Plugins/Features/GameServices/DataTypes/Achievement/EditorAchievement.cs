using UnityEngine;
using System.Collections;
using System;

#if UNITY_EDITOR
namespace VoxelBusters.NativePlugins.Internal
{
	internal sealed class EditorAchievement : Achievement
	{
		#region Fields

		private			string				m_identifier;

		#endregion

		#region Properties

		public override string Identifier
		{
			get
			{
				return m_identifier;
			}

			protected set
			{
				m_identifier	= value;
			}
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

		internal EditorAchievement (string _identifier) : base (_identifier)
		{}

		internal EditorAchievement (EditorGameCenter.EGCAchievement _achievement)
		{
			Identifier			= _achievement.Identifier;
			PointsScored		= _achievement.PointsScored;
			Completed			= _achievement.Completed;
			LastReportedDate	= _achievement.LastReportedDate;
		}
		
		#endregion

		#region Methods

		public override void ReportProgress (Action<bool> _onCompletion)
		{
			base.ReportProgress(_onCompletion);

			if (Description == null)
				return;

			// Report progress
			EditorGameCenter.Instance.ReportProgress(this, (EditorAchievement _achievementInfo)=>{

				if (_achievementInfo == null)
				{
					// Invoke completion handler
					OnReportProgressFinished(false);
				}
				else
				{
					// Set properties
					LastReportedDate	= _achievementInfo.LastReportedDate;
					Completed			= _achievementInfo.Completed;

					// Invoke completion handler
					OnReportProgressFinished(true);
				}
			});
		}

		#endregion
	}
}
#endif