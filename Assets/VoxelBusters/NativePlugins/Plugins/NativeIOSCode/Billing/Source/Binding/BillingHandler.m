//
//  BillingHandler.mm
//  Cross Platform Native Plugins
//
//  Created by Ashwin kumar on 09/12/14.
//  Copyright (c) 2015 Voxel Busters Interactive LLP. All rights reserved.
//

#import "BillingHandler.h"
#import "SKProduct+Serialization.h"
#import "ReceiptVerificationManager.h"

@interface BillingHandler ()

// Properties
@property(nonatomic)	BOOL	needsReceiptVerfication;
@property(nonatomic)	BOOL	registeredForTransactionEvents;

@end

@implementation BillingHandler

#define kBillingProductRequestFinishedEvent		"DidReceiveBillingProducts"
#define kTransactionFinishedEvent				"DidFinishBillingTransaction"

#define kProductsKey							@"products"
#define kTransactionsKey						@"transactions"
#define kErrorKey								@"error"

@synthesize productsRequest;
@synthesize consumableProductIDs;
@synthesize nonConsumableProductIDs;
@synthesize purchasedProductIDs;
@synthesize storekitProductsList;

#pragma mark - LifeCycle Methods

- (id)init
{
    if ((self = [super init]))
    {
        // Initialize
		self.registeredForTransactionEvents	= NO;
		self.purchasedProductIDs   			= [NSMutableSet set];
    }
    return self;
}

- (void)dealloc
{
	// Unregister from callbacks
	if ([self registeredForTransactionEvents])
		[[SKPaymentQueue defaultQueue] removeTransactionObserver:self];

	// Release objects
    self.productsRequest       		= NULL;
    self.consumableProductIDs   	= NULL;
    self.nonConsumableProductIDs	= NULL;
    self.purchasedProductIDs   		= NULL;
    self.storekitProductsList		= NULL;
    
    [super dealloc];
}

#pragma mark - Products

- (void)configureVerificationSettings:(BOOL)verifyReceipt
						  usingServer:(NSString *)serverURL
						 sharedSecret:(NSString *)secretKey
{
	NSLog(@"[BillingHandler] application supports receipt verification: %d", verifyReceipt);
	
	// Update properties
	self.needsReceiptVerfication	= verifyReceipt;
	
	// Set verifier properties
	[[ReceiptVerificationManager Instance] setCustomServerURLString:serverURL];
	[[ReceiptVerificationManager Instance] setSharedSecretKey:secretKey];
}

#pragma mark - Products

- (void)setConsummabledProducts:(NSSet *)listOfConsummableProductIDs andNonConsummableProducts:(NSSet *)listOfNonConsummableProductIDs
{
	// Register as transaction observer
	if (![self registeredForTransactionEvents])
	{
		[self setRegisteredForTransactionEvents:YES];
		[[SKPaymentQueue defaultQueue] addTransactionObserver:self];
	}
	
	// Store product identifiers
	self.consumableProductIDs    	= listOfConsummableProductIDs;
	self.nonConsumableProductIDs 	= listOfNonConsummableProductIDs;
	
	// Update purchase info
	[self readPurchaseHistoryFromDisk:listOfNonConsummableProductIDs];
}

- (void)requestForBillingProducts
{
	NSLog(@"[BillingHandler] requesting billing product details");
	
	// Get single list of product id's
    NSSet *productIdentifiers       = [self.consumableProductIDs setByAddingObjectsFromSet:self.nonConsumableProductIDs];
	
	// Initialise
    self.productsRequest            = [[[SKProductsRequest alloc] initWithProductIdentifiers:productIdentifiers] autorelease];
    self.productsRequest.delegate   = self;
    
    // Start request
    [self.productsRequest start];
}

- (void)onFinishingProductsRequest:(NSArray *)storeProducts error:(NSError *)error
{
	// Reset product request
	self.productsRequest        	= NULL;
	
	// Cache products info
	self.storekitProductsList	    = storeProducts;

	// Notify Unity
	NSMutableDictionary	*dataDict	= [NSMutableDictionary dictionary];
	
	if (error != NULL)
	{
		[dataDict setObject:[error description] forKey:kErrorKey];
	}
	
	if (storeProducts != NULL)
	{
		NSMutableArray *productsJSONList	= [NSMutableArray array];
		
		// Iterate through product details
		for (SKProduct *skProduct in storeProducts)
		{
			// Add product id and localized data
			[productsJSONList addObject:[skProduct toJsonObject]];
			NSLog(@"[BillingHandler] loaded product with id: %@ title: %@", skProduct.productIdentifier, skProduct.localizedTitle);
		}
		
		[dataDict setObject:productsJSONList forKey:kProductsKey];
	}
	
	NotifyEventListener(kBillingProductRequestFinishedEvent, ToJsonCString(dataDict));
}

