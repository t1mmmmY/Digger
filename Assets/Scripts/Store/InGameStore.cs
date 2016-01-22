using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using VoxelBusters.NativePlugins;
using UnityEngine.UI;
using UnityEngine.Purchasing;

public class InGameStore : BaseSingleton<InGameStore>, IStoreListener
{
	[SerializeField] Button fadeButton;

	private static IStoreController m_StoreController;                                                                  // Reference to the Purchasing system.
	private static IExtensionProvider m_StoreExtensionProvider;   

#if UNITY_ANDROID
	private string kProductIDNonConsumable = "nonconsumable"; 
#endif

	private BillingProduct[] products;
	private System.Action<bool> onTransacionFinishedCallback;
	private int currentProductNumber = 0;

	protected override void Awake ()
	{
#if UNITY_ANDROID


#elif UNITY_IOS

		Billing.TransactionFinishedEvent += HandleTransactionFinishedEvent;
		Billing.BillingProductsRequestFinishedEvent += HandleBillingProductsRequestFinishedEvent;

#endif
		base.Awake ();
	}

	void Start()
	{
#if UNITY_ANDROID
//		var module = StandardPurchasingModule.Instance();

		// Create a builder, first passing in a suite of Unity provided stores.
		var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

		// Add a product to sell / restore by way of its identifier, associating the general identifier with its store-specific identifiers.
		IDs ids = new IDs();
//		List<ProductDefinition> definitions = new List<ProductDefinition>();
		foreach (string key in CONST.PLAYER_KEYS)
		{
			builder.AddProduct(key, ProductType.NonConsumable, new IDs() {key, GooglePlay.Name});
//			definitions.Add(new ProductDefinition(key, GooglePlay.Name, ProductType.NonConsumable));
//			builder.AddProduct(key, ProductType.NonConsumable);
//			ids.Add(key, AppleAppStore.Name);
//			ids.Add(key, GooglePlay.Name);
		}
//		builder.AddProducts(definitions);
//		builder.AddProduct(kProductIDNonConsumable, ProductType.NonConsumable, ids);// And finish adding the subscription product.
//		builder.AddProduct(kProductIDNonConsumable, ProductType.NonConsumable, new IDs(){{ kProductNameAppleNonConsumable,       AppleAppStore.Name },{ kProductNameGooglePlayNonConsumable,  GooglePlay.Name },});// And finish adding the subscription product.
		UnityPurchasing.Initialize(this, builder);


#elif UNITY_IOS
		products = NPSettings.Billing.Products;
		NPBinding.Billing.RequestForBillingProducts(products);
#endif
	}

	private bool IsInitialized()
	{
		// Only say we are initialized if both the Purchasing references are set.
		return m_StoreController != null && m_StoreExtensionProvider != null;
	}

	protected override void OnDestroy ()
	{
#if UNITY_ANDROID

#elif UNITY_IOS
		Billing.TransactionFinishedEvent -= HandleTransactionFinishedEvent;
		Billing.BillingProductsRequestFinishedEvent -= HandleBillingProductsRequestFinishedEvent;

#endif
		base.OnDestroy ();
	}

	public bool IsProductPurchased(int characterNumber)
	{
//#if UNITY_EDITOR
//		return true;
//#endif
		if (characterNumber >= CONST.PLAYER_KEYS.Length)
		{
			return false;
		}

		if (characterNumber == 0)
		{
			return true;
		}

#if UNITY_ANDROID
//		return PlayerStatsController.Instance.GetStatus(CONST.PLAYER_KEYS[characterNumber]) == PlayerStatus.Bought ? true : false;
		if (m_StoreController != null)
		{
			return m_StoreController.products.all[characterNumber].availableToPurchase ? false : true;
		}
		else
		{
			return PlayerStatsController.Instance.GetStatus(CONST.PLAYER_KEYS[characterNumber]) == PlayerStatus.Bought ? true : false;
		}
#elif UNITY_IOS
		return PlayerStatsController.Instance.GetStatus(products[characterNumber].ProductIdentifier) == PlayerStatus.Bought ? true : false;
#endif

	}

	public float GetProductPrice(int characterNumber)
	{
		if (characterNumber >= CONST.PLAYER_KEYS.Length)
		{
			Debug.LogError("GetProductPrice Out of range!");
			return 0;
		}

		if (characterNumber == 0)
		{
			return 0;
		}

#if UNITY_ANDROID
		if (m_StoreController != null)
		{
			return (float)m_StoreController.products.all[characterNumber].metadata.localizedPrice;
		}
		else
		{
			return 0;
		}
#elif UNITY_IOS
		return products[characterNumber].Price;
#endif

	}

