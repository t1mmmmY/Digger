using UnityEngine;
using System.Collections;

public enum PlayerStatus
{
	NotBought = 0,
	Bought = 1
}

public class PlayerStatsController : BaseSingleton<PlayerStatsController> 
{
//	PlayerStatus[] playerStatus;
	string currentPlayerKey = "CURRENT_PLAYER";

	void Start()
	{
//		SetDefault();
	}

	void OnEnable()
	{
		GeneralGameController.onSelectCharacter += OnSelectCharacter;
	}

	void OnDisable()
	{
		GeneralGameController.onSelectCharacter -= OnSelectCharacter;
	}

	void OnSelectCharacter(Character character)
	{
		SetCurrentPlayer(character.number);
	}

	void SetCurrentPlayer(int number)
	{
		PlayerPrefs.SetInt(currentPlayerKey, number);
	}

	public int GetCurrentPlayerNumber()
	{
		return PlayerPrefs.GetInt(currentPlayerKey, 0);
	}

	public PlayerStatus GetStatus(int number)
	{
		return LoadState(number);
	}

	public void SetStatus(int number, PlayerStatus status)
	{
		SaveState(number, status);
	}

	PlayerStatus LoadState(int number)
	{
		if (number == 0)
		{
			return PlayerStatus.Bought;
		}
		else
		{
			return (PlayerStatus)PlayerPrefs.GetInt(CONST.PLAYER_KEYS[number], (int)PlayerStatus.NotBought);
		}
	}

	void SaveState(int number, PlayerStatus status)
	{
		PlayerPrefs.SetInt(CONST.PLAYER_KEYS[number], (int)status);
	}

	void SetDefault()
	{
		foreach(string key in CONST.PLAYER_KEYS)
		{
			PlayerPrefs.SetInt(key, (int)PlayerStatus.NotBought);
		}

		PlayerPrefs.SetInt(CONST.PLAYER_KEYS[0], (int)PlayerStatus.Bought);
	}

}
