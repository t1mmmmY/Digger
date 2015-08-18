using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins
{
	/// <summary>
	/// WebViewMessage is a wrapped data which is sent from <see cref="WebView"/> to Unity.
	/// </summary>
	public class WebViewMessage  
	{
		#region Properties

		/// <summary>
		/// Gets the scheme name.
		/// </summary>
		/// <value>The scheme name.</value>
		public string SchemeName 						
		{ 
			get; 
			protected set; 
		}

		/// <summary>
		/// Gets the host name.
		/// </summary>
		/// <value>The host name.</value>
		public string Host	 							
		{ 
			get; 
			protected set; 
		}

		/// <summary>
		/// Gets the arguments. Each key-value pair represents name and value of the argument
		/// </summary>
		/// <value>The arguments.</value>
		public Dictionary<string, string> Arguments  	
		{ 
			get; 
			protected set; 
		}

		#endregion

		#region Constructor

		protected WebViewMessage ()
		{
			SchemeName	= null;
			Host		= null;
			Arguments	= null;
		}

		#endregion
		
		#region Overriden Methods
		
		public override string ToString ()
		{
			return string.Format("[WebViewMessage SchemeName={0}, Host={1}, Arguments={2}]", 
			                     SchemeName, Host, Arguments.ToJSON());
		}
		
		#endregion
	}
}