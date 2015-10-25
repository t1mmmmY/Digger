using UnityEngine;
using System.Collections;

#if USES_BILLING && UNITY_EDITOR
using System.Collections.Generic;
using VoxelBusters.DebugPRO;

namespace VoxelBusters.NativePlugins
{	
	using Internal;

	public partial class BillingEditor : Billing 
	{
		#region API's

		protected override void Initialise (BillingSettings _settings)
		{
			EditorStore.Initialise();
		}

		public override void RequestForBillingProducts (BillingProduct[] _billingProducts)
		{
			// Cache requested products
			RequestedProducts	= _billingProducts;

			// Request store for product information
			EditorStore.RequestForBillingProducts(_billingProducts);
		}
		
		public override bool IsProductPurchased (string _productID)
		{
			bool _isPurchased	= EditorStore.IsProductPurchased(_productID);
			Console.Log(Constants.kDebugTag, string.Format("[Billing] Product= {0} IsPurchased= {1}.", _productID, _isPurchased));

			return _isPurchased;
		}
		
		public override void BuyProduct (string _productID)
		{
			EditorStore.BuyProduct(_productID);
		}
		
		public override void RestoreCompletedTransactions ()
		{
			EditorStore.RestoreCompletedTransactions();
		}

		public override void CustomVerificationFinished (BillingTransaction _transaction)
		{
			EditorStore.CustomVerificationFinished(_transaction);
		}

		#endregion
	}
}
#endif