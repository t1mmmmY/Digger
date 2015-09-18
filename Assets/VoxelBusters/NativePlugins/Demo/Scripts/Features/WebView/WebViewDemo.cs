using UnityEngine;
using System.Collections;
using VoxelBusters.Utility.UnityGUI.MENU;
using VoxelBusters.Utility;
using VoxelBusters.NativePlugins;
using VoxelBusters.AssetStoreProductUtility.Demo;

namespace VoxelBusters.NativePlugins.Demo
{
	public class WebViewDemo : DemoSubMenu 
	{
		#region Properties

		[SerializeField]
		private string 				m_url;

		[SerializeField, Multiline(6)]
		private string				m_HTMLString;
		
		[SerializeField]
		private string				m_javaScript;
		
		[SerializeField]
		private string				m_evalString;

		[SerializeField]
		private string				m_URLSchemeName	= "unity";

		[SerializeField]
		private WebView				m_webview;

		#endregion

		#region Unity Methods

		protected override void OnEnable ()
		{
			base.OnEnable();

			// Set frame
			SetFrame();

			// Registering callbacks
			WebView.DidShowEvent						+= DidShowEvent;
			WebView.DidHideEvent						+= DidHideEvent;
			WebView.DidDestroyEvent						+= DidDestroyEvent;
			WebView.DidStartLoadEvent					+= DidStartLoadEvent;
			WebView.DidFinishLoadEvent					+= DidFinishLoadEvent;
			WebView.DidFailLoadWithErrorEvent			+= DidFailLoadWithErrorEvent;
			WebView.DidFinishEvaluatingJavaScriptEvent	+= DidFinishEvaluatingJavaScriptEvent;
			WebView.DidReceiveMessageEvent				+= DidReceiveMessageEvent;
		}

		protected override void OnDisable ()
		{
			base.OnDisable();

			// Deregistering callbacks
			WebView.DidShowEvent						-= DidShowEvent;
			WebView.DidHideEvent						-= DidHideEvent;
			WebView.DidDestroyEvent						-= DidDestroyEvent;
			WebView.DidStartLoadEvent					-= DidStartLoadEvent;
			WebView.DidFinishLoadEvent					-= DidFinishLoadEvent;
			WebView.DidFailLoadWithErrorEvent			-= DidFailLoadWithErrorEvent;
			WebView.DidFinishEvaluatingJavaScriptEvent	-= DidFinishEvaluatingJavaScriptEvent;
			WebView.DidReceiveMessageEvent				-= DidReceiveMessageEvent;
		}

		#endregion

		#region API Calls

		private void LoadRequest ()
		{
			m_webview.LoadRequest(m_url);
		}

		private void LoadHTMLString ()
		{
			m_webview.LoadHTMLString(m_HTMLString);
		}
		
		private void LoadHTMLStringWithJavaScript ()
		{
			m_webview.LoadHTMLStringWithJavaScript(m_HTMLString, m_javaScript);						
		}
		
		private void LoadFile ()
		{
			m_webview.LoadFile(Demo.Utility.GetScreenshotPath(), "image/png", null, null);
		}
		
		private void EvaluateJavaScriptFromString ()
		{
			m_webview.EvaluateJavaScriptFromString(m_evalString);
		}

		private void ShowWebView ()
		{
			m_webview.Show();
		}

		private void HideWebView ()
		{
			m_webview.Hide();
		}
		
		private void DestroyWebView ()
		{
			m_webview.Destroy();
		}

		private void AddNewURLSchemeName ()
		{
			m_webview.AddNewURLSchemeName(m_URLSchemeName);
		}

		private void ClearCache ()
		{
			m_webview.ClearCache();
		}

		private void SetFrame ()
		{
			m_webview.Frame	= new Rect(0f, Screen.height * 0.75f, Screen.width, Screen.height * 0.2f);
		}

		#endregion

		#region API Callbacks
		
		private void DidShowEvent (WebView _webview)
		{
			AddNewResult("Received Did Show Webview Event");
		}
		
		private void DidHideEvent (WebView _webview)
		{
			AddNewResult("Received Did Hide Webview Event");
		}
		
		private void DidDestroyEvent (WebView _webview)
		{
			AddNewResult("Received Did Destroy Webview Event");
		}
		
		private void DidStartLoadEvent (WebView _webview)
		{
			AddNewResult("Received Did Start Load Event");
		}
		
		private void DidFinishLoadEvent (WebView _webview)
		{
			AddNewResult("Received Did Finish Load Event");
		}
		
