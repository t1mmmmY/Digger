using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Soomla.Store;

public class AndroidStore : IStoreAssets 
{

	public const string CHARACTER_CURRENCY_ITEM_ID = "one_character";
	public const string CHARACTER_CURRENCY_PRODUCT_ID = "one_character";
	public const string CHARACTER_ITEM_ID = "one_character";
	
    public static VirtualCurrency CHARACTER_CURRENCY = new VirtualCurrency
        (
			"Test character",										// name
			"Test character description",												// description
            CHARACTER_CURRENCY_ITEM_ID							// item id
        );

    

    public static VirtualGood SELECTED_CHARACTER = new SingleUseVG(
		"One character",                                       		// name
                "Character for real money", // description
		"one_character",                                       		// item id
		new PurchaseWithMarket(CHARACTER_CURRENCY_ITEM_ID, 1)); // the way this virtual good is purchased

    public static VirtualCurrencyPack CHARACTER_PACK = new VirtualCurrencyPack(
		"Test character",                                   // name
		"Test character description",                       // description
		"one_character",                                   // item id
        1,												// number of currencies in the pack
        CHARACTER_CURRENCY_ITEM_ID,                        // the currency associated with this pack
        new PurchaseWithMarket(CHARACTER_CURRENCY_PRODUCT_ID, 0.99)
   	);

    public static VirtualCategory GENERAL_CATEGORY = new VirtualCategory(
                "General", new List<string>(new string[] { CHARACTER_ITEM_ID })
        );


    public int GetVersion()
    {
        return 0;
        //throw new System.NotImplementedException();
    }

    public VirtualCurrency[] GetCurrencies()
    {
        return new VirtualCurrency[] { CHARACTER_CURRENCY };
        //throw new System.NotImplementedException();
    }

    public VirtualGood[] GetGoods()
    {
        return new VirtualGood[] { SELECTED_CHARACTER };
        //throw new System.NotImplementedException();
    }

    public VirtualCurrencyPack[] GetCurrencyPacks()
    {
        return new VirtualCurrencyPack[] { CHARACTER_PACK };
        //throw new System.NotImplementedException();
    }

    public VirtualCategory[] GetCategories()
    {
        return new VirtualCategory[] { GENERAL_CATEGORY };
        //throw new System.NotImplementedException();
    }
}
