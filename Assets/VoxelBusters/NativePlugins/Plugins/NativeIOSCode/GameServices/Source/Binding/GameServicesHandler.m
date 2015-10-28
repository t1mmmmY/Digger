//
//  GameServicesHandler.m
//  Cross Platform Native Plugins
//
//  Created by Ashwin kumar on 27/05/15.
//
//

#import "GameServicesHandler.h"
#import "UIImage+Save.h"
#import "GKAchievement+Extensions.h"
#import "GKAchievementDescription+Extensions.h"
#import "GKLeaderboard+Extensions.h"
#import "GKLocalPlayer+Extensions.h"
#import "GKPlayer+Extensions.h"
#import "GKScore+Extensions.h"

const NSString *kGameServicesError							= @"error";
const NSString *kGameServicesObjectInstanceID				= @"object-id";
const NSString *kGameServicesAchievementsList				= @"achievements";
const NSString *kGameServicesAchievementDescriptionsList	= @"descriptions";
const NSString *kGameServicesLeaderboardsList				= @"leaderboards";
const NSString *kGameServicesPlayersList					= @"players";

@implementation GameServicesHandler

@synthesize descriptionInfoCache;
@synthesize leaderboardInfoCache;
@synthesize playerInfoCache;
@synthesize showsCompletionBanner;

// Unity Events
#define kShowLeaderboardViewFinished			"ShowLeaderboardViewFinished"
#define kShowAchievementViewFinished			"ShowAchievementViewFinished"

#define kLoadAchievementsFinished				"LoadAchievementsFinished"
#define kReportAchievementFinished				"ReportProgressFinished"

#define kLoadDescriptionsFinished				"LoadAchievementDescriptionsFinished"
#define kLoadAchievementImageFinished			"RequestForAchievementImageFinished"

#define kLoadLeaderboardsFinished				"LoadLeaderboardsFinished"
#define kLoadLeaderboardScoresFinished			"LoadScoresFinished"
#define kLoadLeaderboardImageFinished			"LoadLeaderboardImageFinished"

#define kLocalPlayerAuthFinished				"AuthenticationFinished"
#define kLoadFriendsFinished					"LoadFriendsFinished"

#define kReportScoreFinished					"ReportScoreFinished"

#define kLoadPlayersFinished					"LoadUsersFinished"
#define kLoadPlayerPhotoFinished				"RequestForUserImageFinished"

#pragma mark - Init Methods

- (id)init
{
	self = [super init];
	
	if (self != NULL)
	{
		self.showsCompletionBanner	= YES;
		self.descriptionInfoCache	= NULL;
		self.leaderboardInfoCache	= NULL;
		self.playerInfoCache		= [NSMutableDictionary dictionary];
	}
	
	return self;
}

- (void)dealloc
{
	// Release
	self.playerInfoCache			= nil;
	self.descriptionInfoCache		= nil;
	self.leaderboardInfoCache		= nil;
	
	// Base method call
	[super dealloc];
}

#pragma mark - Methods

- (BOOL)isGameCenterAvailable
{
	// check for presence of GKLocalPlayer API
	Class gcClass 			= (NSClassFromString(@"GKLocalPlayer"));
	
	// check if the device is running iOS 4.1 or later
	NSString *reqSysVer 	= @"4.1";
	NSString *currSysVer 	= [[UIDevice currentDevice] systemVersion];
	BOOL osVersionSupported = ([currSysVer compare:reqSysVer options:NSNumericSearch] != NSOrderedAscending);
	
	return (gcClass && osVersionSupported);
}

#pragma mark - UI Methods

