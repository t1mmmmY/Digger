using UnityEngine;
using System.Collections;

#if USES_GAME_SERVICES && UNITY_IOS
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using VoxelBusters.Utility;
using DownloadTexture = VoxelBusters.Utility.DownloadTexture;

namespace VoxelBusters.NativePlugins.Internal
{
	public sealed class iOSLocalUser : LocalUser 
	{
		#region Constants

		private 	const 	string 		kFriendPlayersKey	= "friend-players";
		private 	const 	string 		kLocalPlayerInfoKey	= "player-info";

		#endregion
	
		#region Fields

		private 	iOSUser			m_userInfo;
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

				return isAuthenticated();
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

		#region Constructors

		public iOSLocalUser () : base ()
		{}

		#endregion
		
		#region External Methods
		
		[DllImport("__Internal")]
		private static extern bool isAuthenticated ();

		[DllImport("__Internal")]
		private static extern void authenticatePlayer ();

		[DllImport("__Internal")]
		private static extern void loadFriendPlayers ();
		
		#endregion

		#region Methods
		
		public override void GetImageAsync (DownloadTexture.Completion _onCompletion)
		{
			if (m_userInfo == null)
			{
				if (_onCompletion != null)
					_onCompletion (null, Constants.kGameServicesUserAuthMissingError);
				
				return;
			}
			
			m_userInfo.GetImageAsync (_onCompletion);
		}

		public override void Authenticate (AuthenticationCompletion _onCompletion)
		{
			base.Authenticate (_onCompletion);

			// Native call
			authenticatePlayer ();
		}

		public override void LoadFriends (LoadFriendsCompletion _onCompletion)
		{
			base.LoadFriends (_onCompletion);

			// Verify user
			if (!IsAuthenticated)
				return;

			// Native call
			loadFriendPlayers ();
		}

		public override void SignOut (SignOutCompletion _onCompletion)
		{
			base.SignOut (_onCompletion);

			// Feature not supported
			SignOutFinished (false, "The operation could not be completed because this feature is not supported in iOS.");
		}

		#endregion

		#region Event Callback Methods

		protected override void AuthenticationFinished (IDictionary _dataDict)
		{
			string		_error		= _dataDict.GetIfAvailable<string>(GameServicesIOS.kNativeMessageErrorKey);

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
			string		_error			= _dataDict.GetIfAvailable<string>(GameServicesIOS.kNativeMessageErrorKey);
			IList		_friendJSONList	= _dataDict.GetIfAvailable<IList>(kFriendPlayersKey);

			if (_friendJSONList != null)
			{
				int 		_count		= _friendJSONList.Count;
				iOSUser[]	_friends	= new iOSUser[_count];

				for (int _iter = 0; _iter < _count; _iter++)
					_friends[_iter]		= new iOSUser((IDictionary)_friendJSONList[_iter]);

				// Update property
				Friends					= _friends;
			}

			LoadFriendsFinished(Friends, _error);
		}

		#endregion

		#region Init Methods

		protected override void OnInitSuccess ()
		{
			IDictionary _infoDict	= m_authResponseData.GetIfAvailable<IDictionary>(kLocalPlayerInfoKey);

			// Update properties
			m_userInfo				= new iOSUser(_infoDict);

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