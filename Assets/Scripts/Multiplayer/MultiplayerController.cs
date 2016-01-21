using UnityEngine;
using System.Collections;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;

public class MultiplayerController : MonoBehaviour
{
	private static MultiplayerController _instance = null;
	public static System.Action<bool, string, byte[]> onRealTimeMessageReceived;

	private uint minimumOpponents = 1;
	private uint maximumOpponents = 1;
	private uint gameVariation = 0;

	bool signingIn = false;

#if UNITY_IOS
#endif


	public void Awake() 
	{
		_instance = this;

#if UNITY_ANDROID
		// recommended for debugging:
		PlayGamesPlatform.DebugLogEnabled = true;
		
		// Activate the Google Play Games platform
		PlayGamesPlatform.Activate ();


//		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
//			// enables saving game progress.
//			.EnableSavedGames()
//				// registers a callback to handle game invitations received while the game is not running.
////				.WithInvitationDelegate(() => {})
//				// registers a callback for turn based match notifications received while the
//				// game is not running.
////				.WithMatchDelegate(() => {})
//				.Build();
//		
//		PlayGamesPlatform.InitializeInstance(config);
//		// recommended for debugging:
//		PlayGamesPlatform.DebugLogEnabled = true;
//		// Activate the Google Play Games platform
//		PlayGamesPlatform.Activate();



//		PlayGamesPlatform.DebugLogEnabled = false;
//		PlayGamesPlatform.Activate ();
#elif UNITY_IOS
#endif
	}
	
	public static MultiplayerController Instance 
	{
		get 
		{
//			if (_instance == null) 
//			{
//				_instance = new MultiplayerController();
//			}
			return _instance;
		}
	}


	public void SignIn(System.Action<bool> callback) 
	{
#if UNITY_ANDROID
		//ANDROID CHANGES
//		UnityEngine.Social.localUser.Authenticate(callback);


//		if (!PlayGamesPlatform.Instance.localUser.authenticated) 
//		{
			Social.localUser.Authenticate((bool success) => 
			{
				if (success) 
				{
					Debug.Log ("We're signed in! Welcome!");
					// We could start our game now
				} 
				else 
				{
					Debug.Log ("Oh... we're not signed in.");
				}

				if (callback != null)
				{
					callback(success);
				}
			});
//		} 
//		else 
//		{
//			Debug.Log ("You're already signed in.");
//			// We could also start our game now
//		}
#elif UNITY_IOS

		UnityEngine.Social.localUser.Authenticate(callback);
		
#endif
	}

	public void TrySilentSignIn(System.Action<bool> callback) 
	{
#if UNITY_ANDROID
		SignIn(callback);



//		if (!PlayGamesPlatform.Instance.localUser.authenticated) 
//		{
//			PlayGamesPlatform.Instance.Authenticate((bool success) => 
//			{
//				if (success) 
//				{
//					Debug.Log ("Silently signed in! Welcome " + PlayGamesPlatform.Instance.localUser.userName);
//				} 
//				else 
//				{
//					Debug.Log ("Oh... we're not signed in.");
//				}
//
//				if (callback != null)
//				{
//					callback(success);
//				}
//
//			}, true);
//		} 
//		else 
//		{
//			Debug.Log("We're already signed in");
//		}

#elif UNITY_IOS
		SignIn(callback);
#endif
	}

//	private void OnComleteAuthentication(bool result)
//	{
//		Debug.Log("OnComleteAuthentication " + result);
//	}

	public void SetBestSore(int score, System.Action<bool> callback = null)
	{
#if UNITY_ANDROID
		if (Social.localUser.authenticated) 
		{
			Social.ReportScore ((long)score, CONST.ANDROID_LEADERBOARD_ID, (bool success) =>
			                    {
				if (success) {
//					Debug.Log ("Update Score Success");
					
				} else {
//					Debug.Log ("Update Score Fail");
				}

				if (callback != null)
				{
					callback(success);
				}
			});
		}

//		Social.ReportScore((long)score, CONST.ANDROID_LEADERBOARD_ID, callback);
		
		//		UnityEngine.Social.ReportScore((long)score, CONST.ANDROID_LEADERBOARD_ID, callback);
		

//		PlayGamesPlatform.Instance.ReportScore((long)score, "CgkImYnr8fAKEAIQAg", callback);
#elif UNITY_IOS
		UnityEngine.Social.ReportScore((long)score, CONST.IOS_LEADERBOARD_ID, callback);
#endif
	}

//	public int GetRang()
//	{
//		//Local for now
//		return PlayerPrefs.GetInt("PlayerRang", 100);
//	}


//	public void ChangeRang(int score, System.Action<bool> callback = null)
//	{
//#if UNITY_ANDROID
////		PlayGamesPlatform.Instance.ReportScore((long)(GetRang() + score), "CgkImYnr8fAKEAIQAw", callback);
//#elif UNITY_IOS
//#endif
//		PlayerPrefs.SetInt("PlayerRang", GetRang() + score);
//	}