- (void)showLeaderboardView:(NSString *)leaderboardID withTimeScope:(GKLeaderboardTimeScope)timeScope
{
	GKGameCenterViewController *gameCenterController	= [[[GKGameCenterViewController alloc] init] autorelease];
	gameCenterController.gameCenterDelegate 			= self;
	gameCenterController.viewState 						= GKGameCenterViewControllerStateLeaderboards;
	
#ifdef __IPHONE_7_0
	if (SYSTEM_VERSION_GREATER_THAN_OR_EQUAL_TO(@"7.0"))
	{
		gameCenterController.leaderboardIdentifier 		= leaderboardID;
	}
	else
#endif
	{
		gameCenterController.leaderboardTimeScope 		= timeScope;
		gameCenterController.leaderboardCategory 		= leaderboardID;
	}
	
	// Present view
	[UnityGetGLViewController() presentViewController:gameCenterController
											 animated:YES
										   completion:nil];
}

- (void)showAchievementView
{
	GKGameCenterViewController *gameCenterController 	= [[[GKGameCenterViewController alloc] init] autorelease];
	gameCenterController.gameCenterDelegate 			= self;
	gameCenterController.viewState 						= GKGameCenterViewControllerStateAchievements;
	
	// Present view
	[UnityGetGLViewController() presentViewController:gameCenterController
											 animated:YES
										   completion:nil];
}

#pragma mark - UI Delegate Methods

- (void)gameCenterViewControllerDidFinish:(GKGameCenterViewController *)gameCenterViewController
{
	if (gameCenterViewController != NULL)
	{
		if (gameCenterViewController.viewState == GKGameCenterViewControllerStateLeaderboards)
		{
			NotifyEventListener(kShowLeaderboardViewFinished, kCStringEmpty);
		}
		else
		{
			NotifyEventListener(kShowAchievementViewFinished, kCStringEmpty);
		}
		
		// Dismiss
		[UnityGetGLViewController() dismissViewControllerAnimated:YES completion:nil];
	}
}

#pragma mark - Achievements Methods

- (void)loadAchievements
{
	[GKAchievement loadAchievementsWithCompletionHandler:^(NSArray *achievements, NSError *error) {
		
		NSMutableDictionary	*dataDict	= [NSMutableDictionary dictionary];
		
		if (error != nil)
		{
			[dataDict setObject:[error description] forKey:kGameServicesError];
		}
		
		if (achievements != nil)
		{
			NSMutableArray *achievementJSONList	= [NSMutableArray array];
			
			for (GKAchievement *curAchievement in achievements)
				[achievementJSONList addObject:[curAchievement toJsonObject]];
			
			// Add to callback data dictionary
			[dataDict setObject:achievementJSONList forKey:kGameServicesAchievementsList];
		}
		
		// Send event
		NotifyEventListener(kLoadAchievementsFinished, ToJsonCString(dataDict));
	}];
}

- (void)reportAchievementProgress:(NSDictionary *)achievementInfoDict percentComplete:(double)percentComplete
{
	NSString *identifier		= [achievementInfoDict objectForKey:kGKAchievementIdentifier];
	NSString *instanceID		= [achievementInfoDict objectForKey:kGameServicesObjectInstanceID];
	
	// Create object
	GKAchievement *achievement	= [[[GKAchievement alloc] initWithIdentifier:identifier] autorelease];
	achievement.percentComplete	= percentComplete;
	achievement.showsCompletionBanner	= self.showsCompletionBanner;
	
	// Report progress
	[GKAchievement reportAchievements:@[achievement] withCompletionHandler:^(NSError *error) {
		
		NSMutableDictionary	*dataDict	= [NSMutableDictionary dictionary];
		
		[dataDict setObject:instanceID forKey:kGameServicesObjectInstanceID];
		[dataDict setObject:[achievement toJsonObject] forKey:kGKAchievementInfo];
		
		if (error != NULL)
		{
			[dataDict setObject:[error description] forKey:kGameServicesError];
		}
		
		// Notify
		NotifyEventListener(kReportAchievementFinished, ToJsonCString(dataDict));
	}];
}

#pragma mark - Achievement Description Methods

