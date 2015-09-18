using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using VoxelBusters.Utility;
using UnityEngine.SocialPlatforms;

namespace VoxelBusters.NativePlugins.Internal
{
	internal partial class EditorGameCenter : AdvancedScriptableObject <EditorGameCenter>
	{
		[Serializable]
		internal sealed class EGCLeaderboard
		{
			#region Fields

			[SerializeField]
			private				string					m_identifier;

			[SerializeField]
			private				string					m_title;

			[SerializeField]
			private				List<EGCScore>			m_scores;

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
			
			public string Title
			{
				get
				{
					return m_title;
				}
				
				private set
				{
					m_title		= value;
				}
			}
			
			public List<EGCScore> Scores
			{
				get
				{
					return m_scores;
				}

				private set
				{
					m_scores	= value;
				}
			}

			public Range Range
			{
				get;
				set;
			}
			
			#endregion

			#region Constructor

			public EGCLeaderboard (string _identifier, string _title) 
			{
				// Initialize properties
				Identifier	= _identifier;
				Title		= _title;
				Scores		= new List<EGCScore>();
				Range		= new Range(0, 0);
			}

			#endregion
			
			#region Static Methods
			
			public static EditorAchievementDescription[] ConvertToEditorAchievementDescriptionList (EGCAchievementDescription[] _gcAchievementDescriptionList)
			{
				if (_gcAchievementDescriptionList == null)
					return null;
				
				int								_count				= _gcAchievementDescriptionList.Length;
				EditorAchievementDescription[]	_newDescriptionList	= new EditorAchievementDescription[_count];
				
				for (int _iter = 0; _iter < _count; _iter++)
					_newDescriptionList[_iter]						= _gcAchievementDescriptionList[_iter].GetEditorFormatData();
				
				return _newDescriptionList;
			}
			
			#endregion

			#region Methods

			public EGCScore GetScoreWithUserID (string _userID)
			{
				return Scores.FirstOrDefault(_score => _score.User.Identifier.Equals(_userID));
			}

			#endregion
		}
	}
}
#endif