	public void ShowLeaderboard()
	{
#if UNITY_ANDROID
		//        Social.ShowLeaderboardUI (); // Show all leaderboard
		((PlayGamesPlatform)Social.Active).ShowLeaderboardUI (CONST.ANDROID_LEADERBOARD_ID); // Show current (Active) leaderboard

//		Social.ShowLeaderboardUI();
//		PlayGamesPlatform.Instance.ShowLeaderboardUI();//"CgkImYnr8fAKEAIQAg");
#elif UNITY_IOS
		UnityEngine.SocialPlatforms.GameCenter.GameCenterPlatform.ShowLeaderboardUI(CONST.IOS_LEADERBOARD_ID, UnityEngine.SocialPlatforms.TimeScope.Today);
#endif
	}


//	public void StartMatchMakingRealTime() 
//	{
//#if UNITY_EDITOR || UNITY_WEBPLAYER
//#else
//		PlayGamesPlatform.Instance.RealTime.CreateWithInvitationScreen(minimumOpponents, maximumOpponents, gameVariation, this);
//#endif
//	}

//	public void Disconnect()
//	{
//#if UNITY_EDITOR || UNITY_WEBPLAYER
//#else
//		PlayGamesPlatform.Instance.RealTime.LeaveRoom();
//#endif
//	}
//
//	public void StartMatchMakingRealTimeFast() 
//	{
//#if UNITY_EDITOR || UNITY_WEBPLAYER
//#else
//		PlayGamesPlatform.Instance.RealTime.CreateQuickGame(minimumOpponents, maximumOpponents, gameVariation, this);
//#endif
//	}
//
//	private void TurnCallback(bool success, TurnBasedMatch match)
//	{
//	}
//	
//	
//	private void ShowMPStatus(string message) 
//	{
//		Debug.Log(message);
//	}
////
////
//	public void OnRoomSetupProgress (float percent)
//	{
//		ShowMPStatus ("We are " + percent + "% done with setup");
//	}

//	public void OnRoomConnected (bool success)
//	{
//	}
//
//	public void OnLeftRoom ()
//	{
//		ShowMPStatus ("We have left the room. We should probably perform some clean-up tasks.");
//	}
//
//	public void OnPeersConnected(string[] participantIds)
//	{
//		foreach (string participantID in participantIds) 
//		{
//			ShowMPStatus ("Player " + participantID + " has joined.");
//		}
//	}
//
//	public void OnPeersDisconnected (string[] participantIds)
//	{
//		foreach (string participantID in participantIds) 
//		{
//			ShowMPStatus ("Player " + participantID + " has left.");
//		}
//	}
//
//	public void OnRealTimeMessageReceived (bool isReliable, string senderId, byte[] data)
//	{
//		if (onRealTimeMessageReceived != null)
//		{
//			onRealTimeMessageReceived(isReliable, senderId, data);
//		}
//		ShowMPStatus ("We have received some gameplay messages from participant ID:" + senderId);
//	}

//	public static byte[] Serialize(object obj)
//	{
//		string text = JsonConvert.SerializeObject(obj);
//		return MultiplayerController.GetBytes(text);
//	}
//
//	public static object Deserialize(byte[] data, System.Type type = null)
//	{
//		string text = GetString(data);
//		if (type == null)
//		{
//			return JsonConvert.DeserializeObject(text);
//		}
//		else
//		{
//			object obj = JsonConvert.DeserializeObject(text, type);
//			return obj;
//		}
//	}
//
//	public static byte[] GetBytes(string str)
//	{
//		byte[] bytes = new byte[str.Length * sizeof(char)];
//		System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
//		return bytes;
//	}
//	
//	public static string GetString(byte[] bytes)
//	{
//		char[] chars = new char[bytes.Length / sizeof(char)];
//		System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
//		return new string(chars);
//	}
	
//	public void SendRealTimeMessage(object data, bool toAll = false, bool reliable = true)
//	{
//		byte[] bData = MultiplayerController.Serialize(data);
//#if UNITY_EDITOR || UNITY_WEBPLAYER
//		OnRealTimeMessageReceived(true, "", bData);
//#else
//		if (!toAll)
//		{
//			foreach (Participant opponent in PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants())
//			{
//				if (opponent != PlayGamesPlatform.Instance.RealTime.GetSelf())
//				{
//					PlayGamesPlatform.Instance.RealTime.SendMessage(reliable, opponent.ParticipantId, bData);
//				}
//				else
//				{
//				}
//			}
//		}
//		else
//		{
//			foreach (Participant opponent in PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants())
//			{
//				if (opponent != PlayGamesPlatform.Instance.RealTime.GetSelf())
//				{
//					Debug.LogWarning("Opponent");
//				}
//				else
//				{
//					Debug.LogWarning("Me");
//				}
//			}
//			PlayGamesPlatform.Instance.RealTime.SendMessageToAll(reliable, bData);
//		}
//#endif
//	}
//
//	public void OnParticipantLeft (Participant participant)
//	{
////		throw new System.NotImplementedException ();
//	}
//
//	public bool IsFirstPlayer()
//	{
//#if UNITY_EDITOR || UNITY_WEBPLAYER
//		return true;
//#else
//
//		int number = 0;
//		foreach (Participant participant in PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants())
//		{
//			if (participant == PlayGamesPlatform.Instance.RealTime.GetSelf() && number == 0)
//			{
//				return true;
//			}
//			else
//			{
//				return false;
//			}
//			number++;
//		}
//		return false;
//#endif
//	}
	
}