#pragma mark - Buy Methods

- (BOOL)isProductPurchased:(NSString *)productID
{
    bool isPurchased	= [self.purchasedProductIDs containsObject:productID];
	NSLog(@"[BillingHandler] product with id: %@ is already purchased: %d", productID, isPurchased);
	
	return isPurchased;
}

- (void)buyProduct:(NSString *)productID quanity:(int)quantity
{
    SKProduct *buyProduct = NULL;
    
    for (SKProduct *skProduct in self.storekitProductsList)
    {
        if ([productID isEqualToString:skProduct.productIdentifier])
        {
            buyProduct = skProduct;
            break;
        }
    }
    
    if (buyProduct != NULL)
    {
        NSLog(@"[BillingHandler] buying product with id: %@", productID);
        
		// Start transaction
        SKMutablePayment *payment  = [SKMutablePayment paymentWithProduct:buyProduct];
		[payment setQuantity:quantity];
		
        [[SKPaymentQueue defaultQueue] addPayment:payment];
    }
}

- (void)readPurchaseHistoryFromDisk:(NSSet *)productIDs
{
	// Clear previous information
	[self.purchasedProductIDs removeAllObjects];
	
	// Read stored info
	for (NSString *productID in productIDs)
	{
		BOOL productPurchased = [[NSUserDefaults standardUserDefaults] boolForKey:productID];
		
		if (productPurchased)
		{
			NSLog(@"[BillingHandler] previously purchased product id: %@", productID);
			[self.purchasedProductIDs addObject:productID];
		}
	}
}

- (void)updatePurchaseHistory:(NSString *)productID
			 transactionState:(SKPaymentTransactionState)transactionState
			verificationState:(SKPaymentTransactionVerificationState)verificationState
{
	// Update purchase history for successfull purchases
	if (verificationState != SKPaymentTransactionVerificationStateSuccess)
		return;
	
	// Incase if its non consumable product then track its purchase
	if (transactionState == SKPaymentTransactionStatePurchased || transactionState == SKPaymentTransactionStateRestored)
	{
		if ([self.nonConsumableProductIDs containsObject:productID])
		{
			// Add this info, only if it doesn't exist
			if (![self.purchasedProductIDs containsObject:productID])
			{
				[self.purchasedProductIDs addObject:productID];
				
				// Update user defaults, marking this product is purchased
				[[NSUserDefaults standardUserDefaults] setBool:YES forKey:productID];
				[[NSUserDefaults standardUserDefaults] synchronize];
			}
		}
	}
}

#pragma mark - Transaction

- (void)restoreCompletedTransactions
{
    NSLog(@"[BillingHandler] restore completed transactions");
    [[SKPaymentQueue defaultQueue] restoreCompletedTransactions];
}

- (void)customVerificationFinished:(NSString *)productID
				  transactionState:(SKPaymentTransactionState)transactionState
				 verificationState:(SKPaymentTransactionVerificationState)verificationState
{
	[self updatePurchaseHistory:productID
			   transactionState:transactionState
			  verificationState:verificationState];
}

- (void)finishTransaction:(SKPaymentTransactionAdvanced *)skPaymentTransaction
{
	// Better to own transactions array
	[skPaymentTransaction retain];
	
	// Remove transaction from queue
	[[SKPaymentQueue defaultQueue] finishTransaction:skPaymentTransaction.transaction];
	
	// Update purchased items info
	NSString *productID							= skPaymentTransaction.transaction.payment.productIdentifier;
	SKPaymentTransactionState transactionState	= skPaymentTransaction.transaction.transactionState;
	
	// Update purchase history
	[self updatePurchaseHistory:productID
			   transactionState:transactionState
			  verificationState:[skPaymentTransaction verificationState]];
	
	// Releasing ownership
	[skPaymentTransaction release];
}

#pragma mark - Verify Transaction

