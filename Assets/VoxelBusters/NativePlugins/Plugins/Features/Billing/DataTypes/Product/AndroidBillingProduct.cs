using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VoxelBusters.Utility;

namespace VoxelBusters.NativePlugins.Internal
{
	/*
		"productId":"54units",
		"type":"inapp",
		"price":"Rs. 60.00",
		"price_amount_micros":60000000,
		"price_currency_code":"INR",
		"title":"Units_54",
		"description":"54 units"
	*/

	public sealed class AndroidBillingProduct : BillingProduct 
	{
		#region Constants

		private const string	kProductID				= "productId";
		private const string	kType					= "type";
		private const string	kLocalizedPrice			= "price";
		private const string	kPriceAmount			= "price_amount_micros";
		private const string	kPriceCurrencyCode		= "price_currency_code";
		private const string	kTitle					= "title";
		private const string	kDescription			= "description";

		#endregion

		#region Constructors
		
		public AndroidBillingProduct (IDictionary _productJsonDict)
		{
			ProductIdentifier	= _productJsonDict[kProductID] as string;
			Name				= _productJsonDict[kTitle] as string;
			Description			= _productJsonDict[kDescription] as string;
			Price				= _productJsonDict.GetIfAvailable<long>(kPriceAmount)/1000000.0f;//As the value is in microns
			LocalizedPrice		= _productJsonDict.GetIfAvailable<string>(kLocalizedPrice);
		}
		
		#endregion

		#region Static Methods

		public static IDictionary CreateJSONObject (BillingProduct _product)
		{
			IDictionary _productJsonDict		= new Dictionary<string, string>();
			_productJsonDict[kProductID]		= _product.ProductIdentifier;
			_productJsonDict[kTitle]			= _product.Name;
			_productJsonDict[kDescription]		= _product.Description;
			_productJsonDict[kPriceAmount]		= (_product.Price * 1000000).ToString();
			_productJsonDict[kLocalizedPrice]	= _product.LocalizedPrice;

			return _productJsonDict;
		}

		#endregion
	}
}