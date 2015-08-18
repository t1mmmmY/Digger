using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	/// <summary>
	/// TwitterUser represents a user on Twitter.
	/// </summary>
	public class TwitterUser  
	{
		#region Properties

		/// <summary>
		/// Gets a value indicating ID of the Twitter User.
		/// </summary>
		/// <value>The user ID.</value>
		public string 		UserID 
		{ 
			get; 
			protected set; 
		}

		/// <summary>
		/// Gets a value indicating name of the Twitter User.
		/// </summary>
		/// <value>The name.</value>
		public string 		Name 
		{ 
			get; 
			protected set; 
		}

		/// <summary>
		/// Gets a value indicating whether the user has been verified by Twitter.
		/// </summary>
		/// <value><c>true</c> if this user is verified; otherwise, <c>false</c>.</value>
		public bool			IsVerified  
		{ 
			get; 
			protected set; 
		}

		/// <summary>
		/// Gets a value indicating whether user is protected.
		/// </summary>
		/// <value><c>true</c> if this user is protected; otherwise, <c>false</c>.</value>
		public bool 		IsProtected  
		{ 
			get; 
			protected set; 
		}

		/// <summary>
		/// Gets a value indicating HTTPS URL of the user's profile image.
		/// </summary>
		/// <value>The profile image UR.</value>
		public string 		ProfileImageURL  
		{ 
			get; 
			protected set; 
		}

		#endregion

		#region Constructor

		protected TwitterUser ()
		{
			UserID					= string.Empty;
			Name					= string.Empty;
			IsVerified				= false;
			IsProtected				= false;
			ProfileImageURL			= string.Empty;
		}

		#endregion

		#region Overriden Methods

		/// <summary>
		/// String representation of <see cref="TwitterUser"/>.
		/// </summary>
		public override string ToString ()
		{
			return string.Format("[TwitterUser: UserID={0}, Name={1}, IsVerified={2}, IsProtected={3}, ProfileImageURL={4}]", 
			                     UserID, Name, IsVerified, IsProtected, ProfileImageURL);
		}
		
		#endregion
	}
}
