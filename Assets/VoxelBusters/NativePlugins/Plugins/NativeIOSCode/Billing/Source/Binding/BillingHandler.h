//
//  BillingHandler.h
//  Cross Platform Native Plugins
//
//  Created by Ashwin kumar on 09/12/14.
//  Copyright (c) 2015 Voxel Busters Interactive LLP. All rights reserved.
//  Credits: http://www.raywenderlich.com/21081/introduction-to-in-app-purchases-in-ios-6-tutorial
//

#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>
#import "HandlerBase.h"
#import "SKPaymentTransactionAdvanced.h"

@interface BillingHandler : HandlerBase <SKProductsRequestDelegate, SKPaymentTransactionObserver>

// Properties
@property(nonatomic, retain)    SKProductsRequest                   *productsRequest;
@property(nonatomic, retain)    NSSet                               *consumableProductIDs;
@property(nonatomic, retain)    NSSet                               *nonConsumableProductIDs;
@property(nonatomic, retain)    NSMutableSet                        *purchasedProductIDs;
@property(nonatomic, retain)    NSArray                             *storekitProductsList;

// Static
- (void)configureVerificationSettings:(BOOL)verifyReceipt
						  usingServer:(NSString *)serverURL
						 sharedSecret:(NSString *)secretKey;

// Related to products
- (void)setConsummabledProducts:(NSSet *)listOfConsummableProductIDs
	  andNonConsummableProducts:(NSSet *)listOfNonConsummableProductIDs;
- (void)requestForBillingProducts;
- (BOOL)isProductPurchased:(NSString *)productID;
- (void)buyProduct:(NSString *)productID quanity:(int)quantity;

// Related to transaction
- (void)restoreCompletedTransactions;
- (void)customVerificationFinished:(NSString *)productID
				  transactionState:(SKPaymentTransactionState)transactionState
				 verificationState:(SKPaymentTransactionVerificationState)verificationState;

@end
