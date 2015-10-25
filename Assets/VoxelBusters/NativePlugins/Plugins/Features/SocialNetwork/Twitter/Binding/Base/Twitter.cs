using UnityEngine;
using System.Collections;

#if USES_TWITTER
using VoxelBusters.Utility;
using VoxelBusters.DebugPRO;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	/// <summary>
	/// Provides support for sending Twitter requests on behalf of the user and for composing and sending tweets. 
	/// </summary>
	public partial class Twitter : MonoBehaviour 
	{
		#region Init API
		/// <summary>
		/// Initialise Twitter feature.
		/// </summary>
		/// <description>
		/// Make sure this is called before using twitter functions. Returns true on proper initialisation.
		/// </description>

		public virtual bool Initialise ()
		{
			TwitterSettings _twitterSettings	= NPSettings.SocialNetworkSettings.TwitterSettings;
			
			if (string.IsNullOrEmpty(_twitterSettings.ConsumerKey) || string.IsNullOrEmpty(_twitterSettings.ConsumerSecret))
			{
				Console.LogError(Constants.kDebugTag, "[Twitter] Twitter initialize failed. Please configure Consumer Key and Consumer Secret in NPSettings.");
				return false;
			}

			return true;
		}

		#endregion
		
		#region Account API's

		/// <summary>
		/// Triggers user authentication with Twitter.
		/// </summary>
		/// <param name="_onCompletion">Calls the delegate after authentication is successful or if there is an error.</param>
		/// <description>
		/// This method will present UI to allow the user to log in if there are no saved Twitter login credentials.
		/// </description>
		public virtual void Login (TWTRLoginCompletion _onCompletion)
		{
			// Pause unity player
			this.PauseUnity();

			// Cache callback
			OnLoginFinished	= _onCompletion;
		}

		/// <summary>
		/// Deletes the local Twitter user session from this app.
		/// </summary>
		public virtual void Logout ()
		{}

		/// <summary>
		/// Determines whether user is logged in or not.
		/// </summary>
		/// <returns><c>true</c> if user is logged in; otherwise, <c>false</c>.</returns>
		public virtual bool IsLoggedIn ()
		{
			bool _isLoggedIn	= false;
			Console.Log(Constants.kDebugTag, "[Twitter] IsLoggedIn=" + _isLoggedIn);
			
			return _isLoggedIn;
		}

		/// <summary>
		/// Gets the authorization token. Returns NULL if user isnt logged in.
		/// </summary>
		/// <returns>The authorization token.</returns>
		public virtual string GetAuthToken ()
		{
			string _authToken	= null;
			Console.Log(Constants.kDebugTag, "[Twitter] AuthToken=" + _authToken);

			return _authToken;
		}

		/// <summary>
		/// Gets the authorization token secret. Returns NULL if user isnt logged in.
		/// </summary>
		/// <returns>The authorization token secret.</returns>
		public virtual string GetAuthTokenSecret ()
		{
			string _authTokenSecret	= null;
			Console.Log(Constants.kDebugTag, "[Twitter] AuthTokenSecret=" + _authTokenSecret);
			
			return _authTokenSecret;
		}

		/// <summary>
		/// Gets the user ID of twitter user. Returns NULL if user isnt logged in.
		/// </summary>
		/// <returns>The user I.</returns>
		public virtual string GetUserID ()
		{
			string _userID	= null;
			Console.Log(Constants.kDebugTag, "[Twitter] UserID=" + _userID);
			
			return _userID;
		}

		/// <summary>
		/// Gets the name of the twitter user. Returns NULL if user isnt logged in.
		/// </summary>
		/// <returns>The user name.</returns>
		public virtual string GetUserName ()
		{
			string _userName	= null;
			Console.Log(Constants.kDebugTag, "[Twitter] UserName=" + _userName);
			
			return _userName;
		}
		
		#endregion
		
		#region Tweet API's

		/// <summary>
		/// Presents composer to tweet text.
		/// </summary>
		/// <param name="_message">The text to tweet.</param>
		/// <param name="_onCompletion">Calls the delegate when the user finished or cancelled the Tweet composition.</param>
		public void ShowTweetComposerWithMessage (string _message, TWTRTweetCompletion _onCompletion)
		{
			ShowTweetComposer(_message, null, null, _onCompletion);
		}
		
		/// <summary>
		/// Presents composer to tweet text along with screenshot.
		/// </summary>
		/// <param name="_message">The text to tweet.</param>
		/// <param name="_onCompletion">Calls the delegate when the user finished or cancelled the Tweet composition.</param>
		public void ShowTweetComposerWithScreenshot (string _message, TWTRTweetCompletion _onCompletion)
		{
			// First capture screenshot, once its done tweet about it
			StartCoroutine(TextureExtensions.TakeScreenshot((_texture)=>{
				
				ShowTweetComposerWithImage(_message, _texture, _onCompletion);
			}));
		}

		/// <summary>
		/// Presents composer to tweet text along with attached image.
		/// </summary>
		/// <param name="_message">The text to tweet.</param>
		/// <param name="_texture">The image to attached along with tweet message.</param>
		/// <param name="_onCompletion">Calls the delegate when the user finished or cancelled the Tweet composition.</param>
		public void ShowTweetComposerWithImage (string _message, Texture2D _texture, TWTRTweetCompletion _onCompletion)
		{
			byte[] _imgByteArray	= null;

			// Convert texture into byte array
			if (_texture != null)
			{
				_imgByteArray = _texture.EncodeToPNG();
			}
			else
			{
				Console.LogWarning(Constants.kDebugTag, "[Twitter] Showing tweet composer with message only, texure is null");
			}

			// Show tweet composer
			ShowTweetComposer(_message, null, _imgByteArray, _onCompletion);
		}

		/// <summary>
		/// Presents composer to tweet text along with URL.
		/// </summary>
		/// <param name="_message">The text to tweet.</param>
		/// <param name="_URL">URL to be shared.</param>
		/// <param name="_onCompletion">Calls the delegate when the user finished or cancelled the Tweet composition.</param>
		public void ShowTweetComposerWithLink (string _message, string _URL, TWTRTweetCompletion _onCompletion)
		{
			if (string.IsNullOrEmpty(_URL))
			{
				Console.LogWarning(Constants.kDebugTag, "[Twitter] Showing tweet composer with message only, URL is null/empty");
			}

			// Show tweet composer
			ShowTweetComposer(_message, _URL, null, _onCompletion);
		}
		
		/// <summary>
		/// Presents composer to tweet text along with URL and image.
		/// </summary>
		/// <param name="_message">The text to tweet.</param>
		/// <param name="_URL">URL to be shared.</param>
		/// <param name="_imgByteArray">The image data to be attached along with the tweet.</param>
		/// <param name="_onCompletion">Calls the delegate when the user finished or cancelled the Tweet composition.</param>
		public virtual void ShowTweetComposer (string _message, string _URL, byte[] _imgByteArray, TWTRTweetCompletion _onCompletion)
		{
			// Pause unity player
			this.PauseUnity();

			// Cache callback
			OnTweetComposerClosed	=	_onCompletion;
		}

		#endregion

		#region Request API's

		/// <summary>
		/// Sends request to load users account details.
		/// </summary>
		/// <param name="_onCompletion">Calls the delegate when load user details request succeeds or fails.</param>
		public virtual void RequestAccountDetails (TWTRAccountDetailsCompletion _onCompletion)
		{
			// Cache callback
			OnRequestAccountDetailsFinished	= _onCompletion;
		}

		/// <summary>
		/// Requests user to provide access to his email address.
		/// </summary>
		/// <param name="_onCompletion">Calls the delegate when user either accepts or denies access to their email address.</param>
		public virtual void RequestEmailAccess (TWTREmailAccessCompletion _onCompletion)
		{
			// Pause unity player
			this.PauseUnity();

			// Cache callback
			OnRequestEmailAccessFinished	= _onCompletion;
		}

		/// <summary>
		/// Sends a Twitter GET request.
		/// </summary>
		/// <param name="_URL">Request URL. This is the full Twitter API URL. E.g. https://api.twitter.com/1.1/statuses/user_timeline.json.</param>
		/// <param name="_parameters">Request parameters.</param>
		/// <param name="_onCompletion">Calls the delegate on response.</param>
		public void GetURLRequest (string _URL,	IDictionary _parameters, TWTRResonse _onCompletion)
		{
			URLRequest("GET", _URL, _parameters, _onCompletion);
		}

		/// <summary>
		/// Sends a Twitter POST request.
		/// </summary>
		/// <param name="_URL">Request URL. This is the full Twitter API URL. E.g. https://api.twitter.com/1.1/statuses/user_timeline.json.</param>
		/// <param name="_parameters">Request parameters.</param>
		/// <param name="_onCompletion">Calls the delegate on response.</param>
		public void PostURLRequest (string _URL,	IDictionary _parameters, TWTRResonse _onCompletion)
		{
			URLRequest("POST", _URL, _parameters, _onCompletion);
		}

		/// <summary>
		/// Sends a Twitter PUT request.
		/// </summary>
		/// <param name="_URL">Request URL. This is the full Twitter API URL. E.g. https://api.twitter.com/1.1/statuses/user_timeline.json.</param>
		/// <param name="_parameters">Request parameters.</param>
		/// <param name="_onCompletion">Calls the delegate on response.</param>
		public void PutURLRequest (string _URL,	IDictionary _parameters, TWTRResonse _onCompletion)
		{
			URLRequest("PUT", _URL, _parameters, _onCompletion);
		}

		/// <summary>
		/// Sends a Twitter DELETE request.
		/// </summary>
		/// <param name="_URL">Request URL. This is the full Twitter API URL. E.g. https://api.twitter.com/1.1/statuses/user_timeline.json.</param>
		/// <param name="_parameters">Request parameters.</param>
		/// <param name="_onCompletion">Calls the delegate on response.</param>
		public void DeleteURLRequest (string _URL,	IDictionary _parameters, TWTRResonse _onCompletion)
		{
			URLRequest("DELETE", _URL, _parameters, _onCompletion);
		}
		
		protected virtual void URLRequest (string _methodType, string _URL,	IDictionary _parameters, TWTRResonse _onCompletion)
		{
			// Cache callback
			OnTwitterURLRequestFinished	= _onCompletion;
		}

		#endregion
	}
}
#endif