- (void)loadAchievementDescriptionsWithCompletionHandler:(void(^)(NSArray *descriptions, NSError *error))completionHandler
{
	[GKAchievementDescription loadAchievementDescriptionsWithCompletionHandler:^(NSArray *descriptions, NSError *error) {
		
		// Initialise if required
		if (self.descriptionInfoCache == NULL)
			self.descriptionInfoCache = [NSMutableDictionary dictionary];
		
		// Cache info
		if (descriptions != NULL)
		{
			for (GKAchievementDescription *curDescription in descriptions)
				[self.descriptionInfoCache setObject:curDescription forKey:curDescription.identifier];
		}

		// Invoke callback
		if (completionHandler != NULL)
			completionHandler(descriptions, error);
	}];
}

- (void)getAchievementDescriptionWithIdentifier:(NSString *)achievementID withCompleteionHandler:(void(^)(GKAchievementDescription *description))completionHandler
{
	if (self.descriptionInfoCache == NULL)
	{
		[self loadAchievementDescriptionsWithCompletionHandler:^(NSArray *descriptions, NSError *error) {
			
			if (completionHandler != NULL)
				completionHandler([self.descriptionInfoCache objectForKey:achievementID]);
		}];
	}
	else
	{
		if (completionHandler != NULL)
			completionHandler([self.descriptionInfoCache objectForKey:achievementID]);
	}
}

- (void)loadAchievementDescriptions
{
	[self loadAchievementDescriptionsWithCompletionHandler:^(NSArray *descriptions, NSError *error) {

		NSMutableDictionary *dataDict	= [NSMutableDictionary dictionary];
		
		if (error != nil)
		{
			[dataDict setObject:[error description] forKey:kGameServicesError];
		}
		
		if (descriptions != nil)
		{
			NSMutableArray *descriptionsJSONList	= [NSMutableArray array];
			
			// Convert to JSON object format
			for (GKAchievementDescription *curDescription in descriptions)
				[descriptionsJSONList addObject:[curDescription toJsonObject]];
			
			// Add to callback data dictionary
			[dataDict setObject:descriptionsJSONList forKey:kGameServicesAchievementDescriptionsList];
		}
		
		// Send event
		NotifyEventListener(kLoadDescriptionsFinished, ToJsonCString(dataDict));
	}];
}

- (void)loadAchievementImage:(NSDictionary *)descriptionInfoDict
{
	NSString *identifier		= [descriptionInfoDict objectForKey:kGKAchievementDescriptionIdentifier];
	NSString *instanceID		= [descriptionInfoDict objectForKey:kGameServicesObjectInstanceID];
	
	[self getAchievementDescriptionWithIdentifier:identifier withCompleteionHandler:^(GKAchievementDescription *description) {
		
		[self loadAchievementImage:description forInstanceID:instanceID];
	}];
}

- (void)loadAchievementImage:(GKAchievementDescription *)description forInstanceID:(NSString *)instanceID
{
	if (description == NULL)
	{
		NSMutableDictionary *dataDict		= [NSMutableDictionary dictionary];
		
		[dataDict setObject:instanceID forKey:kGameServicesObjectInstanceID];
		[dataDict setObject:@"The operation could not be completed because Achievement description info could not be found." forKey:kGameServicesError];
		
		// Send event
		NotifyEventListener(kLoadAchievementImageFinished, ToJsonCString(dataDict));
	}
	else
	{
		[description loadImageWithCompletionHandler:^(UIImage *image, NSError *error) {
			
			NSMutableDictionary	*dataDict	= [NSMutableDictionary dictionary];
			
			[dataDict setObject:instanceID forKey:kGameServicesObjectInstanceID];

			if (error != NULL)
			{
				[dataDict setObject:[error description] forKey:kGameServicesError];
			}
			
			if (image != NULL)
			{
				NSString *photoName			= [description imageName];
				NSString *photoPath			= [image saveImageToDocumentsDirectory:photoName];
				
				if (photoPath)
					[dataDict setObject:photoPath forKey:kGKAchievementDescriptionImagePathKey];
			}
			
			// Send event
			NotifyEventListener(kLoadAchievementImageFinished, ToJsonCString(dataDict));
		}];
	}
}

