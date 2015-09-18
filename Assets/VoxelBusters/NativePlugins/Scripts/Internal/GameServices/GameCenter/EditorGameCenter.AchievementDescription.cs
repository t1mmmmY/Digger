using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using System;
using VoxelBusters.Utility;
using DownloadTexture = VoxelBusters.Utility.DownloadTexture;

namespace VoxelBusters.NativePlugins.Internal
{
	internal partial class EditorGameCenter : AdvancedScriptableObject <EditorGameCenter>
	{
		[Serializable]
		internal sealed class EGCAchievementDescription : AchievementDescription
		{
			#region Fields

			[SerializeField]
			private				string				m_identifier;

			[SerializeField]
			private				string				m_title;
			
			[SerializeField]
			private				Texture2D			m_image;
			
			[SerializeField]
			private				int					m_maximumPoints;
			
			[SerializeField]
			private				bool				m_isHidden;

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
			
			public override string Title
			{
				get
				{
					return m_title;
				}
				
				protected set
				{
					m_title	= value;
				}
			}
			
			public override string AchievedDescription
			{
				get
				{
					return "Achieved desciption.";
				}
				
				protected set
				{
					throw new Exception("[GameServices] Only getter is supported.");
				}
			}
			
			public override string UnachievedDescription
			{
				get
				{
					return "Unachieved desciption.";
				}
				
				protected set
				{
					throw new Exception("[GameServices] Only getter is supported.");
				}
			}
			
			public override int MaximumPoints
			{
				get
				{
					return m_maximumPoints;
				}
				
				protected set
				{
					m_maximumPoints	= value;
				}
			}
			
			public Texture2D Image
			{
				get
				{
					return m_image;
				}

				private set
				{
					m_image	= value;
				}
			}
			
			public override bool IsHidden
			{
				get
				{
					return m_isHidden;
				}
				
				protected set
				{
					m_isHidden	= value;
				}
			}
			
			#endregion

			#region Constructors

			public EGCAchievementDescription (string _identifier, string _title, int _maxPoints, Texture2D _image, bool _isHidden)
			{
				// Initialize properties
				Identifier		= _identifier;
				Title			= _title;
				MaximumPoints	= _maxPoints;
				Image			= _image;
				IsHidden		= _isHidden;
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
			
			public void SetIsHidden (bool _hidden)
			{
				IsHidden	= _hidden;
			}
			
			public EditorAchievementDescription GetEditorFormatData ()
			{
				return new EditorAchievementDescription(this);
			}

			public override void GetImageAsync (DownloadTexture.Completion _onCompletion)
			{
				if (_onCompletion != null)
				{
					if (m_image == null)
						_onCompletion(null, "Texture not found.");
					else
						_onCompletion(m_image, null);
				}
			}
			
			#endregion
		}
	}
}
#endif