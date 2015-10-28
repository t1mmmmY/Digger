//
//  NotificationHandler.m
//  Cross Platform Native Plugins
//
//  Created by Ashwin kumar on 08/01/15.
//  Copyright (c) 2015 Voxel Busters Interactive LLP. All rights reserved.
//

#import "NotificationHandler.h"
#import "UILocalNotification+Payload.h"
#import "AppController+Notification.h"

static NSString *userInfoKey	= NULL;

@interface NotificationHandler ()

@property(nonatomic)	BOOL	canSendNotifications;

@end

// Properties
@implementation NotificationHandler

#define kDidReceiveLocalNotificationEvent						"DidReceiveLocalNotification"
#define kDidReceiveRemoteNotificationEvent						"DidReceiveRemoteNotification"
#define kDidRegisterRemoteNotificationEvent						"DidRegisterRemoteNotification"
#define kDidFailRemoteNotificationRegistrationWithErrorEvent	"DidFailToRegisterRemoteNotifications"
#define kDidReceiveAppLaunchInfoEvent							"DidReceiveAppLaunchInfo"

#define kAppLaunchLocalNotification								@"launch-local-notification"
#define kAppLaunchRemoteNotification							@"launch-remote-notification"

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

+ (NSString *)GetUserInfoKey
{
	return userInfoKey;
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
	[userInfoKey release], userInfoKey 	= NULL;
	
	// Release
	[super dealloc];
}

#pragma mark - Init Methods

- (void)initialize:(NSString*)newUserInfoKey
{
	// Set key
	userInfoKey		= [newUserInfoKey retain];
	
	// Sends app launch notification
	[self sendAppLaunchInfo];
}

- (void)sendAppLaunchInfo
{
	// Can send notifications
	[self setCanSendNotifications:YES];
	
	// Send notifications received at launch
	NSMutableDictionary	*launchData	= [NSMutableDictionary dictionary];
	
	if (self.launchLocalNotification != NULL)
	{
		[launchData setObject:[self.launchLocalNotification payload] forKey:kAppLaunchLocalNotification];
		self.launchLocalNotification	= nil;
	}
	
	if (self.launchRemoteNotification != NULL)
	{
		[launchData setObject:self.launchRemoteNotification forKey:kAppLaunchRemoteNotification];
		self.launchRemoteNotification	= nil;
	}
	
	NotifyEventListener(kDidReceiveAppLaunchInfoEvent, ToJsonCString(launchData));
}

#pragma mark - Notification Type Methods

- (int)enabledNotificationTypes
{
#ifdef __IPHONE_8_0
	if (SYSTEM_VERSION_GREATER_THAN_OR_EQUAL_TO(@"8.0"))
	{
		return [[[UIApplication sharedApplication] currentUserNotificationSettings] types];
	}
	else
#endif
	{
		return [[UIApplication sharedApplication] enabledRemoteNotificationTypes];
	}
}

- (void)registerNotificationTypes:(int)notificationTypes
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

#pragma mark - Local Notification Methods

- (void)scheduleLocalNotification:(NSDictionary *)payload
{
	UILocalNotification *newNotification	= [UILocalNotification CreateLocalNotification:payload];
	
	[[UIApplication sharedApplication] scheduleLocalNotification:newNotification];
}

- (void)cancelLocalNotification:(NSString *)notificationID
{
	NSArray *scheduledNotifications			= [[UIApplication sharedApplication] scheduledLocalNotifications];
	
	for (UILocalNotification *currentNotification in scheduledNotifications)
	{
		NSDictionary *userInfo				= currentNotification.userInfo;
		NSString *scheduledNotificationID	= [userInfo objectForKey:@"np-notification-identifier"];
		
		if (scheduledNotificationID != nil && [scheduledNotificationID isEqualToString:notificationID])
		{
			[[UIApplication sharedApplication] cancelLocalNotification:currentNotification];
			break;
		}
	}
}

- (void)cancelAllLocalNotifications
{
	[[UIApplication sharedApplication] cancelAllLocalNotifications];
}

- (void)clearNotifications
{
	NSArray *scheduledNotifications	= [[UIApplication sharedApplication] scheduledLocalNotifications];
	
	// Clear notifications
	[[UIApplication sharedApplication] setApplicationIconBadgeNumber:0];
	[[UIApplication sharedApplication] cancelAllLocalNotifications];
	
	// Reschedule old notifications
	for (UILocalNotification *curNotification in scheduledNotifications)
	{
		UILocalNotification *newCopy	= [curNotification copy];
		
		[[UIApplication sharedApplication] scheduleLocalNotification:newCopy];
		
		// Release
		[newCopy release], newCopy = NULL;
	}
	
	NSLog(@"count %d", [[[UIApplication sharedApplication] scheduledLocalNotifications] count]);
}

#pragma mark - Remote Notification Methods

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

#pragma mark - Registration Callbacks

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

#pragma mark - Received Notification Callbacks

- (void)didLaunchWithLocalNotification:(NSNotification *)notification
{
	UILocalNotification *localNotification	= (UILocalNotification *)[notification userInfo];
	self.launchLocalNotification			= localNotification;
}

- (void)didReceiveLocalNotification:(NSNotification *)notification
{
	UILocalNotification *localNotification	= (UILocalNotification *)[notification userInfo];
	
	// If launch notification is same as current then ignore
	if (localNotification == self.launchLocalNotification)
		return;

	// Notify unity
	if (localNotification != NULL && self.canSendNotifications)
	{
		NotifyEventListener(kDidReceiveLocalNotificationEvent, ToJsonCString([localNotification payload]));
	}
}

- (void)didLaunchWithRemoteNotification:(NSNotification *)notification
{
	NSDictionary *payload			= (NSDictionary *)[notification userInfo];
	self.launchRemoteNotification	= payload;
}

- (void)didReceiveRemoteNotification:(NSNotification *)notification
{
	NSDictionary *payload	= (NSDictionary *)[notification userInfo];
	
	// Notify unity
	if (payload != NULL && self.canSendNotifications)
	{
		NotifyEventListener(kDidReceiveRemoteNotificationEvent, ToJsonCString(payload));
	}
}

@end