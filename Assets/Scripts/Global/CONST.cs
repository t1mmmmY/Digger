using UnityEngine;
using System.Collections;

public enum Scene
{
	Splash = 0,
	Lobby = 1,
	SinglePlayer = 2,
//	Multiplayer = 3,
	Tavern = 3
}


public static class CONST 
{
    public static readonly int RANDOM_CHARACTER_COST = 250;
	public static float BUY_CHARACTER_COST = 0.99f;

	public static readonly string MUSIC_PATH = "Music/Sweet_MusicLoop";
	public static readonly string TAVERN_PLAYERS_PATH = "Tavern/";
	public static readonly string PLAYABLE_PLAYERS_PATH = "Playable/";

	public static readonly string TAVERN_PREFIX = "_Tavern";

	public static readonly string IOS_LEADERBOARD_ID = "SmartestDigger";

//	public static readonly string IOS_BUY_CHARACTER_ID = "com.octogames.digger.coins";

	public static readonly string[] PLAYER_NAMES = { 
		"DrunkenDigger",
		"WalterWhite",
		"Batman",
		"Charlie",
		"Finn",
		"Pippi",
		"Leprechaun"
	};

	public static readonly string[] DESCRIPTOIN_NAMES = { 
		"Drunken Digger",
		"Walker White",
		"BadMan",
		"Charlie",
		"Finn",
		"Pippi",
		"Leprechaun"
	};

	public static readonly string[] PLAYER_KEYS = {
		"Standard",
		"com.octogames.digger.walkerwhite",
		"com.octogames.digger.badman",
		"com.octogames.digger.charlie",
		"com.octogames.digger.finn",
		"com.octogames.digger.pippi",
		"com.octogames.digger.leprechaun"
	};

//	public static readonly float[] CHARACTER_COSTS = {
//		0.99f,
//		0.99f,
//		0.99f,
//		0.99f,
//		0.99f,
//		0.99f,
//		1.99f
//	};

	public static readonly int[] SPECIAL_CHARACTER_NUMBERS = {
		6
	};

	public static bool IsSpecialCharacter(int number)
	{
		foreach (int val in SPECIAL_CHARACTER_NUMBERS)
		{
			if (val == number)
			{
				return true;
			}
		}
		return false;
	}

	public static bool InGame()
	{
		if (Application.loadedLevel == (int)Scene.SinglePlayer /*|| Application.loadedLevel == (int)Scene.Multiplayer*/)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}
