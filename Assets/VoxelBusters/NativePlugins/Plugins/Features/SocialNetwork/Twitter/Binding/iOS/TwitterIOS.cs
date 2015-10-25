using UnityEngine;
using System.Collections;

#if USES_TWITTER && UNITY_IOS
using System.Runtime.InteropServices;
using VoxelBusters.Utility;
using VoxelBusters.DebugPRO;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	public partial class TwitterIOS : Twitter
	{
		#region Native Methods

		[DllImport("__Internal")]
		private static extern void initTwitterKit (string _consumerKey, string _consumerSecret);
		
		[DllImport("__Internal")]
		private static extern void twitterLogin ();
		
		[DllImport("__Internal")]
		private static extern void twitterLogout ();
		
		[DllImport("__Internal")]
		private static extern bool twitterIsLoggedIn ();
		
		[DllImport("__Internal")]
		private static extern string twitterGetAuthToken ();
		
		[DllImport("__Internal")]
		private static extern string twitterGetAuthTokenSecret ();
		
		[DllImport("__Internal")]
		private static extern string twitterGetUserID ();
		
		[DllImport("__Internal")]
		private static extern string twitterGetUserName ();
		
		[DllImport("__Internal")]
		private static extern void showTweetComposer (string _message, string _URLString, byte[] _imgByteArray, int _imgByteArrayLength);
		
		[DllImport("__Internal")]
		private static extern void twitterRequestAccountDetails ();
		
		[DllImport("__Internal")]
		private static extern void twitterRequestEmailAccess ();
		
		[DllImport("__Internal")]
		private static extern void twitterURLRequest (string _methodType, string _URLString, string _parameters);

		#endregion

		#region Init API's

		public override bool Initialise ()
		{
			if (base.Initialise())
			{
				// Get twitter settings info
				TwitterSettings _twitterSettings	= NPSettings.SocialNetworkSettings.TwitterSettings;
				
				// Initalize twitter component
				initTwitterKit(_twitterSettings.ConsumerKey, _twitterSettings.ConsumerSecret);

				return true;
			}

			return false;
		}

		#endregion

		#region Account API's

		public override void Login (TWTRLoginCompletion _onCompletion)
		{
			base.Login(_onCompletion);

			// Native method is called
			twitterLogin();
		}
		
		public override void Logout ()
		{
			// Native method is called
			twitterLogout();
		}
		
		public override bool IsLoggedIn ()
		{
			bool _isLoggedIn	= twitterIsLoggedIn();
			Console.Log(Constants.kDebugTag, "[Twitter] IsLoggedIn=" + _isLoggedIn);
			
			return _isLoggedIn;
		}
		
		public override string GetAuthToken ()
		{	
			string _authToken	= twitterGetAuthToken();
			Console.Log(Constants.kDebugTag, "[Twitter] AuthToken=" + _authToken);
			
			return _authToken;
		}
		
		public override string GetAuthTokenSecret ()
		{
			string _authTokenSecret	= twitterGetAuthTokenSecret();
			Console.Log(Constants.kDebugTag, "[Twitter] AuthTokenSecret=" + _authTokenSecret);
			
			return _authTokenSecret;
		}
		
		public override string GetUserID ()
		{
			string _userID	= twitterGetUserID();
			Console.Log(Constants.kDebugTag, "[Twitter] UserID=" + _userID);
			
			return _userID;
		}
		
		public override string GetUserName ()
		{
			string _userName	= twitterGetUserName();
			Console.Log(Constants.kDebugTag, "[Twitter] UserName=" + _userName);
			
			return _userName;
		}

		#endregion

		#region Tweet API's

		public override void ShowTweetComposer (string _message, string _URL, byte[] _imgByteArray, TWTRTweetCompletion _onCompletion)
		{
			base.ShowTweetComposer(_message, _URL, _imgByteArray, _onCompletion);

			// Get byte array length
			int _arrayLength	= 0;

			if (_imgByteArray != null)
				_arrayLength	= _imgByteArray.Length;
			
			// Native method is called
			showTweetComposer(_message, _URL, _imgByteArray, _arrayLength);
		}
		
		#endregion
		
		#region Request API's
		
		public override void RequestAccountDetails (TWTRAccountDetailsCompletion _onCompletion)
		{
			base.RequestAccountDetails(_onCompletion);

			// Native method is called
			twitterRequestAccountDetails();
		}
		
		public override void RequestEmailAccess (TWTREmailAccessCompletion _onCompletion)
		{
			base.RequestEmailAccess(_onCompletion);

			// Native method is called
			twitterRequestEmailAccess();
		}
		
		protected override void URLRequest (string _methodType, string _URL, IDictionary _parameters, TWTRResonse _onCompletion)
		{
			base.URLRequest(_methodType, _URL, _parameters, _onCompletion);

			// Native method is called
			twitterURLRequest(_methodType, _URL, _parameters.ToJSON());
		}
		
		#endregion
	}
}
#endif