- (NSString *)getIncompleteAchievementDefaultImagePath
{
	return [[GKAchievementDescription incompleteAchievementImage] saveImageToDocumentsDirectory:@"GC_IncompleteAchievement"];
}

- (NSString *)getCompletedAchievementPlaceholderImagePath
{
	return [[GKAchievementDescription incompleteAchievementImage] saveImageToDocumentsDirectory:@"GC_CompletedAchievementPlaceholder"];
}

#pragma mark - Leaderboard Methods

- (void)loadLeaderboardsWithCompletionHandler:(void(^)(NSArray *leaderboards, NSError *error))completionHandler
{
	[GKLeaderboard loadLeaderboardsWithCompletionHandler:^(NSArray *leaderboards, NSError *error) {
		
		// Initialise if required
		if (self.leaderboardInfoCache == NULL)
			[self setLeaderboardInfoCache:[NSMutableDictionary dictionary]];
		
		// Cache leaderboard info for future use
		if (leaderboards != NULL)
		{
			for (GKLeaderboard *curLeaderboard in leaderboards)
				[self.leaderboardInfoCache setObject:curLeaderboard forKey:[curLeaderboard getLeaderboardIdentifier]];
		}
		
		// Invoke callback
		if (completionHandler != NULL)
			completionHandler(leaderboards, error);
	}];
}

- (void)getLeaderboardWithIdentifier:(NSString *)leaderboardID withCompleteionHandler:(void(^)(GKLeaderboard *leaderboard))completionHandler
{
	if (self.leaderboardInfoCache == NULL)
	{
		[self loadLeaderboardsWithCompletionHandler:^(NSArray *leaderboards, NSError *error) {
			
			if (completionHandler != NULL)
				completionHandler([self.leaderboardInfoCache objectForKey:leaderboardID]);
		}];
	}
	else
	{
		if (completionHandler != NULL)
			completionHandler([self.leaderboardInfoCache objectForKey:leaderboardID]);
	}
}

- (void)loadLeaderboards
{
	[self loadLeaderboardsWithCompletionHandler:^(NSArray *leaderboards, NSError *error) {
		
		NSMutableDictionary	*dataDict	= [NSMutableDictionary dictionary];
		
		if (error != nil)
		{
			[dataDict setObject:[error description] forKey:kGameServicesError];
		}
		
		if (leaderboards != nil)
		{
			NSMutableArray *leaderboardJSONList	= [NSMutableArray array];
			
			for (GKLeaderboard *curLeaderboard in leaderboards)
				[leaderboardJSONList addObject:[curLeaderboard toJsonObject]];

			[dataDict setObject:leaderboardJSONList forKey:kGameServicesLeaderboardsList];
		}
		
		NotifyEventListener(kLoadLeaderboardsFinished, ToJsonCString(dataDict));
	}];
}

- (void)loadLeaderboardImage:(NSDictionary *)leaderboardInfoDict
{
	NSString *identifier	= [leaderboardInfoDict objectForKey:kGKLeaderboardIdentifier];
	NSString *instanceID	= [leaderboardInfoDict objectForKey:kGameServicesObjectInstanceID];
	
	// Firstly get leaderboard info followed by load image
	[self getLeaderboardWithIdentifier:identifier withCompleteionHandler:^(GKLeaderboard *leaderboard) {
		
		[self loadLeaderboardImage:leaderboard forInstanceID:instanceID];
	}];
}

