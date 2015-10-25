#if !(UNITY_WINRT || UNITY_WEBPLAYER || UNITY_WEBGL)
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using VoxelBusters.Utility;
using VoxelBusters.ThirdParty.XUPorter;
using PlayerSettings = UnityEditor.PlayerSettings;

namespace VoxelBusters.NativePlugins
{
	public class PostProcessBuild
	{
		#region Constants

		// File folders
		private const string	kRelativePathNativePluginsFolder		= "Assets/VoxelBusters/NativePlugins";
		private	const string	kRelativePathIOSNativeCodeFolder		= kRelativePathNativePluginsFolder + "/Plugins/NativeIOSCode";
		private const string	kRelativePathXcodeModDataCollectionFile	= kRelativePathNativePluginsFolder + "/XCodeModData.txt";
		private const string 	kRelativePathInfoPlistFile				= "Info.plist";
		private const string 	kRelativePathInfoPlistBackupFile		= "Info.backup.plist";
		private	const string	kRelativePathNativePluginsTempFolder	= "NativePlugins";

		// Mod keys
		private	const string	kModKeyAddressBook						= "NativePlugins-AddressBook";
		private	const string	kModKeyBilling							= "NativePlugins-Billing";
		private	const string	kModKeyCommon							= "NativePlugins-Common";
		private	const string	kModKeyGameServices						= "NativePlugins-GameServices";
		private	const string	kModKeyMediaLibrary						= "NativePlugins-MediaLibrary";
		private	const string	kModKeyNetworkConnectivity				= "NativePlugins-NetworkConnectivity";
		private	const string	kModKeyNotification						= "NativePlugins-Notification";
		private	const string	kModKeySharing							= "NativePlugins-Sharing";
		private	const string	kModKeyTwitter							= "NativePlugins-Twitter";
		private	const string	kModKeyTwitterFramework					= "NativePlugins-TwitterFramework";
		private	const string	kModKeyWebView							= "NativePlugins-WebView";

		// PlayerPrefs keys
		private	const string	kTwitterConfigKey						= "twitter-config";
	
		// Fabric data
		private const string 	kFabricKitJsonStringFormat				= "{{\"Fabric\":{{\"APIKey\":\"{0}\",\"Kits\":[{{\"KitInfo\":{{\"consumerKey\":\"\",\"consumerSecret\":\"\"}},\"KitName\":\"Twitter\"}}]}}}}";
		
		// Pch file modification
		private const string 	kPrecompiledFileRelativeDirectoryPath	= "Classes/";
		private const string 	kPrecompiledHeaderExtensionPattern		= "*.pch";
		private const string	kPrecompiledHeaderRegexPattern			= @"#ifdef __OBJC__(\n?\t?[#|//](.)*)*";
		private const string	kPrecompiledHeaderEndIfTag				= "#endif";
		private const string	kPrecompiledHeaderInsertText			= "#import \"Defines.h\"";

		#endregion

		#region Methods

		[PostProcessBuild(0)]
		public static void OnPostProcessBuild (BuildTarget _target, string _buildPath) 
		{			
			string 	_targetStr	= _target.ToString();
			
			if (_targetStr.Equals ("iOS") || _targetStr.Equals ("iPhone"))
			{
				iOSPostProcessBuild (_target, _buildPath);
				return;
			}
		}

		private static void iOSPostProcessBuild (BuildTarget _target, string _buildPath) 
		{
			// Removing old automation related files
			CleanAndResetProject ();

			// Post process actions
			ProcessFeatureSpecificOperations ();
			GenerateXcodeModFiles ();
			ModifyInfoPlist (_buildPath);
			ModifyPchFile (_buildPath);
		}

