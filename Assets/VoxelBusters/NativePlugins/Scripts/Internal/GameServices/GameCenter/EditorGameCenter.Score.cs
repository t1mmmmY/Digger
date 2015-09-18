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
		internal sealed class EGCScore : Score
		{
			#region Fields

			[SerializeField]
			private				string					m_leaderboardID;

			[SerializeField]
			private				long					m_value;

			[SerializeField]
			private				string					m_userID;

			[NonSerialized]
			private				User					m_user;

			[SerializeField]
			private				string					m_date;

			[SerializeField]
			private				int						m_rank;

			#endregion

			#region Properties

			public override string LeaderboardID
			{
				get
				{
					return m_leaderboardID;
				}
				
				protected set
				{
					m_leaderboardID		= value;
				}
			}
			
			public override User User
			{
				get
				{
					if (m_user == null) 
						m_user			= EditorGameCenter.Instance.GetUserWithID(m_userID);

					return m_user;
				}

				protected set
				{
					throw new Exception("[GameServices] Only getter is supported.");
				}
			}
			
			public override long Value
			{
				get
				{
					return m_value;
				}
				
				set
				{
					m_value		= value;
				}
			}
			
			public override DateTime Date
			{
				get
				{
					if (string.IsNullOrEmpty(m_date))
						return new DateTime();
					
					return DateTime.Parse(m_date);
				}
				
				protected set
				{
					m_date		= value.ToString();
				}
			}
			
			public override int Rank
			{
				get
				{
					return m_rank;
				}

				protected set
				{
					m_rank		= value;	
				}
			}
			
			#endregion

			#region Constructors

			public EGCScore (string _leaderboardID, string _userID, long _scoreValue = 0L) 
			{
				// Initialize properties
				LeaderboardID		= _leaderboardID;
				m_userID			= _userID;
				Value				= _scoreValue;
				Date				= DateTime.Now;
			}

			#endregion
			
			#region Static Methods
			
			public static EditorScore[] ConvertToEditorScoreList (EGCScore[] _gcScoreList)
			{
				if (_gcScoreList == null)
					return null;
				
				int				_count			= _gcScoreList.Length;
				EditorScore[]	_newScoreList	= new EditorScore[_count];
				
				for (int _iter = 0; _iter < _count; _iter++)
					_newScoreList[_iter]		= _gcScoreList[_iter].GetEditorFormatData();
				
				return _newScoreList;
			}
			
			#endregion
			
			#region Methods

			public void SetRank (int _rank)
			{
				Rank	= _rank;
			}

			public override void ReportScore (Action<bool> _onCompletion)
			{
				throw new Exception("[GameServices] Invalid operation.");
			}

			public EditorScore GetEditorFormatData ()
			{
				return new EditorScore(this);
			}

			#endregion
		}
	}
}
#endif