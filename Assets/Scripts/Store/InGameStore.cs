using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VoxelBusters.NativePlugins;
using UnityEngine.UI;

public class InGameStore : BaseSingleton<InGameStore>
{
	[SerializeField] Button fadeButton;

	private BillingProduct[] products;
	private System.Action<bool> onTransacionFinishedCallback;
	private int currentProductNumber = 0;

	protected override void Awake ()
	{
		Billing.TransactionFinishedEvent += HandleTransactionFinishedEvent;
		Billing.BillingProductsRequestFinishedEvent += HandleBillingProductsRequestFinishedEvent;

		base.Awake ();
	}

	void Start()
	{
		products = NPSettings.Billing.Products;
		NPBinding.Billing.RequestForBillingProducts(products);
	}

	protected override void OnDestroy ()
	{
		Billing.TransactionFinishedEvent -= HandleTransactionFinishedEvent;
		Billing.BillingProductsRequestFinishedEvent -= HandleBillingProductsRequestFinishedEvent;

		base.OnDestroy ();
	}

	public bool IsProductPurchased(int characterNumber)
	{
		if (characterNumber >= products.Length)
		{
			return false;
		}

		if (characterNumber == 0)
		{
			return true;
		}

		return PlayerStatsController.Instance.GetStatus(characterNumber) == PlayerStatus.Bought ? true : false;
		
//		return NPBinding.Billing.IsProductPurchased(products[characterNumber].ProductIdentifier);
	}

	public string GetProductPrice(int characterNumber)
	{
		if (characterNumber >= products.Length)
		{
			return "out of range";
		}

		if (characterNumber == 0)
		{
			return "free";
		}

		return string.Format("{0} {1}", products[characterNumber].CurrencyCode, products[characterNumber].Price);
	}

	public bool BuyProduct(int characterNumber, System.Action<bool> callback)
	{
		if (characterNumber >= products.Length)
		{
			return false;
		}

		if (characterNumber == 0)
		{
			return false;
		}

		//Do not buy the same products twice
		if (!IsProductPurchased(characterNumber))
		{
			fadeButton.gameObject.SetActive(true);
			onTransacionFinishedCallback = callback;
			currentProductNumber = characterNumber;
			NPBinding.Billing.BuyProduct(products[characterNumber].ProductIdentifier);
			return true;
		}

		return false;
	}

	public void RestoreCompletedTransactions()
	{
		NPBinding.Billing.RestoreCompletedTransactions();
		NPBinding.Billing.RequestForBillingProducts(products);

		//without standart digger
		for (int i = 1; i < products.Length; i++)
		{
			PlayerStatsController.Instance.SetStatus(i, PlayerStatus.NotBought);
		}
		Debug.Log("Restore products");
	}


	void HandleBillingProductsRequestFinishedEvent (List<BillingProduct> _regProductsList, string _error)
	{
//		if (_error != "")
//		{
//			Debug.LogError("Request products error " + _error);
//		}

		for (int i = 0; i < _regProductsList.Count; i++)
		{
			products[i] = FindProductById(_regProductsList, products[i].ProductIdentifier);
			Debug.Log("product " + i.ToString() + " = " + products[i].ToString());
		}

	}

	BillingProduct FindProductById(List<BillingProduct> allProducts, string id)
	{
		foreach (BillingProduct bp in allProducts)
		{
			if (bp.ProductIdentifier == id)
			{
				return bp;
			}
		}

		return null;
	}

	void HandleTransactionFinishedEvent (List<BillingTransaction> _finishedTransactions)
	{
		fadeButton.gameObject.SetActive(false);
		
		foreach (VoxelBusters.NativePlugins.BillingTransaction transaction in _finishedTransactions)
		{
			Debug.Log(transaction.ToString());
			if (transaction.ProductIdentifier == products[currentProductNumber].ProductIdentifier)
			{
				bool isSuccess = false;

				if (transaction.TransactionState == eBillingTransactionState.PURCHASED)
				{
					isSuccess = true;
					NPBinding.Billing.RequestForBillingProducts(products);

					PlayerStatsController.Instance.SetStatus(currentProductNumber, PlayerStatus.Bought);
				}

				if (onTransacionFinishedCallback != null)
				{
					onTransacionFinishedCallback(isSuccess);
				}
			}
		}

	}
	


}