using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	/// <summary>
	/// Application Settings provides interface to configure properties related to this application.
	/// </summary>
	[System.Serializable]
	public partial class ApplicationSettings 
	{
		#region Fields

		[SerializeField]
		private		iOSSettings		m_iOS;
		[SerializeField]
		private 	AndroidSettings	m_android;
		[SerializeField]
		private		Features		m_supportedFeatures;

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value indicating whether Native plugin is in debug mode.
		/// </summary>
		/// <value><c>true</c> if this instance is debug mode; otherwise, <c>false</c>.</value>
		public bool	IsDebugMode
		{
			get
			{
				return Debug.isDebugBuild;
			}
		}

		/// <summary>
		/// Gets or sets the Application Settings specific to iOS platform.
		/// </summary>
		/// <value>The Application Settings specific to iOS platform.</value>
		public iOSSettings IOS
		{
			get
			{
				return m_iOS;
			}
			
			private set
			{
				m_iOS	= value;
			}
		}
		
		/// <summary>
		/// Gets or sets the Application Settings specific to Android platform.
		/// </summary>
		/// <value>The Application Settings specific to Android platform.</value>
		public AndroidSettings Android
		{
			get
			{
				return m_android;
			}
			
			private set
			{
				m_android	= value;
			}
		}

		public Features SupportedFeatures
		{
			get
			{
				return m_supportedFeatures;
			}
			
			private set
			{
				m_supportedFeatures	= value;
			}
		}
		
		/// <summary>
		/// Gets the store identifier for current build platform.
		/// </summary>
		/// <value>The store identifier for current build platform.</value>
		public string StoreIdentifier
		{
			get
			{
#if UNITY_ANDROID
				return m_android.StoreIdentifier;
#else
				return m_iOS.StoreIdentifier;
#endif
			}
		}
		
		#endregion
	}
}