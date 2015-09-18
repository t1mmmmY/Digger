using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VoxelBusters.Utility;
using VoxelBusters.NativePlugins;
using VoxelBusters.AssetStoreProductUtility.Demo;

namespace VoxelBusters.NativePlugins.Demo
{
	public class BillingDemo : DemoSubMenu 
	{
		#region Properties

		private int						m_productIter;
		private List<BillingProduct> 	m_products;

		#endregion

		#region Unity Methods

		protected override void Start ()
		{
			base.Start();
			m_products	= NPSettings.Billing.Products;
		}


		protected override void OnEnable ()
		{
			base.OnEnable();

			#if UNITY_ANDROID
			if (string.IsNullOrEmpty(NPSettings.Billing.Android.PublicKey))
			{
				AddNewResult("Alert : Add public key in NPSettings for billing on Android. Else purchase process will be aborted.");
			}
			#endif

			// Register for callbacks
			Billing.BillingProductsRequestFinishedEvent	+= BillingProductsRequestFinishedEvent;
			Billing.TransactionFinishedEvent			+= TransactionFinishedEvent;
		}

		protected override void OnDisable ()
		{
			base.OnDisable();

			// Deregister for callbacks
			Billing.BillingProductsRequestFinishedEvent	-= BillingProductsRequestFinishedEvent;
			Billing.TransactionFinishedEvent			-= TransactionFinishedEvent;
		}
		
		#endregion

		#region API Calls
			
		private void RequestBillingProducts(List<BillingProduct>  _products)
		{
			NPBinding.Billing.RequestForBillingProducts(_products);
		}

		private void RestoreCompletedTransactions()
		{
			NPBinding.Billing.RestoreCompletedTransactions();
		}

		private void BuyProduct(string _productIdentifier)
		{
			NPBinding.Billing.BuyProduct(_productIdentifier);
		}

		private bool IsProductPurchased(string _productIdentifier)
		{
			return NPBinding.Billing.IsProductPurchased(_productIdentifier);
		}

		#endregion


		#region API Callbacks

		private void BillingProductsRequestFinishedEvent (IList _regProductsList, string _error)
		{
			AddNewResult("Received Billing Products Request Event");

			if(string.IsNullOrEmpty(_error))
			{
				AppendResult("Received products count : " + _regProductsList.Count);
				foreach(BillingProduct _eachProduct in _regProductsList)
				{
					AppendResult(_eachProduct.ToString());
				}
			}
			else
			{
				AppendResult("Error = "+_error);
			}
		}

		private void TransactionFinishedEvent (IList _finishedTransactions)
		{
			AddNewResult("New Transaction Event Received. Transactions Received = " + _finishedTransactions.Count);

			//Getting the results of each transaction status.
			foreach(BillingTransaction _eachTransaction in _finishedTransactions)
			{

				//Product ID for which this transaction happened
				string _productIdentifier  						= _eachTransaction.ProductIdentifier;

				//Time of purchase details
				System.DateTime _transactionDateUTC 			= _eachTransaction.TransactionDateUTC;	
				System.DateTime _transactionDateLocal 			= _eachTransaction.TransactionDateLocal;

				//Transaction unique identifier
				string _transactionIdentifier					= _eachTransaction.TransactionIdentifier;

				//Receipt and original json data - Required for external Transaction validation
				string _transactionReceipt						= _eachTransaction.TransactionReceipt;

				//Fetching Transaction State and Verirification States
				eBillingTransactionState _transactionState				= _eachTransaction.TransactionState;	
				eBillingTransactionVerificationState _verificationState = _eachTransaction.VerificationState; 

				//Error description if any
				string _error									= _eachTransaction.Error;

				// Raw purchase data
				string _rawPurchaseData							= _eachTransaction.RawPurchaseData;


				if(!string.IsNullOrEmpty(_error))
				{
					AppendResult("Error Description = "				+ _error);
				}

				AppendResult("Product Identifier = " 			+ _productIdentifier);
				AppendResult("Transaction State "				+ _transactionState);
				AppendResult("Transaction Verification State "	+ _verificationState);
				AppendResult("Transaction Date[UTC] = "			+ _transactionDateUTC);
				AppendResult("Transaction Date[Local] = "		+ _transactionDateLocal);
				AppendResult("Transaction Identifier = "		+ _transactionIdentifier);
				AppendResult("Transaction Receipt = "			+ _transactionReceipt);
				AppendResult("Raw Purchase Data = "				+ _rawPurchaseData);
				
			}
		}
		
		#endregion

		#region UI
		
		protected override void OnGUIWindow()
		{		
			base.OnGUIWindow();

			RootScrollView.BeginScrollView();
			{
				GUILayout.Label("Product Requests", kSubTitleStyle);
				
				if (GUILayout.Button("RequestForBillingProducts"))
				{
					RequestBillingProducts(m_products);
				}
				
				if (GUILayout.Button("RestorePurchases"))
				{
					RestoreCompletedTransactions();
				}
				
				if (m_products.Count == 0)
				{
					GUILayout.Box("There are no billing products. Add products in NPSettings");
				}
				else
				{
					GUILayout.Label("Product Purchases", kSubTitleStyle);
					GUILayout.Box("Current Product = " + GetCurrentProduct().Name + " " + "[Products Available = " + GetProductsCount() + "]");
					
					GUILayout.BeginHorizontal();
					{
						if (GUILayout.Button("Next Product"))
						{
							GotoNextProduct();
						}
						
						if (GUILayout.Button("Previous Product"))
						{ 
							GotoPreviousProduct();
						}
					}
					GUILayout.EndHorizontal();
					
					if (GUILayout.Button("Buy"))
					{
						AddNewResult("Requesting to buy product = " + GetCurrentProduct().Name);
						BuyProduct(GetCurrentProduct().ProductIdentifier);
					}
					
					if (GUILayout.Button("IsProductPurchased"))
					{
						bool _isPurchased = IsProductPurchased(GetCurrentProduct().ProductIdentifier);
						
						AddNewResult("Is " + GetCurrentProduct().Name +  "Purchased ? " + _isPurchased);
					}
					
					if (GUILayout.Button("IsConsumableProduct"))
					{
						bool _isConsumable = GetCurrentProduct().IsConsumable;
						
						AddNewResult("Is " + GetCurrentProduct().Name +  "Consumable ? " + _isConsumable);
					}
				}
			}
			RootScrollView.EndScrollView();

			DrawResults();
			DrawPopButton();
		}
		
		#endregion
		
		#region Unexposed Methods
		
		private BillingProduct GetCurrentProduct ()
		{
			return m_products[m_productIter];
		}
		
		private void GotoNextProduct ()
		{
			m_productIter++;
			
			if (m_productIter >= m_products.Count)
				m_productIter	= 0;
		}
		
		private void GotoPreviousProduct ()
		{
			m_productIter--;
			
			if (m_productIter < 0)
				m_productIter	= m_products.Count - 1;
		}
		
		private int GetProductsCount()
		{
			return m_products.Count;
		}
		
		#endregion
	}
}