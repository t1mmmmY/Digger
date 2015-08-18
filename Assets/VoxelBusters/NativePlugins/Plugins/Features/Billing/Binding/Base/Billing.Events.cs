using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VoxelBusters.Utility;
using VoxelBusters.DebugPRO;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	public partial class Billing : MonoBehaviour 
	{
		#region Delegates

		///	<summary>
		///	Use this delegate type to get callback when products request is finished.
		///	</summary>
		///	<param name="_regProductsList"> List of billing products with detailed information.</param>
		///	<param name="_error"> Error description if request billing products fail.</param>
		public delegate void BillingProductsRequestCompletion (List<BillingProduct> _regProductsList, string _error);

		///	<summary>
		///	Use this delegate type to get callback when any transaction gets finished.
		///	</summary>
		///	<param name="_finishedTransactions"> List of billing transactions. This can be multiple for restore purchases. Rest of the times it will be a single transaction.</param>
		public delegate void TransactionCompletion (List<BillingTransaction> _finishedTransactions);

		#endregion

		#region Events
		
		/// <summary>
		/// Occurs when billing products request finished.
		/// </summary>
		public static event BillingProductsRequestCompletion	BillingProductsRequestFinishedEvent;

		/// <summary>
		/// Occurs when transaction is finished and ready with a result. See <see cref="eBillingTransactionState"/> for the status of the transaction.
		/// </summary>
		public static event TransactionCompletion				TransactionFinishedEvent;
		
		#endregion

		#region Products Callback Methods

		private void RequestForBillingProductsSuccess (string _regProductsJsonStr)
		{
			// Parse message
			IList _regProductsJsonList				= JSONUtility.FromJSON(_regProductsJsonStr) as IList;
			int _regProductsCount					= _regProductsJsonList.Count;
			List<BillingProduct> _regProductsList	= new List<BillingProduct>(_regProductsCount);
			
			for (int _iter = 0; _iter < _regProductsCount; _iter++)
			{
				IDictionary _registeredProductDict	= _regProductsJsonList[_iter] as IDictionary;
				BillingProduct _registeredProduct;

				// Parse received data
				ParseProductData(_registeredProductDict, out _registeredProduct);

				// Add it to the list
				_regProductsList.Add(_registeredProduct);
			}

			// Triggers event
			RequestForBillingProductsSuccess(_regProductsList);
		}
		
		private void RequestForBillingProductsSuccess (List<BillingProduct> _regProductsList)
		{
			Console.Log(Constants.kDebugTag, "[Billing] Request for billing products finished successfully");
			
			if (BillingProductsRequestFinishedEvent != null)
			{
				// Update product information, refering to product details used for requesting
				UpdateProductInfomation(_regProductsList);
				
				// Trigger event
				BillingProductsRequestFinishedEvent(_regProductsList, null);
			}
		}
		
		private void RequestForBillingProductsFailed (string _errorDescription)
		{
			Console.Log(Constants.kDebugTag, "[Billing] Request for billing products failed, Error=" + _errorDescription);

			if (BillingProductsRequestFinishedEvent != null)
				BillingProductsRequestFinishedEvent(null, _errorDescription);
		}

		#endregion

		#region Transaction Callback Methods
		
		private void BillingTransactionFinished (string _finishedTransactionsJsonStr)
		{
			IList _finishedTransactionJsonList				= JSONUtility.FromJSON(_finishedTransactionsJsonStr) as IList;
			int _finishedTransactionCount					= _finishedTransactionJsonList.Count;
			List<BillingTransaction> _finishedTransactions	= new List<BillingTransaction>(_finishedTransactionCount);

			for (int _iter = 0; _iter < _finishedTransactionCount; _iter++)
			{
				IDictionary _transactionDict	= _finishedTransactionJsonList[_iter] as IDictionary;
				BillingTransaction _transaction;

				// Parse received data
				ParseTransactionData(_transactionDict, out _transaction);

				// Add it to the list
				_finishedTransactions.Add(_transaction);
			}

			// Triggers event
			BillingTransactionFinished(_finishedTransactions);
		}
		
		private void BillingTransactionFinished (List<BillingTransaction> _finishedTransactions)
		{
			Console.Log(Constants.kDebugTag, "[Billing] Billing transaction finished");
			
			if (TransactionFinishedEvent != null)
				TransactionFinishedEvent(_finishedTransactions);
		}

		#endregion

		#region Misc. Methods

		private void UpdateProductInfomation (List<BillingProduct> _regProductsList)
		{
			// As we were able to connect to store server, we have local description of product lets use it for presentation purpose
			int _registeredProductsCount	= _regProductsList.Count;
			int _requestedProductsCount		= RequestedProducts.Count;
			
			for (int _rIter = 0; _rIter < _registeredProductsCount; _rIter++)
			{
				BillingProduct _registedProduct	= _regProductsList[_rIter];
				string _registedProductID		= _registedProduct.ProductIdentifier;
				
				for (int _pIter	= 0; _pIter < _requestedProductsCount; _pIter++)
				{
					BillingProduct _requestedProduct	= RequestedProducts[_pIter];
					string _requestedProductID			= _requestedProduct.ProductIdentifier;
					
					// Update information
					if (_registedProductID.Equals(_requestedProductID))
					{
						_registedProduct.IsConsumable	= _requestedProduct.IsConsumable;
						break;
					}
				}
			}
		}

		#endregion

		#region Parse Methods

		protected virtual void ParseProductData (IDictionary _productDict, out BillingProduct _product)
		{
			_product		= null;
		}

		protected virtual void ParseTransactionData (IDictionary _transactionDict, out BillingTransaction _transaction)
		{
			_transaction	= null;
		}

		#endregion
	}
}