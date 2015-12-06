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
        //SetDefault();
	}

	void OnEnable()
	{
		GeneralGameController.onSelectCharacter += OnSelectCharacter;
	}

	void OnDisable()
	{
		GeneralGameController.onSelectCharacter -= OnSelectCharacter;
	}

	void OnSelectCharacter(int characterNumber)
	{
		SetCurrentPlayer(characterNumber);
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
		return GetStatus(CONST.PLAYER_KEYS[number]);
	}

	public PlayerStatus GetStatus(string id)
	{
		return LoadState(id);
	}

	public void SetStatus(int number, PlayerStatus status)
	{
		SaveState(number, status);
	}

//	PlayerStatus LoadState(int number)
//	{
//		if (number == 0)
//		{
//			return PlayerStatus.Bought;
//		}
//		else
//		{
//			return (PlayerStatus)PlayerPrefs.GetInt(CONST.PLAYER_KEYS[number], (int)PlayerStatus.NotBought);
//		}
//	}

	PlayerStatus LoadState(string id)
	{
		if (id == "Standard")
		{
			return PlayerStatus.Bought;
		}
		else
		{
			return NPBinding.Billing.IsProductPurchased(id) ? PlayerStatus.Bought : PlayerStatus.NotBought;
		}
	}

	void SaveState(int number, PlayerStatus status)
	{
		PlayerPrefs.SetInt(CONST.PLAYER_KEYS[number], (int)status);
	}

	void SetDefault()
	{
		Debug.LogWarning("SetDefault");

		foreach(string key in CONST.PLAYER_KEYS)
		{
			PlayerPrefs.SetInt(key, (int)PlayerStatus.NotBought);
		}

		PlayerPrefs.SetInt(CONST.PLAYER_KEYS[0], (int)PlayerStatus.Bought);

        SetCurrentPlayer(0);
        BankController.RemoveCoins(BankController.coins);
	}

}
