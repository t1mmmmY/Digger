using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;
using Newtonsoft.Json;

public class MultiplayerController : RealTimeMultiplayerListener
{
	private static MultiplayerController _instance = null;
	public static System.Action<bool, string, byte[]> onRealTimeMessageReceived;

	private uint minimumOpponents = 1;
	private uint maximumOpponents = 1;
	private uint gameVariation = 0;

//	private List<string> opponentId = "";

	private MultiplayerController() 
	{
		PlayGamesPlatform.DebugLogEnabled = false;
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


	public void SignIn() 
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

	public int GetRang()
	{
		//Local for now
		return PlayerPrefs.GetInt("PlayerRang", 100);
	}


	public void ChangeRang(int score, System.Action<bool> callback = null)
	{
		PlayGamesPlatform.Instance.ReportScore((long)(GetRang() + score), "CgkImYnr8fAKEAIQAw", callback);
		PlayerPrefs.SetInt("PlayerRang", GetRang() + score);
	}

	public void ShowLeaderboard()
	{
		PlayGamesPlatform.Instance.ShowLeaderboardUI();//"CgkImYnr8fAKEAIQAg");
	}


	public void StartMatchMakingRealTime() 
	{
//		opponentId = new List<string>();
#if UNITY_EDITOR || UNITY_WEBPLAYER
//		LevelLoader.Instance.LoadLevel(Scene.Multiplayer);
#else
		MainMenu.Instance.StartTimerTimeout();
		PlayGamesPlatform.Instance.RealTime.CreateWithInvitationScreen(minimumOpponents, maximumOpponents, gameVariation, this);
#endif
	}

	public void Disconnect()
	{
#if UNITY_EDITOR || UNITY_WEBPLAYER
#else
		PlayGamesPlatform.Instance.RealTime.LeaveRoom();
#endif
	}

	public void StartMatchMakingRealTimeFast() 
	{
#if UNITY_EDITOR || UNITY_WEBPLAYER
//		LevelLoader.Instance.LoadLevel(Scene.Multiplayer);
#else
		MainMenu.Instance.StartTimerTimeout();
		PlayGamesPlatform.Instance.RealTime.CreateQuickGame(minimumOpponents, maximumOpponents, gameVariation, this);
#endif
		//		PlayGamesPlatform.Instance.TurnBased.CreateWithInvitationScreen(minimumOpponents, maximumOpponents, gameVariation, TurnCallback);
	}

	private void TurnCallback(bool success, TurnBasedMatch match)
	{
		if (success) 
		{
			LevelLoader.Instance.LoadLevel(Scene.Multiplayer);
			ShowMPStatus("We are connected to the room! I would probably start our game now.");
		} 
		else 
		{
			MainMenu.Instance.SetActiveAllButtons(true);
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
	}

	public void OnRoomConnected (bool success)
	{
		if (success) 
		{
			LevelLoader.Instance.LoadLevel(Scene.Multiplayer);
			ShowMPStatus("We are connected to the room! I would probably start our game now.");
		} 
		else 
		{
			MainMenu.Instance.SetActiveAllButtons(true);
			ShowMPStatus("Uh-oh. Encountered some error connecting to the room.");
		}
	}

	public void OnLeftRoom ()
	{
		ShowMPStatus ("We have left the room. We should probably perform some clean-up tasks.");
	}

	public void OnPeersConnected(string[] participantIds)
	{
		foreach (string participantID in participantIds) 
		{
//			if (participantID != PlayGamesPlatform.Instance.GetUserId())
//			{
//				opponentId.Add(participantID);
//			}
			ShowMPStatus ("Player " + participantID + " has joined.");
		}
	}

	public void OnPeersDisconnected (string[] participantIds)
	{
		foreach (string participantID in participantIds) 
		{
			ShowMPStatus ("Player " + participantID + " has left.");
		}
	}

	public void OnRealTimeMessageReceived (bool isReliable, string senderId, byte[] data)
	{
		if (onRealTimeMessageReceived != null)
		{
			onRealTimeMessageReceived(isReliable, senderId, data);
		}
		ShowMPStatus ("We have received some gameplay messages from participant ID:" + senderId);
	}

	public static byte[] Serialize(object obj)
	{
		string text = JsonConvert.SerializeObject(obj);
		return MultiplayerController.GetBytes(text);
	}

	public static object Deserialize(byte[] data, System.Type type = null)
	{
		string text = GetString(data);
		if (type == null)
		{
			return JsonConvert.DeserializeObject(text);
		}
		else
		{
			object obj = JsonConvert.DeserializeObject(text, type);
			return obj;
		}
	}

	public static byte[] GetBytes(string str)
	{
		byte[] bytes = new byte[str.Length * sizeof(char)];
		System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
		return bytes;
	}
	
	public static string GetString(byte[] bytes)
	{
		char[] chars = new char[bytes.Length / sizeof(char)];
		System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
		return new string(chars);
	}
	
	public void SendRealTimeMessage(object data, bool toAll = false, bool reliable = true)
	{
		byte[] bData = MultiplayerController.Serialize(data);
#if UNITY_EDITOR || UNITY_WEBPLAYER
		OnRealTimeMessageReceived(true, "", bData);
#else
		if (!toAll)
		{
			foreach (Participant opponent in PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants())
			{
				if (opponent != PlayGamesPlatform.Instance.RealTime.GetSelf())
				{
//					Debug.LogWarning("Opponent");
					PlayGamesPlatform.Instance.RealTime.SendMessage(reliable, opponent.ParticipantId, bData);
				}
				else
				{
//					Debug.LogWarning("Me");
				}
			}
		}
		else
		{
			foreach (Participant opponent in PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants())
			{
				if (opponent != PlayGamesPlatform.Instance.RealTime.GetSelf())
				{
					Debug.LogWarning("Opponent");
//					PlayGamesPlatform.Instance.RealTime.SendMessage(reliable, opponent.ParticipantId, bData);
				}
				else
				{
					Debug.LogWarning("Me");
				}
			}
			PlayGamesPlatform.Instance.RealTime.SendMessageToAll(reliable, bData);
		}
#endif
	}

	public bool IsFirstPlayer()
	{
#if UNITY_EDITOR || UNITY_WEBPLAYER
		return true;
#else

		int number = 0;
		foreach (Participant participant in PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants())
		{
			if (participant == PlayGamesPlatform.Instance.RealTime.GetSelf() && number == 0)
			{
				return true;
			}
			else
			{
				return false;
			}
			number++;
		}
		return false;
#endif
	}
	
}
