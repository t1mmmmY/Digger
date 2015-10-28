using UnityEngine;
using System.Collections;

#if USES_BILLING && UNITY_EDITOR
using UnityEditor;
using System;
using System.Collections.Generic;
using VoxelBusters.Utility;
using VoxelBusters.DebugPRO;
using Console	= VoxelBusters.DebugPRO.Console;

namespace VoxelBusters.NativePlugins.Internal
{
	public class EditorStore 
	{
		#region Constants
		
		// Event names
		private 	const 	string 		kDidReceiveBillingProductsEventName		= "DidReceiveBillingProducts";
		private 	const 	string 		kDidFinishBillingTransactionEventName	= "DidFinishBillingTransaction";
		
		#endregion

		#region Properties

		private 	static 	BillingProduct[]	registeredProducts				= new EditorBillingProduct[0];

		#endregion

		#region Exposed Methods

		public static void Initialise ()
		{
			CheckIfInitialised();
		}

		public static void RequestForBillingProducts (BillingProduct[] _productsList)
		{
			CheckIfInitialised();

			if (_productsList == null)
			{
				registeredProducts	= new EditorBillingProduct[0];

				// Trigger handler
				OnFinishedProductsRequest(null, "The operation could not be completed beacuse product list is null.");
				return;
			}
			else
			{
				// Create new registered product list
				List<EditorBillingProduct> _newlyRegisteredProductList	= new List<EditorBillingProduct>();

				// Create list of registered products with price info
				foreach (BillingProduct _curProduct in _productsList)
				{
					if (_curProduct != null)
					{
						EditorBillingProduct 	_newRegProduct	= new EditorBillingProduct(_curProduct);

						_newRegProduct.SetLocalizePrice(string.Format("${0:0.00}", _curProduct.Price));
						_newRegProduct.SetCurrencyCode("USD");
						_newRegProduct.SetCurrencySymbol("$");

						// Add to store products
						_newlyRegisteredProductList.Add(_newRegProduct);
					}
				}

				// Cache new list
				registeredProducts	= _newlyRegisteredProductList.ToArray();

				// Trigger handler
				OnFinishedProductsRequest(registeredProducts, null);
				return;
			}
		}

		private static void OnFinishedProductsRequest (BillingProduct[] _registeredProducts, string _error)
		{
			// Callback is sent to binding event listener
			if (NPBinding.Billing != null)
				NPBinding.Billing.InvokeMethod(kDidReceiveBillingProductsEventName, new object[] {
					_registeredProducts,
					_error
				}, new Type[] {
					typeof(BillingProduct[]),
					typeof(string)
				});
		}
		
		public static bool IsProductPurchased (string _productID)
		{
			CheckIfInitialised();

			return System.Convert.ToBoolean(EditorPrefs.GetInt(_productID, 0));
		}
		
		public static void BuyProduct (string _productID)
		{
			CheckIfInitialised();

			BillingProduct _buyProduct	= GetProduct(_productID);

			if (_buyProduct == null)
			{
				OnTransactionFailed(_productID, "The operation could not be completed because given product id information not found.");
				return;
			}

			if (NPBinding.UI != null)
			{
				string 		_message	= string.Format("Do you want to buy {0} for {1}?", _buyProduct.Name, _buyProduct.LocalizedPrice);

				NPBinding.UI.ShowAlertDialogWithMultipleButtons("Confirm your purchase", _message, new string[]{ "Cancel", "Buy" },
				(string _buttonPressed)=>{
					if (_buttonPressed.Equals("Buy"))
					{
						OnConfirmingPurchase(_buyProduct);
					}
					else
					{
						OnTransactionFailed(_productID, "The operation could not be completed because user cancelled purchase.");
					}
				});
			}
			else
			{
				Console.LogWarning(Constants.kDebugTag, "[EditorStore] Native UI component is null");
				return;
			}
		}
		
		public static void RestoreCompletedTransactions ()
		{
			CheckIfInitialised();

			if (registeredProducts == null)
			{
				Console.LogError(Constants.kDebugTag, "[EditorStore] Restore purchases can be done only after getting products information from store.");
				return;
			}

			List<BillingTransaction> 	_restoredTransactions	= new List<BillingTransaction>();

			foreach (BillingProduct _curProduct in registeredProducts)
			{
				if (IsProductPurchased(_curProduct.ProductIdentifier))
				{
					BillingTransaction _transaction	= GetTransactionDetails(_curProduct.ProductIdentifier, eBillingTransactionState.RESTORED, null);

					// Add it to list of restored transactions
					_restoredTransactions.Add(_transaction);
				}
			}

			// Send callback
			SendFinishedTransactionCallback(_restoredTransactions.ToArray());
		}
		
		public static void CustomVerificationFinished (BillingTransaction _transaction)
		{
			Console.LogError(Constants.kDebugTag, Constants.kFeatureNotSupported);
		}

		#endregion

		#region Misc Methods

		private static void OnConfirmingPurchase (BillingProduct _product)
		{
			// Non consummable purchases are tracked
			if (!_product.IsConsumable)
			{
				EditorPrefs.SetInt(_product.ProductIdentifier, 1);
			}

			BillingTransaction 		_newTransaction		= GetTransactionDetails(_product.ProductIdentifier, eBillingTransactionState.PURCHASED, null);

			// Send callback
			SendFinishedTransactionCallback(new BillingTransaction[1] { 
				_newTransaction 
			});
		}

		private static void OnTransactionFailed (string _productID, string _errorDescription)
		{
			BillingTransaction 		_newTransaction		= GetTransactionDetails(_productID, eBillingTransactionState.FAILED, _errorDescription);

			// Send callback
			SendFinishedTransactionCallback(new BillingTransaction[1] { 
				_newTransaction 
			});
		}

		private static BillingTransaction GetTransactionDetails (string _productID, eBillingTransactionState _transactionState, string _error)
		{
			BillingTransaction _transaction;

			if (_transactionState == eBillingTransactionState.FAILED)
			{
				_transaction = new EditorBillingTransaction(_productID, System.DateTime.MinValue, null, null, _transactionState, eBillingTransactionVerificationState.NOT_CHECKED, _error);
			}
			else
			{
				_transaction = new EditorBillingTransaction(_productID, System.DateTime.UtcNow, "transactionIdentifier", "receipt", _transactionState, eBillingTransactionVerificationState.SUCCESS, null);
			}

			return _transaction;
		}

		private static BillingProduct GetProduct (string _productID)
		{
			foreach (BillingProduct _curProduct in registeredProducts)
			{
				if (_curProduct.ProductIdentifier.Equals(_productID))
				{
					return _curProduct;
				}
			}

			return null;
		}

		private static void CheckIfInitialised ()
		{
#if UNITY_ANDROID
			if (string.IsNullOrEmpty(NPSettings.Billing.Android.PublicKey))
			{
				Console.LogError(Constants.kDebugTag, "[EditorStore] Please add public key in NPSettings for Billing to work on Android.");
			}
#endif
		}

		private static void SendFinishedTransactionCallback (BillingTransaction[] _finishedTransaction)
		{
			if (NPBinding.Billing != null)
				NPBinding.Billing.InvokeMethod(kDidFinishBillingTransactionEventName, new object[] {
					_finishedTransaction,
					null
				}, new Type[] {
					typeof(BillingTransaction[]),
					typeof(string)
				});
		}

		#endregion
	}
}
#endif