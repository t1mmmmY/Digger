//
//  NotificationHandler.h
//  Unity-iPhone
//
//  Created by Ashwin kumar on 08/01/15.
//  Copyright (c) 2015 Voxel Busters Interactive LLP. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "HandlerBase.h"

@interface NotificationHandler : HandlerBase

// Properties
@property(nonatomic, retain)	UILocalNotification 	*launchLocalNotification;
@property(nonatomic, retain)	NSDictionary			*launchRemoteNotification;
@property(nonatomic)			int						supportedNotificationTypes;

// Static Methods
+ (void)SetKeyForUserInfo:(NSString *)value;
+ (NSString *)GetKeyForUserInfo;

// Send Notification to unity
- (void)sendLaunchNotifications;

// Register
- (void)registerUserNotificationTypes:(int)notificationTypes;
- (void)registerForRemoteNotifications;
- (void)unregisterForRemoteNotifications;

@end
