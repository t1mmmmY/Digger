using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShopInGame : BaseSingleton<ShopInGame> 
{
    [Range(0.0f, 1.0f)]
    [SerializeField] float randonCharacterFrequency = 0.5f;

	[Range(0.0f, 1.0f)]
	[SerializeField] float buyCharacterFrequency = 0.2f;

    public bool NeedToShowProposalInGame(int oldCoinsCount, bool forRealMoney)
    {
        //Not enough coins
        if (!forRealMoney && BankController.coins < CONST.RANDOM_CHARACTER_COST)
        {
            return false;
        }

        //Find all free diggers
        List<int> canBuy = new List<int>();
        for (int i = 0; i < CONST.PLAYER_KEYS.Length; i++)
        {
			if (PlayerStatsController.Instance != null)
			{
	            PlayerStatus status = PlayerStatsController.Instance.GetStatus(i);
				if (status == PlayerStatus.NotBought && (forRealMoney || !CONST.IsSpecialCharacter(i)))
	            {
	                canBuy.Add(i);
	            }
			}
        }

        //nothing to buy
        if (canBuy.Count == 0)
        {
            return false;
        }

        if (!forRealMoney && oldCoinsCount < CONST.RANDOM_CHARACTER_COST && BankController.coins >= CONST.RANDOM_CHARACTER_COST)
        {
            return true;
        }

        //check if need to show
        float randomNumber = Random.Range(0.0f, 1.0f);
		if ((!forRealMoney && randomNumber < randonCharacterFrequency) || (forRealMoney && randomNumber < buyCharacterFrequency))
        {
            return true;
        }

        return false;
    }

    public bool BuyRandomCharacter()
    {
        //Find all free diggers
        List<int> canBuy = new List<int>();
        for (int i = 0; i < CONST.PLAYER_KEYS.Length; i++)
        {
            PlayerStatus status = PlayerStatsController.Instance.GetStatus(i);
			if (status == PlayerStatus.NotBought && !CONST.IsSpecialCharacter(i))
            {
                canBuy.Add(i);
            }
        }

        //nothing to buy
        if (canBuy.Count == 0)
        {
            return false;
        }

        int randomNumber = Random.Range(0, canBuy.Count);
        int randomCharacterNumber = canBuy[randomNumber];

        if (GeneralGameController.Instance != null)
        {
            GeneralGameController.Instance.SelectCharacter(randomCharacterNumber);
        }

        return true;
    }

	public int GetRandomCharacterNumber(bool forRealMoney)
	{
		List<int> canBuy = new List<int>();
		for (int i = 0; i < CONST.PLAYER_KEYS.Length; i++)
		{
			PlayerStatus status = PlayerStatsController.Instance.GetStatus(i);
			if (status == PlayerStatus.NotBought && (forRealMoney || !CONST.IsSpecialCharacter(i)))
			{
				canBuy.Add(i);
			}
		}
		
		//nothing to buy
		if (canBuy.Count == 0)
		{
			return 0;
		}
		
		int randomNumber = Random.Range(0, canBuy.Count);
		int randomCharacterNumber = canBuy[randomNumber];

		return randomCharacterNumber;
	}

	public bool BuyCharacter(int characterNumber)
	{
		
		if (GeneralGameController.Instance != null)
		{
			GeneralGameController.Instance.SelectCharacter(characterNumber);
		}
		
		return true;
	}
}