		private static void CleanAndResetProject ()
		{
			// Remove old xmod files
			string		_nativeCodePath			= AssetsUtility.AssetPathToAbsolutePath(kRelativePathIOSNativeCodeFolder);
			string[] 	_modFiles			 	= Directory.GetFiles(_nativeCodePath, "*.xcodemods", SearchOption.AllDirectories);
			
			foreach (string _curFile in _modFiles)
			{
				File.SetAttributes(_curFile, FileAttributes.Normal);
				File.Delete(_curFile);
			}
			
			// Remove old NP files which are placed outside Assets path
			if (Directory.Exists(kRelativePathNativePluginsTempFolder))
			{
				IOExtensions.AssignPermissionRecursively(kRelativePathNativePluginsTempFolder, FileAttributes.Normal);
				Directory.Delete(kRelativePathNativePluginsTempFolder, true);
			}
			
			// Create new folder
			Directory.CreateDirectory(kRelativePathNativePluginsTempFolder);
		}

		private static void ProcessFeatureSpecificOperations ()
		{
			ApplicationSettings.Features	_supportedFeatures	= NPSettings.Application.SupportedFeatures;

			if (_supportedFeatures.UsesBilling)
				AddBuildInfoToReceiptVerificationManger();

			// Decompress zip files and add it to project
			if (_supportedFeatures.UsesTwitter)
				DecompressTwitterFrameworkFiles();
		}
		
		private static void AddBuildInfoToReceiptVerificationManger ()
		{
			string		_rvFilePath			= Path.Combine(kRelativePathIOSNativeCodeFolder, "Billing/Source/ReceiptVerification/Manager/ReceiptVerificationManager.m");
			string[]	_contents			= File.ReadAllLines(_rvFilePath);
			int			_lineCount			= _contents.Length;
			
			for (int _iter = 0; _iter < _lineCount; _iter++)
			{
				string	_curLine			= _contents[_iter];
				
				if (!_curLine.StartsWith("const"))
					continue;
				
				if (_curLine.Contains("bundleIdentifier"))
				{
					_contents[_iter]		= string.Format("const NSString *bundleIdentifier\t= @\"{0}\";", PlayerSettings.bundleIdentifier);
					_contents[_iter + 1]	= string.Format("const NSString *bundleVersion\t\t= @\"{0}\";", PlayerSettings.bundleVersion);
					break;
				}
			}
			
			// Now rewrite updated contents
			File.WriteAllLines(_rvFilePath, _contents);
		}
		
		private static void DecompressTwitterFrameworkFiles ()
		{
			string		_projectPath					= AssetsUtility.GetProjectPath();
			string		_twitterNativeCodeFolderPath	= Path.Combine(_projectPath, kRelativePathIOSNativeCodeFolder + "/Twitter");
			string		_twitterTempFolderPath			= Path.Combine(_projectPath, kRelativePathNativePluginsTempFolder + "/Twitter");
			
			if (!Directory.Exists(_twitterNativeCodeFolderPath)) 
				return;
			
			Directory.CreateDirectory(_twitterTempFolderPath);
			
			// ***********************
			// Framework Section
			// ***********************
			string[] 	_zippedFiles		= Directory.GetFiles(_twitterNativeCodeFolderPath, "*.gz", SearchOption.AllDirectories);
			string		_destFolder			= Path.Combine(_twitterTempFolderPath, "Framework");
			
			Directory.CreateDirectory(_destFolder);
			
			// Iterate through each zip files
			foreach (string _curZippedFile in _zippedFiles) 
				Zip.DecompressToDirectory(_curZippedFile, _destFolder);
		}

