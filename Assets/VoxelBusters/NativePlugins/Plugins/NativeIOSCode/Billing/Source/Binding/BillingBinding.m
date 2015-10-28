//
//  BillingBinding.m
//  Cross Platform Native Plugins
//
//  Created by Ashwin kumar on 13/01/15.
//  Copyright (c) 2015 Voxel Busters Interactive LLP. All rights reserved.
//

#import "BillingBinding.h"
#import "BillingHandler.h"

void init (bool verifyReceipt, const char* validateUsingServerURL, const char* sharedSecret)
{
	// Set needs verification
	[[BillingHandler Instance] configureVerificationSettings:verifyReceipt
												 usingServer:ConvertToNSString(validateUsingServerURL)
												sharedSecret:ConvertToNSString(sharedSecret)];
}

void requestForBillingProducts (const char* consumableProductIDs, const char* nonConsumableProductIDs)
{
	[[BillingHandler Instance] setConsummabledProducts:[NSSet setWithArray:ConvertToNSArray(consumableProductIDs)]
							 andNonConsummableProducts:[NSSet setWithArray:ConvertToNSArray(nonConsumableProductIDs)]];
	 
    // Requesting for store products
	[[BillingHandler Instance] requestForBillingProducts];
}

bool isProductPurchased (const char* productID)
{
    return [[BillingHandler Instance] isProductPurchased:ConvertToNSString(productID)];
}

void buyProduct (const char* productID)
{
    [[BillingHandler Instance] buyProduct:ConvertToNSString(productID) quanity:1];
}

void restoreCompletedTransactions ()
{
    [[BillingHandler Instance] restoreCompletedTransactions];
}

void customVerificationFinished (const char* productID, int transactionState, int verificationState)
{
	[[BillingHandler Instance] customVerificationFinished:ConvertToNSString(productID)
										 transactionState:(SKPaymentTransactionState)transactionState
										verificationState:(SKPaymentTransactionVerificationState)verificationState];
}