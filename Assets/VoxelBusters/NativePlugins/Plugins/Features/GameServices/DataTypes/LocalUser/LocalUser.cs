using UnityEngine;
using System.Collections;

#if USES_GAME_SERVICES 
using System;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	/// <summary>
	/// The <see cref="VoxelBusters.NativePlugins.LocalUser"/> class is a special subclass of <see cref="VoxelBusters.NativePlugins.User"/> that represents the authenticated user running your game on the device. 
	/// At any given time, only one user may be authenticated on the device.
	/// </summary>
	public abstract class LocalUser	: User
	{
		#region Properties

		/// <summary>
		/// Gets a value indicating whether this <see cref="VoxelBusters.NativePlugins.LocalUser"/> is currently signed in or not.
		/// </summary>
		/// <value>States whether <see cref="VoxelBusters.NativePlugins.LocalUser"/> is currently signed in or not.</value>
		public abstract bool IsAuthenticated
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the friends info of <see cref="VoxelBusters.NativePlugins.LocalUser"/>.
		/// </summary>
		/// <value>The friends info of <see cref="VoxelBusters.NativePlugins.LocalUser"/>.</value>
		public abstract	User[] Friends 
		{
			get;
			protected set;
		}

		#endregion

		#region Delegates

		/// <summary>
		/// The callback delegate used when <see cref="VoxelBusters.NativePlugins.LocalUser"/> is authenticated.
		/// </summary>
		/// <param name="_success">The operation completion status.</param>
		/// <param name="_error">If the operation was successful, this value is nil; otherwise, this parameter holds the description of the problem that occurred.</param>
		public delegate void AuthenticationCompletion (bool _success, string _error);

		/// <summary>
		/// The callback delegate used when <see cref="VoxelBusters.NativePlugins.LocalUser"/> friends information is retrieved.
		/// </summary>
		/// <param name="_users">An array of <see cref="VoxelBusters.NativePlugins.User"/> objects that are friends of the local user.</param>
		/// <param name="_error">If the operation was successful, this value is nil; otherwise, this parameter holds the description of the problem that occurred.</param>
		public delegate void LoadFriendsCompletion (User[] _users, string _error);

		/// <summary>
		/// The callback delegate used when <see cref="VoxelBusters.NativePlugins.LocalUser"/> is signed out of the Game Service.
		/// </summary>
		/// <param name="_success">The operation completion status.</param>
		/// <param name="_error">If the operation was successful, this value is nil; otherwise, this parameter holds the description of the problem that occurred.</param>
		public delegate void SignOutCompletion (bool _success, string _error);

		#endregion

		#region Events
		
		protected AuthenticationCompletion AuthenticationFinishedEvent;
		protected LoadFriendsCompletion	LoadFriendsFinishedEvent;
		protected SignOutCompletion	SignOutFinishedEvent;

		#endregion
		
		#region Constructor
		
		protected LocalUser () : base ()
		{}
		
		#endregion

		#region Methods

		/// <summary>
		/// Authenticates the local player on the device.
		/// </summary>
		/// <param name="_onCompletion">Callback to be called when request completes.</param>
		public virtual void Authenticate (AuthenticationCompletion _onCompletion)
		{
			// Cache callback
			AuthenticationFinishedEvent	= _onCompletion;
		}

		/// <summary>
		/// Retrieves friends info of the <see cref="VoxelBusters.NativePlugins.LocalUser"/>.
		/// </summary>
		/// <param name="_onCompletion">Callback to be called when request completes.</param>
		public virtual void LoadFriends (LoadFriendsCompletion _onCompletion)
		{
			// Cache callback
			LoadFriendsFinishedEvent	= _onCompletion;

			if (!IsAuthenticated)
			{
				LoadFriendsFinished(null, Constants.kGameServicesUserAuthMissingError);
				return;
			}
		}

		/// <summary>
		/// Signs the local player out of the Game Service.
		/// </summary>
		public virtual void SignOut (SignOutCompletion _onCompletion)
		{
			// Cache callback
			SignOutFinishedEvent	= _onCompletion;
		}

		public override string ToString ()
		{
			return string.Format("[LocalUser: Name={0}, IsAuthenticated={1}]", Name, IsAuthenticated);
		}

		#endregion

		#region Event Callback Methods

		protected virtual void AuthenticationFinished (IDictionary _dataDict)
		{}

		protected void AuthenticationFinished (string _error)
		{
			if (_error == null)
			{
				Init();
				return;
			}
			else
			{
				if (AuthenticationFinishedEvent != null)
					AuthenticationFinishedEvent(false, _error);
			}
		}

		protected virtual void LoadFriendsFinished (IDictionary _dataDict)
		{}

		protected void LoadFriendsFinished (User[] _users, string _error)
		{
			if (LoadFriendsFinishedEvent != null)
				LoadFriendsFinishedEvent(_users, _error);
		}

		protected virtual void SignOutFinished (IDictionary _dataDict)
		{}

		protected void SignOutFinished (bool _success, string _error)
		{
			if (SignOutFinishedEvent != null)
				SignOutFinishedEvent(_success, _error);
		}

		#endregion

		#region Init Methods
		
		private void Init ()
		{
			NPBinding.GameServices.InvokeMethod("LoadAchievementDescriptions", new object[] { 
				false, 
				(AchievementDescription.LoadAchievementDescriptionsCompletion)((AchievementDescription[] _descriptionList, string _error)=>{
					
					if (_error == null)
						OnInitSuccess();
					else
						OnInitFail();
				}) 
			}, new Type[] { 
				typeof(bool), 
				typeof(AchievementDescription.LoadAchievementDescriptionsCompletion) 
			});
		}

		protected virtual void OnInitSuccess ()
		{
			if (AuthenticationFinishedEvent != null)
				AuthenticationFinishedEvent(true, null);
		}

		protected virtual void OnInitFail ()
		{
			if (AuthenticationFinishedEvent != null)
				AuthenticationFinishedEvent(false, "The requested operation could not be completed because Game-Services failed to intialise.");
		}

		#endregion
	}
}
#endif