	public string GetProductCurrency(int characterNumber)
	{
		if (characterNumber >= CONST.PLAYER_KEYS.Length)
		{
			return "out of range";
		}
		
		if (characterNumber == 0)
		{
			return "free";
		}

#if UNITY_ANDROID
		if (m_StoreController != null)
		{
			return m_StoreController.products.all[characterNumber].metadata.isoCurrencyCode;
		}
		else
		{
			return "";
		}
#elif UNITY_IOS
		return products[characterNumber].CurrencyCode;
#endif
		

	}

	public bool BuyProduct(int characterNumber, System.Action<bool> callback)
	{
		if (characterNumber >= CONST.PLAYER_KEYS.Length)
		{
			return false;
		}

		if (characterNumber == 0)
		{
			return false;
		}

		//Do not buy the same products twice
		if (!IsProductPurchased(characterNumber))
		{
			fadeButton.gameObject.SetActive(true);
			onTransacionFinishedCallback = callback;
			currentProductNumber = characterNumber;

#if UNITY_ANDROID
			// Buy the non-consumable product using its general identifier. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
			BuyProductID(CONST.PLAYER_KEYS[characterNumber]);
#elif UNITY_IOS
			NPBinding.Billing.BuyProduct(products[characterNumber].ProductIdentifier);
#endif
			return true;
		}

		return false;
	}


	void BuyProductID(string productId)
	{
		// If the stores throw an unexpected exception, use try..catch to protect my logic here.
		try
		{
			// If Purchasing has been initialized ...
			if (IsInitialized())
			{
				// ... look up the Product reference with the general product identifier and the Purchasing system's products collection.
				Product product = m_StoreController.products.WithID(productId);

				// If the look up found a product for this device's store and that product is ready to be sold ... 
				if (product != null && product.availableToPurchase)
				{
					Debug.Log (string.Format("Purchasing product asychronously: '{0}'", product.definition.id));// ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
					m_StoreController.InitiatePurchase(product);
				}
				// Otherwise ...
				else
				{
					// ... report the product look-up failure situation  
					Debug.Log ("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
				}
			}
			// Otherwise ...
			else
			{
				// ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or retrying initiailization.
				Debug.Log("BuyProductID FAIL. Not initialized.");
			}
		}
		// Complete the unexpected exception handling ...
		catch (Exception e)
		{
			// ... by reporting any unexpected exception for later diagnosis.
			Debug.Log ("BuyProductID: FAIL. Exception during purchase. " + e);
		}
	}

	public void RestoreCompletedTransactions()
	{
#if UNITY_ANDROID
		if (!IsInitialized())
		{
			// ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
			Debug.Log("RestorePurchases FAIL. Not initialized.");
			return;
		}

		// If we are running on an Apple device ... 
		if (Application.platform == RuntimePlatform.IPhonePlayer || 
			Application.platform == RuntimePlatform.OSXPlayer)
		{
			// ... begin restoring purchases
			Debug.Log("RestorePurchases started ...");

			// Fetch the Apple store-specific subsystem.
			var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
			// Begin the asynchronous process of restoring purchases. Expect a confirmation response in the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
			apple.RestoreTransactions((result) => {
				// The first phase of restoration. If no more responses are received on ProcessPurchase then no purchases are available to be restored.
				Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
			});
		}
		// Otherwise ...
		else
		{
			// We are not running on an Apple device. No work is necessary to restore purchases.
			Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
		}

#elif UNITY_IOS
		NPBinding.Billing.RestoreCompletedTransactions();
		NPBinding.Billing.RequestForBillingProducts(products);

		//without standart digger
		for (int i = 1; i < products.Length; i++)
		{
			PlayerStatsController.Instance.SetStatus(i, PlayerStatus.NotBought);
		}
		Debug.Log("Restore products");
#endif


	}


	void HandleBillingProductsRequestFinishedEvent (List<BillingProduct> _regProductsList, string _error)
	{
//		if (_error != "")
//		{
//			Debug.LogError("Request products error " + _error);
//		}

		for (int i = 0; i < _regProductsList.Count; i++)
		{
			products[i] = FindProductById(_regProductsList, products[i].ProductIdentifier);
			Debug.Log("product " + i.ToString() + " = " + products[i].ToString());
		}

	}

	BillingProduct FindProductById(List<BillingProduct> allProducts, string id)
	{
		foreach (BillingProduct bp in allProducts)
		{
			if (bp.ProductIdentifier == id)
			{
				return bp;
			}
		}

		return null;
	}

	void HandleTransactionFinishedEvent (List<BillingTransaction> _finishedTransactions)
	{
		fadeButton.gameObject.SetActive(false);

		// This is the extra code that needs to be included, as a temp fix for handling this issue
//		foreach(BillingTransaction _eachTransaction in _finishedTransactions)
//		{
//			_eachTransaction.VerificationState = eBillingTransactionVerificationState.SUCCESS;
//			
//			// marking verification is successful, on doing so this transaction will added to purchase history 
//			NPBinding.Billing.CustomVerificationFinished(_eachTransaction);
//		}
		
		foreach (BillingTransaction transaction in _finishedTransactions)
		{
			Debug.Log(transaction.ToString());
//			if (transaction.ProductIdentifier == products[currentProductNumber].ProductIdentifier)
//			{
			bool isSuccess = false;

			if (transaction.TransactionState == eBillingTransactionState.PURCHASED)
			{
				isSuccess = true;
//				NPBinding.Billing.RequestForBillingProducts(products);

				PlayerStatsController.Instance.SetStatus(currentProductNumber, PlayerStatus.Bought);
			}

			if (onTransacionFinishedCallback != null)
			{
				onTransacionFinishedCallback(isSuccess);
			}
//			}
		}

		NPBinding.Billing.RequestForBillingProducts(products);
		

	}





	//  
	// --- IStoreListener
	//

	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		// Purchasing has succeeded initializing. Collect our Purchasing references.
		Debug.Log("OnInitialized: PASS");

		// Overall Purchasing system, configured with products for this application.
		m_StoreController = controller;
		// Store specific subsystem, for accessing device-specific store features.
		m_StoreExtensionProvider = extensions;
	}


