using UnityEngine;
using System.Collections;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	/// <summary>
	/// Game Services Settings provides interface to configure properties related to Game Services.
	/// </summary>
	[System.Serializable]
	public partial class GameServicesSettings
	{
		#region Fields
		
		[SerializeField]
		private 	AndroidSettings			m_android;
		
		[SerializeField]
		private 	iOSSettings				m_iOS;

		[SerializeField]
		private		IDContainer[]			m_achievementIDCollection	= new IDContainer[0];	
		
		[SerializeField, InspectorButton("Refresh Editor Game Center", "RefreshEditorGameCenter", InspectorButtonAttribute.ePosition.BOTTOM)]
		private		IDContainer[]			m_leaderboardIDCollection	= new IDContainer[0];			

		#endregion

		#region Properties

		/// <summary>
		/// Gets Game Services Settings specific to Android platform.
		/// </summary>
		public AndroidSettings Android
		{
			get 
			{
				return m_android; 
			}
			
			private set 
			{ 
				m_android = value; 
			}
		}

		/// <summary>
		/// Gets Game Services Settings specific to iOS platform.
		/// </summary>
		public iOSSettings IOS
		{
			get 
			{
				return m_iOS; 
			}
			
			private set 
			{ 
				m_iOS = value; 
			}
		}

		public IDContainer[] AchievementIDCollection
		{
			get
			{
				return m_achievementIDCollection;
			}

			private set
			{
				m_achievementIDCollection	= value;
			}
		}

		public IDContainer[] LeaderboardIDCollection
		{
			get
			{
				return m_leaderboardIDCollection;
			}
			
			private set
			{
				m_leaderboardIDCollection	= value;
			}
		}

		#endregion
		
		#region Constructor
		
		public GameServicesSettings ()
		{
			// Intialize
			Android						= new AndroidSettings();
			AchievementIDCollection		= new IDContainer[0];
			LeaderboardIDCollection		= new IDContainer[0];
		}
		
		#endregion
	}
}