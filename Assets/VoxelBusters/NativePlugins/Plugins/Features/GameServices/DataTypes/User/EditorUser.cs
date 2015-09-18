using UnityEngine;
using System.Collections;
using System;
using DownloadTexture = VoxelBusters.Utility.DownloadTexture;

#if UNITY_EDITOR
namespace VoxelBusters.NativePlugins.Internal
{
	internal class EditorUser : User
	{
		#region Fields

		private		Texture2D		m_image;

		#endregion

		#region Properties

		public override string Identifier
		{
			get;
			protected set;
		}
		
		public override string Name
		{
			get;
			protected set;
		}
		
		#endregion
		
		#region Constructors

		internal EditorUser ()
		{}

		public EditorUser (string _id, string _name, Texture2D _image) : base (_id, _name)
		{
			// Initialize properties
			m_image			= _image;
		}

		public EditorUser (EditorGameCenter.EGCUser _user)
		{
			// Initialize properties
			Identifier		= _user.Identifier;
			Name			= _user.Name;
			m_image			= _user.Image;
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
		
		#endregion
	}
}
#endif