//
//  BillingBinding.h
//  Cross Platform Native Plugins
//
//  Created by Ashwin kumar on 13/01/15.
//  Copyright (c) 2015 Voxel Busters Interactive LLP. All rights reserved.
//

#import <Foundation/Foundation.h>

// Init
UIKIT_EXTERN void init (bool supportsReceiptValidation, const char* validateUsingServerURL,
						const char* sharedSecret);

// Product
UIKIT_EXTERN void requestForBillingProducts (const char* consumableProductIDs, const char* nonConsumableProductIDs);

// Purchase
UIKIT_EXTERN bool isProductPurchased (const char* productID);
UIKIT_EXTERN void buyProduct (const char* productID);
UIKIT_EXTERN void restoreCompletedTransactions ();
UIKIT_EXTERN void finishedReceiptVerification (const char* transactionID, int verificationState);