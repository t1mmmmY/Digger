//
//  GameServicesBinding.m
//  Unity-iPhone
//
//  Created by Ashwin kumar on 27/05/15.
//
//

#import "GameServicesBinding.h"
#import "GameServicesHandler.h"

bool isGameCenterAvailable ()
{
	return [[GameServicesHandler Instance] isGameCenterAvailable];
}


void showLeaderboardView (const char* leaderboardID, int timeScope)
{
	[[GameServicesHandler Instance] showLeaderboardView:ConvertToNSString(leaderboardID) withTimeScope:(GKLeaderboardTimeScope)timeScope];
}

void showAchievementView ()
{
	[[GameServicesHandler Instance] showAchievementView];
}