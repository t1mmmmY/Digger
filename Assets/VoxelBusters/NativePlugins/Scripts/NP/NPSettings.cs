using UnityEngine;
using System.Collections;
using VoxelBusters.Utility;
using VoxelBusters.AssetStoreProductUtility;

#if UNITY_EDITOR
using UnityEditor;
using System;
using System.Collections.Generic;
using UnityPlayerSettings	= UnityEditor.PlayerSettings;
using VBPlayerSettings		= VoxelBusters.Utility.PlayerSettings;
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

		// Product info
		private		const	bool						kIsFullVersion					= true;
		private 	const 	string 						kProductName					= "Native Plugins";
		private 	const 	string 						kProductVersion					= "1.2.1";

		// Pref key
		private		const	string						kPrefsKeyBuildIdentifier		= "np-build-identifier";
		internal	const	string						kPrefsKeyPropertyModified		= "np-property-modified";

		// Macro symbols
		internal	const 	string						kLiteVersionMacro				= "NATIVE_PLUGINS_LITE_VERSION";
		private		const	string						kMacroAddressBook				= "USES_ADDRESS_BOOK";
		private		const	string						kMacroBilling					= "USES_BILLING";
		private		const	string						kMacroGameServices				= "USES_GAME_SERVICES";
		private		const	string						kMacroMediaLibrary				= "USES_MEDIA_LIBRARY";
		private		const	string						kMacroNetworkConnectivity		= "USES_NETWORK_CONNECTIVITY";
		private		const	string						kMacroNotificationService		= "USES_NOTIFICATION_SERVICE";
		private		const	string						kMacroSharin					= "USES_SHARING";
		private		const	string						kMacroTwitter					= "USES_TWITTER";
		private		const	string						kMacroWebView					= "USES_WEBVIEW";
		
		// Macro actions
		private 	readonly char[] 					defineSeperators 				= new char[] {
			';',
			',',
			' '
		};
	
#if UNITY_EDITOR
		private		readonly BuildTargetGroup[]			buildTargetGroups				= new BuildTargetGroup[]{
			BuildTargetGroup.Android,
#if UNITY_5 || UNITY_6 || UNITY_7
			BuildTargetGroup.iOS,
#else
			BuildTargetGroup.iPhone, 
#endif
#if UNITY_5 || UNITY_6 || UNITY_7			
			BuildTargetGroup.WSA, 
#else
			BuildTargetGroup.Metro, 
#endif
			BuildTargetGroup.WP8,
			BuildTargetGroup.Standalone
		};
#endif
		
		#endregion

		#region Fields

		[System.NonSerialized]
		private	AssetStoreProduct					m_assetStoreProduct;

		[SerializeField]
		private ApplicationSettings					m_applicationSettings			= new ApplicationSettings();

#if USES_NETWORK_CONNECTIVITY
		[SerializeField]
		private NetworkConnectivitySettings			m_networkConnectivitySettings	= new NetworkConnectivitySettings();
#endif
	
		[SerializeField]
		private UtilitySettings						m_utilitySettings				= new UtilitySettings();

#if USES_BILLING
		[SerializeField]
		private BillingSettings						m_billingSettings				= new BillingSettings();
#endif

#if USES_MEDIA_LIBRARY
		[SerializeField]
		private MediaLibrarySettings				m_mediaLibrarySettings			= new MediaLibrarySettings();
#endif

#if USES_NOTIFICATION_SERVICE
		[SerializeField]
		private NotificationServiceSettings			m_notificationSettings			= new NotificationServiceSettings();
#endif

#if USES_TWITTER
		[SerializeField]
		private SocialNetworkSettings				m_socialNetworkSettings			= new SocialNetworkSettings();
#endif

#if USES_GAME_SERVICES
		[SerializeField]
		private GameServicesSettings				m_gameServicesSettings			= new GameServicesSettings();
#endif

		#endregion

		#region Static Properties

		/// <summary>
		/// Gets the application settings.
		/// </summary>
		/// <value>The application settings.</value>
		public static ApplicationSettings Application
		{
			get 
			{ 
				return Instance.m_applicationSettings; 
			}
		}

#if USES_NETWORK_CONNECTIVITY
		/// <summary>
		/// Gets the network connectivity settings.
		/// </summary>
		/// <value>The network connectivity settings.</value>
		public static NetworkConnectivitySettings NetworkConnectivity
		{
			get 
			{ 
				return Instance.m_networkConnectivitySettings; 
			}
		}
#endif

		/// <summary>
		/// Gets the utility settings.
		/// </summary>
		/// <value>The utility settings.</value>
		public static UtilitySettings Utility
		{
			get 
			{ 
				return Instance.m_utilitySettings; 
			}
		}

