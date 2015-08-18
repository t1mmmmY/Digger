using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	/// <summary>
	/// Specifies the types of control available for webview.
	/// </summary>
	public enum eWebviewControlType
	{
		/// <summary>
		/// No controls are shown for <see cref="WebView"/>. Fits well for banner ads.
		/// </summary>
		NO_CONTROLS,

		/// <summary>
		/// Close button is shown at top-right corner of the webview. 
		/// Note that <see cref="WebView"/> is hide on pressing this button. 
		/// Please call Destroy, to permanetly remove webview instance.
		/// </summary>
		CLOSE_BUTTON,

		/// <summary>
		/// Toolbar consists of 4 buttons Back, Forward, Reload, Done
		/// Back and Forward buttons allows the user to move back and forward through the webpage history.
		/// Reload button allows user to reload the webpage contents.
		/// Done buttons allows user to hide <see cref="WebView"/>.
		/// </summary>
		TOOLBAR
	}
}
