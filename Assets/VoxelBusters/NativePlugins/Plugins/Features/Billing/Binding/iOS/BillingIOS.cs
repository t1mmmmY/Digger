using UnityEngine;
using System.Collections;

#if USES_BILLING && UNITY_IOS
using System.Collections.Generic;
using System.Runtime.InteropServices;
using VoxelBusters.Utility;
using VoxelBusters.DebugPRO;

namespace VoxelBusters.NativePlugins
{
	using Internal;

#if !USES_BILLING
	public partial class BillingIOS : Billing
	{}
#else
	public partial class BillingIOS : Billing
	{
		#region Native Methods

		[DllImport("__Internal")]
		private static extern void init (bool _supportsReceiptValidation, string _validateUsingServerURL, string _sharedSecret);

		[DllImport("__Internal")]
		private static extern void requestForBillingProducts (string _consumableProductIDs, string _nonConsumableProductIDs);
		
		[DllImport("__Internal")]
		private static extern bool isProductPurchased (string _productID);
		
		[DllImport("__Internal")]
		private static extern void buyProduct (string _productID);
		
		[DllImport("__Internal")]
		private static extern void restoreCompletedTransactions ();

		[DllImport("__Internal")]
		private static extern void customVerificationFinished (string _productID, int _transactionState, int _verificationState);

		#endregion

		#region Overriden API's

		protected override void Initialise (BillingSettings _settings)
		{
			BillingSettings.iOSSettings _iOSSettings			= _settings.iOS;
			string 						_validateUsingServerURL	= null;

			if (_iOSSettings.SupportsReceiptValidation)
			{
				// But user has forgot to set it, safe case we will use apple server
				if (string.IsNullOrEmpty(_iOSSettings.ValidateUsingServerURL))
				{
					_validateUsingServerURL	= null;
				}
				else
				{
					_validateUsingServerURL	= _iOSSettings.ValidateUsingServerURL;
				}
			}

			// Native store init is called
			init(_iOSSettings.SupportsReceiptValidation, _validateUsingServerURL, null);
		}

		protected override void RequestForBillingProducts (string[] _consumableProductIDs, string[] _nonConsumableProductIDs)
		{
			// Send request to native store
			requestForBillingProducts(_consumableProductIDs.ToJSON(), _nonConsumableProductIDs.ToJSON());
		}
		
		public override bool IsProductPurchased (string _productID)
		{
			bool _isPurchased	= isProductPurchased(_productID);
			Console.Log(Constants.kDebugTag, "[Billing] Product=" + _productID + " IsPurchased=" + _isPurchased);
			
			return _isPurchased;
		}
		
		public override void BuyProduct (string _productID)
		{
			buyProduct(_productID);
		}

		public override void RestoreCompletedTransactions ()
		{
			restoreCompletedTransactions();
		}		
		
		public override void CustomVerificationFinished (BillingTransaction _transaction)
		{
			customVerificationFinished(_transaction.ProductIdentifier, (int)_transaction.TransactionState, (int)_transaction.VerificationState);
		}

		#endregion
	}
#endif
}
#endif	