- (void)loadLeaderboardImage:(GKLeaderboard *)leaderboard forInstanceID:(NSString *)instanceID
{
	if (leaderboard == NULL || SYSTEM_VERSION_LESS_THAN(@"7.0"))
	{
		// Notify Unity
		NSMutableDictionary	*dataDict		= [NSMutableDictionary dictionary];
		[dataDict setObject:instanceID forKey:kGameServicesObjectInstanceID];
		
		if (leaderboard == NULL)
			[dataDict setObject:@"The operation could not be completed because leaderboard info could not be found." forKey:kGameServicesError];
		else
			[dataDict setObject:@"The operation could not be completed because feature is not supported in this version." forKey:kGameServicesError];
		
		NotifyEventListener(kLoadLeaderboardImageFinished, ToJsonCString(dataDict));
		return;
	}
	
#ifdef __IPHONE_7_0
	[leaderboard loadImageWithCompletionHandler:^(UIImage *image, NSError *error) {
		
		NSMutableDictionary	*dataDict	= [NSMutableDictionary dictionary];
		
		[dataDict setObject:instanceID forKey:kGameServicesObjectInstanceID];
		
		if (error != NULL)
		{
			[dataDict setObject:[error description] forKey:kGameServicesError];
		}
		
		if (image != NULL)
		{
			NSString *photoName	= [leaderboard imageName];
			NSString *photoPath	= [image saveImageToDocumentsDirectory:photoName];
			
			if (photoPath)
				[dataDict setObject:photoPath forKey:kGKLeaderboardImagePath];
		}
		
		NotifyEventListener(kLoadLeaderboardImageFinished, ToJsonCString(dataDict));
	}];
#endif
}

- (void)loadScores:(NSDictionary *)leaderboardInfoDict
{
	NSString *identifier	= [leaderboardInfoDict objectForKey:kGKLeaderboardIdentifier];
	NSString *instanceID	= [leaderboardInfoDict objectForKey:kGameServicesObjectInstanceID];
	
	// Get range
	NSRange	range;
	range.location			= [[leaderboardInfoDict objectForKey:kGKLeaderboardRangeFrom] intValue];
	range.length			= [[leaderboardInfoDict objectForKey:kGKLeaderboardRangeLength] intValue];
	
	// Firstly get leaderboard info, followed by load scores
	[self getLeaderboardWithIdentifier:identifier withCompleteionHandler:^(GKLeaderboard *leaderboard) {
		
		// Set leaderboard properties
		if (leaderboard != NULL)
		{
			leaderboard.range			= range;
			leaderboard.timeScope		= (GKLeaderboardTimeScope)[[leaderboardInfoDict objectForKey:kGKLeaderboardTimeScope] intValue];
			leaderboard.playerScope		= (GKLeaderboardPlayerScope)[[leaderboardInfoDict objectForKey:kGKLeaderboardPlayerScope] intValue];
		}
		
		[self loadScoresFromLeaderboard:leaderboard forInstanceID:instanceID];
	}];
}

