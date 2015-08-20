//
//  TwitterHandler.m
//  Unity-iPhone
//
//  Created by Ashwin kumar on 27/12/14.
//  Copyright (c) 2015 Voxel Busters Interactive LLP. All rights reserved.
//

#import "TwitterHandler.h"
#import "TWTRSession+Additions.h"
#import "TWTRUser+Additions.h"

@implementation TwitterHandler

#define kTWTRLoginSuccess				"TwitterLoginSuccess"
#define kTWTRLoginFailed				"TwitterLoginFailed"
#define kDismissedTweetComposer   		"TweetComposerDismissed"
#define kRequestAccountDetailsSuccess	"RequestAccountDetailsSuccess"
#define kRequestAccountDetailsFailed  	"RequestAccountDetailsFailed"
#define kEmailAccesSuccess    			"RequestEmailAccessSuccess"
#define kEmailAccessFailed      		"RequestEmailAccessFailed"
#define kTwitterURLRequestSuccess     	"TwitterURLRequestSuccess"
#define kTwitterURLRequestFailed      	"TwitterURLRequestFailed"

#pragma mark - Init

+ (void)InitTwitterKitWithConsumerKey:(NSString *)consumerKey consumerSecret:(NSString *)consumerSecret
{
    NSLog(@"[TwitterHandler] initialising with consumer key %@ secret key %@", consumerKey, consumerSecret);
	
	if (consumerKey == NULL || consumerSecret == NULL)
	{
		NSLog(@"[TwitterHandler] failed initialising twitterkit");
		return;
	}
	
    // Set consumer key and secret key
    [TwitterKit startWithConsumerKey:consumerKey consumerSecret:consumerSecret];
	
    // Initalises the component
    [Fabric with:@[TwitterKit]];
}

#pragma mark - Login

- (void)login
{
    NSLog(@"[TwitterHandler] login request received");
    
    [TwitterKit logInWithCompletion:^(TWTRSession *session, NSError *error) {

        // Check for errors
        if (error == NULL)
		{
			const char* sessionJsonStr	= [session toCString];
			NSLog(@"[TwitterHandler] login successfull, username: %s", sessionJsonStr);
             
			// Notify
			NotifyEventListener(kTWTRLoginSuccess, sessionJsonStr);
		}
		else
		{
			NSLog(@"[TwitterHandler] login failed");
             
			// Notify
			[self notifyError:kTWTRLoginFailed withError:error];
		}
     }];
}

- (void)logout
{
	NSLog(@"[TwitterHandler] logged out");
	[TwitterKit logOut];
}

- (BOOL)isLoggedIn
{
	bool isLoggedIn	= ([TwitterKit session] != NULL);
    NSLog(@"[TwitterHandler] isLoggedIn: %d", isLoggedIn);
    
    return isLoggedIn;
}

- (NSString *)authToken
{
    if ([self isLoggedIn])
	{
		NSString *authToken	= [[TwitterKit session] authToken];
		NSLog(@"[TwitterHandler] auth token: %@", authToken);
		
        return authToken;
	}
    
    return NULL;
}

- (NSString *)authTokenSecret
{
    if ([self isLoggedIn])
	{
		NSString *authTokenSecret	= [[TwitterKit session] authTokenSecret];
		NSLog(@"[TwitterHandler] auth token secret: %@", authTokenSecret);
		
        return authTokenSecret;
	}
    
    return NULL;
}

- (NSString *)userID
{
    if ([self isLoggedIn])
	{
		NSString *userID	= [[TwitterKit session] userID];
		NSLog(@"[TwitterHandler] user id: %@", userID);
		
        return userID;
	}
    
    return NULL;
}

- (NSString *)userName
{
   if ([self isLoggedIn])
   {
	   NSString *userName	= [[TwitterKit session] userName];
	   NSLog(@"[TwitterHandler] username: %@", userName);
	   
	   return userName;
   }
    
    return NULL;
}

#pragma mark - Tweet

- (void)showTweetComposerWithMessage:(NSString *)message URL:(NSString *)URLString image:(UIImage *)image
{
    TWTRComposer *composer = [[[TWTRComposer alloc] init] autorelease];
    
    // Set message
    [composer setText:message];
    
    // Set URL
    if (URLString != NULL)
        [composer setURL:[NSURL URLWithString:URLString]];
    
    // Set image
    if (image != NULL)
        [composer setImage:image];
    
    // Execute
    [composer showWithCompletion:^(TWTRComposerResult result) {
        NSString *resultStr = [NSString stringWithFormat:@"%d", result];
        NSLog(@"[TwitterHandler] tweet composer dismissed, ressult %@", resultStr);
        
        // Notify
        NotifyEventListener(kDismissedTweetComposer, [resultStr UTF8String]);
    }];
}

