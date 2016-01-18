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
		//ANDROID CHANGES
//		Billing.TransactionFinishedEvent += HandleTransactionFinishedEvent;
//		Billing.BillingProductsRequestFinishedEvent += HandleBillingProductsRequestFinishedEvent;

		base.Awake ();
	}

	void Start()
	{
		//ANDROID CHANGES
//		products = NPSettings.Billing.Products;
//		NPBinding.Billing.RequestForBillingProducts(products);
	}

	protected override void OnDestroy ()
	{

		Billing.TransactionFinishedEvent -= HandleTransactionFinishedEvent;
		Billing.BillingProductsRequestFinishedEvent -= HandleBillingProductsRequestFinishedEvent;

		base.OnDestroy ();
	}

	public bool IsProductPurchased(int characterNumber)
	{
//#if UNITY_EDITOR
//		return true;
//#endif
		if (characterNumber >= products.Length)
		{
			return false;
		}

		if (characterNumber == 0)
		{
			return true;
		}


//		return PlayerStatsController.Instance.GetStatus(characterNumber) == PlayerStatus.Bought ? true : false;
//		Debug.LogWarning(products[characterNumber].ProductIdentifier);
		return PlayerStatsController.Instance.GetStatus(products[characterNumber].ProductIdentifier) == PlayerStatus.Bought ? true : false;

//		return NPBinding.Billing.IsProductPurchased(products[characterNumber].ProductIdentifier);
	}

	public float GetProductPrice(int characterNumber)
	{
		if (characterNumber >= products.Length)
		{
			Debug.LogError("GetProductPrice Out of range!");
			return 0;
		}

		if (characterNumber == 0)
		{
			return 0;
		}

		return products[characterNumber].Price;

//		return string.Format("{0} {1}", products[characterNumber].CurrencyCode, products[characterNumber].Price);
	}

	public string GetProductCurrency(int characterNumber)
	{
		if (characterNumber >= products.Length)
		{
			return "out of range";
		}
		
		if (characterNumber == 0)
		{
			return "free";
		}
		
		return products[characterNumber].CurrencyCode;
		
//		return string.Format("{0} {1}", products[characterNumber].CurrencyCode, products[characterNumber].Price);
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

		// This is the extra code that needs to be included, as a temp fix for handling this issue
//		foreach(BillingTransaction _eachTransaction in _finishedTransactions)
//		{
//			_eachTransaction.VerificationState = eBillingTransactionVerificationState.SUCCESS;
//			
//			// marking verification is successful, on doing so this transaction will added to purchase history 
//			NPBinding.Billing.CustomVerificationFinished(_eachTransaction);
//		}
		
		foreach (BillingTransaction transaction in _finishedTransactions)
		{
			Debug.Log(transaction.ToString());
//			if (transaction.ProductIdentifier == products[currentProductNumber].ProductIdentifier)
//			{
			bool isSuccess = false;

			if (transaction.TransactionState == eBillingTransactionState.PURCHASED)
			{
				isSuccess = true;
//				NPBinding.Billing.RequestForBillingProducts(products);

				PlayerStatsController.Instance.SetStatus(currentProductNumber, PlayerStatus.Bought);
			}

			if (onTransacionFinishedCallback != null)
			{
				onTransacionFinishedCallback(isSuccess);
			}
//			}
		}

		NPBinding.Billing.RequestForBillingProducts(products);
		

	}

//	void DidFinishProductsRequestEvent (BillingProduct[] _regProductsList, string _error)
//	{
//		Debug.Log(string.Format("Billing products request finished. Error = {0}.", _error));
//		
//		if (_regProductsList != null)
//		{
//			foreach (BillingProduct _eachProduct in _regProductsList)
//			{
//				Debug.Log(_eachProduct.ToString());
//			}
//		}
//	}
//
//	void DidReceiveTransactionInfoEvent (BillingTransaction[] _finishedTransactions, string _error)
//	{
//		Debug.Log(string.Format("Billing transaction finished. Error = {0}.", _error));
//
//		foreach (BillingTransaction transaction in _finishedTransactions)
//		{
//			Debug.Log(transaction.ToString());
//		}
//	}
	


}