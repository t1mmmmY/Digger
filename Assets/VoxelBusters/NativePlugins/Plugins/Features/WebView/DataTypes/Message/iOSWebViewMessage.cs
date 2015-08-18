using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins.Internal
{
	public sealed class iOSWebViewMessage : WebViewMessage
	{
//		{
//			"host": "move",
//			"arguments": {
//				"cmd": "showAlert",
//				"var": "myVar"
//			},
//			"url-scheme": "unity"
//		}

		#region Constants

		private const string 	kHost		= "host";
		private const string 	kArguments	= "arguments";
		private const string 	kURLScheme	= "url-scheme";

		#endregion

		#region Constructor

		public iOSWebViewMessage (IDictionary _schemeDataJsonDict)
		{
			string _schemeName	= _schemeDataJsonDict.GetIfAvailable<string>(kURLScheme);
			string _host		= _schemeDataJsonDict.GetIfAvailable<string>(kHost);
			IDictionary _args	= _schemeDataJsonDict[kArguments] as IDictionary;

			// Assign value
			SchemeName			= _schemeName;
			Host				= _host;
			Arguments			= new Dictionary<string, string>();

			foreach (object _key in _args.Keys)
			{
				string _keyStr		= _key.ToString();
				string _valueStr	= _args[_key].ToString();

				// Add key and value
				Arguments[_keyStr]	= _valueStr;
			}
		}

		#endregion
	}
}