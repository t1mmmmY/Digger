using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR && !(UNITY_WEBPLAYER || UNITY_WEBGL || NETFX_CORE)
using System.IO;
using System.Collections;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins.Internal
{
	public class UninstallPlugin
	{
		#region Constants
	
		private const	string	kUninstallAlertTitle	= "Uninstall - Cross Platform Native Plugin";
		private const	string	kUninstallAlertMessage	= "Backup before doing this step to preserve changes done in this plugin. This deletes files only related to CPNP plugin. Do you want to proceed?";

		private static string[] kPluginFolders	=	new string[]
		{
			"Assets/Plugins/Android/native_plugins_lib",
			"Assets/Plugins/Android/voxelbusters_utility_lib",
			"Assets/VoxelBusters/NativePlugins",
			"Assets/VoxelBusters/Common",
			"Assets/VoxelBusters/DebugPro"
		};
		
		#endregion	
	
		#region Methods
	
		public static void Uninstall()
		{
			bool _startUninstall = EditorUtility.DisplayDialog(kUninstallAlertTitle, kUninstallAlertMessage, "Uninstall", "Cancel");

			if (_startUninstall)
			{
				foreach (string _eachFolder in kPluginFolders)
				{
					string _absolutePath = AssetsUtility.AssetPathToAbsolutePath(_eachFolder);

					if (Directory.Exists(_absolutePath))
					{
						Directory.Delete(_absolutePath, true);
						
						// Delete meta files.
						FileOperations.Delete(_absolutePath + ".meta");
					}
				}
				
				// For LITE version we need to remove defines.
				GlobalDefinesManager _definesManager	= new GlobalDefinesManager();

				foreach (int _eachCompiler in System.Enum.GetValues(typeof(GlobalDefinesManager.eCompiler)))
				{
					_definesManager.RemoveDefineSymbol((GlobalDefinesManager.eCompiler)_eachCompiler, NPSettings.kLiteVersionMacro);
				}

				_definesManager.SaveAllCompilers();
				
				AssetDatabase.Refresh();
				EditorUtility.DisplayDialog("Cross Platform Native Plugin",
				                            "Uninstall successful!", 
				                            "ok");
			}
		}
		
		#endregion
	}
}
#endif