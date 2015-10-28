using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	public partial class GameServicesSettings
	{
		/// <summary>
		/// Game Services Settings specific to Android platform.
		/// </summary>
		[System.Serializable]
		public class AndroidSettings 
		{
			#region Fields

			[SerializeField, NotifyNPSettingsOnValueChange]
			[Tooltip ("This will be the application id that will be listed in Google Play Dev Console.")]
			private 	string		m_playServicesApplicationID;
			
			[SerializeField]
			[Tooltip ("# will be replaced with achievement title.")]
			private 	string[]	m_achievedDescriptionFormats = new string[]{"Awesome! Achievement # completed."};

			#endregion

			#region Properties

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
				
				private set
				{
					m_playServicesApplicationID = value;
				}
			}

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
				
				private set
				{
					m_achievedDescriptionFormats = value;
				}
			}

			#endregion
		}
	}
}