using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using System;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins.Internal
{
	internal partial class EditorGameCenter : AdvancedScriptableObject <EditorGameCenter>
	{
		[Serializable]
		internal sealed class EGCAchievement : Achievement
		{
			#region Fields
	
			[SerializeField]
			private				string				m_identifier;

			[SerializeField]
			private				int					m_pointsScored;

			[SerializeField]
			private				bool				m_completed;

			[SerializeField]
			private				string				m_lastReportedDate;

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
				get
				{
					return m_pointsScored;
				}
				
				set
				{
					m_pointsScored	= value;
				}
			}

			public override bool Completed
			{
				get
				{
					return m_completed;
				}
				
				protected set
				{
					m_completed		= value;
				}
			}
			
			public override DateTime LastReportedDate
			{
				get
				{
					if (string.IsNullOrEmpty(m_lastReportedDate))
						return new DateTime();

					return DateTime.Parse(m_lastReportedDate);
				}

				protected set
				{
					m_lastReportedDate	= value.ToString();
				}
			}
			
			#endregion

			#region Constructor

			public EGCAchievement (string _identifier, int _pointsScored, bool _completed, DateTime _reportedDate) : base (_identifier, _pointsScored, _completed, _reportedDate)
			{}

			#endregion
			
			#region Static Methods
			
			public static EditorAchievement[] ConvertToEditorAchievementList (EGCAchievement[] _gcAchievementList)
			{
				if (_gcAchievementList == null)
					return null;
				
				int					_count				= _gcAchievementList.Length;
				EditorAchievement[]	_newAchievementList	= new EditorAchievement[_count];
				
				for (int _iter = 0; _iter < _count; _iter++)
					_newAchievementList[_iter]			= _gcAchievementList[_iter].GetEditorFormatData();
				
				return _newAchievementList;
			}
			
			#endregion

			#region Methods

			public void SetCompleted (bool _completed)
			{
				Completed			= _completed;
			}
			
			public void SetLastReportedDate (DateTime _lastReportedDate)
			{
				LastReportedDate	= _lastReportedDate;
			}
			
			public EditorAchievement GetEditorFormatData ()
			{
				return new EditorAchievement(this);
			}

			#endregion
		}
	}
}
#endif