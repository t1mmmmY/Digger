using UnityEngine;
using System.Collections;

#if USES_GAME_SERVICES && UNITY_EDITOR
using System;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins.Internal
{
	[Serializable]
	public sealed class EditorLocalUser : LocalUser
	{
		#region Fields
		
		private 	EditorUser		m_userInfo;

		private		bool			m_isAuthenticated;

		private		IDictionary		m_authResponseData;
		
		#endregion
		
		#region Properties
		
		public override string Identifier
		{
			get
			{
				if (m_userInfo == null)
					return null;
				
				return m_userInfo.Identifier;
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
				if (m_userInfo == null)
					return null;
				
				return m_userInfo.Name;
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
				if (m_userInfo == null)
					return false;
				
				return EditorGameCenter.Instance.IsAuthenticated();
			}
			
			protected set
			{
				throw new Exception("[GameServices] Only getter is supported.");
			}
		}
		
		public override User[] Friends 
		{
			get;
			protected set;
		}
		
		#endregion

		#region Methods

		public EditorLocalUser ()
		{}

		public EditorLocalUser (EGCLocalUser _localUser)
		{
			// Initialize properties
			m_userInfo			= new EditorUser(_localUser.Info);
			IsAuthenticated		= _localUser.IsAuthenticated;
			Friends				= null;
		}

		#endregion

		#region Methods
		
		protected override void RequestForImage ()
		{
			if (m_userInfo == null)
			{
				DownloadImageFinished(null, Constants.kGameServicesUserAuthMissingError);
				return;
			}

			EditorGameCenter.Instance.GetUserImage(m_userInfo);
		}

		public override void Authenticate (AuthenticationCompletion _onCompletion)
		{
			base.Authenticate (_onCompletion);

			EditorGameCenter.Instance.Authenticate();
		}

		public override void LoadFriends (LoadFriendsCompletion _onCompletion)
		{
			base.LoadFriends (_onCompletion);
			
			// Verify user
			if (!IsAuthenticated)
				return;
			
			EditorGameCenter.Instance.LoadFriends();
		}

		public override void SignOut (SignOutCompletion _onCompletion)
		{
			base.SignOut (_onCompletion);

			// Invoke method
			EditorGameCenter.Instance.SignOut();
			SignOutFinished(true, null);
		}
		
		#endregion
		
		#region Event Callback Methods
		
		protected override void RequestForImageFinished (IDictionary _dataDict)
		{
			string		_error		= _dataDict.GetIfAvailable<string>(EditorGameCenter.kErrorKey);
			Texture2D	_image		= _dataDict.GetIfAvailable<Texture2D>(EditorGameCenter.kImageKey);
			
			DownloadImageFinished(_image, _error);
		}

		protected override void AuthenticationFinished (IDictionary _dataDict)
		{
			string		_error		= _dataDict.GetIfAvailable<string>(EditorGameCenter.kErrorKey);

			if (_error == null)
			{
				m_authResponseData	= _dataDict;
			}
			else
			{
				// Update properties
				Friends				= null;
				m_userInfo			= null;
			}
			
			AuthenticationFinished(_error);
		}
		
		protected override void LoadFriendsFinished (IDictionary _dataDict)
		{
			string		_error			= _dataDict.GetIfAvailable<string>(EditorGameCenter.kErrorKey);
			EGCUser[]	_gcFriendsList	= _dataDict.GetIfAvailable<EGCUser[]>(EditorGameCenter.kFriendUsersKey);
			
			if (_gcFriendsList != null)
			{
				int 			_count		= _gcFriendsList.Length;
				EditorUser[]	_friends	= new EditorUser[_count];
				
				for (int _iter = 0; _iter < _count; _iter++)
					_friends[_iter]		= new EditorUser(_gcFriendsList[_iter]);
				
				// Update property
				Friends					= _friends;
			}
			
			LoadFriendsFinished(Friends, _error);
		}

		#endregion

		#region Init Methods

		protected override void OnInitSuccess ()
		{
			EGCLocalUser _localUser	= m_authResponseData.GetIfAvailable<EGCLocalUser>(EditorGameCenter.kLocalUserInfoKey);
			
			// Update properties
			m_userInfo				= new EditorUser(_localUser.Info);
			
			// Reset needless data
			m_authResponseData		= null;
			
			base.OnInitSuccess ();
		}
		
		protected override void OnInitFail ()
		{
			// Update properties
			m_userInfo				= null;
			
			// Reset needless data
			m_authResponseData		= null;
			
			base.OnInitFail ();
		}
		
		#endregion
	}
}
#endif