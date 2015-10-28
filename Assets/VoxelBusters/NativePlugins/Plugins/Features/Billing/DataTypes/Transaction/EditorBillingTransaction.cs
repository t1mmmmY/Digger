using UnityEngine;
using System.Collections;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins.Internal
{
	public sealed class EditorBillingTransaction : BillingTransaction 
	{
		#region Constructor
		
		public EditorBillingTransaction (string _productID, System.DateTime _timeUTC, string _transactionID, string _receipt, eBillingTransactionState _transactionState, eBillingTransactionVerificationState _verificationState, string _error)
		{
			ProductIdentifier		= _productID;
			TransactionDateUTC		= _timeUTC;
			TransactionDateLocal	= TransactionDateUTC.ToLocalTime();
			TransactionIdentifier	= _transactionID;
			TransactionReceipt		= _receipt;
			TransactionState		= _transactionState;
			VerificationState		= _verificationState;
			Error					= _error;
			
			// Create raw data
			RawPurchaseData			= ToJSONObject().ToJSON();
		}
		
		#endregion
		
		#region Methods
		
		private IDictionary ToJSONObject ()
		{
#if UNITY_ANDROID
			return AndroidBillingTransaction.CreateJSONObject(this);
#else
			return iOSBillingTransaction.CreateJSONObject(this);
#endif
		}
		
		#endregion
	}
}
