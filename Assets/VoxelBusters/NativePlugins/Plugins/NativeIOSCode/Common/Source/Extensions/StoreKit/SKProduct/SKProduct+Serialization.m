//
//  SKProduct+Serialization.m
//  Unity-iPhone
//
//  Created by Ashwin kumar on 06/01/15.
//  Copyright (c) 2015 Voxel Busters Interactive LLP. All rights reserved.
//

#import "SKProduct+Serialization.h"

@implementation SKProduct (Serialization)

#define kTitle				@"localized-title"
#define kProductID			@"product-identifier"
#define kDescription		@"localized-description"
#define kPrice				@"price"
#define kLocalizedPrice		@"localized-price"

- (id)toJsonObject
{
    NSMutableDictionary *jsonDict	= [NSMutableDictionary dictionary];
    jsonDict[kProductID]  			= [self productIdentifier];
    jsonDict[kTitle]     			= [self localizedTitle];
    jsonDict[kDescription] 			= [self localizedDescription];
	
	// Price
	NSNumberFormatter *numberFormatter 	= [[[NSNumberFormatter alloc] init] autorelease];
    [numberFormatter setFormatterBehavior:NSNumberFormatterBehavior10_4];
    [numberFormatter setNumberStyle:NSNumberFormatterCurrencyStyle];
    [numberFormatter setLocale:self.priceLocale];
    
	NSString *localizedPrice 		= [numberFormatter stringFromNumber:self.price];
	jsonDict[kPrice]				= self.price;
    jsonDict[kLocalizedPrice]		= localizedPrice;
	
	return jsonDict;
}

- (const char *)toCString
{
    return ToJsonCString([self toJsonObject]);
}

@end
