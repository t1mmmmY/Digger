//
//  GameServicesHandler.h
//  Unity-iPhone
//
//  Created by Ashwin kumar on 27/05/15.
//
//

#import "HandlerBase.h"
#import <GameKit/GameKit.h>

@interface GameServicesHandler : HandlerBase <GKGameCenterControllerDelegate>

// Methods
- (BOOL)isGameCenterAvailable;

// UI methods
- (void)showLeaderboardView:(NSString *)leaderboardID withTimeScope:(GKLeaderboardTimeScope)timeScope;
- (void)showAchievementView;

@end