		private void DidFailLoadWithErrorEvent (WebView _webview, string _error)
		{
			AddNewResult("Received Did Fail To Load Event");
			AppendResult("Error= " + _error);
		}
		
		private void DidFinishEvaluatingJavaScriptEvent (WebView _webview, string _result)
		{
			AddNewResult("Received Did Finish Evaluating JS Event");
			AppendResult("Result= " + _result);
		}
		
		private void DidReceiveMessageEvent (WebView _webview,  WebViewMessage _message)
		{
			AddNewResult("Received Did Receive Message Event");
			AppendResult("Message= " + _message);
		}

		#endregion

		#region UI

		protected override void OnGUIWindow()
		{		
			base.OnGUIWindow();

			if (m_webview == null)
			{
				GUILayout.Label("Create WebView", kSubTitleStyle);
				
				if (GUILayout.Button("Create"))
				{
					GameObject _newWebviewGO	= new GameObject("WebView");
					m_webview					= _newWebviewGO.AddComponent<WebView>();
					
					AddNewResult("Successfully created new WebView.");
				}
				
				return;
			}

			RootScrollView.BeginScrollView();
			{
				DrawLoadAPI();
				DrawLifeCycleAPI();
				DrawPropertiesAPI();

				// Misc
				GUILayout.Label("Misc.", kSubTitleStyle);
				
				if (GUILayout.Button("AddNewURLSchemeName"))
				{
					AddNewURLSchemeName();
				}
				
				if (GUILayout.Button("ClearCache"))
				{		
					ClearCache();
				}
			}
			RootScrollView.EndScrollView();
			
			DrawResults();
			DrawPopButton();
		}

		private void DrawLoadAPI ()
		{
			GUILayout.Label("Load API's", kSubTitleStyle);
			
			if (GUILayout.Button("LoadRequest"))
			{
				LoadRequest();
			}
			
			if (GUILayout.Button("LoadHTMLString"))
			{
				LoadHTMLString();
			}
			
			if (GUILayout.Button("LoadHTMLStringWithJavaScript"))
			{
				LoadHTMLStringWithJavaScript();
			}
			
			if (GUILayout.Button("EvaluateJavaScript"))
			{
				EvaluateJavaScriptFromString();
			}
			
			if (GUILayout.Button("LoadFile"))
			{
				LoadFile();
			}
		}

		private void DrawLifeCycleAPI ()
		{
			GUILayout.Label("Lifecycle", kSubTitleStyle);
			
			if (GUILayout.Button("Show"))
			{		
				ShowWebView();
			}
			
			if (GUILayout.Button("Hide"))
			{		
				HideWebView();
			}
			
			if (GUILayout.Button("Destroy"))
			{		
				DestroyWebView();
			}
		}

		private void DrawPropertiesAPI ()
		{
			GUILayout.Label("Properties", kSubTitleStyle);
			
			GUILayout.BeginVertical(UISkin.scrollView);
			GUILayout.BeginHorizontal();
			
			bool _canHideNewValue				= GUILayout.Toggle(m_webview.CanHide, "CanHide");
			bool _canBounceNewValue				= GUILayout.Toggle(m_webview.CanBounce, "CanBounce");
			bool _showSpinnerOnLoadNewValue		= GUILayout.Toggle(m_webview.ShowSpinnerOnLoad, "ShowSpinnerOnLoad");
			
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal();
			
			bool _autoShowOnLoadFinishNewValue	= GUILayout.Toggle(m_webview.AutoShowOnLoadFinish, "AutoShowOnLoadFinish");
			bool _scalesPageToFitNewValue		= GUILayout.Toggle(m_webview.ScalesPageToFit, "ScalesPageToFit");
			
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			
			// Update the value only on value change
			if (_canHideNewValue != m_webview.CanHide)
				m_webview.CanHide				= _canHideNewValue;
			
			if (_canBounceNewValue != m_webview.CanBounce)
				m_webview.CanBounce				= _canBounceNewValue;
			
			if (_showSpinnerOnLoadNewValue != m_webview.ShowSpinnerOnLoad)
				m_webview.ShowSpinnerOnLoad		= _showSpinnerOnLoadNewValue;
			
			if (_autoShowOnLoadFinishNewValue != m_webview.AutoShowOnLoadFinish)
				m_webview.AutoShowOnLoadFinish	= _autoShowOnLoadFinishNewValue;
			
			if (_scalesPageToFitNewValue != m_webview.ScalesPageToFit)
				m_webview.ScalesPageToFit		= _scalesPageToFitNewValue;
			
			if (GUILayout.Button("SetFrame"))
			{		
				SetFrame();
			}
		}

		#endregion
	}
}