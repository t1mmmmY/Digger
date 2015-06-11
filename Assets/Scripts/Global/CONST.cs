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
	public static readonly string MUSIC_PATH = "Music/Sweet_MusicLoop";

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
