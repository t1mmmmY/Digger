using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VoxelBusters.Utility;
using VoxelBusters.AssetStoreProductUtility.Demo;

namespace VoxelBusters.NativePlugins.Demo
{
	public class TwitterDemo : DemoSubMenu
	{
		#region Properties

		[SerializeField]
		private string 			m_shareMessage		= "this is what I wanted to share";

		[SerializeField]
		private string 			m_shareURL			= "http://www.voxelbusters.com";

		[SerializeField]
		private Texture2D		m_shareImage;

		#endregion

		#region API Calls

		private void Initialise ()
		{
			AddNewResult("Initialised=" + NPBinding.Twitter.Initialise());
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
				AddNewResult("Closed tweet composer");
				AppendResult("Result=" + _result);
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

		#region API Callbacks

		private void LoginFinished (TwitterSession _session, string _error)
		{
			if (string.IsNullOrEmpty(_error))
			{
				AddNewResult("Successfully logged-in");
				AppendResult("Twitter Session=" + _session);
			}
			else
			{
				AddNewResult("Failed to login");
				AppendResult("Error=" + _error);
			}
		}

		private void AccountDetailsRequestFinished (TwitterUser _user, string _error)
		{
			if (string.IsNullOrEmpty(_error))
			{
				AddNewResult("Received account details");
				AppendResult("Twitter User=" + _user);
			}
			else
			{
				AddNewResult("Failed to receive account details");
				AppendResult("Error=" + _error);
			}
		}

		private void DismissedTweetComposer (eTwitterComposerResult _result)
		{
			AddNewResult("Closed tweet composer");
			AppendResult("Result=" + _result);
		}

		private void EmailAccessRequestFinished (string _email, string _error)
		{
			if (string.IsNullOrEmpty(_error))
			{
				AddNewResult("Received access to user's emailID");
				AppendResult("EmailID=" + _email);
			}
			else
			{
				AddNewResult("Failed to access user's emailID information");
				AppendResult("Error=" + _error);
			}
		}

		private void URLRequestFinished (IDictionary _response, string _error)
		{
			if (string.IsNullOrEmpty(_error))
			{
				AddNewResult("Received response for URL request");
				AppendResult("Response Data=" + _response.ToJSON());
			}
			else
			{
				AddNewResult("URL request failed with errors");
				AppendResult("Error=" + _error);
			}

			AppendResult("Dont forget to check PostURLRequest, PutURLRequest, DeleteURLRequest");
		}

		#endregion

		#region UI
		
		protected override void OnGUIWindow()
		{		
			base.OnGUIWindow();

			if (!NPSettings.Application.SupportedFeatures.UsesTwitter)
			{
				GUILayout.Box("If you want to use twitter settings then, please set NeedsTwitter to TRUE in NPSettings.");
				return;
			}

			RootScrollView.BeginScrollView();
			{
				DrawAuthenticationAPI();
				DrawSessionDetailsAPI();
				DrawTweetComposeAPI();
				DrawAccountDetailsAPI();
				DrawRequestAPI();
			}
			RootScrollView.EndScrollView();
			
			DrawResults();
			DrawPopButton();
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
			
			if (GUILayout.Button("IsLoggedIn"))
			{
				bool _isLoggedIn 	= NPBinding.Twitter.IsLoggedIn();
				AddNewResult("Is Loggedin=" + _isLoggedIn);
			}
		}

		private void DrawSessionDetailsAPI ()
		{
			GUILayout.Label("Session Details", kSubTitleStyle);
			
			if (GUILayout.Button("GetAuthToken"))
			{
				string _authToken		= NPBinding.Twitter.GetAuthToken();
				AddNewResult("Auth Token=" + _authToken);
			}
			
			if (GUILayout.Button("GetAuthTokenSecret"))
			{
				string _authTokenSecret	= NPBinding.Twitter.GetAuthTokenSecret();
				AddNewResult("Auth Token Secret=" + _authTokenSecret);
			}
			
			if (GUILayout.Button("GetUserID"))
			{
				string _userID			= NPBinding.Twitter.GetUserID();
				AddNewResult("User ID=" + _userID);
			}
			
			if (GUILayout.Button("GetUserName"))
			{
				string _userName		= NPBinding.Twitter.GetUserName();
				AddNewResult("Username=" + _userName);
			}
		}

		private void DrawTweetComposeAPI ()
		{
			GUILayout.Label("Tweet Composer", kSubTitleStyle);
			
			if (GUILayout.Button("ShowTweetComposerWithMessage"))
			{
				ShowTweetComposerWithMessage();
			}
			
			if (GUILayout.Button("ShowTweetComposerWithLink"))
			{
				ShowTweetComposerWithLink();
			}
			
			if (GUILayout.Button("ShowTweetComposerWithScreenshot"))
			{
				ShowTweetComposerWithScreenshot();
			}
			
			if (GUILayout.Button("ShowTweetComposerWithImage"))
			{
				ShowTweetComposerWithImage();
			}
		}

		private void DrawAccountDetailsAPI ()
		{
			GUILayout.Label("Account Details", kSubTitleStyle);
			
			if (GUILayout.Button("RequestAccountDetails"))
			{
				RequestAccountDetails();
			}
			
			if (GUILayout.Button("RequestEmailAccess"))
			{
				RequestEmailAccess();
			}
		}

		private void DrawRequestAPI ()
		{
			GUILayout.Label("API Request Access", kSubTitleStyle);

			if (GUILayout.Button("URLRequest"))
			{
				MakeURLRequest();
			}
		}

		#endregion
	}
}