- (void)loadScoresFromLeaderboard:(GKLeaderboard *)leaderboard forInstanceID:(NSString *)instanceID
{
	if (leaderboard == NULL)
	{
		NSError *error = [NSError errorWithDomain:kNativePluginsErrorDomain
											 code:0
										 userInfo:@{NSLocalizedDescriptionKey : @"The operation could not be completed because leaderboard info could not be found."}];
		
		[self loadScoresForInstanceID:instanceID failedWithError:error];
		return;
	}

	// Load scores
	[leaderboard loadScoresWithCompletionHandler:^(NSArray *scores, NSError *loadScoreError) {
		
#ifdef __IPHONE_8_0
		if (SYSTEM_VERSION_GREATER_THAN_OR_EQUAL_TO(@"8.0"))
		{
			// Cache player inforamtion to avoid requesting from server
			if (scores != NULL)
			{
				NSMutableArray *players	= [NSMutableArray array];
				
				for (GKScore *score in scores)
					[players addObject:score.player];
				
				[self cachePlayerInfo:players];
			}
			
			// Invoke handler
			[self loadScoresFromLeaderboard:leaderboard finishedForInstanceID:instanceID error:loadScoreError];
			return;
		}
		else
#endif
		{
			if (scores == NULL || scores.count == 0)
			{
				[self loadScoresFromLeaderboard:leaderboard finishedForInstanceID:instanceID error:loadScoreError];
			}
			else
			{
				NSMutableArray *playerIdentifiers	= [NSMutableArray array];
				
				for (GKScore *curScore in scores)
					[playerIdentifiers addObject:curScore.playerID];
				
				// Load player info
				[self loadGKPlayersForIdentifiers:playerIdentifiers withCompletionHandler:^(NSArray *players, NSError *loadPlayerError) {
					
					if (players == NULL || players.count != playerIdentifiers.count)
					{
						[self loadScoresForInstanceID:instanceID failedWithError:loadPlayerError];
					}
					else
					{
						[self loadScoresFromLeaderboard:leaderboard finishedForInstanceID:instanceID error:loadScoreError];
					}
				}];
			}
		}
	}];
}

- (void)loadScoresFromLeaderboard:(GKLeaderboard *)leaderboard finishedForInstanceID:(NSString *)instanceID error:(NSError *)error
{
	NSMutableDictionary	*dataDict	= [NSMutableDictionary dictionary];
	[dataDict setObject:instanceID forKey:kGameServicesObjectInstanceID];
	
	if (leaderboard != NULL)
		[dataDict setObject:[leaderboard toJsonObject] forKey:kGKLeaderboardInfo];
	
	if (error != NULL)
		[dataDict setObject:[error description] forKey:kGameServicesError];
	
	NotifyEventListener(kLoadLeaderboardScoresFinished, ToJsonCString(dataDict));
}

- (void)loadScoresForInstanceID:(NSString *)instanceID failedWithError:(NSError *)error
{
	NSMutableDictionary	*dataDict	= [NSMutableDictionary dictionary];
	[dataDict setObject:instanceID forKey:kGameServicesObjectInstanceID];
	
	if (error != NULL)
		[dataDict setObject:[error description] forKey:kGameServicesError];
	
	NotifyEventListener(kLoadLeaderboardScoresFinished, ToJsonCString(dataDict));
}

#pragma mark - Local Player Methods

- (BOOL)isAuthenticated
{
	return [[GKLocalPlayer localPlayer] isAuthenticated];
}

- (void)authenticatePlayer
{
	GKLocalPlayer *localPlayer 		= [GKLocalPlayer localPlayer];
	localPlayer.authenticateHandler = ^(UIViewController *viewController, NSError *error){
		
		if (viewController != nil)
		{
			[UnityGetGLViewController() presentViewController:viewController animated:YES completion:nil];
		}
		else
		{
			// Cache local player info as well
			if ([localPlayer isAuthenticated])
				[self cachePlayerInfo:@[localPlayer]];
			
			// Send details to Unity
			NSMutableDictionary	*dataDict	= [NSMutableDictionary dictionary];
			[dataDict setObject:[localPlayer toJsonObject] forKey:kGKLocalPlayerInfo];
			
			if (error != nil)
			{
				[dataDict setObject:[error description] forKey:kGameServicesError];
			}
			
			NotifyEventListener(kLocalPlayerAuthFinished, ToJsonCString(dataDict));
		}
	};
}

