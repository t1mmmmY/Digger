using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins.Internal
{
	public sealed class AndroidTwitterSession : TwitterSession 
	{
		#region Constants
		
		private const string	kUserID				= "user-identifier";
		private const string	kAuthToken			= "auth-token";
		private const string	kUserName			= "user-name";
		private const string	kAuthTokenSecret	= "auth-token-secret";
		
		#endregion

		#region Constructor

		public AndroidTwitterSession (IDictionary _sessionJsonDict)
		{
			AuthToken		= _sessionJsonDict[kAuthToken] as string;
			AuthTokenSecret	= _sessionJsonDict[kAuthTokenSecret] as string;
			UserName		= _sessionJsonDict[kUserName] as string;
			UserID			= _sessionJsonDict[kUserID] as string;
		}
		
		#endregion
	}
}
