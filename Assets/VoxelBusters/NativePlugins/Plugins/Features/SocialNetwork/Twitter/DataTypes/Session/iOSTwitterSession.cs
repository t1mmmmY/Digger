using UnityEngine;
using System.Collections;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins.Internal
{
	public sealed class iOSTwitterSession : TwitterSession 
	{
//		{
//			"user-ID": "612693",
//			"user-name": "itsjoey",
//			"auth-token": "61263-zXHyy1qcZVhk4cA2PIT5XqYmPBXAu8MypxUpkRj",
//			"auth-token-secret": "km6BstbUQ7acK1ht5XIO66MNmG13UHthPNZNGYCEyr2"
//		}

		#region Constants
		
		private const string	kUserID				= "user-ID";
		private const string	kAuthToken			= "auth-token";
		private const string	kUserName			= "user-name";
		private const string	kAuthTokenSecret	= "auth-token-secret";
		
		#endregion

		#region Constructor
		
		public iOSTwitterSession (IDictionary _sessionJsonDict)
		{
			AuthToken		= _sessionJsonDict.GetIfAvailable<string>(kAuthToken);
			AuthTokenSecret	= _sessionJsonDict.GetIfAvailable<string>(kAuthTokenSecret);
			UserName		= _sessionJsonDict.GetIfAvailable<string>(kUserName);
			UserID			= _sessionJsonDict.GetIfAvailable<string>(kUserID);
		}
		
		#endregion
	}
}
