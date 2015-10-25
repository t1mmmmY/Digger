using UnityEngine;
using System.Collections;
using System;
using VoxelBusters.Utility;
using DownloadTexture = VoxelBusters.Utility.DownloadTexture;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	/// <summary>
	/// An <see cref="VoxelBusters.NativePlugins.AchievementDescription"/> object holds text and images used to describe an <see cref="VoxelBusters.NativePlugins.Achievement"/>.
	/// </summary>
	public abstract class AchievementDescription : NPObject
	{
		#region Fields

		private		Texture2D		m_image;

		#endregion

		#region Properties
		
		/// <summary>
		/// Gets the unique identifier of <see cref="VoxelBusters.NativePlugins.AchievementDescription"/> which is common for all supported platforms.
		/// </summary>
		/// <value>A string used to uniquely identify <see cref="VoxelBusters.NativePlugins.AchievementDescription"/> across all supported platforms.</value>
		public string GlobalIdentifier
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the identifier of <see cref="VoxelBusters.NativePlugins.AchievementDescription"/> specific to current platform.
		/// </summary>
		/// <value>A string used to uniquely identify <see cref="VoxelBusters.NativePlugins.AchievementDescription"/> specific to current platform.</value>
		public abstract string Identifier
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the title for the <see cref="VoxelBusters.NativePlugins.Achievement"/>.
		/// </summary>
		/// <value>The title for the <see cref="VoxelBusters.NativePlugins.Achievement"/>.</value>
		public abstract string Title
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the description to be used when the <see cref="VoxelBusters.NativePlugins.LocalUser"/> has completed the achievement.
		/// </summary>
		/// <value>A description to be used after the <see cref="VoxelBusters.NativePlugins.LocalUser"/> has completed the achievement.</value>
		public abstract string AchievedDescription
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the description to be used when the <see cref="VoxelBusters.NativePlugins.LocalUser"/> has not completed the achievement.
		/// </summary>
		/// <value>A description to be used when the <see cref="VoxelBusters.NativePlugins.LocalUser"/> has not completed the achievement.</value>
		public abstract string UnachievedDescription
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the maximum point that user needs to earn to complete this achievement.
		/// </summary>
		/// <value>The maximum point that user needs to earn to complete this achievement.</value>
		public abstract	int MaximumPoints
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets a value indicating this achievement is initially visible to users.
		/// </summary>
		/// <value><c>true</c> if this achievement is initially hidden; otherwise, <c>false</c>.</value>
		public abstract bool IsHidden
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets the instance ID of the object.
		/// </summary>
		/// <value>Returns the instance id of the object.</value>
		public string InstanceID
		{
			get;
			private set;
		}

		#endregion

		#region Delegates

		/// <summary>
		/// The callback delegate used when load <see cref="VoxelBusters.NativePlugins.AchievementDescription"/> request completes.
		/// </summary>
		/// <param name="_descriptions">An array of <see cref="VoxelBusters.NativePlugins.AchievementDescription"/> containing the <see cref="VoxelBusters.NativePlugins.Achievement"/> in your game..</param>
		/// <param name="_error">If the operation was successful, this value is nil; otherwise, this parameter holds the description of the problem that occurred.</param>
		public delegate void LoadAchievementDescriptionsCompletion (AchievementDescription[] _descriptions, string _error);
	
		#endregion
		
		#region Events
	
		private event DownloadTexture.Completion DownloadImageFinishedEvent;
		
		#endregion

		#region Constructor

		protected AchievementDescription () : base (NPObjectManager.eCollectionType.GAME_SERVICES)
		{}

		#endregion
	
		#region Methods

		/// <summary>
		/// Asynchronously loads the image.
		/// </summary>
		/// <param name="_onCompletion">Callback to be triggered after loading the image.</param>
		public void GetImageAsync (DownloadTexture.Completion _onCompletion)
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
			return string.Format("[AchievementDescription: Identifier={0}, Title={1}, MaximumPoints={2}, IsHidden={3}]", Identifier, Title, MaximumPoints, IsHidden);
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

					// Invoke result handler
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