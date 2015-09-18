using UnityEngine;
using System.Collections;
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

		private List<BillingProduct>		m_requestedProducts;

		protected List<BillingProduct>		RequestedProducts
		{
			get 
			{ 
				return m_requestedProducts; 
			}

			set 
			{ 
				m_requestedProducts	= value; 
			}
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
		/// \warning Details of billing products needs to be fetched first with this method before purchasing any product.
		/// </summary>
		/// <param name="_billingProducts">List of billing products which needs details.</param>
		public virtual void RequestForBillingProducts (List<BillingProduct> _billingProducts)
		{
			if(_billingProducts == null || _billingProducts.Count == 0)
			{
				Console.LogWarning(Constants.kDebugTag, "[Billing] products list is empty");
			}

			List<string> _consumableProductIDs, _nonConsumableProductIDs;
			
			// Cache requested products details
			RequestedProducts	= _billingProducts;

			// Get consumable and non consumable product ids
			ExtractProductIDs(_billingProducts, out _consumableProductIDs, 
			                  out _nonConsumableProductIDs);

			RequestForBillingProducts(_consumableProductIDs, _nonConsumableProductIDs);
		}

		protected virtual void RequestForBillingProducts (List<string> _consumableProductIDs, List<string> _nonConsumableProductIDs)
		{}

		/// <summary>
		/// Check purchase status of specified billing product.
		/// </summary>
		/// <remarks> This works only for non-consumable/Managed billing products. For consumable, this always returns false. </remarks>
		/// <returns><c>true</c> if the specified _productID is already purchased; otherwise, <c>false</c>.</returns>
		/// <param name="_productID">Product ID to check for purchase status.</param>
		public virtual bool IsProductPurchased (string _productID)
		{
			bool _isPurchased	= false;
			Console.Log(Constants.kDebugTag, "[Billing] Product=" + _productID + " IsPurchased=" + _isPurchased);

			return _isPurchased;
		}
		
		/// <summary>
		/// Buy the specified billing product.
		/// </summary>
		///	<remarks>
		///	\note	_productID should already have billing product details ahead. see <see cref="RequestForBillingProducts"/> for getting the details.
		///	</remarks>
		/// <param name="_productID">product identifier that needs to be purchased.</param>
		public virtual void BuyProduct (string _productID)
		{}
		
		/// <summary>
		/// Restores the previous non-consumable/Managed billing products that were purchased previously, if any.
		/// \note On Android, It internally sends consumable list at the start of Billing initialisation, but if you are initializing your products runtime(not form NPSettings), make sure you call RequestBillingProdicts prior to this call. As, billing internally needs consumable list to be updated first for proper functionality.
		/// </summary>
		public virtual void RestoreCompletedTransactions ()
		{}

		/// <summary>
		/// This adds the specified billing transaction to the inventory of purchased products.
		/// </summary>
		///	<remarks>
		///	\note	iOS has external validation option. So once external validation is done, this should be called with the transaction details.
		///	On Android this call has no effect as the products are internally verified and products will be updated internally right after the purchase.
		///	</remarks>
		/// <param name="_transaction">Transaction instance thats updated with verification state after custom verification.</param>
		public virtual void CustomVerificationFinished (BillingTransaction _transaction)
		{}

		#endregion

		#region Filter Products

		protected void ExtractProductIDs (List<BillingProduct> _products, out List<string> _consumableProductIDs,
		                                  out List<string> _nonConsumableProductIDs)
		{
			// Initialise array
			_consumableProductIDs		= new List<string>();
			_nonConsumableProductIDs	= new List<string>();
			
			// Total product count
			int _totalProducts			= _products.Count;
			
			for (int _iter = 0; _iter < _totalProducts; _iter++)
			{
				BillingProduct _product	= _products[_iter];
				string _productID		= _product.ProductIdentifier;
				bool _isConsumable		= _product.IsConsumable;

				// Add products based on flag "IsConsumable" value
				if (_isConsumable)
					_consumableProductIDs.Add(_productID);
				else
					_nonConsumableProductIDs.Add(_productID);
			}
		}

		#endregion
	}
}