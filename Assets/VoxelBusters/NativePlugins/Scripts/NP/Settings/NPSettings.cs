using UnityEngine;
using System.Collections;
using VoxelBusters.Utility;
using VoxelBusters.AssetStoreProductUtility;
using PlayerSettings	= VoxelBusters.Utility.PlayerSettings;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VoxelBusters.NativePlugins
{
	using Internal;

#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	public class NPSettings : AdvancedScriptableObject <NPSettings>, IAssetStoreProduct
	{
		#region Constants
		
		private const 	string 						kProductName					= "Native Plugins";
		private const 	string 						kProductVersion					= "1.04.2";
		
		#endregion

		#region Properties

		[System.NonSerialized]
		private	AssetStoreProduct					m_assetStoreProduct;
		public AssetStoreProduct					AssetStoreProduct 
		{
			get 
			{
				return m_assetStoreProduct;
			}
		}

		[SerializeField]
		private ApplicationSettings					m_applicationSettings			= new ApplicationSettings();
		/// <summary>
		/// Gets the application settings.
		/// </summary>
		/// <value>The application settings.</value>
		public static ApplicationSettings			Application
		{
			get 
			{ 
				return Instance.m_applicationSettings; 
			}
		}

		[SerializeField]
		private NetworkConnectivitySettings			m_networkConnectivitySettings	= new NetworkConnectivitySettings();
		/// <summary>
		/// Gets the network connectivity settings.
		/// </summary>
		/// <value>The network connectivity settings.</value>
		public static NetworkConnectivitySettings	NetworkConnectivity
		{
			get 
			{ 
				return Instance.m_networkConnectivitySettings; 
			}
		}

		[SerializeField]
		private UtilitySettings						m_utilitySettings				= new UtilitySettings();
		/// <summary>
		/// Gets the utility settings.
		/// </summary>
		/// <value>The utility settings.</value>
		public static UtilitySettings				Utility
		{
			get 
			{ 
				return Instance.m_utilitySettings; 
			}
		}

#if !NATIVE_PLUGINS_LITE_VERSION
		[SerializeField]
		private BillingSettings						m_billingSettings				= new BillingSettings();
		/// <summary>
		/// Gets the billing settings.
		/// </summary>
		/// <value>The billing settings.</value>
		public static BillingSettings				Billing
		{
			get 
			{ 
				return Instance.m_billingSettings; 
			}
		}
		
		[SerializeField]
		private MediaLibrarySettings				m_mediaLibrarySettings			= new MediaLibrarySettings();
		/// <summary>
		/// Gets the media library settings.
		/// </summary>
		/// <value>The media library settings.</value>
		public static MediaLibrarySettings			MediaLibrary
		{
			get 
			{ 
				return Instance.m_mediaLibrarySettings; 
			}
		}
	
		[SerializeField]
		private NotificationServiceSettings			m_notificationSettings			= new NotificationServiceSettings();
		/// <summary>
		/// Gets the notification settings.
		/// </summary>
		/// <value>The notification settings.</value>
		public static NotificationServiceSettings	Notification
		{
			get 
			{ 
				return Instance.m_notificationSettings; 
			}
		}

		[SerializeField]
		private SocialNetworkSettings				m_socialNetworkSettings			= new SocialNetworkSettings();
		/// <summary>
		/// Gets the twitter settings.
		/// </summary>
		/// <value>The twitter settings.</value>
		public static SocialNetworkSettings			SocialNetworkSettings
		{
			get 
			{ 
				return Instance.m_socialNetworkSettings; 
			}
		}

#endif

		public		static	string						Version
		{
			get
			{
				return kProductVersion;
			}
		}

		#endregion

		#region Constants

		private		const	string						kBuildIdentifierKey				= "np-build-identifier";
		private		const	string						kTwitterDefine					= "USES_TWITTER";

		#endregion

		#region Constructor

#if UNITY_EDITOR
		static NPSettings ()
		{
#if DISABLE_AUTO_GENERATE_SETTINGS_FILES
			EditorInvoke.Invoke(()=>{
				#pragma warning disable
				NPSettings _instance	= NPSettings.Instance;
				#pragma warning restore
			}, 1f);
#else
			EditorInvoke.InvokeRepeating(()=>{
				NPSettings _instance	= NPSettings.Instance;
				
				// Monitor player settings changes
				_instance.MonitorPlayerSettings();
			}, 1f, 1f);
#endif
		}
#endif

		#endregion

		#region Unity Methods

		protected override void Reset ()
		{
			base.Reset();

#if UNITY_EDITOR
			m_assetStoreProduct			= new AssetStoreProduct(kProductName, kProductVersion, Constants.kLogoPath);
#endif
		}

		protected override void OnEnable ()
		{
			base.OnEnable ();

#if UNITY_EDITOR
			m_assetStoreProduct	= new AssetStoreProduct(kProductName, kProductVersion, Constants.kLogoPath);
#endif

			// Set debug mode
			if (m_applicationSettings.IsDebugMode)
				DebugPRO.Console.RemoveIgnoreTag(Constants.kDebugTag);
			else
				DebugPRO.Console.AddIgnoreTag(Constants.kDebugTag);
		}

		#endregion

		#region Methods

#if UNITY_EDITOR

		#if !NATIVE_PLUGINS_LITE_VERSION
		private void OnTwitterConfigChanged ()
		{
			// Take action on configuration changes
			OnApplicationConfigurationChanged();

			#if !(UNITY_WEBPLAYER || UNITY_WEBGL)

			// Update defines
			GlobalDefinesManager _definesManager	= new GlobalDefinesManager();

			if (Application.SupportedFeatures.UsesTwitter)
			{
				_definesManager.AddNewDefineSymbol(GlobalDefinesManager.eCompiler.CSHARP, 		kTwitterDefine);
				_definesManager.AddNewDefineSymbol(GlobalDefinesManager.eCompiler.BOO, 			kTwitterDefine);
				_definesManager.AddNewDefineSymbol(GlobalDefinesManager.eCompiler.EDITOR, 		kTwitterDefine);
				_definesManager.AddNewDefineSymbol(GlobalDefinesManager.eCompiler.UNITY_SCRIPT, kTwitterDefine);
			}
			else
			{
				_definesManager.RemoveDefineSymbol(GlobalDefinesManager.eCompiler.CSHARP, 		kTwitterDefine);
				_definesManager.RemoveDefineSymbol(GlobalDefinesManager.eCompiler.BOO, 			kTwitterDefine);
				_definesManager.RemoveDefineSymbol(GlobalDefinesManager.eCompiler.EDITOR, 		kTwitterDefine);
				_definesManager.RemoveDefineSymbol(GlobalDefinesManager.eCompiler.UNITY_SCRIPT, kTwitterDefine);
			}

			_definesManager.SaveAllCompilers();

			#endif
		}
		#endif

		private void OnApplicationConfigurationChanged ()
		{
			string _manifestFolderPath = "Assets/Plugins/Android/native_plugins_lib/";

			if (AssetsUtility.FolderExists(_manifestFolderPath))
			{
				NPAndroidManifestGenerator _generator	= new NPAndroidManifestGenerator();

				// Save file
				_generator.SaveManifest("com.voxelbusters.androidnativeplugin", _manifestFolderPath + "AndroidManifest.xml");

				// Refresh
				AssetDatabase.Refresh();
			}
		}

		private void MonitorPlayerSettings ()
		{
			string	_currentBuildIdentifier	= EditorPrefs.GetString(kBuildIdentifierKey, null);

			if (string.Equals(_currentBuildIdentifier, PlayerSettings.GetBundleIdentifier()))
				return;

			// Update value
			EditorPrefs.SetString(kBuildIdentifierKey, PlayerSettings.GetBundleIdentifier());

			// Update Android manifest
			OnApplicationConfigurationChanged();
		}
#endif
		#endregion
	}
}