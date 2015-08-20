//
//  TwitterHandler.h
//  Unity-iPhone
//
//  Created by Ashwin kumar on 27/12/14.
//  Copyright (c) 2015 Voxel Busters Interactive LLP. All rights reserved.
//

#import "HandlerBase.h"
#import <Fabric/Fabric.h>
#import <TwitterKit/TwitterKit.h>

@interface TwitterHandler : HandlerBase

// Init
+ (void)InitTwitterKitWithConsumerKey:(NSString *)consumerKey consumerSecret:(NSString *)consumerSecret;

// Related to login
- (void)login;
- (void)logout;
- (BOOL)isLoggedIn;
- (NSString *)authToken;
- (NSString *)authTokenSecret;
- (NSString *)userID;
- (NSString *)userName;

// Tweet
- (void)showTweetComposerWithMessage:(NSString *)message URL:(NSString *)URLString image:(UIImage *)image;

// Advanced
- (void)requestAccountDetails;
- (void)requestEmailAccess;
- (void)URLRequestWithMethod:(NSString *)method URL:(NSString *)URLString parameters:(NSDictionary *)parameters;

@end
