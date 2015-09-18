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
		internal sealed class EGCUser : User
		{
			#region Fields

			[SerializeField]
			private				string				m_identifier;

			[SerializeField]
			private				string				m_name;

			[SerializeField]
			private				Texture2D			m_image;

			[SerializeField]
			private				string[]			m_friends;

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
			
			public override string Name
			{
				get
				{
					return m_name;
				}
				
				protected set
				{
					m_name			= value;
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
					m_image			= value;
				}
			}

			public string[] Friends
			{
				get
				{
					return m_friends;
				}
			
				private set
				{
					m_friends		= value;
				}
			}
			
			#endregion

			#region Constructors

			public EGCUser (string _id) : this (_id, _id, null, new string[0])
			{}

			public EGCUser (string _id, string _name, Texture2D _image, string[] _friends) : base (_id, _name)
			{
				// Initialize properties
				Image		= _image;
				Friends		= _friends;
			}

			public EGCUser (EGCUser _user)
			{
				// Initialize properties
				Identifier	= _user.Identifier;
				Name		= _user.Name;
				Image		= _user.Image;
				Friends		= _user.Friends;
			}

			#endregion
			
			#region Static Methods
			
			public static EditorUser[] ConvertToEditorUserList (EGCUser[] _gcUserList)
			{
				if (_gcUserList == null)
					return null;
				
				int 			_count			= _gcUserList.Length;
				EditorUser[] 	_newUserList	= new EditorUser[_count];
				
				for (int _iter = 0; _iter < _count; _iter++)
					_newUserList[_iter]			= _gcUserList[_iter].GetEditorFormatData();
				
				return _newUserList;
			}
			
			#endregion

			#region Methods
			
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

			public EditorUser GetEditorFormatData ()
			{
				return new EditorUser(this);
			}
			
			#endregion

		}
	}
}
#endif