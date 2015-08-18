using UnityEngine;
using System.Collections;

#if UNITY_IOS
namespace VoxelBusters.NativePlugins
{	
	using Internal;

	public partial class BillingIOS : Billing
	{
		#region Parse Methods

		protected override void ParseProductData (IDictionary _productDict, out BillingProduct _product)
		{
			_product		= new iOSBillingProduct(_productDict);
		}

		protected override void ParseTransactionData (IDictionary _transactionDict, out BillingTransaction _transaction)
		{
			_transaction	= new iOSBillingTransaction(_transactionDict);
		}

		#endregion
	}
}
#endif