- (void)loadFriendPlayers
{
#ifdef __IPHONE_8_0
	if (SYSTEM_VERSION_GREATER_THAN_OR_EQUAL_TO(@"8.0"))
	{
		[[GKLocalPlayer localPlayer] loadFriendPlayersWithCompletionHandler:^(NSArray *friendPlayers, NSError *error) {
			
			// Cache players
			[self cachePlayerInfo:friendPlayers];
			
			// Call handler
			[self onLoadFriendsFinished:friendPlayers withError:error];
		}];
	}
	else
#endif
	{
		[[GKLocalPlayer localPlayer] loadFriendsWithCompletionHandler:^(NSArray *friendIDs, NSError *loadFriendIDsError) {
			
			// Load friend info
			if (friendIDs == NULL)
			{
				[self onLoadFriendsFinished:NULL withError:loadFriendIDsError];
			}
			else if (friendIDs.count == 0)
			{
				[self onLoadFriendsFinished:[NSArray array] withError:loadFriendIDsError];
			}
			else
			{
				[self loadGKPlayersForIdentifiers:friendIDs withCompletionHandler:^(NSArray *friendPlayers, NSError *loadFriendPlayersError) {
					
					if (friendPlayers == NULL || friendPlayers.count != friendIDs.count)
					{
						[self onLoadFriendsFinished:NULL withError:loadFriendPlayersError];
					}
					else
					{
						[self onLoadFriendsFinished:friendPlayers withError:loadFriendIDsError];
					}
				}];
			}
		}];
	}
}

- (void)onLoadFriendsFinished:(NSArray *)friendPlayers withError:(NSError *)error
{
	NSMutableDictionary	*dataDict		= [NSMutableDictionary dictionary];
	
	if (error != NULL)
		[dataDict setObject:[error description] forKey:kGameServicesError];
	
	if (friendPlayers != NULL)
	{
		NSMutableArray *friendJSONList	= [NSMutableArray array];
		
		for (GKPlayer *curPlayer in friendPlayers)
			[friendJSONList addObject:[curPlayer toJsonObject]];
		
		[dataDict setObject:friendJSONList forKey:kGKLocalPlayerFriendPlayers];
	}
	
	NotifyEventListener(kLoadFriendsFinished, ToJsonCString(dataDict));
}

#pragma mark - Score Methods

- (void)reportScore:(NSDictionary *)scoreInfoDict withValue:(long)value
{
	NSString *leaderboardID	= [scoreInfoDict objectForKey:kGKScoreLeaderboardIdentifier];
	NSString *instanceID	= [scoreInfoDict objectForKey:kGameServicesObjectInstanceID];
	GKScore	*score;
	
	// Create score object
#ifdef __IPHONE_7_0
	if (SYSTEM_VERSION_GREATER_THAN_OR_EQUAL_TO(@"7.0"))
	{
		score				= [[GKScore alloc] initWithLeaderboardIdentifier:leaderboardID];
	}
	else
#endif
	{
		score				= [[GKScore alloc] initWithCategory:leaderboardID];
	}
	
	// Set new score
	score.value				= value;
	
	// Report progress
	[GKScore reportScores:@[score] withCompletionHandler:^(NSError *error) {
		
		NSMutableDictionary	*dataDict	= [NSMutableDictionary dictionary];
		[dataDict setObject:instanceID forKey:kGameServicesObjectInstanceID];
		[dataDict setObject:[score toJsonObject] forKey:kGKScoreInfoKey];

		if (error != NULL)
		{
			[dataDict setObject:[error description] forKey:kGameServicesError];
		}
		
		// Notify
		NotifyEventListener(kReportScoreFinished, ToJsonCString(dataDict));
	}];
}

#pragma mark - Player Methods

- (void)loadGKPlayersForIdentifiers:(NSArray *)identifiers withCompletionHandler:(void(^)(NSArray *players, NSError *error))completionHandler
{
	[GKPlayer loadPlayersForIdentifiers:identifiers withCompletionHandler:^(NSArray *players, NSError *error) {
		
		// Cache received player information
		[self cachePlayerInfo:players];
		
		if (completionHandler != NULL)
			completionHandler(players, error);
	}];
}

- (void)cachePlayerInfo:(NSArray *)players
{
	if (players == NULL)
		return;
	
	for (GKPlayer *curPlayer in players)
		[self.playerInfoCache setObject:curPlayer forKey:curPlayer.playerID];
}

