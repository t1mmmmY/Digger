using UnityEngine;
using System.Collections;

public enum Scene
{
	Splash = 0,
	Lobby = 1,
	SinglePlayer = 2,
	Multiplayer = 3,
	Tavern = 4
}


public static class CONST 
{
    public static readonly int RANDOM_CHARACTER_COST = 250;
	public static float BUY_CHARACTER_COST = 0.99f;

	public static readonly string MUSIC_PATH = "Music/Sweet_MusicLoop";
	public static readonly string TAVERN_PLAYERS_PATH = "Tavern/";
	public static readonly string PLAYABLE_PLAYERS_PATH = "Playable/";

	public static readonly string TAVERN_PREFIX = "_Tavern";

	public static readonly string[] PLAYER_NAMES = { 
		"DrunkenDigger",
		"WalterWhite",
		"Batman",
		"Charlie",
        "Finn"
	};

	public static readonly string[] DESCRIPTOIN_NAMES = { 
		"Drunken Digger",
		"Walter White",
		"BadMan",
		"Charlie",
        "Finn"
	};

	public static readonly string[] PLAYER_KEYS = {
		"DrunkenDigger",
		"WalterWhite",
		"Batman",
		"Charlie",
        "Finn"
	};


	public static bool InGame()
	{
		if (Application.loadedLevel == (int)Scene.SinglePlayer || Application.loadedLevel == (int)Scene.Multiplayer)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}
