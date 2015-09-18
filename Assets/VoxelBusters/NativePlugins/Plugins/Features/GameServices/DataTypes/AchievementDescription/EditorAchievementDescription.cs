using UnityEngine;
using System.Collections;
using System;
using DownloadTexture = VoxelBusters.Utility.DownloadTexture;

#if UNITY_EDITOR
namespace VoxelBusters.NativePlugins.Internal
{
	internal sealed class EditorAchievementDescription : AchievementDescription
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
		
		public override string Title
		{
			get;
			protected set;
		}
		
		public override string AchievedDescription
		{
			get;
			protected set;
		}
		
		public override string UnachievedDescription
		{
			get;
			protected set;
		}
		
		public override int MaximumPoints
		{
			get;
			protected set;
		}
		
		public override bool IsHidden
		{
			get;
			protected set;
		}

		#endregion

		#region Constructors

		private EditorAchievementDescription ()
		{}

		internal EditorAchievementDescription (EditorGameCenter.EGCAchievementDescription _achievementDescription)
		{
			// Initialize properties
			Identifier				= _achievementDescription.Identifier;
			Title					= _achievementDescription.Title;
			AchievedDescription		= _achievementDescription.AchievedDescription;
			UnachievedDescription	= _achievementDescription.UnachievedDescription;
			MaximumPoints			= _achievementDescription.MaximumPoints;
			IsHidden				= _achievementDescription.IsHidden;
			m_image					= _achievementDescription.Image;
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
		
		#endregion
	}
}
#endif