	public void OnInitializeFailed(InitializationFailureReason error)
	{
		// Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
		Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
	}


	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) 
	{
		fadeButton.gameObject.SetActive(false);


		// A consumable product has been purchased by this user.
//		if (String.Equals(args.purchasedProduct.definition.id, kProductIDConsumable, StringComparison.Ordinal))
//		{
//			Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));//If the consumable item has been successfully purchased, add 100 coins to the player's in-game score.
//			ScoreManager.score += 100;
//		}

		// Or ... a non-consumable product has been purchased by this user.
		if (String.Equals(args.purchasedProduct.definition.id, kProductIDNonConsumable, StringComparison.Ordinal))
		{
			Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
		}// Or ... a subscription product has been purchased by this user.
//		else if (String.Equals(args.purchasedProduct.definition.id, kProductIDSubscription, StringComparison.Ordinal))
//		{
//			Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
//		}// Or ... an unknown product has been purchased by this user. Fill in additional products here.
		else 
		{
			Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
		}// Return a flag indicating wither this product has completely been received, or if the application needs to be reminded of this purchase at next app launch. Is useful when saving purchased products to the cloud, and when that save is delayed.

		if (onTransacionFinishedCallback != null)
		{
			onTransacionFinishedCallback(true);
		}
		PlayerStatsController.Instance.SetStatus(currentProductNumber, PlayerStatus.Bought);


		return PurchaseProcessingResult.Complete;
	}


	public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
	{
		fadeButton.gameObject.SetActive(false);

		if (onTransacionFinishedCallback != null)
		{
			onTransacionFinishedCallback(false);
		}

		// A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing this reason with the user.
		Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}",product.definition.storeSpecificId, failureReason));}




//	void DidFinishProductsRequestEvent (BillingProduct[] _regProductsList, string _error)
//	{
//		Debug.Log(string.Format("Billing products request finished. Error = {0}.", _error));
//		
//		if (_regProductsList != null)
//		{
//			foreach (BillingProduct _eachProduct in _regProductsList)
//			{
//				Debug.Log(_eachProduct.ToString());
//			}
//		}
//	}
//
//	void DidReceiveTransactionInfoEvent (BillingTransaction[] _finishedTransactions, string _error)
//	{
//		Debug.Log(string.Format("Billing transaction finished. Error = {0}.", _error));
//
//		foreach (BillingTransaction transaction in _finishedTransactions)
//		{
//			Debug.Log(transaction.ToString());
//		}
//	}
	


}