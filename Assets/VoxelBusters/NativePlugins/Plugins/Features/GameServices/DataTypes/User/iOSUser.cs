using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SocialPlatforms;
using DownloadTexture = VoxelBusters.Utility.DownloadTexture;

#if UNITY_IOS
namespace VoxelBusters.NativePlugins.Internal
{
	internal sealed class iOSUser : User 
	{
		#region Properties

		private		IUserProfile		m_userProfileData;

		#endregion

		#region Properties

		public override string Identifier
		{
			get
			{
				return m_userProfileData.id;
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
				return m_userProfileData.userName;
			}
			
			protected set
			{
				throw new Exception("[GameServices] Only getter is supported.");
			}
		}

		#endregion

		#region Constructors

		internal iOSUser (IUserProfile _userProfileData)
		{
			m_userProfileData	= _userProfileData;
		}

		#endregion

		#region Static Methods

		public static iOSUser ConvertToUser (IUserProfile _user)
		{
			if (_user == null)
				return null;

			return new iOSUser(_user);
		}

		public static iOSUser[] ConvertToUserList (IUserProfile[] _userList)
		{
			if (_userList == null)
				return null;

			int				_count				= _userList.Length;
			iOSUser[]		_iosUsersList		= new iOSUser[_count];
			
			for (int _iter = 0; _iter < _count; _iter++)
				_iosUsersList[_iter]			= new iOSUser(_userList[_iter]);

			return _iosUsersList;
		}

		#endregion

		#region Methods

		public override void GetImageAsync (DownloadTexture.Completion _onCompletion)
		{
			if (_onCompletion != null)
			{
				if (m_userProfileData.image == null)
					_onCompletion(null, "Texture not found.");
				else
					_onCompletion(m_userProfileData.image, null);
			}
		}

		#endregion
	}
}
#endif