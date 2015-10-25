using UnityEngine;
using System.Collections;
using System.IO;
using VoxelBusters.NativePlugins;
using VoxelBusters.DebugPRO;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	/// <summary>
	/// Provides interface to embed web content in your application.
	/// </summary>
	/// <description>
	/// Package includes WebView prefab, which is placed under "Assets/VoxelBusters/NativePlugins/Prefab" folder.
	/// Drag and drop WebView prefab into heirarchy and send requests to display local content or content that is loaded from the network. 
	/// </description>	
	public class WebView : MonoBehaviour 
	{
		#region Fields

#pragma warning disable
		[SerializeField]
		private 	bool				m_canHide			= true;

		[SerializeField]
		private 	bool				m_canBounce			= true;

		[SerializeField]
		private 	eWebviewControlType	m_controlType;

		[SerializeField]
		private 	bool				m_showSpinnerOnLoad;

		[SerializeField]
		private 	bool				m_autoShowOnLoadFinish	= true;
		
		[SerializeField]
		private 	bool				m_scalesPageToFit	= true;
		
		[SerializeField]
		private 	Rect				m_frame				= new Rect(0f, 0f, -1f, -1f);
		
		[SerializeField]
		private 	Color				m_backgroundColor	= Color.white;
#pragma warning restore

		#endregion

#if !USES_WEBVIEW
		#region Properties

		public string UniqueID 
		{ 
			get; 
			private set; 
		}

		#endregion
	}
