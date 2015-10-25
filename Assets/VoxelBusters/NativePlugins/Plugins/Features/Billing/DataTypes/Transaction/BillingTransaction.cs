using UnityEngine;
using System.Collections;

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
		/// <value>The string that identifies the product to the store.</value>
		public string ProductIdentifier 		
		{ 
			get; 
			protected set; 
		}

		/// <summary>
		/// Gets the transaction date in UTC time zone.
		/// </summary>
		/// <value>Transaction date in universal time zone.</value>
		public System.DateTime TransactionDateUTC 		
		{ 
			get; 
			protected set; 
		}

		/// <summary>
		/// Gets the transaction date in local timezone.
		/// </summary>
		/// <value>Transaction date in local time format.</value>
		public System.DateTime TransactionDateLocal 	
		{ 
			get; 
			protected set; 
		}
		
		/// <summary>
		/// Gets the transaction identifier.
		/// </summary>
		/// <value>Unique identifier to identify the transaction.</value>
		public string TransactionIdentifier 	
		{ 
			get; 
			protected set; 
		}

		/// <summary>
		/// Gets the transaction receipt.
		/// </summary>
		///	<remarks>
		///	\note	On iOS this alone is enough to validate a transaction. On Android along with this(signature), original json may be required if you need external validation.
		///	</remarks>
		/// <value>Receipt used to validate this transaction.</value>
		public string TransactionReceipt 		
		{ 
			get; 
			protected set; 
		}

		/// <summary>
		/// Gets the state of the transaction.
		/// </summary>
		/// <value>State of the transaction.</value>
		public eBillingTransactionState TransactionState 		
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
			protected set; 
		}

		/// <summary>
		/// Gets the error if transaction fails.
		/// </summary>
		/// <value>Description of the problem that occured during this transaction.</value>
		public string Error					
		{ 
			get; 
			protected set; 
		}

		/// <summary>
		/// Gets the purchase data.
		/// </summary>
		/// <value>The purchase data in raw format.</value>
		public string RawPurchaseData
		{
			get; 
			protected set; 
		}

		#endregion

		#region Constructor

		protected BillingTransaction ()
		{}

		#endregion

		#region Methods

		public void UpdateVerificationState (eBillingTransactionVerificationState _newState)
		{
			this.VerificationState	= _newState;
		}

		public override string ToString ()
		{
			return string.Format("[BillingTransaction: ProductIdentifier={0}, TransactionDateUTC={1}, TransactionIdentifier={2}, TransactionState={3}, VerificationState={4}, Error={5}]", 
			                     ProductIdentifier, TransactionDateUTC, TransactionIdentifier, TransactionState, VerificationState, Error);
		}
	
		#endregion
	}
}