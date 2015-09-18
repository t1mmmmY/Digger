using UnityEngine;
using System.Collections;
using System;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins
{
	/// <summary>
	/// The <see cref="VoxelBusters.NativePlugins.LocalUser"/> class is a special subclass of <see cref="VoxelBusters.NativePlugins.User"/> that represents the authenticated user running your game on the device. 
	/// At any given time, only one user may be authenticated on the device.
	/// </summary>
	public abstract class LocalUser	: User
	{
		#region Fields
		
		protected 	Action<bool> 	m_authCallback;
		
		#endregion

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
		
		#region Constructor
		
		protected LocalUser ()
		{}
		
		#endregion

		#region Methods

		/// <summary>
		/// Method call to process an authentication-related event.
		/// </summary>
		/// <param name="_onCompletion">Callback to be called when request completes.</param>
		public virtual void Authenticate (Action<bool> _onCompletion)
		{
			m_authCallback	= _onCompletion;
		}

		protected void OnAuthenticationFinish (bool _success)
		{
			if (_success)
			{
				Initialize(m_authCallback);
			}
			else
			{
				// Send callback
				if (m_authCallback != null)
					m_authCallback(false);
			}
		}

		private void Initialize (Action<bool> _completionCallback)
		{
			NPBinding.GameServices.InvokeMethod("LoadAchievementDescriptions", new object[] { false, (Action<AchievementDescription[]>)((AchievementDescription[] _descriptionList)=>{

				// Trigger callback
				if (_completionCallback != null)
					_completionCallback(_descriptionList != null);
				}) 
			}, new Type[] { typeof(bool), typeof(Action<AchievementDescription[]>) });
		}

		#endregion

		#region Abstract Methods

		/// <summary>
		/// Retrieves friends info of the <see cref="VoxelBusters.NativePlugins.LocalUser"/>.
		/// </summary>
		/// <param name="_onCompletion">Callback to be called when request completes.</param>
		public abstract void LoadFriends (Action<User[]> _onCompletion);

		#endregion

		#region Override Methods

		public override string ToString ()
		{
			return string.Format("[LocalUser: Name={0}, IsAuthenticated={1}]", Name, IsAuthenticated);
		}

		#endregion
	}
}