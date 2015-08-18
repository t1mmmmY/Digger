using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	/// <summary>
	/// TwitterSession represents a user's session authenticated with the Twitter API.
	/// </summary>
	public class TwitterSession 
	{
		#region Properties

		/// <summary>
		/// Gets a value indicating authorization token.
		/// </summary>
		/// <value>The auth token.</value>
		public string 		AuthToken 
		{ 
			get; 
			protected set; 
		}

		/// <summary>
		/// Gets a value indicating authorization token secret.
		/// </summary>
		/// <value>The auth token secret.</value>
		public string 		AuthTokenSecret 
		{ 
			get; 
			protected set; 
		}

		/// <summary>
		/// Gets a value indicating username associated with the access token.
		/// </summary>
		/// <value>The name of the user.</value>
		public string 		UserName 
		{ 
			get; 
			protected set; 
		}

		/// <summary>
		/// Gets a value indicating user ID associated with the access token.
		/// </summary>
		/// <value>The user ID.</value>
		public string 		UserID 
		{ 
			get; 
			protected set; 
		}
		
		#endregion
		
		#region Constructor
		
		protected TwitterSession ()
		{
			AuthToken		= string.Empty;
			AuthTokenSecret	= string.Empty;
			UserName		= string.Empty;
			UserID			= string.Empty;
		}
		
		#endregion

		#region Overriden Methods

		/// <summary>
		/// String representation of <see cref="TwitterSession"/>.
		/// </summary>
		public override string ToString ()
		{
			return string.Format("[TwitterSession: AuthToken={0}, AuthTokenSecret={1}, UserName={2}, UserID={3}]", 
			                     AuthToken, AuthTokenSecret, UserName, UserID);
		}
		
		#endregion
	}
}