#if USES_BILLING
		/// <summary>
		/// Gets the billing settings.
		/// </summary>
		/// <value>The billing settings.</value>
		public static BillingSettings Billing
		{
			get 
			{ 
				return Instance.m_billingSettings; 
			}
		}
#endif

#if USES_MEDIA_LIBRARY
		/// <summary>
		/// Gets the media library settings.
		/// </summary>
		/// <value>The media library settings.</value>
		public static MediaLibrarySettings MediaLibrary
		{
			get 
			{ 
				return Instance.m_mediaLibrarySettings; 
			}
		}
#endif

#if USES_NOTIFICATION_SERVICE
		/// <summary>
		/// Gets the notification settings.
		/// </summary>
		/// <value>The notification settings.</value>
		public static NotificationServiceSettings Notification
		{
			get 
			{ 
				return Instance.m_notificationSettings; 
			}
		}
#endif

#if USES_TWITTER
		/// <summary>
		/// Gets the twitter settings.
		/// </summary>
		/// <value>The twitter settings.</value>
		public static SocialNetworkSettings SocialNetworkSettings
		{
			get 
			{ 
				return Instance.m_socialNetworkSettings; 
			}
		}
#endif

#if USES_GAME_SERVICES
		/// <summary>
		/// Gets the Game Services settings.
		/// </summary>
		/// <value>The Game Services settings.</value>
		public static GameServicesSettings GameServicesSettings
		{
			get 
			{ 
				return Instance.m_gameServicesSettings; 
			}
		}
#endif

		#endregion

		#region Properties
		
		public AssetStoreProduct AssetStoreProduct 
		{
			get 
			{
				return m_assetStoreProduct;
			}
		}

		#endregion

		#region Constructor
#if !DISABLE_NPSETTINGS_GENERATION
#if UNITY_EDITOR
		static NPSettings ()
		{
			EditorInvoke.Invoke(()=>{
				NPSettings _instance	= NPSettings.Instance;

				// Save configuration once
				_instance.SaveConfigurationChanges();
			}, 1f);


			EditorInvoke.InvokeRepeating(()=>{
				NPSettings _instance	= NPSettings.Instance;
				
				// Monitor player settings changes
				_instance.MonitorPlayerSettings();
			}, 1f, 1f);
		}
