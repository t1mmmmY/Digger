using UnityEngine;
using System.Collections;
using VoxelBusters.NativePlugins;

public class TestInApp : MonoBehaviour 
{

	void OnEnable()
	{
//		Billing.BillingProductsRequestFinishedEvent += HandleBillingProductsRequestFinishedEvent;
//		Billing.TransactionFinishedEvent += HandleTransactionFinishedEvent;
	}

	void OnDisable()
	{
	}

	void HandleTransactionFinishedEvent (System.Collections.Generic.List<BillingTransaction> _finishedTransactions)
	{
		foreach (BillingTransaction bt in _finishedTransactions)
		{
			Debug.Log(bt.ProductIdentifier);
		}
	}

	void HandleBillingProductsRequestFinishedEvent (System.Collections.Generic.List<BillingProduct> _regProductsList, string _error)
	{
		if (_error != string.Empty)
		{
			Debug.LogError("Error " + _error);
		}

		foreach (BillingProduct bt in _regProductsList)
		{
			Debug.Log(bt.ProductIdentifier);
		}
	}



	void OnGUI()
	{
		if (GUI.Button(new Rect(10, 10, Screen.width / 4, Screen.height / 4), "Buy"))
		{
			Debug.Log("Click buy");
//			NPBinding.Billing.BuyProduct(CONST.IOS_BUY_CHARACTER_ID);
		}
	}

}
