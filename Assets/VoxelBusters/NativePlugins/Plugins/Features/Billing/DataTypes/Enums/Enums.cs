using UnityEngine;
using System.Collections;

namespace VoxelBusters.NativePlugins
{
	/// <summary>
	/// State of purchased transaction
	/// </summary>
	public enum eBillingTransactionState
	{
		/// <summary> Billing product purchase transaction successful. </summary>
		PURCHASED,
		/// <summary> Billing product purchase transaction failed. </summary>
		FAILED,
		/// <summary> Billing product purchase transaction restored. This will be the state when <see cref="RestoreCompletedTransactions"/> is called. </summary>
		RESTORED,
		/// <summary> Billing product purchase transaction refunded back to the user. </summary>
		REFUNDED
	}


	/// <summary>
	/// Verification state of purchased transaction.
	/// </summary>
	public enum eBillingTransactionVerificationState
	{
		/// <summary> Transaction verification not initiated.</summary>
		NOT_CHECKED,
		/// <summary> Transaction verification is successful.</summary>
		SUCCESS,
		/// <summary> Transaction verification failed because of invalid receipt/signature.</summary>
		FAILED
	}
}