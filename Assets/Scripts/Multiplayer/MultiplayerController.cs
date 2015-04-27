using UnityEngine;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;

public class MultiplayerController : RealTimeMultiplayerListener
{
	private static MultiplayerController _instance = null;

	private uint minimumOpponents = 1;
	private uint maximumOpponents = 1;
	private uint gameVariation = 0;
	
	private MultiplayerController() 
	{
		PlayGamesPlatform.DebugLogEnabled = true;
		PlayGamesPlatform.Activate ();
	}
	
	public static MultiplayerController Instance 
	{
		get 
		{
			if (_instance == null) 
			{
				_instance = new MultiplayerController();
			}
			return _instance;
		}
	}


	public void SignInAndStartMPGame() 
	{
		if (!PlayGamesPlatform.Instance.localUser.authenticated) 
		{
			PlayGamesPlatform.Instance.localUser.Authenticate((bool success) => 
			{
				if (success) 
				{
					Debug.Log ("We're signed in! Welcome " + PlayGamesPlatform.Instance.localUser.userName);
					// We could start our game now
				} 
				else 
				{
					Debug.Log ("Oh... we're not signed in.");
				}
			});
		} 
		else 
		{
			Debug.Log ("You're already signed in.");
			// We could also start our game now
		}
	}

	public void TrySilentSignIn() 
	{
		if (!PlayGamesPlatform.Instance.localUser.authenticated) 
		{
			PlayGamesPlatform.Instance.Authenticate((bool success) => 
			{
				if (success) 
				{
					Debug.Log ("Silently signed in! Welcome " + PlayGamesPlatform.Instance.localUser.userName);
				} 
				else 
				{
					Debug.Log ("Oh... we're not signed in.");
				}
			}, true);
		} 
		else 
		{
			Debug.Log("We're already signed in");
		}
	}

	public void SetBestSore(int score, System.Action<bool> callback = null)
	{
		PlayGamesPlatform.Instance.ReportScore((long)score, "CgkImYnr8fAKEAIQAg", callback);
	}

	public void ShowLeaderboard()
	{
		PlayGamesPlatform.Instance.ShowLeaderboardUI();//"CgkImYnr8fAKEAIQAg");
	}


	public void StartMatchMakingRealTime() 
	{
		PlayGamesPlatform.Instance.RealTime.CreateWithInvitationScreen(minimumOpponents, maximumOpponents, gameVariation, this);
	}

	public void StartMatchMakingTurnBased() 
	{
		PlayGamesPlatform.Instance.TurnBased.CreateWithInvitationScreen(minimumOpponents, maximumOpponents, gameVariation, TurnCallback);
	}

	private void TurnCallback(bool success, TurnBasedMatch match)
	{
		if (success) 
		{
			LevelLoader.Instance.LoadLevel(1);
			ShowMPStatus("We are connected to the room! I would probably start our game now.");
		} 
		else 
		{
			ShowMPStatus("Uh-oh. Encountered some error connecting to the room.");
		}
	}
	
	
	private void ShowMPStatus(string message) 
	{
		Debug.Log(message);
	}


	public void OnRoomSetupProgress (float percent)
	{
		ShowMPStatus ("We are " + percent + "% done with setup");

//		throw new System.NotImplementedException ();
	}

	public void OnRoomConnected (bool success)
	{
		if (success) 
		{
			LevelLoader.Instance.LoadLevel(1);
			ShowMPStatus("We are connected to the room! I would probably start our game now.");
		} 
		else 
		{
			ShowMPStatus("Uh-oh. Encountered some error connecting to the room.");
		}

//		throw new System.NotImplementedException ();
	}

	public void OnLeftRoom ()
	{
		ShowMPStatus ("We have left the room. We should probably perform some clean-up tasks.");

//		throw new System.NotImplementedException ();
	}

	public void OnPeersConnected (string[] participantIds)
	{
		foreach (string participantID in participantIds) 
		{
			ShowMPStatus ("Player " + participantID + " has joined.");
		}
//		throw new System.NotImplementedException ();
	}

	public void OnPeersDisconnected (string[] participantIds)
	{
		foreach (string participantID in participantIds) 
		{
			ShowMPStatus ("Player " + participantID + " has left.");
		}

//		throw new System.NotImplementedException ();
	}

	public void OnRealTimeMessageReceived (bool isReliable, string senderId, byte[] data)
	{
		ShowMPStatus ("We have received some gameplay messages from participant ID:" + senderId);

//		throw new System.NotImplementedException ();
	}

}
