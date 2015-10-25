using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VoxelBusters.Utility;
using VoxelBusters.NativePlugins;

namespace VoxelBusters.NativePlugins.Demo
{
#if !USES_BILLING
	public class BillingDemo : NPDisabledFeatureDemo 
	{}
#else
	public class BillingDemo : NPDemoBase 
	{
		#region Properties

		private 	int					m_productIter;
		private 	BillingProduct[] 	m_products;
		private		bool				m_productRequestFinished;

		#endregion

		#region Unity Methods

		protected override void Start ()
		{
			base.Start ();

			// Intialise
			m_products					= NPSettings.Billing.Products;
			m_productRequestFinished	= false;

			// Set info texts
			AddExtraInfoTexts(
				"You can configure this feature in NPSettings->Billing Settings.",
				"In Billing Settings you can also store all the Product information and access it at runtime using getter NPSettings.Billing.Products.",
				"Billing workflow is pretty much simple. " +
					"\n1. Request for product infomation." +
					"\n2. If your product list includes non consummable products, then restore old purchases." +
					"\n3. Initiate purchase using BuyProduct API. Also use IsPurchased API to check if product is already purchase or not.");
		}

		protected override void OnEnable ()
		{
			base.OnEnable ();

#if UNITY_ANDROID
			if (string.IsNullOrEmpty(NPSettings.Billing.Android.PublicKey))
			{
				AddNewResult ("[NOTE] Add public key in NPSettings for billing on Android. Else purchase process will be aborted.");
			}
#endif
			// Register for callbacks
			Billing.DidFinishProductsRequestEvent	+= DidFinishProductsRequestEvent;
			Billing.DidReceiveTransactionInfoEvent	+= DidReceiveTransactionInfoEvent;
		}

		protected override void OnDisable ()
		{
			base.OnDisable ();

			// Deregister for callbacks
			Billing.DidFinishProductsRequestEvent	-= DidFinishProductsRequestEvent;
			Billing.DidReceiveTransactionInfoEvent	-= DidReceiveTransactionInfoEvent;
		}
		
		#endregion

		#region GUI Methods
		
		protected override void DisplayFeatureFunctionalities ()
		{
			base.DisplayFeatureFunctionalities ();

			if (m_products.Length == 0)
			{
				GUILayout.Box ("We couldn't find any product information. Please configure.");
				return;
			}
			
			GUILayout.Label ("Product Requests", kSubTitleStyle);
			
			if (GUILayout.Button ("Request For Billing Products"))
			{
				AddNewResult ("Sending request to load product information from store.");
				RequestBillingProducts (m_products);
			}
			
			GUILayout.Box ("[NOTE] On finishing product request, DidFinishProductsRequestEvent is triggered.");
			
			if (m_productRequestFinished)
			{
				if (GUILayout.Button ("Restore Purchases"))
				{
					AddNewResult ("Sending request to restore old purchases.");
					RestoreCompletedTransactions ();
				}
				
				GUILayout.Box ("[NOTE] On finishing restore request, DidReceiveTransactionInfoEvent is triggered.");
				
				GUILayout.Label ("Product Purchases", kSubTitleStyle);
				GUILayout.Box ("Current billing product = " + GetCurrentProduct ().Name);
				
				GUILayout.BeginHorizontal ();
				{
					if (GUILayout.Button ("Previous Product"))
					{ 
						GotoPreviousProduct ();
					}
					
					if (GUILayout.Button ("Next Product"))
					{
						GotoNextProduct ();
					}
				}
				GUILayout.EndHorizontal ();
				
				if (GUILayout.Button ("Buy Product"))
				{
					AddNewResult (string.Format ("Requesting to buy product = {0}.", GetCurrentProduct().Name));
					BuyProduct (GetCurrentProduct ().ProductIdentifier);
				}
				
				GUILayout.Box ("[NOTE] On finishing product purchase request, DidReceiveTransactionInfoEvent is triggered.");
				
				if (GUILayout.Button ("Is Product Purchased"))
				{
					BillingProduct	 _product		= GetCurrentProduct ();
					bool 			_isPurchased 	= IsProductPurchased (_product.ProductIdentifier);
					
					AddNewResult (string.Format ("{0} {1}", _product.Name, _isPurchased ? "is already purchased." : "is not yet purchased!"));
				}
				
				GUILayout.Box ("[NOTE] Purchase history is tracked only for non-consumable products.");
				
				if (GUILayout.Button ("Is Consumable Product"))
				{
					BillingProduct	 _product		= GetCurrentProduct ();
					bool 			_isConsumable 	= _product.IsConsumable;
					
					AddNewResult (string.Format ("{0} {1}", _product.Name, _isConsumable ? "is consumable product." : "is non-consumable product."));
				}
			}
		}
		
		#endregion

		#region API Methods
			
		private void RequestBillingProducts (BillingProduct[]  _products)
		{
			NPBinding.Billing.RequestForBillingProducts (_products);
		}

		private void RestoreCompletedTransactions ()
		{
			NPBinding.Billing.RestoreCompletedTransactions ();
		}

		private void BuyProduct (string _productIdentifier)
		{
			NPBinding.Billing.BuyProduct (_productIdentifier);
		}

		private bool IsProductPurchased (string _productIdentifier)
		{
			return NPBinding.Billing.IsProductPurchased (_productIdentifier);
		}

		#endregion

		#region API Callback Methods

		private void DidFinishProductsRequestEvent (BillingProduct[] _regProductsList, string _error)
		{
			AddNewResult(string.Format("Billing products request finished. Error = {0}.", _error.GetPrintableString()));

			if (_regProductsList != null)
			{
				m_productRequestFinished	= true;
				AppendResult (string.Format ("Totally {0} billing products information were received.", _regProductsList.Length));

				foreach (BillingProduct _eachProduct in _regProductsList)
					AppendResult (_eachProduct.ToString());
			}
		}

		private void DidReceiveTransactionInfoEvent (BillingTransaction[] _transactionList, string _error)
		{
			AddNewResult(string.Format("Billing transaction finished. Error = {0}.", _error.GetPrintableString()));

			if (_transactionList != null)
			{				
				AppendResult (string.Format ("Count of transaction information received = {0}.", _transactionList.Length));

				foreach (BillingTransaction _eachTransaction in _transactionList)
				{
					AppendResult ("Product Identifier = " 		+ _eachTransaction.ProductIdentifier);
					AppendResult ("Transaction State = "		+ _eachTransaction.TransactionState);
					AppendResult ("Verification State = "		+ _eachTransaction.VerificationState);
					AppendResult ("Transaction Date[UTC] = "	+ _eachTransaction.TransactionDateUTC);
					AppendResult ("Transaction Date[Local] = "	+ _eachTransaction.TransactionDateLocal);
					AppendResult ("Transaction Identifier = "	+ _eachTransaction.TransactionIdentifier);
					AppendResult ("Transaction Receipt = "		+ _eachTransaction.TransactionReceipt);
					AppendResult ("Error = "					+ _eachTransaction.Error.GetPrintableString());
				}
			}
		}
		
		#endregion
	
		#region Misc. Methods
		
		private BillingProduct GetCurrentProduct ()
		{
			return m_products[m_productIter];
		}
		
		private void GotoNextProduct ()
		{
			m_productIter++;
			
			if (m_productIter >= m_products.Length)
				m_productIter	= 0;
		}
		
		private void GotoPreviousProduct ()
		{
			m_productIter--;
			
			if (m_productIter < 0)
				m_productIter	= m_products.Length - 1;
		}
		
		private int GetProductsCount ()
		{
			return m_products.Length;
		}
		
		#endregion
	}
#endif
}