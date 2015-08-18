//
//  TwitterBinding.m
//  NativePluginIOSWorkspace
//
//  Created by Ashwin kumar on 11/01/15.
//  Copyright (c) 2015 Voxel Busters Interactive LLP. All rights reserved.
//

#import "TwitterBinding.h"
#import "TwitterHandler.h"

#pragma mark - init

void initTwitterKit (const char* consumerKey, const char* consumerSecret)
{
	// Supported only in iOS 7 and above
#ifdef UNITY_VERSION
	if (SYSTEM_VERSION_LESS_THAN(@"7.0"))
		return;
#endif
	
    [TwitterHandler InitTwitterKitWithConsumerKey:ConvertToNSString(consumerKey)
								   consumerSecret:ConvertToNSString(consumerSecret)];
}

#pragma mark - login

void twitterLogin ()
{
    [[TwitterHandler Instance] login];
}

void twitterLogout ()
{
    [[TwitterHandler Instance] logout];
}

bool twitterIsLoggedIn ()
{
    return [[TwitterHandler Instance] isLoggedIn];
}

char* twitterGetAuthToken ()
{
	NSString *authToken	= [[TwitterHandler Instance] authToken];

	if (authToken != NULL)
		return CStringCopy([authToken UTF8String]);
		
	return NULL;
}

char* twitterGetAuthTokenSecret ()
{
	NSString *authTokenSecret	= [[TwitterHandler Instance] authTokenSecret];
	
	if (authTokenSecret != NULL)
		return CStringCopy([authTokenSecret UTF8String]);
	
	return NULL;
}

char* twitterGetUserID ()
{
	NSString *userID	= [[TwitterHandler Instance] userID];
	
	if (userID != NULL)
		return CStringCopy([userID UTF8String]);
	
	return NULL;
}

char* twitterGetUserName ()
{
	NSString *userName	= [[TwitterHandler Instance] userName];
	
	if (userName != NULL)
		return CStringCopy([userName UTF8String]);
	
	return NULL;
}

#pragma mark - tweet

void showTweetComposer (const char* message, const char* URLString, UInt8* imgByteArray, int imgByteArrayLength)
{
    // Show tweet compose
    [[TwitterHandler Instance] showTweetComposerWithMessage:ConvertToNSString(message)
                                                        URL:ConvertToNSString(URLString)
                                                      image:[Utility CreateImageFromByteArray:(void*)imgByteArray
                                                                                     ofLength:imgByteArrayLength]];
}

#pragma mark - requests

void twitterRequestAccountDetails ()
{
    [[TwitterHandler Instance] requestAccountDetails];
}

void twitterRequestEmailAccess ()
{
    [[TwitterHandler Instance] requestEmailAccess];
}

void twitterURLRequest (const char* methodType, const char* URLString,
						const char* parameters)
{
    [[TwitterHandler Instance] URLRequestWithMethod:ConvertToNSString(methodType)
												URL:ConvertToNSString(URLString)
										 parameters:FromJson(parameters)];
}
