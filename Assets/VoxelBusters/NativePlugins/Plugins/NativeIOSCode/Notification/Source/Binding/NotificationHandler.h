//
//  NotificationHandler.h
//  Cross Platform Native Plugins
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

// Static methods
+ (NSString *)GetUserInfoKey;

// Init methods
- (void)initialize:(NSString*)userInfoKey;

// Notification type methods
- (int)enabledNotificationTypes;
- (void)registerNotificationTypes:(int)notificationTypes;

// Local notification methods
- (void)scheduleLocalNotification:(NSDictionary *)payload;
- (void)cancelLocalNotification:(NSString *)notificationID;
- (void)cancelAllLocalNotifications;
- (void)clearNotifications;

// Remote notification methods
- (void)registerForRemoteNotifications;
- (void)unregisterForRemoteNotifications;

@end