#endif
#endif

		#endregion

		#region Unity Methods

		protected override void Reset ()
		{
			base.Reset();

#if UNITY_EDITOR
			m_assetStoreProduct	= new AssetStoreProduct(kProductName, kProductVersion, Constants.kLogoPath);
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

#if UNITY_EDITOR
		#region Editor Methods

		public void SaveConfigurationChanges ()
		{
			// Reset flags
			EditorPrefs.DeleteKey(kPrefsKeyPropertyModified);

			// Actions
			UpdateDefineSymbols();
			UpdatePluginResources();
			WriteAndroidManifestFile();
		}

		private void UpdateDefineSymbols ()
		{
			foreach (BuildTargetGroup _curBuildTargetGroup in buildTargetGroups)
			{
				string[]		_curDefineSymbols	= UnityPlayerSettings.GetScriptingDefineSymbolsForGroup(_curBuildTargetGroup).Split(defineSeperators, StringSplitOptions.RemoveEmptyEntries);
				List<string>	_newDefineSymbols	= new List<string>(_curDefineSymbols);

				// Asset type
				if (kIsFullVersion)
				{
					if (_newDefineSymbols.Contains(kLiteVersionMacro))
						_newDefineSymbols.Remove(kLiteVersionMacro);
				}
#pragma warning disable
				else
				{
					if (!_newDefineSymbols.Contains(kLiteVersionMacro))
						_newDefineSymbols.Add(kLiteVersionMacro);
				}
#pragma warning restore

				// Regarding features
				ApplicationSettings.Features _supportedFeatures	= m_applicationSettings.SupportedFeatures;

				AddOrRemoveFeatureDefineSymbol(_newDefineSymbols, _supportedFeatures.UsesAddressBook, 	kMacroAddressBook);
				AddOrRemoveFeatureDefineSymbol(_newDefineSymbols, _supportedFeatures.UsesBilling, 		kMacroBilling);
				AddOrRemoveFeatureDefineSymbol(_newDefineSymbols, _supportedFeatures.UsesGameServices, 	kMacroGameServices);
				AddOrRemoveFeatureDefineSymbol(_newDefineSymbols, _supportedFeatures.UsesMediaLibrary, 	kMacroMediaLibrary);
				AddOrRemoveFeatureDefineSymbol(_newDefineSymbols, _supportedFeatures.UsesNetworkConnectivity, kMacroNetworkConnectivity);
				AddOrRemoveFeatureDefineSymbol(_newDefineSymbols, _supportedFeatures.UsesNotificationService, kMacroNotificationService);
				AddOrRemoveFeatureDefineSymbol(_newDefineSymbols, _supportedFeatures.UsesSharing, kMacroSharin);
				AddOrRemoveFeatureDefineSymbol(_newDefineSymbols, _supportedFeatures.UsesTwitter, kMacroTwitter);
				AddOrRemoveFeatureDefineSymbol(_newDefineSymbols, _supportedFeatures.UsesWebView, kMacroWebView);

				// Now save these changes
				UnityPlayerSettings.SetScriptingDefineSymbolsForGroup(_curBuildTargetGroup, string.Join(";", _newDefineSymbols.ToArray()));
			}
		}

		private void AddOrRemoveFeatureDefineSymbol (List<string> _defineSymbols, bool _usesFeature, string _featureSymbol)
		{
			if (_usesFeature)
			{
				if (!_defineSymbols.Contains(_featureSymbol))
					_defineSymbols.Add(_featureSymbol);
			}
			else
			{
				if (_defineSymbols.Contains(_featureSymbol))
					_defineSymbols.Remove(_featureSymbol);
			}
		}

		private void WriteAndroidManifestFile ()
		{
			string _manifestFolderPath = Constants.kAndroidPluginsLibraryPath;
			
			if (AssetsUtility.FolderExists(_manifestFolderPath))
			{
				NPAndroidManifestGenerator _generator	= new NPAndroidManifestGenerator();
				
				// Save file
				_generator.SaveManifest("com.voxelbusters.androidnativeplugin", _manifestFolderPath + "/AndroidManifest.xml");
				
				// Refresh
				AssetDatabase.Refresh();
			}
		}
		
		private void UpdatePluginResources ()
		{
			ApplicationSettings.Features _supportedFeatures	= m_applicationSettings.SupportedFeatures;

			// Update JAR files
			UpdateJARFile(_supportedFeatures.UsesBilling, Constants.kBillingInterfaceJARName, Constants.kDisabledBillingWarning);

			// Copy required assets
			CopyNotificationAssets();
		}

		private void CopyNotificationAssets ()
		{
#if USES_NOTIFICATION_SERVICE
			// Copy save the texture data in res/drawable folder
			Texture2D[] _smallIcons = new Texture2D[]
			{	
				Notification.Android.WhiteSmallIcon,
				Notification.Android.ColouredSmallIcon
			};
			string[]	_paths		= new string[]
			{	
				Constants.kAndroidPluginsLibraryPath + "/res/drawable/app_icon_custom_white.png",
				Constants.kAndroidPluginsLibraryPath + "/res/drawable/app_icon_custom_coloured.png"
			};
			int 		_iconsConfiguredCount = 0;

			for(int _i = 0 ; _i < _smallIcons.Length ; _i++)
			{
				if(_smallIcons[_i] != null)
				{
					string _destinationFile = UnityEngine.Application.dataPath + "/../" + _paths[_i];
					System.IO.File.Copy(AssetDatabase.GetAssetPath(_smallIcons[_i]), _destinationFile, true);
					_iconsConfiguredCount++;
				}
			}
			
			if(_iconsConfiguredCount == 1)
			{
				Debug.LogError("[NPSettings] Should set both(white & coloured) icons for proper functionality on all devices. As, White icon will be used by post Android L devices and coloured one by pre Android L Devices.");
			}
#endif
		}

		private void MonitorPlayerSettings ()
		{
			string	_oldBuildIdentifier	= EditorPrefs.GetString(kPrefsKeyBuildIdentifier, null);
			string	_curBuildIdentifier	= VBPlayerSettings.GetBundleIdentifier();
			
			if (string.Equals(_oldBuildIdentifier, _curBuildIdentifier))
				return;
			
			// Update value
			EditorPrefs.SetString(kPrefsKeyBuildIdentifier, _curBuildIdentifier);
			
			// Save changes
			SaveConfigurationChanges();
		}
		
		private void UpdateJARFile (bool _usesFeature, string _JARName, string _warningMessage)
		{
			string 	_filePath	= Constants.kAndroidPluginsJARPath + "/" + _JARName;
			
			if (_usesFeature)
			{
				FileOperations.Rename(_filePath + ".jar.unused", _JARName + ".jar" );
			}
			else
			{
				FileOperations.Rename(_filePath + ".jar", _JARName + ".jar.unused" );
//				EditorUtility.DisplayDialog("Cross Platform Native Plugins - Warning", _warningMessage, "ok");	
			}
		}

		#endregion

		#region Editor Callback Methods

		private void RefreshEditorGameCenter ()
		{
#if USES_GAME_SERVICES
			EditorGameCenter.Instance.Refresh();
#endif
		}

		private void OnPropertyModified ()
		{
			EditorPrefs.SetBool(kPrefsKeyPropertyModified, true);
		}

		#endregion
#endif
	}
}