		private static void	GenerateXcodeModFiles ()
		{
			string		_xcodeModDataCollectionText	= File.ReadAllText (kRelativePathXcodeModDataCollectionFile);

			if (_xcodeModDataCollectionText == null)
				return;

			IDictionary	_xcodeModDataCollectionDict	= JSONUtility.FromJSON (_xcodeModDataCollectionText) as IDictionary;
			ApplicationSettings.Features	_supportedFeatures	= NPSettings.Application.SupportedFeatures;

			// Create mod file to add common files
			ExtractAndSerializeXcodeModInfo (_xcodeModDataCollectionDict,		kModKeyCommon, 			kRelativePathIOSNativeCodeFolder);

			if (_supportedFeatures.UsesAddressBook)
				ExtractAndSerializeXcodeModInfo (_xcodeModDataCollectionDict,	kModKeyAddressBook,		kRelativePathIOSNativeCodeFolder);

			if (_supportedFeatures.UsesBilling)
				ExtractAndSerializeXcodeModInfo(_xcodeModDataCollectionDict,	kModKeyBilling, 		kRelativePathIOSNativeCodeFolder);
			
			if (_supportedFeatures.UsesGameServices)
				ExtractAndSerializeXcodeModInfo(_xcodeModDataCollectionDict,	kModKeyGameServices, 	kRelativePathIOSNativeCodeFolder);
			
			if (_supportedFeatures.UsesMediaLibrary)
				ExtractAndSerializeXcodeModInfo(_xcodeModDataCollectionDict,	kModKeyMediaLibrary, 	kRelativePathIOSNativeCodeFolder);
			
			if (_supportedFeatures.UsesNetworkConnectivity)
				ExtractAndSerializeXcodeModInfo (_xcodeModDataCollectionDict,	kModKeyNetworkConnectivity, kRelativePathIOSNativeCodeFolder);

			if (_supportedFeatures.UsesNotificationService)
				ExtractAndSerializeXcodeModInfo(_xcodeModDataCollectionDict,	kModKeyNotification, 	kRelativePathIOSNativeCodeFolder);
			
			if (_supportedFeatures.UsesSharing)
				ExtractAndSerializeXcodeModInfo (_xcodeModDataCollectionDict,	kModKeySharing, 		kRelativePathIOSNativeCodeFolder);

			if (_supportedFeatures.UsesTwitter)
			{
				ExtractAndSerializeXcodeModInfo(_xcodeModDataCollectionDict,	kModKeyTwitter, 		kRelativePathIOSNativeCodeFolder);
				ExtractAndSerializeXcodeModInfo(_xcodeModDataCollectionDict,	kModKeyTwitterFramework,kRelativePathNativePluginsTempFolder);
			}

			if (_supportedFeatures.UsesWebView)
				ExtractAndSerializeXcodeModInfo(_xcodeModDataCollectionDict,	kModKeyWebView, 		kRelativePathIOSNativeCodeFolder);
		}

		private static void ExtractAndSerializeXcodeModInfo (IDictionary _modCollectionDict, string _modKey, string _folderRelativePath)
		{
			IDictionary		_modInfoDict	= (IDictionary)_modCollectionDict[_modKey];
			string			_newModFileName	= _modKey + ".xcodemods";

			File.WriteAllText(Path.Combine(_folderRelativePath, _newModFileName), _modInfoDict.ToJSON());
		}

//		{
//			"Fabric": {
//				"APIKey": "{0}",
//				"Kits": [
//				    {
//					"KitInfo": {
//						"consumerKey": "",
//						"consumerSecret": ""
//					},
//					"KitName": "Twitter"
//				    }
//				    ]
//			}
//		}

