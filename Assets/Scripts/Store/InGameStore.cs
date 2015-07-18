using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Soomla.Store;
using Soomla;

public class InGameStore : MonoBehaviour 
{

	void Start ()
    {
        StoreEvents.OnSoomlaStoreInitialized += OnSoomlaStoreInitialized;
        SoomlaStore.Initialize(new AndroidStore());
        StoreEvents.OnItemPurchaseStarted += OnItemPurchaseStarted;
        StoreEvents.OnItemPurchased += OnItemPurchased;
        StoreEvents.OnMarketPurchaseStarted += OnMarketPurchaseStarted;
        StoreEvents.OnMarketPurchase += OnMarketPurchase;
        StoreEvents.OnMarketPurchaseCancelled += OnMarketPurchaseCancelled;
	}

    void OnDestroy()
    {
        StoreEvents.OnSoomlaStoreInitialized -= OnSoomlaStoreInitialized;
        StoreEvents.OnItemPurchaseStarted -= OnItemPurchaseStarted;
        StoreEvents.OnItemPurchased -= OnItemPurchased;
        StoreEvents.OnMarketPurchaseStarted -= OnMarketPurchaseStarted;
        StoreEvents.OnMarketPurchase -= OnMarketPurchase;
        StoreEvents.OnMarketPurchaseCancelled -= OnMarketPurchaseCancelled;

		SoomlaStore.StopIabServiceInBg();
    }

	void OnGUI()
	{
		if (GUILayout.Button("BuySomething"))
		{
			SoomlaStore.BuyMarketItem(AndroidStore.CHARACTER_ITEM_ID, "Nice work!");
			//StoreInventory.BuyItem(AndroidStore.CHARACTER_PACK.ItemId);
		}
	}

    public void OnSoomlaStoreInitialized()
    {
		SoomlaStore.StartIabServiceInBg();
        Debug.Log("Store initialized");
        // ... your game specific implementation here ...
    }

    void OnItemPurchaseStarted(PurchasableVirtualItem item)
    {

    }

    void OnItemPurchased(PurchasableVirtualItem item, string name)
    {

    }

    void OnMarketPurchaseStarted(PurchasableVirtualItem item)
    {
        // pvi - the PurchasableVirtualItem whose purchase operation has just started

        // ... your game specific implementation here ...
    }

    public void OnMarketPurchase(PurchasableVirtualItem pvi, string payload, Dictionary<string, string> extra)
    {
        // pvi - the PurchasableVirtualItem that was just purchased
        // payload - a text that you can give when you initiate the purchase operation and
        //    you want to receive back upon completion
        // extra - contains platform specific information about the market purchase
        //    Android: The "extra" dictionary will contain: 'token', 'orderId', 'originalJson', 'signature', 'userId'
        //    iOS: The "extra" dictionary will contain: 'receiptUrl', 'transactionIdentifier', 'receiptBase64', 'transactionDate', 'originalTransactionDate', 'originalTransactionIdentifier'

        // ... your game specific implementation here ...
    }

    public void OnMarketPurchaseCancelled(PurchasableVirtualItem pvi)
    {
        // pvi - the PurchasableVirtualItem whose purchase operation was cancelled

        // ... your game specific implementation here ...
    }
	
}
