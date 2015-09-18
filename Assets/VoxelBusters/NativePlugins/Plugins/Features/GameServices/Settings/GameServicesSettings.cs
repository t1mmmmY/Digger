using UnityEngine;
using System.Collections;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins
{
	/// <summary>
	/// Game Services Settings provides interface to configure properties related to Game Services.
	/// </summary>
	[System.Serializable]
	public class GameServicesSettings
	{
		#region Android Settings
		
		/// <summary>
		/// Game Services Settings specific to Android platform.
		/// </summary>
		[System.Serializable]
		public class AndroidSettings 
		{
			[SerializeField, ExecuteOnValueChange("OnApplicationConfigurationChanged")]
			[Tooltip ("This will be the application id that will be listed in Google Play Dev Console.")]
			private string	 		m_playServicesApplicationID;
			
			/// <summary>
			/// Gets or sets the application ID.
			/// </summary>
			/// <value>	Contains Application ID used for accessing Google Play Game Services.
			///</value>
			public string PlayServicesApplicationID
			{
				get
				{
					return m_playServicesApplicationID;
				}
				
				set
				{
					m_playServicesApplicationID = value;
				}
			}

			[SerializeField]
			[Tooltip ("# will be replaced with achievement title.")]
			private string[]	 	m_achievedDescriptionFormats = new string[]{"Awesome! Achievement # completed."};
			
			/// <summary>
			/// Gets or sets the achievement description format.
			/// </summary>
			/// <value>	Allows to set different formats for achieved description text.
			///</value>
			public string[] AchievedDescriptionFormats
			{
				get
				{
					return m_achievedDescriptionFormats;
				}
				
				set
				{
					m_achievedDescriptionFormats = value;
				}
			}
			
		}
		
		#endregion
		
		#region Fields
		
		[SerializeField]
		private AndroidSettings			m_android;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the Game Services Settings specific to Android platform.
		/// </summary>
		/// <value>The android.</value>
		public	AndroidSettings	Android
		{
			get 
			{
				return m_android; 
			}
			
			set 
			{ 
				m_android = value; 
			}
		}
		
		#endregion
		
		#region Constructor
		
		public GameServicesSettings ()
		{
			Android		= new AndroidSettings();
		}
		
		#endregion
	}
}