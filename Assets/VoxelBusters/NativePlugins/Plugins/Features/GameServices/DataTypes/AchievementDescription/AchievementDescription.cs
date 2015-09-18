using UnityEngine;
using System.Collections;
using System;
using DownloadTexture = VoxelBusters.Utility.DownloadTexture;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	/// <summary>
	/// An <see cref="VoxelBusters.NativePlugins.AchievementDescription"/> object holds text and images used to describe an <see cref="VoxelBusters.NativePlugins.Achievement"/>.
	/// </summary>
	public abstract class AchievementDescription
	{
		#region Properties

		/// <summary>
		/// Gets the identifier of <see cref="VoxelBusters.NativePlugins.Achievement"/>.
		/// </summary>
		/// <value>A string used to uniquely identify the specific <see cref="VoxelBusters.NativePlugins.Achievement"/> object refers to.</value>
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

		#endregion

		#region Constructor

		protected AchievementDescription ()
		{}

		protected AchievementDescription (string _identifier, string _title, string _achievedDescription, string _unachievedDescription, int _maxPoints, bool _isHidden)
		{
			// Initialize properties
			Identifier 				= _identifier;
			Title 					= _title;
			AchievedDescription		= _achievedDescription;
			UnachievedDescription	= _unachievedDescription;
			MaximumPoints			= _maxPoints;
			IsHidden 				= _isHidden;
		}

		#endregion
		
		#region Abstract Methods

		/// <summary>
		/// Asynchronously loads the image.
		/// </summary>
		/// <param name="_onCompletion">Callback to be triggered after loading the image.</param>
		public abstract void GetImageAsync (DownloadTexture.Completion _onCompletion);
		
		#endregion
		
		#region Methods
		
		public override string ToString ()
		{
			return string.Format("[AchievementDescription: Identifier={0}, Title={1}, MaximumPoints={2}, IsHidden={3}]", Identifier, Title, MaximumPoints, IsHidden);
		}
		
		#endregion
	}
}