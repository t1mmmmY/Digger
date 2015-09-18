using UnityEngine;
using System.Collections;
using System;
using DownloadTexture = VoxelBusters.Utility.DownloadTexture;

#if UNITY_EDITOR
namespace VoxelBusters.NativePlugins.Internal
{
	[Serializable]
	internal sealed class EditorLocalUser : LocalUser
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

		public override bool IsAuthenticated
		{
			get;
			protected set;
		}
		
		public override	User[] Friends 
		{
			get;
			protected set;
		}
		
		#endregion

		#region Methods

		public EditorLocalUser ()
		{}

		public EditorLocalUser (EditorGameCenter.EGCLocalUser _gcLocalUser)
		{
			// Initialize properties
			Identifier			= _gcLocalUser.Info.Identifier;
			Name				= _gcLocalUser.Info.Name;
			m_image				= _gcLocalUser.Info.Image;
			IsAuthenticated		= _gcLocalUser.IsAuthenticated;
			Friends				= null;
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

		public override void Authenticate (Action<bool> _onCompletion)
		{
			// Request authentication
			EditorGameCenter.Instance.Authenticate((EditorLocalUser _localUserInfo)=>{

				// Check auth status
				bool 	_authSuccess	= _localUserInfo != null;

				// Custom authentication callback
				m_authCallback			= (bool _success)=>{

					// Update properties
					if (_success)
					{
						Identifier		= _localUserInfo.Identifier;
						Name			= _localUserInfo.Name;
						m_image 		= _localUserInfo.m_image;
						IsAuthenticated	= true;
					}
					else
					{
						// Set properties
						Identifier		= null;
						Name			= null;
						m_image			= null;
						IsAuthenticated	= false;
					}

					if (_onCompletion != null)
						_onCompletion(_success);
				};

				OnAuthenticationFinish(_authSuccess);
			});
		}

		public override void LoadFriends (Action<User[]> _onCompletion)
		{
			EditorGameCenter.Instance.LoadFriends((EditorUser[] _friendsList)=>{

				// Set properties
				Friends				= _friendsList;

				// Invoke callback
				if (_onCompletion != null)
					_onCompletion(_friendsList);
			});
		}

		#endregion
	}
}
#endif