using UnityEngine;
using System.Collections;
using VoxelBusters.Utility;
using DownloadTexture = VoxelBusters.Utility.DownloadTexture;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	/// <summary>
	/// <see cref="VoxelBusters.NativePlugins.User"/> class provides information about a user playing your game.
	/// </summary>
	public abstract class User : NPObject
	{
		#region Fields
		
		private		Texture2D		m_image;
		
		#endregion

		#region Properties

		/// <summary>
		/// Gets the identifier for this <see cref="VoxelBusters.NativePlugins.User"/>.
		/// </summary>
		/// <value>A string assigned to uniquely identify a <see cref="VoxelBusters.NativePlugins.User"/>.</value>
		public abstract string Identifier
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the name for this <see cref="VoxelBusters.NativePlugins.User"/>.
		/// </summary>
		/// <value>Display name chosen by <see cref="VoxelBusters.NativePlugins.User"/> to identify themselves to other players.</value>
		public abstract string Name
		{
			get;
			protected set;
		}
		
		#endregion

		#region Delegates

		/// <summary>
		/// The callback delegate used when load <see cref="VoxelBusters.NativePlugins.Users"/> request completes.
		/// </summary>
		/// <param name="_users">An array of <see cref="VoxelBusters.NativePlugins.User"/> objects that holds information of requested user id's.</param>
		/// <param name="_error">If the operation was successful, this value is nil; otherwise, this parameter holds the description of the problem that occurred.</param>
		public delegate void LoadUsersCompletion (User[] _users, string _error);

		#endregion
		
		#region Events
	
		protected event DownloadTexture.Completion DownloadImageFinishedEvent;
		
		#endregion
		
		#region Constructors
		
		protected User () : base (NPObjectManager.eCollectionType.GAME_SERVICES)
		{}

		protected User (string _id, string _name) : base (NPObjectManager.eCollectionType.GAME_SERVICES)
		{
			// Initialize properties
			Identifier		= _id;
			Name			= _name;
		}
		
		#endregion

		#region Methods

		/// <summary>
		/// Asynchronously loads the image.
		/// </summary>
		/// <param name="_onCompletion">Callback to be triggered after loading the image.</param>
		public virtual void GetImageAsync (DownloadTexture.Completion _onCompletion)
		{
			// Cache callback
			DownloadImageFinishedEvent	= _onCompletion;
			
			// Using cached information
			if (m_image != null)
			{
				DownloadImageFinishedEvent(m_image, null);
				return;
			}
			
			// Request for image
			RequestForImage();
		}
		
		protected virtual void RequestForImage ()
		{}

		public override string ToString ()
		{
			return string.Format("[User: Name={0}]", Name);
		}

		#endregion

		#region Event Callback Methods

		protected virtual void RequestForImageFinished (IDictionary _dataDict)
		{}

		protected void RequestForImageFinished (URL _imagePathURL, string _error)
		{
			if (_error != null)
			{
				DownloadImageFinished(null, _error);
				return;
			}
			else
			{
				DownloadTexture _newRequest		= new DownloadTexture(_imagePathURL, true, true);
				_newRequest.OnCompletion		= (Texture2D _image, string _downloadError)=>{
					
					// Invoke handler
					DownloadImageFinished(_image, _downloadError);
				};
				_newRequest.StartRequest();
			}
		}
		
		protected void DownloadImageFinished (Texture2D _image, string _error)
		{
			// Set properties
			m_image	= _image;
			
			// Send callback
			if (DownloadImageFinishedEvent != null)
				DownloadImageFinishedEvent(_image, _error);
		}
		
		#endregion
	}
}