- (void)getPlayerwithIdentifier:(NSString *)identifier withCompletionHandler:(void(^)(GKPlayer *player))completionHandler
{
	GKPlayer *player	= [self getPlayer:identifier];
	
	if (player != NULL)
	{
		if (completionHandler != NULL)
			completionHandler(player);
		
		return;
	}
	
	// Load players info
	[self loadGKPlayersForIdentifiers:@[identifier] withCompletionHandler:^(NSArray *players, NSError *error) {
		
		if (players == NULL || players.count != 1)
		{
			// Invoke handler
			if (completionHandler != NULL)
				completionHandler(NULL);
		}
		else
		{
			// Cache this player info
			[self cachePlayerInfo:players];
			
			// Invoke handler
			if (completionHandler != NULL)
				completionHandler(players[0]);
		}
	}];
}

- (void)loadPlayers:(NSArray *)identifiers
{
	[self loadGKPlayersForIdentifiers:identifiers withCompletionHandler:^(NSArray *players, NSError *error) {
		
		// Send request data to unity
		NSMutableDictionary	*dataDict			= [NSMutableDictionary dictionary];
		
		if (error != NULL)
		{
			[dataDict setObject:[error description] forKey:kGameServicesError];
		}
		
		if (players != NULL)
		{
			NSMutableArray *playersJSONList		= [NSMutableArray array];
			
			for (GKPlayer *curPlayer in players)
				[playersJSONList addObject:[curPlayer toJsonObject]];
			
			[dataDict setObject:playersJSONList forKey:kGameServicesPlayersList];
		}
		
		NotifyEventListener(kLoadPlayersFinished, ToJsonCString(dataDict));
	}];
}

- (void)loadPhoto:(NSDictionary *)playerInfoDict withPhotoSize:(GKPhotoSize)photoSize
{
	NSString *playerID		= [playerInfoDict objectForKey:kGKPlayerPlayerID];
	NSString *instanceID	= [playerInfoDict objectForKey:kGameServicesObjectInstanceID];
	
	[self getPlayerwithIdentifier:playerID withCompletionHandler:^(GKPlayer *player) {
		
		[self loadPlayerPhoto:player forInstanceID:instanceID havingSizeAs:photoSize];
	}];
}

- (void)loadPlayerPhoto:(GKPlayer *)player forInstanceID:(NSString *)instanceID havingSizeAs:(GKPhotoSize)photoSize
{
	if (player == NULL)
	{
		NSMutableDictionary *dataDict		= [NSMutableDictionary dictionary];
		[dataDict setObject:instanceID forKey:kGameServicesObjectInstanceID];
		[dataDict setObject:@"The operation could not be completed because player info could not be found." forKey:kGameServicesError];
		
		// Send event
		NotifyEventListener(kLoadPlayerPhotoFinished, ToJsonCString(dataDict));
	}
	else
	{
		[player loadPhotoForSize:photoSize withCompletionHandler:^(UIImage *photo, NSError *error) {

			NSMutableDictionary	*dataDict	= [NSMutableDictionary dictionary];
			[dataDict setObject:instanceID forKey:kGameServicesObjectInstanceID];

			if (error != NULL)
				[dataDict setObject:[error description] forKey:kGameServicesError];

			if (photo != NULL)
			{
				NSString *photoName			= [player photoName];
				NSString *photoPath			= [photo saveImageToDocumentsDirectory:photoName];
				
				if (photoPath)
					[dataDict setObject:photoPath forKey:kGKPlayerImagePathKey];
			}
			
			// Send event
			NotifyEventListener(kLoadPlayerPhotoFinished, ToJsonCString(dataDict));
		}];
	}
}

- (GKPlayer *)getPlayer:(NSString *)identifier
{
	if (identifier == NULL)
		return NULL;
	
	return [self.playerInfoCache objectForKey:identifier];
}

@end