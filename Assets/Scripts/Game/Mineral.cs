using UnityEngine;
using System.Collections;

public class Mineral : MonoBehaviour 
{
	[SerializeField] int cost = 1;

	public void TakeMineral()
	{
		BankController.AddCoins(cost);

		this.GetComponent<SpriteRenderer>().enabled = false;
	}
}
