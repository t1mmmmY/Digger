//
//  NotificationHandler.m
//  Unity-iPhone
//
//  Created by Ashwin kumar on 08/01/15.
//  Copyright (c) 2015 Voxel Busters Interactive LLP. All rights reserved.
//

#import "NotificationHandler.h"
#import "UILocalNotification+Payload.h"
#import "AppController+Notification.h"

static NSString 	*keyUserInfo		= NULL;

@interface NotificationHandler ()

@property(nonatomic)	BOOL	canSendNotifications;

@end

// Properties
@implementation NotificationHandler

#define kDidReceiveLocalNotificationEvent						"DidReceiveLocalNotification"
#define kDidReceiveRemoteNotificationEvent						"DidReceiveRemoteNotification"
#define kDidRegisterRemoteNotificationEvent						"DidRegisterRemoteNotification"
#define kDidFailRemoteNotificationRegistrationWithErrorEvent	"DidFailToRegisterRemoteNotifications"

@synthesize launchLocalNotification;
@synthesize launchRemoteNotification;
@synthesize supportedNotificationTypes;

+ (void)load
{
	id instance = [self Instance];
	
	// Add observer for notification callbacks
	[[NSNotificationCenter defaultCenter] addObserver:instance
											 selector:@selector(didRegisterForRemoteNotificationsWithDeviceToken:)
												 name:kDidRegisterForRemoteNotification
											   object:Nil];
	
	[[NSNotificationCenter defaultCenter] addObserver:instance
											 selector:@selector(didFailToRegisterForRemoteNotificationsWithError:)
												 name:kDidFailToRegisterForRemoteNotification
											   object:Nil];
	
	[[NSNotificationCenter defaultCenter] addObserver:instance
											 selector:@selector(didLaunchWithLocalNotification:)
												 name:kDidLaunchWithLocalNotification
											   object:Nil];
	
	[[NSNotificationCenter defaultCenter] addObserver:instance
											 selector:@selector(didReceiveLocalNotification:)
												 name:kDidReceiveLocalNotification
											   object:Nil];
	
	[[NSNotificationCenter defaultCenter] addObserver:instance
											 selector:@selector(didLaunchWithRemoteNotification:)
												 name:kDidLaunchWithRemoteNotification
											   object:Nil];
	
	[[NSNotificationCenter defaultCenter] addObserver:instance
											 selector:@selector(didReceiveRemoteNotification:)
												 name:kDidReceiveRemoteNotification
											   object:Nil];
}

+ (void)SetKeyForUserInfo:(NSString *)value
{
	keyUserInfo	= [value retain];
}

+ (NSString *)GetKeyForUserInfo
{
	return keyUserInfo;
}

#pragma mark - Lifecycle Methods

- (id)init
{
	self	= [super init];
	
	if (self)
	{
		self.launchLocalNotification	= NULL;
		self.launchRemoteNotification	= NULL;
		self.canSendNotifications		= false;
	}

	return  self;
}

- (void)dealloc
{
	// Remove observer
	[[NSNotificationCenter defaultCenter] removeObserver:self];
	
	// Release
	self.launchLocalNotification		= NULL;
	self.launchRemoteNotification		= NULL;
	
	// Release
	[super dealloc];
}

#pragma mark - Methods

- (void)registerUserNotificationTypes:(int)notificationTypes
{
	self.supportedNotificationTypes						= notificationTypes;
	
	// ios 8+ feature
#ifdef __IPHONE_8_0
	if (SYSTEM_VERSION_GREATER_THAN_OR_EQUAL_TO(@"8.0"))
	{
		UIUserNotificationType userNotificationTypes	= (UIUserNotificationType)supportedNotificationTypes;
		UIUserNotificationSettings* mySettings	 		= [UIUserNotificationSettings settingsForTypes:userNotificationTypes categories:nil];
		
		// Settings are set
		[[UIApplication sharedApplication] registerUserNotificationSettings:mySettings];
	}
#endif
}

- (void)registerForRemoteNotifications
{
	// ios 8+ feature
#ifdef __IPHONE_8_0
	if (SYSTEM_VERSION_GREATER_THAN_OR_EQUAL_TO(@"8.0"))
	{
		[[UIApplication sharedApplication] registerForRemoteNotifications];
	}
	else
#endif
	{
		[[UIApplication sharedApplication] registerForRemoteNotificationTypes:(UIRemoteNotificationType)self.supportedNotificationTypes];
	}
}

- (void)unregisterForRemoteNotifications
{
	// Unregister
	[[UIApplication sharedApplication] unregisterForRemoteNotifications];
}

- (void)sendLaunchNotifications
{
	// Can send notifications
	[self setCanSendNotifications:YES];
	
	// Send notifications received at launch
	[self notifyReceivedLocalNotification:self.launchLocalNotification];
	[self notifyReceivedRemoteNotification:self.launchRemoteNotification];
}

- (void)notifyReceivedLocalNotification:(UILocalNotification *)localNotification;
{
	// Notify unity
	if (localNotification != NULL && self.canSendNotifications)
	{
		NotifyEventListener(kDidReceiveLocalNotificationEvent, ToJsonCString([localNotification payload]));
	}
}

- (void)notifyReceivedRemoteNotification:(NSDictionary *)remoteNotification
{
	// Notify unity
	if (remoteNotification != NULL && self.canSendNotifications)
	{
		NotifyEventListener(kDidReceiveRemoteNotificationEvent, ToJsonCString(remoteNotification));
	}
}

#pragma mark - Register notifications

- (void)didRegisterForRemoteNotificationsWithDeviceToken:(NSNotification *)notification
{
	NSString *deviceToken	= (NSString *)[notification userInfo];
	
	// Notify unity
	NotifyEventListener(kDidRegisterRemoteNotificationEvent, [deviceToken UTF8String]);
}

- (void)didFailToRegisterForRemoteNotificationsWithError:(NSNotification *)notification
{
	NSError *error			= (NSError *)[notification userInfo];
	
	// Notify unity
	NotifyEventListener(kDidFailRemoteNotificationRegistrationWithErrorEvent, [[error description] UTF8String]);
}

#pragma mark - Received notifications

- (void)didLaunchWithLocalNotification:(NSNotification *)notification
{
	UILocalNotification *localNotification	= (UILocalNotification *)[notification userInfo];
	self.launchLocalNotification			= localNotification;
}

- (void)didReceiveLocalNotification:(NSNotification *)notification
{
	UILocalNotification *localNotification	= (UILocalNotification *)[notification userInfo];
	
	// Not yet ready
	if (![self canSendNotifications])
	{
		return;
	}
	
	// Notify unity
	[self notifyReceivedLocalNotification:localNotification];
}

- (void)didLaunchWithRemoteNotification:(NSNotification *)notification
{
	NSDictionary *payload			= (NSDictionary *)[notification userInfo];
	self.launchRemoteNotification	= payload;
}

- (void)didReceiveRemoteNotification:(NSNotification *)notification
{
	NSDictionary *payload	= (NSDictionary *)[notification userInfo];
	
	// Not yet ready
	if (![self canSendNotifications])
	{
		return;
	}
	
	// Notify unity
	[self notifyReceivedRemoteNotification:payload];
}

@end
