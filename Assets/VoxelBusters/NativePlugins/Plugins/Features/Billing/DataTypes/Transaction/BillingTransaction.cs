using UnityEngine;
using System.Collections;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins
{
	using Internal;

	/// <summary>
	/// Maintains transaction information for the purchased product.
	/// </summary>
	public class BillingTransaction 
	{
		#region Properties
		/// <summary>
		/// Gets the product identifier.
		/// </summary>
		/// <value>Product identifier which uniquely defines a billing product.</value>
		public string 					ProductIdentifier 		
		{ 
			get; 
			protected set; 
		}

		/// <summary>
		/// Gets the transaction date in UTC time zone.
		/// </summary>
		/// <value>Transaction date in universal time zone</value>
		public System.DateTime 			TransactionDateUTC 		
		{ 
			get; 
			protected set; 
		}

		/// <summary>
		/// Gets the transaction date in local timezone.
		/// </summary>
		/// <value>Transaction date in local time format</value>
		public System.DateTime 			TransactionDateLocal 	
		{ 
			get; 
			protected set; 
		}
		
		/// <summary>
		/// Gets the transaction identifier.
		/// </summary>
		/// <value>Unique identifier to identify the transaction.</value>
		public string 					TransactionIdentifier 	
		{ 
			get; 
			protected set; 
		}

		/// <summary>
		/// Gets or sets the transaction receipt.
		/// </summary>
		///	<remarks>
		///	\note	On iOS this alone is enough to validate a transaction. On Android along with this(signature), original json may be required if you need external validation.
		///	</remarks>
		/// <value>Receipt used to validate this transaction.</value>
		public string					TransactionReceipt 		
		{ 
			get; 
			protected set; 
		}

		/// <summary>
		/// Gets the state of the transaction.
		/// </summary>
		/// <value>State of the transaction.</value>
		public eBillingTransactionState 			TransactionState 		
		{ 
			get; 
			protected set; 
		}

		/// <summary>
		/// Gets the state of the transaction verification.
		/// </summary>
		/// <value>Transaction verification state.</value>
		public eBillingTransactionVerificationState VerificationState		
		{ 
			get; 
			set; 
		}

		/// <summary>
		/// Gets the error if transaction fails.
		/// </summary>
		/// <value>Error description if any failure in the transaction happens.</value>
		public string					Error					
		{ 
			get; 
			protected set; 
		}

		/// <summary>
		/// Gets the purchase data.
		/// </summary>
		/// <value>The purchase data in raw format.</value>
		public string					RawPurchaseData
		{
			get; 
			protected set; 
		}

		#endregion

		#region Constructor

		protected BillingTransaction ()
		{}

		public BillingTransaction (string _productID, System.DateTime _timeUTC, string _transactionID, string _receipt, eBillingTransactionState _transactionState, eBillingTransactionVerificationState _verificationState, string _error)
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
			RawPurchaseData			= JSONObject().ToJSON();
		}

		#endregion

		#region Methods

		private IDictionary JSONObject ()
		{
#if UNITY_ANDROID
			return AndroidBillingTransaction.CreateJSONObject(this);
#else
			return iOSBillingTransaction.CreateJSONObject(this);
#endif
		}

		public override string ToString ()
		{
			return string.Format("[BillingTransaction: ProductIdentifier={0}, TransactionDateUTC={1}, TransactionDateLocal={2}, TransactionIdentifier={3}, TransactionReceipt={4}, TransactionState={5}, VerificationState={6}, Error={7}]", 
			                     ProductIdentifier, TransactionDateUTC, TransactionDateLocal, TransactionIdentifier, TransactionReceipt, TransactionState, VerificationState, Error);
		}
	
		#endregion
	}
}