#pragma mark - Advanced

- (void)requestAccountDetails
{
    NSLog(@"[TwitterHandler] requesting user details");
	
	// Check if user is logged in
	if (![self isLoggedIn])
	{
		// Notify
		[self notifyError:kRequestAccountDetailsFailed withError:[self createNoAuthError]];
		return;
	}
    
    [[TwitterKit APIClient] loadUserWithID:[self userID] completion:^(TWTRUser *user, NSError *error) {
		// Check for errors
        if (error == NULL)
        {
			const char* userJsonStr		= [user toCString];
            NSLog(@"[TwitterHandler] fetched user details: %s", userJsonStr);
			
            // Notify
            NotifyEventListener(kRequestAccountDetailsSuccess, userJsonStr);
        }
        else
        {
            NSLog(@"[Twitter] failed to load user details");
            
            // Notify
			[self notifyError:kRequestAccountDetailsFailed withError:error];
        }
    }];
}

- (void)requestEmailAccess
{
	// Check if user is logged in
	if (![self isLoggedIn])
	{
		// Notify
		[self notifyError:kEmailAccessFailed withError:[self createNoAuthError]];
		return;
	}
	
    TWTRShareEmailViewController *shareEmailViewController = [[TWTRShareEmailViewController alloc] initWithCompletion:^(NSString *email, NSError *error) {
        // Check for errors
        if (error == NULL)
        {
            NSLog(@"[TwitterHandler] fetched user email: %@", email);
            
            // Notify
            NotifyEventListener(kEmailAccesSuccess, [email UTF8String]);
        }
        // Got few errors
        else
        {
            NSLog(@"[TwitterHandler] failed to access user email");
            
            // Notify
			[self notifyError:kEmailAccessFailed withError:error];
        }
    }];
    
    // Present view controller where user grants permission
    [UnityGetGLViewController() presentViewController:shareEmailViewController
                                             animated:YES
                                           completion:nil];
}

- (void)URLRequestWithMethod:(NSString *)method URL:(NSString *)URLString parameters:(NSDictionary *)parameters
{
	// Check if user is logged in
	if (![self isLoggedIn])
	{
		// Notify
		[self notifyError:kTwitterURLRequestFailed withError:[self createNoAuthError]];
		return;
	}
	
    NSError         *clientError	= NULL;
    NSURLRequest    *request        = [[TwitterKit APIClient] URLRequestWithMethod:method
                                                                               URL:URLString
                                                                        parameters:parameters
                                                                             error:&clientError];
    
    if (request)
    {
        [[TwitterKit APIClient] sendTwitterRequest:request
                                        completion:^(NSURLResponse *response, NSData *data, NSError *connectionError) {
		
            // Check for errors
            if (connectionError == NULL)
            {
                NSLog(@"[TwitterHandler] request with URL: %@ finished", URLString);
                
                // Hanndle the response data
                NSError *jsonError	= NULL;
                NSDictionary *json 	= [NSJSONSerialization JSONObjectWithData:data
																	  options:0
																		error:&jsonError];
                
                // Notify
                NotifyEventListener(kTwitterURLRequestSuccess, ToJsonCString(json));
            }
            else
            {
                NSLog(@"[TwitterHandler] request with URL: %@ failed", URLString);
                
                // Notify
				[self notifyError:kTwitterURLRequestFailed withError:connectionError];
            }
        }];
    }
    else
    {
		NSLog(@"[TwitterHandler] Failed to create request");
		
		// Notify
		[self notifyError:kTwitterURLRequestFailed withError:clientError];
    }
}

#pragma mark - Misc

- (void)notifyError:(const char *)methodName withError:(NSError *)error
{
	NSString *errorDescription	= error ? [error description] : kNSStringDefault;
	NSLog(@"[TwitterHandler] error with description: %@", errorDescription);
	
	// Notify
	NotifyEventListener(methodName, [errorDescription UTF8String]);
}

- (NSError*)createNoAuthError
{
	NSError* noAuthError	= [[[NSError alloc] initWithDomain:@"TWTRErrorDomain"
													   code:TWTRErrorCodeNoAuthentication
												   userInfo:@{ NSLocalizedDescriptionKey : @"Auth token is missing. Please login before calling this API"}] autorelease];
	
	return noAuthError;
}

@end
