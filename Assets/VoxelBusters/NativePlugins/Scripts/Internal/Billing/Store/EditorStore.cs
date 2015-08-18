using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VoxelBusters.Utility;
using VoxelBusters.DebugPRO;

#if UNITY_EDITOR
using UnityEditor;

namespace VoxelBusters.NativePlugins.Internal
{
	public class EditorStore 
	{
		#region Properties

		private static List<BillingProduct>		registeredProducts						= new List<BillingProduct>();

		#endregion

		#region Constants

		// Event callbacks
		private const string 					kRequestForBillingProductsSuccessEvent	= "RequestForBillingProductsSuccess";
		private const string 					kBillingTransactionFinishedEvent		= "BillingTransactionFinished";

		#endregion

		#region Store API's

		public static void Initialise ()
		{
			CheckIfInitialised();
		}

		public static void RequestForBillingProducts (List<BillingProduct> _productsList)
		{
			CheckIfInitialised();

			// Remove current products
			registeredProducts.Clear();

			// Create list of registered products with price info
			int _totalProducts						= _productsList.Count;

			for (int _iter = 0; _iter < _totalProducts; _iter++)
			{
				BillingProduct _storeProduct		= _productsList[_iter];
				BillingProduct _registeredProduct	= _storeProduct.Copy();

				if (_registeredProduct != null)
				{
					_registeredProduct.LocalizedPrice	= string.Format("${0:0.00}", _registeredProduct.Price);

					// Add to store products
					registeredProducts.Add(_registeredProduct);
				}
			}

			// Callback is sent to binding event listener
			if (NPBinding.Billing != null)
				NPBinding.Billing.InvokeMethod(kRequestForBillingProductsSuccessEvent, registeredProducts);
		}
		
		public static bool IsProductPurchased (string _productID)
		{
			CheckIfInitialised();

			// Return
			return System.Convert.ToBoolean(EditorPrefs.GetInt(_productID, 0));
		}
		
		public static void BuyProduct (string _productID)
		{
			CheckIfInitialised();

			BillingProduct _buyProduct	= GetProduct(_productID);

			if (_buyProduct == null)
			{
				OnTransactionFailed(_productID, "Product ID isn't registered");
				return;
			}

			if (NPBinding.UI != null)
			{
				string _message				= string.Format("Do you want to buy {0} for {1}?", _buyProduct.Name, _buyProduct.LocalizedPrice);

				NPBinding.UI.ShowAlertDialogWithMultipleButtons("Confirm your purchase", _message, new string[]{ "Cancel", "Buy" },
				(string _buttonPressed)=>{
					if (_buttonPressed.Equals("Buy"))
					{
						OnConfirmingPurchase(_buyProduct);
					}
					else
					{
						OnTransactionFailed(_productID, "Purchase was cancelled by user");
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
				Console.LogError(Constants.kDebugTag, "[EditorStore] Restore purchases can be done only after getting products information from store");
				return;
			}

			int _totalProducts								= registeredProducts.Count;
			List<BillingTransaction> _restoredTransactions	= new List<BillingTransaction>();

			for (int _iter = 0; _iter < _totalProducts; _iter++)
			{
				BillingProduct _product	= registeredProducts[_iter];

				if (IsProductPurchased(_product.ProductIdentifier))
				{
					BillingTransaction _transaction	= GetTransactionDetails(_product.ProductIdentifier, eBillingTransactionState.RESTORED, null);

					// Add it to list of restored transactions
					_restoredTransactions.Add(_transaction);
				}
			}

			// Send callback
			SendFinishedTransactionCallback(_restoredTransactions);
		}
		
		public static void CustomVerificationFinished (BillingTransaction _transaction)
		{
			Console.LogError(Constants.kDebugTag, Constants.kErrorMessage);
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

			BillingTransaction _newTransaction			= GetTransactionDetails(_product.ProductIdentifier, eBillingTransactionState.PURCHASED, null);
			List<BillingTransaction> _transactionList	= new List<BillingTransaction>(new BillingTransaction[1] { _newTransaction });

			// Send callback
			SendFinishedTransactionCallback(_transactionList);
		}

		private static void OnTransactionFailed (string _productID, string _errorDescription)
		{
			BillingTransaction _newTransaction			= GetTransactionDetails(_productID, eBillingTransactionState.FAILED, _errorDescription);
			List<BillingTransaction> _transactionList	= new List<BillingTransaction>(new BillingTransaction[1] { _newTransaction });

			// Send callback
			SendFinishedTransactionCallback(_transactionList);
		}

		private static BillingTransaction GetTransactionDetails (string _productID, eBillingTransactionState _transactionState, string _error)
		{
			BillingTransaction _transaction;

			if (_transactionState == eBillingTransactionState.FAILED)
			{
				_transaction = new BillingTransaction(_productID, System.DateTime.MinValue, null, null, _transactionState, eBillingTransactionVerificationState.NOT_CHECKED, _error);
			}
			else
			{
				_transaction = new BillingTransaction(_productID, System.DateTime.UtcNow, "transactionIdentifier", "receipt", _transactionState, eBillingTransactionVerificationState.SUCCESS, null);
			}

			return _transaction;
		}

		private static BillingProduct GetProduct (string _productID)
		{
			int _totalProducts	= registeredProducts.Count;

			for (int _iter = 0; _iter < _totalProducts; _iter++)
			{
				if (registeredProducts[_iter].ProductIdentifier.Equals(_productID))
				{
					return registeredProducts[_iter];
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

		private static void SendFinishedTransactionCallback (List<BillingTransaction> _finishedTransaction)
		{
			if (NPBinding.Billing != null)
				NPBinding.Billing.InvokeMethod(kBillingTransactionFinishedEvent, _finishedTransaction);
		}

		#endregion
	}
}
#endif