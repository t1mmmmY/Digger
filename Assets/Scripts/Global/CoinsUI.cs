using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CoinsUI : MonoBehaviour 
{
	[SerializeField] Text coinsLabel;
	[SerializeField] Animator coinsAnimator;

	private int showCoinsHash = Animator.StringToHash("ShowLabel");
	private int hideCoinsHash = Animator.StringToHash("HideLabel");

	void OnEnable()
	{
		BankController.OnChangeCoins += OnChangeCoins;
		GeneralGameController.onLoadLobby += OnLoadLobby;
	}

	void OnDisable()
	{
		BankController.OnChangeCoins -= OnChangeCoins;
		GeneralGameController.onLoadLobby -= OnLoadLobby;
	}

	void OnChangeCoins(int coins)
	{
		coinsLabel.text = coins.ToString();
	}

	void OnLoadLobby()
	{
		coinsAnimator.SetTrigger(showCoinsHash);
	}

}
