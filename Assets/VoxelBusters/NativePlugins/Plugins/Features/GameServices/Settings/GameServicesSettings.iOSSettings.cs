using UnityEngine;
using System.Collections;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins
{
	public partial class GameServicesSettings
	{
		/// <summary>
		/// Game Services Settings specific to iOS platform.
		/// </summary>
		[System.Serializable]
		public class iOSSettings 
		{
			#region Fields

			[SerializeField]
			[Tooltip ("Show the default banner when a achievement is completed")]
			private 	bool 	m_showDefaultAchievementCompletionBanner	=	true;

			#endregion

			#region Properties

			/// <summary>
			/// Gets or sets the application ID.
			/// </summary>
			/// <value>	Contains Application ID used for accessing Google Play Game Services.
			///</value>
			public bool ShowDefaultAchievementCompletionBanner
			{
				get
				{
					return m_showDefaultAchievementCompletionBanner;
				}
				
				private set
				{
					m_showDefaultAchievementCompletionBanner = value;
				}
			}

			#endregion
		}
	}
}