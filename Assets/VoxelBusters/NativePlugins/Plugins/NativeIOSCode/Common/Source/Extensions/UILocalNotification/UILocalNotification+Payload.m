//
//  UILocalNotification+Payload.m
//  NativePluginIOSWorkspace
//
//  Created by Ashwin kumar on 27/01/15.
//  Copyright (c) 2015 Voxel Busters Interactive LLP. All rights reserved.
//

#import "UILocalNotification+Payload.h"
#import "NotificationHandler.h"

@implementation UILocalNotification (Payload)

#define kAPS			@"aps"
#define kAlert			@"alert"
#define kBody			@"body"
#define kAction			@"action-loc-key"
#define kLaunchImage	@"launch-image"
#define kFireDate		@"fire-date"
#define kRepeatInterval	@"repeat-interval"
#define kBadge			@"badge"
#define kSound			@"sound"

- (id)payload
{
	NSMutableDictionary *payload	= [NSMutableDictionary dictionary];
	NSMutableDictionary *aps		= [NSMutableDictionary dictionary];
	
	// Payload: Add aps info
	[payload setObject:aps forKey:kAPS];
	
	// APS: Add alert info
	NSMutableDictionary *alert = [NSMutableDictionary dictionary];
	
	if ([self alertBody] != NULL)
	{
		[alert setObject:[self alertBody] forKey:kBody];
	}
	
	if ([self alertAction] != NULL)
	{
		[alert setObject:[self alertAction] forKey:kAction];
	}
	
	if ([self alertLaunchImage] != NULL)
	{
		[alert setObject:[self alertLaunchImage] forKey:kLaunchImage];
	}
	
	[aps setObject:alert forKey:kAlert];
	
	// APS: Add badge and sound info
	[aps setObject:[NSNumber numberWithInteger:[self applicationIconBadgeNumber]] forKey:kBadge];
	
	if ([self soundName] != NULL)
	{
		[aps setObject:[self soundName] forKey:kSound];
	}
	
	// Payload: Add user info, fire data, repeat interval
	if ([self userInfo] != NULL)
	{
		[payload setObject:[self userInfo] forKey:[NotificationHandler GetKeyForUserInfo]];
	}
	
	if ([self fireDate] != NULL)
	{
		[payload setObject:[Utility ConvertNSDateToNSString:[self fireDate]]forKey:kFireDate];
	}
	
	[payload setObject:[NSNumber numberWithInt:[self repeatInterval]] forKey:kRepeatInterval];
	
	return payload;
}

@end
