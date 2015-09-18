//
//  GameServicesBinding.h
//  Unity-iPhone
//
//  Created by Ashwin kumar on 27/05/15.
//
//

#import <Foundation/Foundation.h>

UIKIT_EXTERN bool isGameCenterAvailable ();
UIKIT_EXTERN void showLeaderboardView (const char* leaderboardID, int timeScope);
UIKIT_EXTERN void showAchievementView ();