- (void)verifyTransactions:(NSArray *)transactions
{
	// Retain this transactions list, can be released once we verify all transactions
	[transactions retain];
	
	// Iterate through each transaction and verify it
	for (SKPaymentTransactionAdvanced *skPaymentTransaction in transactions)
	{
		[[ReceiptVerificationManager Instance] verifyPurchase:skPaymentTransaction.transaction:^(BOOL success) {
			
			// Check status
			if (success)
				[skPaymentTransaction setVerificationState:SKPaymentTransactionVerificationStateSuccess];
			else
				[skPaymentTransaction setVerificationState:SKPaymentTransactionVerificationStateFailed];
			
			// Check if all transaction are verified
			BOOL isFinished	= [self isReceiptVerificationFinished:transactions];
			
			if (isFinished)
			{
				// Handle finished transactions
				[self onFinishedProcessingTransaction:transactions error:nil];
				
				// Release
				[transactions release];
			}
		}];
	}
}

- (BOOL)isReceiptVerificationFinished:(NSArray *)transactions
{
	for (SKPaymentTransactionAdvanced *skPaymentTransaction in transactions)
	{
		if ([skPaymentTransaction verificationState] == SKPaymentTransactionVerificationStateNotChecked)
			return NO;
	}
	
	return YES;
}

- (void)onFinishedProcessingTransaction:(NSArray *)transactions error:(NSError *)error
{
	NSMutableDictionary	*dataDict			= [NSMutableDictionary dictionary];
	
	if (error != NULL)
	{
		[dataDict setObject:[error description] forKey:kErrorKey];
	}
	
	if (transactions != NULL)
	{
		NSMutableArray *transactionJSONList	= [NSMutableArray array];
		
		// Better to own transactions array
		[transactions retain];
		
		for (SKPaymentTransactionAdvanced *skPaymentTransaction in transactions)
		{
			[self finishTransaction:skPaymentTransaction];
			
			// Add to json list
			[transactionJSONList addObject:[skPaymentTransaction toJsonObject]];
		}
		
		// Releasing ownership
		[transactions release];
		
		// Add transaction list info
		[dataDict setObject:transactionJSONList forKey:kTransactionsKey];
	}
	
	// Notify Unity
	NotifyEventListener(kTransactionFinishedEvent, ToJsonCString(dataDict));
}

#pragma mark - SKProductsRequestDelegate

- (void)productsRequest:(SKProductsRequest *)request didReceiveResponse:(SKProductsResponse *)response
{
	NSLog(@"[BillingHandler] store products successfully loaded");
	NSLog(@"Date %@", [NSDate date]);
	NSLog(@"Date %@", [Utility ConvertNSDateToNSString:[NSDate date]]);

	// Invoke handler
	[self onFinishingProductsRequest:response.products error:nil];
}

- (void)request:(SKRequest *)request didFailWithError:(NSError *)error
{
    NSLog(@"[BillingHandler] failed to load store products %@", [error description]);
    
	// Invoke handler
	[self onFinishingProductsRequest:nil error:error];
}

#pragma mark SKPaymentTransactionObserver

- (void)paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray *)transactions
{
	NSMutableArray *finishedTransactions	= [NSMutableArray array];
	
    for (SKPaymentTransaction *transaction in transactions)
    {
		SKPaymentTransactionState transactionState	= [transaction transactionState];
		
		// Deferred transaction dont need to be completed or verified
		// Supported only in iOS 8+
#ifdef __IPHONE_8_0
		if (transactionState == SKPaymentTransactionStateDeferred)
		{
			continue;
		}
#endif
		
		// Purchasing transaction dont need to be completed or verified
		if (transactionState == SKPaymentTransactionStatePurchasing)
		{
			continue;
		}
		
		// Failed, Restored, Purchased needs verification and finished
		SKPaymentTransactionAdvanced *transactionCopy	= [SKPaymentTransactionAdvanced CreateTransaction:transaction];
		
		// Add completed transactions
		[finishedTransactions addObject:transactionCopy];
	}
	
	// Completed transactions are moved to validating stage
	if ([finishedTransactions count] > 0)
	{
		if ([self needsReceiptVerfication])
			[self verifyTransactions:finishedTransactions];
		else
			[self onFinishedProcessingTransaction:finishedTransactions error:nil];
	}
}

- (void)paymentQueue:(SKPaymentQueue *)queue restoreCompletedTransactionsFailedWithError:(NSError *)error
{
	[self onFinishedProcessingTransaction:nil error:error];
}

@end