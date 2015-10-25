using UnityEngine;
using System.Collections;

#if USES_WEBVIEW && UNITY_IOS
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins.Internal
{
	public partial class WebViewNativeIOS : WebViewNative
	{
		#region Parse Methods
		
		protected override void ParseLoadErrorData (IDictionary _dataDict, out string _tag, out string _error)
		{
			_tag	= _dataDict.GetIfAvailable<string>("tag");		
			_error	= _dataDict.GetIfAvailable<string>("error");
		}
		
		protected override void ParseEvalJSData (IDictionary _resultData, out string _tag, out string _result)
		{
			_tag	= _resultData.GetIfAvailable<string>("tag");
			_result	= _resultData.GetIfAvailable<string>("result");
		}

//		{
//			"tag": "tag",
//			"message-data": {
//				"host": "move",
//				"arguments": {
//					"cmd": "showAlert",
//					"var": "myVar"
//				},
//				"url-scheme": "unity"
//			}
//		}
		
		protected override void ParseMessageData (IDictionary _dataDict, out string _tag, out WebViewMessage _message)
		{
			_tag		= _dataDict.GetIfAvailable<string>("tag");
			_message	= new iOSWebViewMessage(_dataDict["message-data"] as IDictionary);
		}
	
		#endregion
	}
}
#endif