#else
		#region Properties
		
		public string UniqueID 
		{ 
			get; 
			private set; 
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="WebView"/> can hide on pressing dismiss button.
		/// </summary>
		/// <value><c>true</c> if this instance can hide; otherwise, <c>false</c>.</value>
		public bool CanHide
		{
			get 
			{ 
				return m_canHide; 
			}

			set 
			{ 
				m_canHide	= value; 

				// Native webview call
				if (NPBinding.WebView != null)
					NPBinding.WebView.SetCanHide(value, this);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="WebView"/> bounces past the edge of content and back again.
		/// </summary>
		/// <value><c>true</c> if this instance can bounce; otherwise, <c>false</c>.</value>
		public bool CanBounce
		{
			get 
			{ 
				return m_canBounce; 
			}

			set 
			{ 
				m_canBounce		= value; 
				
				// Native webview call
				if (NPBinding.WebView != null)
					NPBinding.WebView.SetCanBounce(value, this);
			}
		}

		/// <summary>
		/// Gets or sets the type of the control shown along with <see cref="WebView"/>.
		/// </summary>
		/// <value>The type of the control.</value>
		public eWebviewControlType ControlType
		{
			get 
			{
				return m_controlType; 
			}

			set 
			{ 
				m_controlType	= value; 
				
				// Native webview call
				if (NPBinding.WebView != null)
					NPBinding.WebView.SetControlType(m_controlType, this);
			}
		}
		
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="WebView"/> shows spinner when loading requests.
		/// </summary>
		/// <value><c>true</c> if show spinner on load; otherwise, <c>false</c>.</value>
		public bool ShowSpinnerOnLoad
		{
			get 
			{ 
				return m_showSpinnerOnLoad; 
			}
			
			set 
			{ 
				m_showSpinnerOnLoad	= value; 
				
				// Native webview call
				if (NPBinding.WebView != null)
					NPBinding.WebView.SetShowSpinnerOnLoad(value, this);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="WebView"/> can autoshow itself when load request is finished.
		/// </summary>
		/// <value><c>true</c> if auto show on load finish; otherwise, <c>false</c>.</value>
		public bool AutoShowOnLoadFinish
		{
			get
			{ 
				return m_autoShowOnLoadFinish; 
			}

			set 
			{ 
				m_autoShowOnLoadFinish	= value; 
				
				// Native webview call
				if (NPBinding.WebView != null)
					NPBinding.WebView.SetAutoShowOnLoadFinish(value, this);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="WebView"/> scales webpages to fit the view and the user can change the scale.
		/// </summary>
		/// <value><c>true</c> if scales page to fit; otherwise, <c>false</c>.</value>
		public bool ScalesPageToFit
		{
			get 
			{
				return m_scalesPageToFit; 
			}

			set 
			{ 
				m_scalesPageToFit		= value; 
				
				// Native webview call
				if (NPBinding.WebView != null)
					NPBinding.WebView.SetScalesPageToFit(value, this);
			}
		}

		/// <summary>
		/// Gets or sets the rectangle, which describes this <see cref="WebView"/> position and size.
		/// </summary>
		/// <value>The frame.</value>
		public Rect Frame
		{
			get 
			{ 
				return m_frame; 
			}

			set 
			{ 
				m_frame	= value; 

				// Incase if user forgets to set width
				if (m_frame.width == -1f)
					m_frame.width	= Screen.width;

				// Incase if user forgets to set height
				if (m_frame.height	== -1f)
					m_frame.height	= Screen.height;
				
				// Native webview call
				if (NPBinding.WebView != null)
					NPBinding.WebView.SetFrame(value, this);
			}
		}

		/// <summary>
		/// Gets or sets the color of the background.
		/// </summary>
		/// <value>The color of the background.</value>
		public Color BackgroundColor
		{
			get 
			{ 
				return m_backgroundColor; 
			}

			set 
			{ 
				m_backgroundColor	= value; 
				
				// Native webview call
				if (NPBinding.WebView != null)
					NPBinding.WebView.SetBackgroundColor(value, this);
			}
		}

		#endregion

		#region Events

		/// <summary>
		/// Generic  web view delegate.
		/// </summary>
		/// <param name="_webview">The webview that is responsible for this event.</param>
		public delegate void WebViewEvent (WebView _webview);

		/// <summary>
		/// Use this delegate type to get callback when <see cref="WebView"/> fails to load requested content.
		/// </summary>
		/// <param name="_webview">The webview that failed to load.</param>
		/// <param name="_error">The error that occurred during loading.</param>
		public delegate void WebViewFailedLoad (WebView _webview, string _error);

		/// <summary>
		/// Use this delegate type to get callback when <see cref="WebView"/> finishes running a script.
		/// </summary>
		/// <param name="_webview">The webview that finshed running input script .</param>
		/// <param name="_result">The result of running script or NULL if it fails.</param>
		public delegate void WebViewFinishedEvaluatingJS (WebView _webview, string _result);

		/// <summary>
		/// Use this delegate type to get callback when <see cref="WebView"/> sends messages to unity using URL scheme.
		/// </summary>
		/// <param name="_webview">The webview that received message.</param>
		/// <param name="_message">Message that was received from webview.</param>
		public delegate void WebViewReceivedMessage (WebView _webview, WebViewMessage _message);
	
		#endregion

		#region Events

		/// <summary>
		/// Occurs when <see cref="WebView"/> is shown.
		/// </summary>
		public static event WebViewEvent 					DidShowEvent;

		/// <summary>
		/// Occurs when <see cref="WebView"/> is hide.
		/// </summary>
		public static event WebViewEvent 					DidHideEvent;

		/// <summary>
		/// Occurs when <see cref="WebView"/> is destroyed.
		/// </summary>
		public static event WebViewEvent 					DidDestroyEvent;

		/// <summary>
		/// Occurs when <see cref="WebView"/> starts loading.
		/// </summary>
		public static event WebViewEvent 					DidStartLoadEvent;

		/// <summary>
		/// Occurs when <see cref="WebView"/> finishes loading.
		/// </summary>
		public static event WebViewEvent 					DidFinishLoadEvent;

		/// <summary>
		/// Occurs when <see cref="WebView"/> failed to load.
		/// </summary>
		public static event WebViewFailedLoad				DidFailLoadWithErrorEvent;

		/// <summary>
		/// Occurs when <see cref="WebView"/> finishes running a script.
		/// </summary>
		public static event WebViewFinishedEvaluatingJS 	DidFinishEvaluatingJavaScriptEvent;

		/// <summary>
		/// Occurs when <see cref="WebView"/> sends a message to unity by parsing URL scheme.
		/// </summary>
		/// <description>
		/// We make use of URL schemes, inorder to send messages from webview to unity. 
		/// Use <see cref="AddNewURLScheme"/> to add the scheme strings.
		/// <see cref="WebView"/> will listen only to these added schemes.
		/// Whenever webview starts to load requests which starts with added scheme, then URL is parsed as <see cref="WebViewMessage"/> and will raise this event.
		/// </description>
		public static event WebViewReceivedMessage	 		DidReceiveMessageEvent;

		#endregion

		#region Unity Methods

		private void Awake ()
		{			
			// Assign unique id
			UniqueID				= GetInstanceID().ToString();
			
			//Consider updating with predefined constants.
			if (m_frame.width 	==	-1f)
				m_frame.width	= Screen.width;
			
			if (m_frame.height	==	-1f)
				m_frame.height	= Screen.height;
			
			// Create webview
			NPBinding.WebView.Create(this, m_frame);
			
			// Set properties
			CanHide					= m_canHide;
			CanBounce				= m_canBounce;
			ControlType				= m_controlType; 
			ShowSpinnerOnLoad		= m_showSpinnerOnLoad;
			AutoShowOnLoadFinish	= m_autoShowOnLoadFinish;
			ScalesPageToFit			= m_scalesPageToFit;
			BackgroundColor			= m_backgroundColor;
		}

		private void OnDestroy ()
		{
			// Destroys webview
			if (NPBinding.Instance != null)
				NPBinding.WebView.Destroy(this);
		}

		#endregion


		#region Webview Lifecycle

		/// <summary>
		/// Destroys this instance.
		/// </summary>
		public void Destroy ()
		{
			Destroy(gameObject);
		}

		/// <summary>
		/// Show this instance.
		/// </summary>
		public void Show ()
		{
			if (NPBinding.WebView != null)
				NPBinding.WebView.Show(this);
		}

		/// <summary>
		/// Hide this instance.
		/// </summary>
		public void Hide ()
		{
			if (NPBinding.WebView != null)
				NPBinding.WebView.Hide(this);
		}

		#endregion

		#region Load API's

		/// <summary>
		/// Connects to a given URL by initiating an asynchronous request.
		/// </summary>
		/// <param name="_URL">URL of the content to load.</param>
		public void LoadRequest (string _URL)
		{
			if (string.IsNullOrEmpty(_URL) || !(_URL.StartsWith("http://") || _URL.StartsWith("https://")))
			{
				Console.LogError(Constants.kDebugTag, "[WebView] Load request failed, please use a valid URL");
				return;
			}

			if (NPBinding.WebView != null)
				NPBinding.WebView.LoadRequest(_URL, this);
		}

		/// <summary>
		/// Loads the HTML string contents of file.
		/// </summary>
		/// <param name="_HTMLFilePath">Path to the file with HTML contents.</param>
		/// <param name="_baseURL">Base URL for the content.</param>
		public void LoadHTMLStringContentsOfFile (string _HTMLFilePath, string _baseURL) 
		{
			// Check if file exists
			if (!File.Exists(_HTMLFilePath))
			{
				Console.LogError(Constants.kDebugTag, "[WebView] File doesnt exist, _HTMLFilePath=" + _HTMLFilePath);
				return;
			}
			
			// Load
			LoadHTMLString(FileOperations.ReadAllText(_HTMLFilePath), _baseURL);
		}

		/// <summary>
		/// Loads the HTML string with javascript.
		/// </summary>
		/// <param name="_HTMLString">HTML content.</param>
		/// <param name="_javaScript">Javascript to run.</param>
		/// <param name="_baseURL">Base URL for the content.</param>
		public void LoadHTMLStringWithJavaScript (string _HTMLString, string _javaScript, string _baseURL = null)
		{
			// Invalid HTML string
			if (string.IsNullOrEmpty(_HTMLString))
			{
				LoadHTMLString(_HTMLString, _baseURL);
				return;
			}

			// Injecting javascript to html string
			string _HTMLStringWithJS	= _HTMLString;

			if (_javaScript != null)
				_HTMLStringWithJS	+= _javaScript;

			// Load
			LoadHTMLString(_HTMLStringWithJS, _baseURL);
		}

		/// <summary>
		/// Loads the HTML string.
		/// </summary>
		/// <param name="_HTMLString">HTML content.</param>
		/// <param name="_baseURL">Base URL for the content.</param>
		public void LoadHTMLString (string _HTMLString, string _baseURL = null)
		{
			// Invalid HTML string
			if (string.IsNullOrEmpty(_HTMLString))
			{
				Console.LogError(Constants.kDebugTag, "[WebView] Failed to load HTML contents, HTMLString=" + _HTMLString);
				return;
			}

			if (NPBinding.WebView != null)
				NPBinding.WebView.LoadHTMLString(_HTMLString, _baseURL, this);
		}

		/// <summary>
		/// Loads the file.
		/// </summary>
		/// <param name="_filepath">Path to the file to be loaded.</param>
		/// <param name="_MIMEType">MIME type of the content.</param>
		/// <param name="_textEncodingName">IANA encoding name as in utf-8 or utf-16.</param>
		/// <param name="_baseURL">Base URL for the content.</param>
		public void LoadFile (string _filepath, string _MIMEType, string _textEncodingName, string _baseURL)
		{
			// Check if file exists
			if (!File.Exists(_filepath))
			{
				Console.LogError(Constants.kDebugTag, "[WebView] File doesnt exist, Path=" + _filepath);
				return;
			}

			// Load
			LoadData(FileOperations.ReadAllBytes(_filepath), _MIMEType, _textEncodingName, _baseURL);
		}

		/// <summary>
		/// Loads the data contents.
		/// </summary>
		/// <param name="_byteArray">File contents as byte array.</param>
		/// <param name="_MIMEType">MIME type of the content.</param>
		/// <param name="_textEncodingName">IANA encoding name as in utf-8 or utf-16.</param>
		/// <param name="_baseURL">Base URL for the content.</param>
		public void LoadData (byte[] _byteArray, string _MIMEType, string _textEncodingName, string _baseURL)
		{
			if (_byteArray == null)
			{
				Console.LogError(Constants.kDebugTag, "[WebView] Load data failed");
				return;
			}

			if (NPBinding.WebView != null)
				NPBinding.WebView.LoadData(_byteArray, _MIMEType, _textEncodingName, _baseURL, this);
		}

		/// <summary>
		/// Evaluates the java script from string.
		/// </summary>
		/// <param name="_javaScript">Javascript to run.</param>
		public void EvaluateJavaScriptFromString (string _javaScript)
		{
			if (NPBinding.WebView != null)
				NPBinding.WebView.EvaluateJavaScriptFromString(_javaScript, this);
		}

		/// <summary>
		/// Reloads the current page.
		/// </summary>
		public void Reload ()
		{
			if (NPBinding.WebView != null)
				NPBinding.WebView.Reload(this);
		}

		/// <summary>
		/// Stops the loading of web content.
		/// </summary>
		public void StopLoading ()
		{
			if (NPBinding.WebView != null)
				NPBinding.WebView.StopLoading(this);
		}

		#endregion

		#region URL Scheme

		/// <summary>
		/// Adds the new URL scheme name to which <see cref="WebView"/> will listen.
		/// </summary>
		/// <param name="_URLSchemeName">URL scheme name.</param>
		/// <description>
		/// It is possible to send messages from webview to unity by using URL schemes.
		/// <see cref="WebView"/> will listen only to these added schemes.
		/// Whenever webview starts to load requests which starts with added scheme, then URL is parsed as <see cref="WebViewMessage"/> and will raise <see cref="WebViewReceivedMessage"/>.
		/// </description>
		public void AddNewURLSchemeName (string _URLSchemeName)
		{
			if (string.IsNullOrEmpty(_URLSchemeName))
			{
				Console.LogError(Constants.kDebugTag, "[WebView] Failed to add URL scheme");
				return;
			}

			if (NPBinding.WebView != null)
				NPBinding.WebView.AddNewURLSchemeName(_URLSchemeName, this);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Sets the <see cref="WebView"/> frame size to screen bounds.
		/// </summary>
		public void SetFullScreenFrame ()
		{
			Frame	= new Rect(0f, 0f, Screen.width, Screen.height);
		}

		#endregion

		#region Cache Clearence

		/// <summary>
		/// Clears all stored cached URL responses.
		/// </summary>
		public void ClearCache ()
		{
			if (NPBinding.WebView != null)
				NPBinding.WebView.ClearCache();
		}

		#endregion

		#region Event Callback Methods

		private void OnDidShow ()
		{
			if (DidShowEvent != null)
				DidShowEvent(this);
		}
		
		private void OnDidHide ()
		{
			if (DidHideEvent != null)
				DidHideEvent(this);
		}

		private void OnDidDestroy ()
		{
			if (DidDestroyEvent != null)
				DidDestroyEvent(this);
		}
		
		private void OnDidStartLoad ()
		{
			if (DidStartLoadEvent != null)
				DidStartLoadEvent(this);
		}
		
		private void OnDidFinishLoad ()
		{
			if (DidFinishLoadEvent != null)
				DidFinishLoadEvent(this);
		}

		private void OnDidFailLoadWithError (string _error)
		{
			if (DidFailLoadWithErrorEvent != null)
				DidFailLoadWithErrorEvent(this, _error);
		}

		private void OnDidFinishEvaluatingJavaScript (string _result)
		{
			if (DidFinishEvaluatingJavaScriptEvent != null)
				DidFinishEvaluatingJavaScriptEvent(this,_result);
		}

		private void OnDidReceiveMessage (WebViewMessage _message)
		{
			if (DidReceiveMessageEvent != null)
				DidReceiveMessageEvent(this, _message);
		}

		#endregion
	}
#endif
}