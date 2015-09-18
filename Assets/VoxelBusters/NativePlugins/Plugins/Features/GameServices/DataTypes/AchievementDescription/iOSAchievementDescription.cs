using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SocialPlatforms;
using DownloadTexture = VoxelBusters.Utility.DownloadTexture;

#if UNITY_IOS
namespace VoxelBusters.NativePlugins.Internal
{
	internal sealed class iOSAchievementDescription : AchievementDescription 
	{
		#region Properties
		
		private		IAchievementDescription		m_achievementDescriptionData;

		#endregion

		#region Properties

		public override string Identifier
		{
			get
			{
				return m_achievementDescriptionData.id;
			}

			protected set
			{
				throw new Exception("[GameServices] Only getter is supported.");
			}
		}
		
		public override	string Title
		{
			get
			{
				return m_achievementDescriptionData.title;
			}
			
			protected set
			{
				throw new Exception("[GameServices] Only getter is supported.");
			}
		}
		
		public override string AchievedDescription
		{
			get
			{
				return m_achievementDescriptionData.achievedDescription;
			}
			
			protected set
			{
				throw new Exception("[GameServices] Only getter is supported.");
			}
		}
		
		public override	string UnachievedDescription
		{
			get
			{
				return m_achievementDescriptionData.unachievedDescription;
			}
			
			protected set
			{
				throw new Exception("[GameServices] Only getter is supported.");
			}
		}
		
		public override	int MaximumPoints
		{
			get
			{
				return m_achievementDescriptionData.points;
			}
			
			protected set
			{
				throw new Exception("[GameServices] Only getter is supported.");
			}
		}
		
		public override bool IsHidden
		{
			get
			{
				return m_achievementDescriptionData.hidden;
			}
			
			protected set
			{
				throw new Exception("[GameServices] Only getter is supported.");
			}
		}
		
		#endregion
		
		#region Constructors

		private iOSAchievementDescription ()
		{}
		
		internal iOSAchievementDescription (IAchievementDescription _descriptionData)
		{
			m_achievementDescriptionData	= _descriptionData;
		}
		
		#endregion

		#region Static Methods

		internal static iOSAchievementDescription[] ConvertAchievementDescriptionList (IAchievementDescription[] _achievementDescriptionList)
		{
			if (_achievementDescriptionList == null)
				return null;

			int  							_count 							= _achievementDescriptionList.Length;
			iOSAchievementDescription[]		_iOSAchievementDescriptionList	= new iOSAchievementDescription[_count];
			
			for (int _iter = 0; _iter < _count; _iter++)
				_iOSAchievementDescriptionList[_iter] 						= new iOSAchievementDescription(_achievementDescriptionList[_iter]);

			return _iOSAchievementDescriptionList;
		}

		#endregion

		#region Methods
		
		public override void GetImageAsync (DownloadTexture.Completion _onCompletion)
		{
			if (_onCompletion != null)
			{
				if (m_achievementDescriptionData.image == null)
					_onCompletion(null, "Texture not found.");
				else
					_onCompletion(m_achievementDescriptionData.image, null);
			}
		}
		
		#endregion
	}
}
#endif