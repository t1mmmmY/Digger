using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins.Demo
{
#if !USES_TWITTER
	public class TwitterDemo : NPDisabledFeatureDemo 
#else
	public class TwitterDemo : NPDemoBase
#endif
	{
		#region Properties

#pragma warning disable
		[SerializeField]
		private 	string 		m_shareMessage	= "this is what I wanted to share";
		[SerializeField]
		private 	string 		m_shareURL		= "http://www.voxelbusters.com";
		[SerializeField]
		private 	Texture2D	m_shareImage;
#pragma warning restore

		#endregion

#if !USES_TWITTER
	}
#else
		#region Unity Methods
		
		protected override void Start ()
		{
			base.Start ();

			// Set info texts
			AddExtraInfoTexts(
				"You can configure this feature in NPSettings->Social Network Settings-> Twitter Settings.");
		}

		#endregion

		#region GUI Methods
		
		protected override void DisplayFeatureFunctionalities ()
		{
			base.DisplayFeatureFunctionalities ();
			
			if (!NPSettings.Application.SupportedFeatures.UsesTwitter)
			{
				GUILayout.Box("If you want to use this feature, then please enable it in NPSettings.");
				return;
			}
			
			DrawAuthenticationAPI ();
			DrawSessionDetailsAPI ();
			DrawTweetComposeAPI ();
			DrawAccountDetailsAPI ();
			DrawRequestAPI ();
		}
		
		private void DrawAuthenticationAPI ()
		{
			GUILayout.Label("Authentication", kSubTitleStyle);
			
			if (GUILayout.Button("Initialise"))
			{
				Initialise();
			}
			
			if (GUILayout.Button("Login"))
			{
				Login();
			}
			
			if (GUILayout.Button("Logout"))
			{
				Logout();
			}
			
			if (GUILayout.Button("Is Logged In"))
			{
				bool _isLoggedIn 	= NPBinding.Twitter.IsLoggedIn();

				AddNewResult(_isLoggedIn ? "User is currently logged in." : "User not yet logged in!");
			}
		}
		
		private void DrawSessionDetailsAPI ()
		{
			GUILayout.Label("Session Details", kSubTitleStyle);
			
			if (GUILayout.Button("Get Auth Token"))
			{
				string _authToken		= NPBinding.Twitter.GetAuthToken();

				AddNewResult(string.Format("Authentication token is {0}.", _authToken));
			}
			
			if (GUILayout.Button("Get Auth Token Secret"))
			{
				string _authTokenSecret	= NPBinding.Twitter.GetAuthTokenSecret();

				AddNewResult(string.Format("Authentication token secret is {0}.", _authTokenSecret));
			}
			
			if (GUILayout.Button("Get User ID"))
			{
				string 	_userID			= NPBinding.Twitter.GetUserID();

				AddNewResult(string.Format("User identifier is {0}.", _userID));
			}
			
			if (GUILayout.Button("Get User Name"))
			{
				string 	_userName		= NPBinding.Twitter.GetUserName();

				AddNewResult(string.Format("User name is {0}.", _userName));
			}
		}
		
		private void DrawTweetComposeAPI ()
		{
			GUILayout.Label("Tweet Composer", kSubTitleStyle);
			
			if (GUILayout.Button("Show Tweet ComposerW ithMessage"))
			{
				ShowTweetComposerWithMessage();
			}
			
			if (GUILayout.Button("Show Tweet Composer With Link"))
			{
				ShowTweetComposerWithLink();
			}
			
			if (GUILayout.Button("Show Tweet Composer With Screenshot"))
			{
				ShowTweetComposerWithScreenshot();
			}
			
			if (GUILayout.Button("Show Tweet Composer With Image"))
			{
				ShowTweetComposerWithImage();
			}
		}
		
		private void DrawAccountDetailsAPI ()
		{
			GUILayout.Label("Account Details", kSubTitleStyle);
			
			if (GUILayout.Button("Request Account Details"))
			{
				RequestAccountDetails();
			}
			
			if (GUILayout.Button("Request Email Access"))
			{
				RequestEmailAccess();
			}
		}
		
		private void DrawRequestAPI ()
		{
			GUILayout.Label("API Request Access", kSubTitleStyle);
			
			if (GUILayout.Button("URL Request"))
			{
				MakeURLRequest();
			}
		}
		
		#endregion

		#region API Methods

		private void Initialise ()
		{
			AddNewResult( NPBinding.Twitter.Initialise() ? "Feature is initialised." : "Feature is not yet initialised.");
		}

		private void Login ()
		{
			NPBinding.Twitter.Login(LoginFinished);
		}

		private void Logout ()
		{
			NPBinding.Twitter.Logout();

			AddNewResult("Logged out successfully");
		}

		private void ShowTweetComposerWithMessage ()
		{
			NPBinding.Twitter.ShowTweetComposerWithMessage(m_shareMessage, DismissedTweetComposer);
		}

		private void ShowTweetComposerWithLink ()
		{
			NPBinding.Twitter.ShowTweetComposerWithLink(m_shareMessage, m_shareURL, DismissedTweetComposer);
		}
		
		private void ShowTweetComposerWithScreenshot ()
		{
			NPBinding.Twitter.ShowTweetComposerWithScreenshot(m_shareMessage, (eTwitterComposerResult _result)=>{
				AddNewResult("Tweet composer is dismissed.");
				AppendResult(string.Format("Result= {0}.", _result));
			});
		}

		private void ShowTweetComposerWithImage ()
		{
			NPBinding.Twitter.ShowTweetComposerWithImage(m_shareMessage, m_shareImage, DismissedTweetComposer);
		}

		private void RequestAccountDetails ()
		{
			NPBinding.Twitter.RequestAccountDetails(AccountDetailsRequestFinished);
		}

		private void RequestEmailAccess ()
		{
			NPBinding.Twitter.RequestEmailAccess(EmailAccessRequestFinished);
		}

		private void MakeURLRequest ()
		{
			string _URL 		= "https://api.twitter.com/1.1/statuses/show.json";
			IDictionary _params = new Dictionary<string, string>(){
				{"id", "20"}
			};
			
			NPBinding.Twitter.GetURLRequest(_URL, _params, URLRequestFinished);
		}
	
		#endregion

		#region API Callback Methods

		private void LoginFinished (TwitterSession _session, string _error)
		{
			AddNewResult(string.Format("Twitter login request finished. Error= {0}.", _error.GetPrintableString()));

			if (_error == null)
				AppendResult("Session info = " + _session + ".");
		}

		private void AccountDetailsRequestFinished (TwitterUser _user, string _error)
		{
			AddNewResult(string.Format("Request for account details finished. Error= {0}.", _error.GetPrintableString()));
			
			if (_error == null)
				AppendResult("User info = " + _user + ".");
		}

		private void DismissedTweetComposer (eTwitterComposerResult _result)
		{
			AddNewResult("Tweet composer is dismissed.");
			AppendResult("Result = " + _result + ".");
		}

		private void EmailAccessRequestFinished (string _email, string _error)
		{
			AddNewResult(string.Format("Request for accessing email info finished. Error= {0}.", _error.GetPrintableString()));
			
			if (_error == null)
				AppendResult("Email id = " + _email + ".");
		}

		private void URLRequestFinished (object _responseData, string _error)
		{
			AddNewResult(string.Format("Twitter request finished. Error= {0}.", _error.GetPrintableString()));
			
			if (_error == null)
				AppendResult("Response data = " + JSONUtility.ToJSON(_responseData) + ".");

			AppendResult("Also, don't forget to check PostURLRequest, PutURLRequest, DeleteURLRequest");
		}

		#endregion
	}
#endif
}