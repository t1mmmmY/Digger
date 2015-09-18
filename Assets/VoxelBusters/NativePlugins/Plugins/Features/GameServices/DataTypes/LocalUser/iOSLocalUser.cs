using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SocialPlatforms;
using DownloadTexture = VoxelBusters.Utility.DownloadTexture;

#if UNITY_IOS
namespace VoxelBusters.NativePlugins.Internal
{
	internal sealed class iOSLocalUser : LocalUser 
	{
		#region Fields

		private		ILocalUser		m_localUserData;

		#endregion

		#region Properties

		public override string Identifier
		{
			get
			{
				return m_localUserData.id;
			}
			
			protected set
			{
				throw new Exception("[GameServices] Only getter is supported.");
			}
		}
		
		public override string Name
		{
			get
			{
				return m_localUserData.userName;
			}
			
			protected set
			{
				throw new Exception("[GameServices] Only getter is supported.");
			}
		}

		public override bool IsAuthenticated
		{
			get
			{
				return m_localUserData.authenticated;
			}

			protected set
			{
				throw new Exception("[GameServices] Only getter is supported.");
			}
		}
		
		public override User[] Friends 
		{
			get
			{
				return iOSUser.ConvertToUserList(m_localUserData.friends);
			}
			
			protected set
			{
				throw new Exception("[GameServices] Only getter is supported.");
			}
		}

		#endregion

		#region Constructors

		public iOSLocalUser ()
		{
			// Initialize properties
			m_localUserData		= Social.localUser;
		}

		#endregion

		#region Methods
		
		public override void GetImageAsync (DownloadTexture.Completion _onCompletion)
		{
			if (_onCompletion != null)
			{
				if (m_localUserData.image == null)
					_onCompletion(null, "Texture not found.");
				else
					_onCompletion(m_localUserData.image, null);
			}
		}

		public override void Authenticate (Action<bool> _onCompletion)
		{
			base.Authenticate(_onCompletion);
			
			// Request authentication
			m_localUserData.Authenticate(OnAuthenticationFinish);
		}

		public override void LoadFriends (Action<User[]> _onCompletion)
		{
			m_localUserData.LoadFriends((bool _success)=>{

				if (_onCompletion != null)
				{
					if (_success)
					{
						_onCompletion(Friends);
					}
					else
					{
						_onCompletion(null);
					}
				}

			});
		}

		#endregion
	}
}
#endif