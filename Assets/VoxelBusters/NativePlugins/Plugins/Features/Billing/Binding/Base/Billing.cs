using UnityEngine;
using System.Collections;

#if USES_BILLING
using System.Collections.Generic;
using VoxelBusters.DebugPRO;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	/// <summary>
	/// Billing provides an unique interface to provide in-app purchases from with in the application.
	/// </summary>
	///	<description>
	///	You can set the list of billing products you will use for billing in the NPSettings editor utility or can provide details of the products runtime.
	///	</description>
	public partial class Billing : MonoBehaviour 
	{
		#region Properties

		protected BillingProduct[] RequestedProducts
		{
			get;
			set;
		}

		#endregion

		#region Unity Methods

		private void Awake ()
		{
			if (NPSettings.Application.SupportedFeatures.UsesBilling)
			{
				// Intialise component
				Initialise(NPSettings.Billing);
			}
		}

		#endregion

		#region API's

		protected virtual void Initialise (BillingSettings _settings)
		{}

		/// <summary>
		/// Request details for given list of billing products.		
		/// </summary>
		///	<remarks> 
		/// Details of billing products needs to be fetched first with this method before purchasing any product. 
		/// </remarks>
		/// <param name="_billingProducts">List of billing products which needs details.</param>
		public virtual void RequestForBillingProducts (BillingProduct[] _billingProducts)
		{
			if (_billingProducts == null || _billingProducts.Length == 0)
			{
				Console.LogWarning(Constants.kDebugTag, "[Billing] products list is empty.");
				DidReceiveBillingProducts(null, "The operation could not be completed because product list is null or empty.");
				return;
			}

			// Cache requested products details
			RequestedProducts	= _billingProducts;

			// Get consumable and non consumable product ids
			string[] _consumableProductIDs, _nonConsumableProductIDs;

			ExtractProductIDs(_billingProducts, out _consumableProductIDs, out _nonConsumableProductIDs);

			// Request for billing products
			RequestForBillingProducts(_consumableProductIDs, _nonConsumableProductIDs);
		}

		protected virtual void RequestForBillingProducts (string[] _consumableProductIDs, string[] _nonConsumableProductIDs)
		{}

		/// <summary>
		/// Check purchase status of specified billing product.
		/// </summary>
		/// <remarks> 
		/// This works only for non-consumable/Managed billing products. For consumable, this always returns false. 
		/// </remarks>
		/// <returns><c>true</c> if the specified _productID is already purchased; otherwise, <c>false</c>.</returns>
		/// <param name="_productID">Product ID to check for purchase status.</param>
		public virtual bool IsProductPurchased (string _productID)
		{
			bool _isPurchased	= false;
			Console.Log(Constants.kDebugTag, string.Format("[Billing] Product= {0} IsPurchased= {1}.", _productID, _isPurchased));

			return _isPurchased;
		}
		
		/// <summary>
		/// Buy the specified billing product.
		/// </summary>
		///	<remarks> 
		/// Product ID should already have billing product details ahead. see <see cref="RequestForBillingProducts"/> for getting the details. 
		/// </remarks>
		/// <param name="_productID">product identifier that needs to be purchased.</param>
		public virtual void BuyProduct (string _productID)
		{}
		
		/// <summary>
		/// Restores the previous non-consumable/Managed billing products that were purchased previously, if any.
		/// </summary>
		///	<remarks> 
		/// On Android, It internally sends consumable list at the start of Billing initialisation, but if you are initializing your products runtime(not form NPSettings), make sure you call RequestBillingProdicts prior to this call. 
		/// As, billing internally needs consumable list to be updated first for proper functionality.
		/// </remarks>
		public virtual void RestoreCompletedTransactions ()
		{}

		/// <summary>
		/// This adds the specified billing transaction to the inventory of purchased products.
		/// </summary>
		///	<remarks>
		///	On iOS, custom validation option is available. So once custom validation is finished, this method should be called along with the transaction details.
		///	On Android this call has no effect as the products are internally verified and products will be updated internally right after the purchase.
		///	</remarks>
		/// <param name="_transaction">Transaction instance thats updated with verification state after custom verification.</param>
		public virtual void CustomVerificationFinished (BillingTransaction _transaction)
		{}

		#endregion

		#region Filter Products

		protected void ExtractProductIDs (BillingProduct[] _products, out string[] _consumableProductIDs, out string[] _nonConsumableProductIDs)
		{
			// Initialise
			List<string> _consumableProductIDList		= new List<string>();
			List<string> _nonConsumableProductIDList	= new List<string>();

			foreach (BillingProduct _curProduct in _products)
			{
				string	_curProductID	= _curProduct.ProductIdentifier;

				// Add products based on flag "IsConsumable" value
				if (_curProduct.IsConsumable)
					_consumableProductIDList.Add(_curProductID);
				else
					_nonConsumableProductIDList.Add(_curProductID);
			}

			// Set value
			_consumableProductIDs		= _consumableProductIDList.ToArray();
			_nonConsumableProductIDs	= _nonConsumableProductIDList.ToArray();
		}

		#endregion

		#region Deprecated Methods

		[System.Obsolete("This delegate is deprecated. Instead use RequestForBillingProducts (BillingProduct[] _billingProducts).")]
		public void RequestForBillingProducts (List<BillingProduct> _billingProducts)
		{
			RequestForBillingProducts(_billingProducts == null ? null : _billingProducts.ToArray());
		}

		#endregion
	}
}
#endif