//
//  BillingHandler.m
//  Unity-iPhone
//
//  Created by Ashwin kumar on 09/12/14.
//  Copyright (c) 2015 Voxel Busters Interactive LLP. All rights reserved.
//

#import "BillingHandler.h"
#import "SKProduct+Serialization.h"
#import "VerificationController.h"

static bool		NeedsReceiptVerfication		= YES;

@implementation BillingHandler

#define kBillingProductRequestSuccess		"RequestForBillingProductsSuccess"
#define kBillingProductRequestFailed		"RequestForBillingProductsFailed"
#define kTransactionFinishedEvent			"BillingTransactionFinished"

@synthesize productsRequest;
@synthesize consumableProductIDs;
@synthesize nonConsumableProductIDs;
@synthesize purchasedProductIDs;
@synthesize storekitProductsList;

#pragma mark - Static Methods

+ (void)VerifyTransactionReceipts:(BOOL)verifyReceipt
					  usingServer:(NSString *)serverURL
					 sharedSecret:(NSString *)sharedSecret
{
	NSLog(@"[BillingHandler] application supports receipt verification: %d", verifyReceipt);
	
	// Update flags
	NeedsReceiptVerfication	= verifyReceipt;
	
	// Set shared secret
	[VerificationController VerifyUsingURL:serverURL sharedSecret:sharedSecret];
}

#pragma mark - Init

- (id)init
{
    if ((self = [super init]))
    {
        // Observe transaction
        [[SKPaymentQueue defaultQueue] addTransactionObserver:self];
    }
    return self;
}

- (void)dealloc
{
	[[SKPaymentQueue defaultQueue] removeTransactionObserver:self];
	
    self.productsRequest       		= NULL;
    self.consumableProductIDs   	= NULL;
    self.nonConsumableProductIDs	= NULL;
    self.purchasedProductIDs   		= NULL;
    self.storekitProductsList		= NULL;
    
    [super dealloc];
}

#pragma mark - Products

- (void)setConsummabledProducts:(NSSet *)listOfConsummableProductIDs andNonConsummableProducts:(NSSet *)listOfNonConsummableProductIDs
{
	// Store product identifiers
	self.consumableProductIDs    	= listOfConsummableProductIDs;
	self.nonConsumableProductIDs 	= listOfNonConsummableProductIDs;
	
	// Check for previously purchased non-consumable products
	self.purchasedProductIDs   		= [NSMutableSet set];
	
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
	
	// Trasaction purchased and restored are tracked
	if (transactionState == SKPaymentTransactionStatePurchased || transactionState == SKPaymentTransactionStateRestored)
	{
		// If product is non consumable then add it to purchased list, to avoid repurchasing
		if ([self.nonConsumableProductIDs containsObject:productID])
		{
			// Add to already purchased item list
			[self.purchasedProductIDs addObject:productID];
			
			// Update user defaults, marking this product is purchased
			[[NSUserDefaults standardUserDefaults] setBool:YES forKey:productID];
			[[NSUserDefaults standardUserDefaults] synchronize];
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
		[[VerificationController sharedInstance] verifyPurchase:skPaymentTransaction.transaction
											  completionHandler:^(BOOL success) {
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
													  [self didFinishReceiptVerification:transactions];
													  
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

- (void)didFinishReceiptVerification:(NSArray *)transactions
{
	NSMutableArray *jsonList	= [NSMutableArray array];
	
	// Better to own transactions array
	[transactions retain];
	
	for (SKPaymentTransactionAdvanced *skPaymentTransaction in transactions)
	{
		[self finishTransaction:skPaymentTransaction];
		
		// Add to json list
		[jsonList addObject:[skPaymentTransaction toJsonObject]];
	}
	
	// Notify Unity
	NotifyEventListener(kTransactionFinishedEvent, ToJsonCString(jsonList));
	
	// Releasing ownership
	[transactions release];
}

#pragma mark - SKProductsRequestDelegate

- (void)productsRequest:(SKProductsRequest *)request didReceiveResponse:(SKProductsResponse *)response
{
	NSLog(@"[BillingHandler] store products successfully loaded");
	NSLog(@"Date %@", [NSDate date]);
	NSLog(@"Date %@", [Utility ConvertNSDateToNSString:[NSDate date]]);
	
    // Reset product request
    self.productsRequest                	= NULL;
    
    // Cache all products
    NSArray *skProducts                		= response.products;
    self.storekitProductsList           	= skProducts;
    
	NSMutableArray *storeProductsJsonList	= [NSMutableArray array];
	
	// Iterate through product details
	for (SKProduct *skProduct in skProducts)
	{
		// Add product id and localized data
		[storeProductsJsonList addObject:[skProduct toJsonObject]];
		NSLog(@"[BillingHandler] loaded product with id: %@ title: %@", skProduct.productIdentifier, skProduct.localizedTitle);
	}

    // Notify unity
	NotifyEventListener(kBillingProductRequestSuccess, ToJsonCString(storeProductsJsonList));
}

- (void)request:(SKRequest *)request didFailWithError:(NSError *)error
{
    NSLog(@"[BillingHandler] failed to load store products %@", [error description]);
    
    // Release product request
    self.productsRequest = NULL;
    
    // Notify unity
	NotifyEventListener(kBillingProductRequestFailed, [[error description] UTF8String]);
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
		SKPaymentTransactionAdvanced *transactionCopy	= [SKPaymentTransactionAdvanced initWithTransaction:transaction];
		
		// Add completed transactions
		[finishedTransactions addObject:transactionCopy];
	}
	
	// Completed transactions are moved to validating stage
	if ([finishedTransactions count] > 0)
	{
		if (NeedsReceiptVerfication)
			[self verifyTransactions:finishedTransactions];
		else
			[self didFinishReceiptVerification:finishedTransactions];
	}
}

@end