//
//  SKPaymentTransactionAdvanced.h
//  NativePluginsIOSWorkspace
//
//  Created by Ashwin kumar on 31/01/15.
//  Copyright (c) 2015 Voxel Busters Interactive LLP. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>

enum SKPaymentTransactionVerificationState
{
	SKPaymentTransactionVerificationStateNotChecked,
	SKPaymentTransactionVerificationStateSuccess,
	SKPaymentTransactionVerificationStateFailed
};
typedef enum SKPaymentTransactionVerificationState SKPaymentTransactionVerificationState;

@interface SKPaymentTransactionAdvanced : NSObject

// Properties
@property(nonatomic, retain)	SKPaymentTransaction						*transaction;
@property(nonatomic)			SKPaymentTransactionVerificationState		verificationState;

// Static methods
+ (id)CreateTransaction:(SKPaymentTransaction *)transaction;

// Related to conversion
- (id)toJsonObject;
- (const char *)toCString;

@end
