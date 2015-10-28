//
//  SKPaymentTransactionAdvanced.h
//  NativePluginsIOSWorkspace
//
//  Created by Ashwin kumar on 31/01/15.
//  Copyright (c) 2015 Voxel Busters Interactive LLP. All rights reserved.
//

#import "SKPaymentTransactionAdvanced.h"
#import "SKPaymentTransaction+Serialization.h"

@implementation SKPaymentTransactionAdvanced

@synthesize transaction;
@synthesize verificationState;

- (id)initWithTransaction:(SKPaymentTransaction *)skTransaction
{
	self	= [super init];
	 
	if (self)
    {
		self.transaction		= skTransaction;
		self.verificationState	= SKPaymentTransactionVerificationStateNotChecked;
	}
	
	return self;
}

- (void)dealloc
{
	self.transaction	= NULL;
	
	[super dealloc];
}

#pragma mark - Static Methods

+(id)CreateTransaction:(SKPaymentTransaction *)transaction
{
	return [[[self alloc] initWithTransaction:transaction] autorelease];
}

#pragma mark - Properties

- (void)setVerificationState:(SKPaymentTransactionVerificationState)state
{
	NSLog(@"[SKValidatedTransaction] new validation state: %d", state);
	verificationState	= state;
}

#pragma mark - Methods

#define kVerificationState	@"verification-state"

- (id)toJsonObject
{
    NSMutableDictionary *jsonDict	= [transaction toJsonObject];
	jsonDict[kVerificationState]	= [NSNumber numberWithInt:[self verificationState]];
	
	return jsonDict;
}

- (const char *)toCString
{
    return ToJsonCString([self toJsonObject]);
}

@end
