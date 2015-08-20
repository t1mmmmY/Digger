//
//  TwitterBinding.h
//  NativePluginIOSWorkspace
//
//  Created by Ashwin kumar on 11/01/15.
//  Copyright (c) 2015 Voxel Busters Interactive LLP. All rights reserved.
//

#import <Foundation/Foundation.h>

// Init
UIKIT_EXTERN void initTwitterKit (const char* consumerKey, const char* consumerSecret);

// Account
UIKIT_EXTERN void twitterLogin ();
UIKIT_EXTERN void twitterLogout ();
UIKIT_EXTERN bool twitterIsLoggedIn ();
UIKIT_EXTERN char* twitterGetAuthToken ();
UIKIT_EXTERN char* twitterGetAuthTokenSecret ();
UIKIT_EXTERN char* twitterGetUserID ();
UIKIT_EXTERN char* twitterGetUserName ();

// Tweet
UIKIT_EXTERN void showTweetComposer (const char* message, 	const char* URLString,
									 UInt8* imgByteArray, 	int imgByteArrayLength);

// Requests
UIKIT_EXTERN void twitterRequestAccountDetails ();
UIKIT_EXTERN void twitterRequestEmailAccess ();
UIKIT_EXTERN void twitterURLRequest (const char* methodType, 	const char* URLString,
									 const char* parameters);