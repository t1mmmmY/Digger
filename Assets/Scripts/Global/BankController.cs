﻿using UnityEngine;
using System.Collections;

public static class BankController 
{
	private static int _coins = 0;
	private static string _coinsKey = "COINS";

	public static int coins
	{
		get { return _coins; }
	}

	public static System.Action<int> OnChangeCoins;

	public static void Init()
	{
		//Local for now
		_coins = PlayerPrefs.GetInt(_coinsKey, 0);

		if (OnChangeCoins != null)
		{
			OnChangeCoins(_coins);
		}
	}

	public static void AddCoins(int count)
	{
		_coins += count;
		PlayerPrefs.SetInt(_coinsKey, _coins);

		if (OnChangeCoins != null)
		{
			OnChangeCoins(_coins);
		}
	}
}
