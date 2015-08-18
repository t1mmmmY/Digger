using UnityEngine;
using System.Collections;
using VoxelBusters.Utility;
using VoxelBusters.DebugPRO;

namespace VoxelBusters.NativePlugins.Internal
{
	public partial class WebViewNative : MonoBehaviour 
	{	
		#region Constants

		// Events
		private const string kOnDidShowEvent						= "OnDidShow";
		private const string kOnDidHideEvent						= "OnDidHide";
		private const string kOnDidDestroyEvent						= "OnDidDestroy";
		private const string kOnDidStartLoadEvent					= "OnDidStartLoad";
		private const string kOnDidFinishLoadEvent					= "OnDidFinishLoad";
		private const string kOnDidFailLoadWithErrorEvent			= "OnDidFailLoadWithError";
		private const string kOnDidFinishEvaluatingJavaScriptEvent	= "OnDidFinishEvaluatingJavaScript";
		private const string kOnDidReceiveMessageEvent				="OnDidReceiveMessage";

		#endregion

		#region Native Callback Methods

		protected void WebViewDidShow (string _tag)
		{
			// Get webview instance and call event handler
			WebViewDidShow(GetWebViewWithTag(_tag));
		}

		protected void WebViewDidShow (WebView _webview)
		{
			Console.Log(Constants.kDebugTag, "[WebView] Received DidShow event, Webview=" + _webview);

			if (_webview != null)
				_webview.InvokeMethod(kOnDidShowEvent);
		}
		
		protected void WebViewDidHide (string _tag)
		{
			// Get webview instance and call event handler
			WebViewDidHide(GetWebViewWithTag(_tag));
		}

		protected void WebViewDidHide (WebView _webview)
		{	
			Console.Log(Constants.kDebugTag, "[WebView] Received DidHide event, Webview=" + _webview);

			if (_webview != null)
				_webview.InvokeMethod(kOnDidHideEvent);
		}

		protected void WebViewDidDestroy (string _tag)
		{
			// Get webview instance and call event handler
			WebViewDidDestroy(GetWebViewWithTag(_tag));
		}

		protected void WebViewDidDestroy (WebView _webview)
		{
			Console.Log(Constants.kDebugTag, "[WebView] Received DidDestroy event, Webview=" + _webview);

			if (_webview != null)
				_webview.InvokeMethod(kOnDidDestroyEvent);
		}
		
		protected void WebViewDidStartLoad (string _tag)
		{
			// Get webview instance and call event handler
			WebViewDidStartLoad(GetWebViewWithTag(_tag));
		}

		protected void WebViewDidStartLoad (WebView _webview)
		{
			Console.Log(Constants.kDebugTag, "[WebView] Received DidStartLoad event, Webview=" + _webview);

			if (_webview != null)
				_webview.InvokeMethod(kOnDidStartLoadEvent);
		}
		
		protected void WebViewDidFinishLoad (string _tag)
		{
			// Get webview instance and call event handler
			WebViewDidFinishLoad(GetWebViewWithTag(_tag));
		}

		protected void WebViewDidFinishLoad (WebView _webview)
		{	
			Console.Log(Constants.kDebugTag, "[WebView] Received DidFinishLoad event, Webview=" + _webview);

			if (_webview != null)
				_webview.InvokeMethod(kOnDidFinishLoadEvent);
		}
		
		protected void WebViewDidFailLoadWithError (string _errorJsonStr)
		{	
			IDictionary _errorJsonDict	= JSONUtility.FromJSON(_errorJsonStr) as IDictionary;
			string _tag;
			string _error;

			// Parse received data
			ParseLoadErrorData(_errorJsonDict, out _tag, out _error);

			// Get webview instance and call event handler
			WebViewDidFailLoadWithError(GetWebViewWithTag(_tag), _error);
		}

		protected void WebViewDidFailLoadWithError (WebView _webview, string _error)
		{
			Console.Log(Constants.kDebugTag, "[WebView] Received DidFailLoadWithError event, Webview=" + _webview + " Error=" + _error);

			if (_webview != null)
				_webview.InvokeMethod(kOnDidFailLoadWithErrorEvent, _error);
		}
		
		protected void WebViewDidFinishEvaluatingJS (string _resultJsonStr)
		{
			IDictionary _resultJsonDict	= JSONUtility.FromJSON(_resultJsonStr) as IDictionary;
			string _tag;
			string _result;

			// Parse received data
			ParseEvalJSData(_resultJsonDict, out _tag, out _result);

			// Get webview instance and call event handler
			WebViewDidFinishEvaluatingJS(GetWebViewWithTag(_tag), _result);
		}

		protected void WebViewDidFinishEvaluatingJS (WebView _webview, string _result)
		{
			Console.Log(Constants.kDebugTag, "[WebView] Received DidFinishEvaluatingJS event, Webview=" + _webview);

			if (_webview != null)
				_webview.InvokeMethod(kOnDidFinishEvaluatingJavaScriptEvent, _result);
		}

		protected void WebViewDidReceiveMessage (string _dataJsonStr)
		{
			IDictionary _dataDict	= JSONUtility.FromJSON(_dataJsonStr) as IDictionary;
			string _tag;
			WebViewMessage	_message;

			// Parse received data
			ParseMessageData(_dataDict, out _tag, out _message);

			// Get webview instance and call event handler
			WebViewDidReceiveMessage(GetWebViewWithTag(_tag), _message);
		}

		protected void WebViewDidReceiveMessage (WebView _webview, WebViewMessage	_message)
		{
			Console.Log(Constants.kDebugTag, "[WebView] Received DidReceiveMessage event, Webview=" + _webview + " " + "Message=" + _message);

			if (_webview != null)
				_webview.InvokeMethod(kOnDidReceiveMessageEvent, _message);
		}

		#endregion

		#region Parse Methods

		protected virtual void ParseLoadErrorData (IDictionary _dataDict, out string _tag, out string _error)
		{
			_tag	= null;
			_error	= null;
		}

		protected virtual void ParseEvalJSData (IDictionary _resultData, out string _tag, out string _result)
		{
			_tag	= null;
			_result	= null;
		}
	
		protected virtual void ParseMessageData (IDictionary _dataDict, out string _tag, out WebViewMessage _message)
		{
			_tag		= null;
			_message	= null;
		}

		#endregion
	}
}