		private static void ModifyInfoPlist (string _buildPath)
		{
			ApplicationSettings.Features _supportedFeatures	= NPSettings.Application.SupportedFeatures;

			string 			_path2InfoPlist				= Path.Combine(_buildPath, kRelativePathInfoPlistFile);
			string 			_path2InfoPlistBackupFile	= Path.Combine(_buildPath, kRelativePathInfoPlistBackupFile);
			Plist 			_infoPlist					= Plist.LoadPlistAtPath(_path2InfoPlist);
			Dictionary<string, object> _newPermissionsDict	= new Dictionary<string, object>();
			
			// Create backup
			_infoPlist.Save(_path2InfoPlistBackupFile);

			// Add twitter related info 
#if USES_TWITTER
			{
				const string 	_kFabricKitRootKey 		= "Fabric";
				TwitterSettings _twitterSettings		= NPSettings.SocialNetworkSettings.TwitterSettings;
				string 			_fabricJsonStr			= string.Format(kFabricKitJsonStringFormat, _twitterSettings.ConsumerKey);
				IDictionary 	_fabricJsonDictionary	= (IDictionary)JSONUtility.FromJSON(_fabricJsonStr);

				// Add fabric
				_newPermissionsDict[_kFabricKitRootKey]	= _fabricJsonDictionary[_kFabricKitRootKey];
			}
#endif

			// Device capablities addition
			if (_supportedFeatures.UsesGameServices)
			{
				const string	_kDeviceCapablitiesKey	= "UIRequiredDeviceCapabilities";
				const string	_kGameKitKey			= "gamekit";
				IList			_deviceCapablitiesList	= (IList)_infoPlist.GetKeyPathValue(_kDeviceCapablitiesKey);

				if (_deviceCapablitiesList == null)
					_deviceCapablitiesList				= new List<string>();

				if (!_deviceCapablitiesList.Contains(_kGameKitKey))
					_deviceCapablitiesList.Add(_kGameKitKey);

				_newPermissionsDict[_kDeviceCapablitiesKey]	= _deviceCapablitiesList;
			}

			// Query scheme related key inclusion
			if (_supportedFeatures.UsesSharing)
			{
				const string	_kQuerySchemesKey		= "LSApplicationQueriesSchemes";
				const string	_kWhatsAppKey			= "whatsapp";
				IList			_queriesSchemesList		= (IList)_infoPlist.GetKeyPathValue(_kQuerySchemesKey);

				if (_queriesSchemesList == null)
					_queriesSchemesList					= new List<string>();

				if (!_queriesSchemesList.Contains(_kWhatsAppKey))
					_queriesSchemesList.Add(_kWhatsAppKey);

				_newPermissionsDict[_kQuerySchemesKey]	= _queriesSchemesList;
			}

			// Apple transport security
			if (_supportedFeatures.UsesNetworkConnectivity || _supportedFeatures.UsesWebView)
			{
				const string	_kATSKey				= "NSAppTransportSecurity";
				const string	_karbitraryLoadsKey		= "NSAllowsArbitraryLoads";
				IDictionary		_transportSecurityDict	= (IDictionary)_infoPlist.GetKeyPathValue(_kATSKey);

				if (_transportSecurityDict == null)
					_transportSecurityDict				= new Dictionary<string, object>();

				_transportSecurityDict[_karbitraryLoadsKey]	= true;
				_newPermissionsDict[_kATSKey]			= _transportSecurityDict;
			}

			if (_newPermissionsDict.Count == 0)
				return;

			// First create a backup of old data
			_infoPlist.Save(_path2InfoPlistBackupFile);

			// Now add new permissions
			foreach (string _key in _newPermissionsDict.Keys)
				_infoPlist.AddValue(_key, _newPermissionsDict[_key]);

			// Save these changes
			_infoPlist.Save(_path2InfoPlist);
		}

		private static void ModifyPchFile (string _buildPath)
		{
			string 		_pchFileDirectory	= Path.Combine(_buildPath, kPrecompiledFileRelativeDirectoryPath);
			string[] 	_pchFiles 			= Directory.GetFiles(_pchFileDirectory, kPrecompiledHeaderExtensionPattern);
			string 		_pchFilePath 		= null;

			// There will be only one file per project if it exists.
			if (_pchFiles.Length > 0)
				_pchFilePath =  _pchFiles[0];

			if (File.Exists(_pchFilePath))
			{
				string 	_fileContents 		= File.ReadAllText(_pchFilePath);

				// Make sure content doesnt exist
				if (_fileContents.Contains(kPrecompiledHeaderInsertText))
					return;

				Regex 	_regex				= new Regex(kPrecompiledHeaderRegexPattern);
				Match 	_match 				= _regex.Match(_fileContents);
				int		_endOfPatternIndex	= _match.Groups[0].Index + _match.Groups[0].Length;

				// We should append text within end tag
				if (_match.Value.Contains(kPrecompiledHeaderEndIfTag))
					_endOfPatternIndex	-= kPrecompiledHeaderEndIfTag.Length;

				string 	_updatedContents	= _fileContents.Insert(_endOfPatternIndex, "\t" + kPrecompiledHeaderInsertText + "\n");

				// Write updated text
				File.WriteAllText(_pchFilePath, _updatedContents);
			}
		}
		
		#endregion
	}
}
#endif