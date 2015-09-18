//
//  GameServicesHandler.m
//  Unity-iPhone
//
//  Created by Ashwin kumar on 27/05/15.
//
//

#import "GameServicesHandler.h"

@implementation GameServicesHandler

#define kShowLeaderboardViewFinished	"ShowLeaderboardViewFinished"
#define kShowAchievementViewFinished	"ShowAchievementViewFinished"

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

#pragma mark - UI

- (void)showLeaderboardView:(NSString *)leaderboardID withTimeScope:(GKLeaderboardTimeScope)timeScope
{
	GKGameCenterViewController *gameCenterController	= [[[GKGameCenterViewController alloc] init] autorelease];
	
	if (gameCenterController != nil)
	{
		gameCenterController.gameCenterDelegate 		= self;
		gameCenterController.viewState 					= GKGameCenterViewControllerStateLeaderboards;
		gameCenterController.leaderboardTimeScope 		= timeScope;
		gameCenterController.leaderboardCategory 		= leaderboardID;
		
		
		// Present view
		[UnityGetGLViewController() presentViewController: gameCenterController animated: YES completion:nil];
	}
}


- (void)showAchievementView
{
	GKGameCenterViewController *gameCenterController 	= [[[GKGameCenterViewController alloc] init] autorelease];
	
	if (gameCenterController != nil)
	{
		gameCenterController.gameCenterDelegate 		= self;
		gameCenterController.viewState 					= GKGameCenterViewControllerStateAchievements;
		
		// Present view
		[UnityGetGLViewController() presentViewController: gameCenterController animated: YES completion:nil];
	}
}

#pragma mark - Delegate

- (void)gameCenterViewControllerDidFinish:(GKGameCenterViewController *)gameCenterViewController
{
	if (gameCenterViewController != NULL)
	{
		if (gameCenterViewController.viewState == GKGameCenterViewControllerStateLeaderboards)
		{
			NotifyEventListener(kShowLeaderboardViewFinished, "");
		}
		else
		{
			NotifyEventListener(kShowAchievementViewFinished, "");
		}
		
		[gameCenterViewController dismissViewControllerAnimated:YES completion:NULL];
	}
}

@end
