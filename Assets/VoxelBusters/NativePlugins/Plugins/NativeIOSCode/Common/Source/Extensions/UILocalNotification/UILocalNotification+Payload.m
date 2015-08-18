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

#define kALS			@"aps"
#define kAlert			@"alert"
#define kBody			@"body"
#define kAction			@"action-loc-key"
#define kLaunchImage	@"launch-image"
#define kFireDate		@"fire-date"
#define kBadge			@"badge"
#define kSound			@"sound"

- (id)payload
{
	NSMutableDictionary *payload	= [NSMutableDictionary dictionaryWithObject:[NSMutableDictionary dictionary] forKey:kALS];
	NSMutableDictionary *als		= [payload objectForKey:kALS];
	
	// Alert
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
	
	[als setObject:alert forKey:kAlert];
	
	// User info, fire date
	if ([self userInfo] != NULL)
	{
		[als setObject:[self userInfo] forKey:[NotificationHandler GetKeyForUserInfo]];
	}
	
	if ([self fireDate] != NULL)
	{
		[als setObject:[Utility ConvertNSDateToNSString:[self fireDate]]forKey:kFireDate];
	}
	
	// Sound, badges
	[als setObject:[NSNumber numberWithInteger:[self applicationIconBadgeNumber]] forKey:kBadge];
	
	if ([self soundName] != NULL)
	{
		[als setObject:[self soundName] forKey:kSound];
	}
	
	return payload;
}

@end
