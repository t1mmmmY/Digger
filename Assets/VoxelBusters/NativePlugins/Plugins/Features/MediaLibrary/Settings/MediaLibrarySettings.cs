using UnityEngine;
using System.Collections;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins
{
	/// <summary>
	/// Media Library Settings provides interface to configure properties related to native media library.
	/// </summary>
	[System.Serializable]
	public class MediaLibrarySettings
	{
		#region Android Settings

		/// <summary>
		/// Media Library Settings specific to Android platform.
		/// </summary>
		[System.Serializable]
		public class AndroidSettings 
		{
			#region Fields

			[SerializeField]
			private 	string 			m_youtubeAPIKey;

			#endregion

			#region Properties

			/// <summary>
			/// Gets or sets the youtube API key.
			/// </summary>
			/// <value>The youtube API key.</value>
			/// <description>
			/// For more information, please check https://developers.google.com/youtube/v3/.
			/// </description>
			public string YoutubeAPIKey
			{
				get 
				{ 
					return m_youtubeAPIKey; 
				}

				private set
				{
					m_youtubeAPIKey	= value;
				}
			}
		}

		#endregion

		#endregion
		
		#region Fields
		
		[SerializeField]
		private 	AndroidSettings		m_android;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the Media Library Settings specific to Android platform.
		/// </summary>
		/// <value>The android.</value>
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
		
		#endregion

		#region Constructors
		
		public MediaLibrarySettings ()
		{
			Android		= new AndroidSettings();
		}
		
		#endregion
	}
}