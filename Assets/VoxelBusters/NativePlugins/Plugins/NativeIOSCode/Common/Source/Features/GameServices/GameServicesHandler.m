//
//  GameServicesHandler.m
//  Unity-iPhone
//
//  Created by Ashwin kumar on 27/05/15.
//
//

#import "GameServicesHandler.h"

@implementation GameServicesHandler

#pragma mark - Methods

- (BOOL)isGameCenterAvailable
{
	// check for presence of GKLocalPlayer API
	Class gcClass = (NSClassFromString(@"GKLocalPlayer"));
	
	// check if the device is running iOS 4.1 or later
	NSString *reqSysVer 	= @"4.1";
	NSString *currSysVer 	= [[UIDevice currentDevice] systemVersion];
	BOOL osVersionSupported = ([currSysVer compare:reqSysVer options:NSNumericSearch] != NSOrderedAscending);
	
	return (gcClass && osVersionSupported);
}

@end
