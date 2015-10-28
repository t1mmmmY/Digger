//
//  NotificationBinding.m
//  Cross Platform Native Plugins
//
//  Created by Ashwin kumar on 11/01/15.
//  Copyright (c) 2015 Voxel Busters Interactive LLP. All rights reserved.
//

#import "NotificationBinding.h"
#import "NotificationHandler.h"

#pragma mark - Init Methods

void initNotificationService (const char* keyForUserInfo)
{
	[(NotificationHandler *)[NotificationHandler Instance] initialize:ConvertToNSString(keyForUserInfo)];
}

#pragma mark - Notification Type Methods

int enabledNotificationTypes ()
{
	return [(NotificationHandler *)[NotificationHandler Instance] enabledNotificationTypes];
}

void registerNotificationTypes (int notificationTypes)
{
	[(NotificationHandler *)[NotificationHandler Instance] registerNotificationTypes:notificationTypes];
}

#pragma mark - Local Notification Methods

void scheduleLocalNotification (const char* payloadJSONString)
{
	[(NotificationHandler *)[NotificationHandler Instance] scheduleLocalNotification:FromJson(payloadJSONString)];
}

void cancelLocalNotification (const char* notificationID)
{
	[(NotificationHandler *)[NotificationHandler Instance] cancelLocalNotification:ConvertToNSString(notificationID)];
}

void cancelAllLocalNotifications ()
{
	[(NotificationHandler *)[NotificationHandler Instance] cancelAllLocalNotifications];
}

void clearNotifications ()
{
	[(NotificationHandler *)[NotificationHandler Instance] clearNotifications];
}

#pragma mark - Remote Notification Methods

void registerForRemoteNotifications ()
{
	[(NotificationHandler *)[NotificationHandler Instance] registerForRemoteNotifications];
}

void unregisterForRemoteNotifications ()
{
	[(NotificationHandler *)[NotificationHandler Instance] unregisterForRemoteNotifications];
}