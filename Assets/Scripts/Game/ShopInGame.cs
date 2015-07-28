using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShopInGame : BaseSingleton<ShopInGame> 
{
    [Range(0.0f, 1.0f)]
    [SerializeField] float frequency = 0.5f;

    public bool NeedToShowProposalInGame(int oldCoinsCount)
    {
        //Not enough coins
        if (BankController.coins < CONST.RANDOM_CHARACTER_COST)
        {
            return false;
        }

        //Find all free diggers
        List<int> canBuy = new List<int>();
        for (int i = 0; i < CONST.PLAYER_KEYS.Length; i++)
        {
            PlayerStatus status = PlayerStatsController.Instance.GetStatus(i);
            if (status == PlayerStatus.NotBought)
            {
                canBuy.Add(i);
            }
        }

        //nothing to buy
        if (canBuy.Count == 0)
        {
            return false;
        }

        if (oldCoinsCount < CONST.RANDOM_CHARACTER_COST && BankController.coins >= CONST.RANDOM_CHARACTER_COST)
        {
            return true;
        }

        //check if need to show
        float randomNumber = Random.Range(0.0f, 1.0f);
        if (randomNumber < frequency)
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
            if (status == PlayerStatus.NotBought)
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
}
