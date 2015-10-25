using UnityEngine;
using System.Collections;

#if USES_BILLING
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
		///	The callback delegate used when products request finishes.
		///	</summary>
		///	<param name="_regProductsList"> List of billing products with detailed information.</param>
		/// <param name="_error">If the operation was successful, this value is nil; otherwise, this parameter holds the description of the problem that occurred.</param>
		public delegate void ProductsRequestCompletion (BillingProduct[] _regProductsList, string _error);

		///	<summary>
		/// The callback delegate used when transaction is finished.
		///	</summary>
		///	<param name="_finishedTransactions"> List of billing transactions. This can be multiple for restore purchases. Rest of the times it will be a single transaction.</param>
		/// <param name="_error">If the operation was successful, this value is nil; otherwise, this parameter holds the description of the problem that occurred.</param>
		public delegate void TransactionResponse (BillingTransaction[] _finishedTransactions, string _error);

		#endregion

		#region Events
		
		/// <summary>
		/// Called when billing products request finished.
		/// </summary>
		public static event ProductsRequestCompletion DidFinishProductsRequestEvent;

		/// <summary>
		/// Called when transaction is finished. See <see cref="eBillingTransactionState"/> for the status of the transaction.
		/// </summary>
		public static event TransactionResponse DidReceiveTransactionInfoEvent;
		
		#endregion

		#region Products Callback Methods

		protected virtual void DidReceiveBillingProducts (string _dataStr)
		{}
		
		protected void DidReceiveBillingProducts (BillingProduct[] _regProductsList, string _error)
		{
			Console.Log(Constants.kDebugTag, "[Billing] Request for billing products finished successfully.");

			// Update product information, refering to product details used for requesting
			UpdateProductInfomation(_regProductsList);

			// Backward compatibility event support
#pragma warning disable
			if (BillingProductsRequestFinishedEvent != null)
				BillingProductsRequestFinishedEvent(_regProductsList == null ? null : new List<BillingProduct>(_regProductsList), null);
#pragma warning restore

			// Event triggered
			if (DidFinishProductsRequestEvent != null)
				DidFinishProductsRequestEvent(_regProductsList, _error);
		}

		#endregion

		#region Transaction Callback Methods
		
		protected virtual void DidFinishBillingTransaction (string _dataStr)
		{}
		
		protected void DidFinishBillingTransaction (BillingTransaction[] _finishedTransactions, string _error)
		{
			Console.Log(Constants.kDebugTag, "[Billing] Billing transaction finished");
		
			// Backward compatibility event support
#pragma warning disable
			if (TransactionFinishedEvent != null)
				TransactionFinishedEvent(_finishedTransactions == null ? null : new List<BillingTransaction>(_finishedTransactions));
#pragma warning restore

			// Event triggered
			if (DidReceiveTransactionInfoEvent != null)
				DidReceiveTransactionInfoEvent(_finishedTransactions, _error);
		}

		#endregion

		#region Misc. Methods
		
		private void UpdateProductInfomation (BillingProduct[] _regProductsList)
		{
			if (_regProductsList == null)
				return;

			foreach (MutableBillingProduct _regProduct in _regProductsList)
			{
				int 	_productIndex	= System.Array.FindIndex(RequestedProducts, (BillingProduct _curProduct) => _curProduct.ProductIdentifier.Equals(_regProduct.ProductIdentifier));
				
				// Update product information by referring to requested products
				if (_productIndex != -1)
					_regProduct.SetIsConsumable(RequestedProducts[_productIndex].IsConsumable);
			}
		}
		
		#endregion

		#region Deprecated Events

		[System.Obsolete("This delegate is deprecated. Instead use ProductsRequestCompletion.")]
		public delegate void BillingProductsRequestCompletion (List<BillingProduct> _regProductsList, string _error);

		[System.Obsolete("This delegate is deprecated. Instead use TransactionResponse.")]
		public delegate void TransactionCompletion (List<BillingTransaction> _finishedTransactions);

		[System.Obsolete("This callback event is deprecated. Instead use DidFinishProductsRequestEvent.")]
		public static event BillingProductsRequestCompletion BillingProductsRequestFinishedEvent;

		[System.Obsolete("This callback event is deprecated. Instead use DidReceiveTransactionInfoEvent.")]
		public static event TransactionCompletion TransactionFinishedEvent;

		#endregion
	}
}
#endif