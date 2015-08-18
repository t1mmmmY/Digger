using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins.Internal
{
	public sealed class iOSBillingProduct : BillingProduct 
	{
//		{
//			"localized-title": "54units",
//			"product-identifier": "54units",
//			"localized-description": "54units",
//			"price": "0",
//			"localized-price": "$0.00"
//		}

		#region Constants

		private const string kTitle				= "localized-title";
		private const string kProductID			= "product-identifier";
		private const string kDescription		= "localized-description";
		private const string kPrice				= "price";
		private const string kLocalizedPrice	= "localized-price";

		#endregion

		#region Constructors
		
		public iOSBillingProduct (IDictionary _productJsonDict)
		{
			Name				= _productJsonDict.GetIfAvailable<string>(kTitle);
			ProductIdentifier	= _productJsonDict.GetIfAvailable<string>(kProductID);
			Description			= _productJsonDict.GetIfAvailable<string>(kDescription);
			Price				= _productJsonDict.GetIfAvailable<float>(kPrice);
			LocalizedPrice		= _productJsonDict.GetIfAvailable<string>(kLocalizedPrice);
		}
		
		#endregion

		#region Static Methods
		
		public static IDictionary CreateJSONObject (BillingProduct _product)
		{
			IDictionary _productJsonDict		= new Dictionary<string, string>();
			_productJsonDict[kTitle]			= _product.Name;
			_productJsonDict[kProductID]		= _product.ProductIdentifier;
			_productJsonDict[kDescription]		= _product.Description;
			_productJsonDict[kPrice]			= _product.Price.ToString();
			_productJsonDict[kLocalizedPrice]	= _product.LocalizedPrice;
			
			return _productJsonDict;
		}
		
		#endregion
	}
}