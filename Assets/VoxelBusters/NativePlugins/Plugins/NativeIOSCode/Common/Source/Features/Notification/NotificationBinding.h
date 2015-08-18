//
//  NotificationBinding.h
//  NativePluginIOSWorkspace
//
//  Created by Ashwin kumar on 11/01/15.
//  Copyright (c) 2015 Voxel Busters Interactive LLP. All rights reserved.
//

#import <Foundation/Foundation.h>

UIKIT_EXTERN void initNotificationService (const char* keyForUserInfo);
UIKIT_EXTERN void registerNotificationTypes (int notificationTypes);
UIKIT_EXTERN void registerForRemoteNotifications ();
UIKIT_EXTERN void unregisterForRemoteNotifications ();