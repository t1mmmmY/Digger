using UnityEngine;
using System.Collections;

#if USES_GAME_SERVICES && UNITY_EDITOR
using System;
using VoxelBusters.Utility;
using DownloadTexture = VoxelBusters.Utility.DownloadTexture;

namespace VoxelBusters.NativePlugins.Internal
{
	[Serializable]
	public sealed class EGCUser
	{
		#region Fields
		
		[SerializeField]
		private				string				m_identifier;
		
		[SerializeField]
		private				string				m_name;
		
		[SerializeField]
		private				Texture2D			m_image;
		
		[SerializeField]
		private				string[]			m_friends	= new string[0];
		
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
		
		public string Name
		{
			get
			{
				return m_name;
			}
			
			private set
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
		
		public EGCUser (string _id, string _name, Texture2D _image, string[] _friends) 
		{
			// Initialize properties
			Identifier	= _id;
			Name		= _name;
			Image		= _image;
			Friends		= _friends;
		}

		#endregion
	}
}
#endif