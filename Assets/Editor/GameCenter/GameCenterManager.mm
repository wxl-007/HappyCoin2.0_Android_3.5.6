//
//  GameCenterManager.m
//  GameCenterTest
//
//  Created by Mike on 9/3/10.
//  Copyright 2010 Prime31 Studios. All rights reserved.
//

#import "GameCenterManager.h"
#import "JSONKit.h"


void UnityPause( bool pause );

void UnitySendMessage( const char * className, const char * methodName, const char * param );

UIViewController *UnityGetGLViewController();


BOOL isGameCenterAvailable();
BOOL isGameCenterAvailable()
{
	// Check for presence of GKLocalPlayer API.
	Class gcClass = NSClassFromString( @"GKLocalPlayer" );
	
	// The device must be running running iOS 4.1 or later
	NSString *reqSysVer = @"4.1";
	NSString *currSysVer = [[UIDevice currentDevice] systemVersion];
	BOOL osVersionSupported = ( [currSysVer compare:reqSysVer options:NSNumericSearch] != NSOrderedAscending );
	
	return ( gcClass && osVersionSupported );
}


@interface GameCenterManager(Private)
- (void)loadAchievements;
- (GKAchievement*)getAchievementForIdentifier:(NSString*)identifier;
@end



@implementation GameCenterManager

@synthesize achievementsDictionary = _achievementsDictionary;

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Class Methods

+ (BOOL)isGameCenterAvailable
{
	return isGameCenterAvailable();
}


+ (GameCenterManager*)sharedManager
{
	static GameCenterManager *sharedSingleton;
	
	if( !sharedSingleton )
		sharedSingleton = [[GameCenterManager alloc] init];
	
	return sharedSingleton;
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark NSObject

- (id)init
{
	// If GameCenter isnt available, early out with nil to avoid a crash
	if( ![GameCenterManager isGameCenterAvailable] )
		return nil;
	
	if( ( self = [super init] ) )
	{
		// setup the acievements holder
		_achievementsDictionary = [[NSMutableDictionary alloc] init];
		
		// listen for auth changes
		[[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(authenticationChanged) name:GKPlayerAuthenticationDidChangeNotificationName object:nil];
		
		// load up our turn based manager if it exists
		Class gctbManagerClass = NSClassFromString( @"GameCenterTurnBasedManager" );
		if( gctbManagerClass )
			[gctbManagerClass sharedManager];
	}
	return self;
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Private - shared with GameCenterMultiplayer

- (UIViewController*)getViewControllerForModalPresentation:(BOOL)destroyIfExists
{
	return UnityGetGLViewController();
	
	
	/* legacy pre Unity 3.4 path
	if( destroyIfExists && _viewControllerWrapper )
	{
		[_viewControllerWrapper dismissModalViewControllerAnimated:NO];
		[_viewControllerWrapper.view removeFromSuperview];
		[_viewControllerWrapper release];
		_viewControllerWrapper = nil;
	}
	else if( !_viewControllerWrapper )
	{
		// Create a wrapper controller to house the picker.  If this is iPad, use a rotating view controller
		if( UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad )
			_viewControllerWrapper = [[GameCenterRotatingViewController alloc] initWithNibName:nil bundle:nil];
		else
			_viewControllerWrapper = [[UIViewController alloc] initWithNibName:nil bundle:nil];
		
		// add the wrapper to the window
		[[UIApplication sharedApplication].keyWindow addSubview:_viewControllerWrapper.view];
	}
	
	// zero the frame so it is hidden
	_viewControllerWrapper.view.frame = CGRectZero;
	
	return _viewControllerWrapper;
	*/
}


- (void)showViewControllerModallyInWrapper:(UIViewController*)viewController
{
	// pause the game
	UnityPause( true );
	
	// cancel the previous delayed call to dismiss the view controller if it exists
	[NSObject cancelPreviousPerformRequestsWithTarget:self];
	
	UIViewController *vc = [self getViewControllerForModalPresentation:YES];
	
	// show the controller
	[vc presentModalViewController:viewController animated:YES];
}


- (void)dismissWrappedController
{
	UnityPause( false );

	UIViewController *vc = [self getViewControllerForModalPresentation:NO];
	
	// No view controller? Get out of here.
	if( !vc )
		return;
	
	// dismiss the picker
	[vc dismissModalViewControllerAnimated:YES];
	
	/* legacy pre Unity 3.4 path
	// remove the wrapper view controller
	[self performSelector:@selector(removeAndReleaseViewControllerWrapper) withObject:nil afterDelay:1.0];
	*/
}


- (void)removeAndReleaseViewControllerWrapper
{
	[_viewControllerWrapper.view removeFromSuperview];
	[_viewControllerWrapper release];
	_viewControllerWrapper = nil;
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark NSNotifications

- (void)authenticationChanged
{
	if( [GKLocalPlayer localPlayer].isAuthenticated )
	{
		// Insert code here to handle a successful authentication.
		UnitySendMessage( "GameCenterManager", "playerDidAuthenticate", [[GKLocalPlayer localPlayer].alias UTF8String] );
		
		// Load achievements to be used later if needed
		[self loadAchievements];
		
		// load up our multiplayer manager if it exists
		Class gcMultiplayerManagerClass = NSClassFromString( @"GameCenterMultiplayer" );
		if( gcMultiplayerManagerClass )
			[gcMultiplayerManagerClass sharedManager];
	}
	else
	{
		// Insert code here to clean up any outstanding Game Center-related classes.
		[_achievementsDictionary removeAllObjects];
		UnitySendMessage( "GameCenterManager", "playerDidLogOut", "" );
	}
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Public
#pragma mark Player Methods

- (void)authenticateLocalPlayer
{
	[[GKLocalPlayer localPlayer] authenticateWithCompletionHandler:^( NSError *error )
	{
		if( error )
			UnitySendMessage( "GameCenterManager", "playerAuthenticationFailed", [[error localizedDescription] UTF8String] );
	}];
}


- (void)retrieveFriends
{
	GKLocalPlayer *lp = [GKLocalPlayer localPlayer];
	
	if( lp.authenticated )
	{
		[lp loadFriendsWithCompletionHandler:^( NSArray *friends, NSError *error )
		{
			if( error )
			{
				// report error to user
				UnitySendMessage( "GameCenterManager", "loadPlayerDataDidFail", [[error localizedDescription] UTF8String] );
			}
			else
			{
				// either return an empty array or load the player data
				if( friends.count )
					[self loadPlayerData:friends];
				else
					UnitySendMessage( "GameCenterManager", "loadPlayerDataDidLoad", "[]" );
			}
		}];
	}
}
	
	
- (void)loadPlayerData:(NSArray*)identifiers
{
	[GKPlayer loadPlayersForIdentifiers:identifiers withCompletionHandler:^( NSArray *players, NSError *error )
	{
		if( error )
		{
			// handle error
			UnitySendMessage( "GameCenterManager", "loadPlayerDataDidFail", [[error localizedDescription] UTF8String] );
		}
		
		if( players )
		{
			NSMutableArray *playerArray = [NSMutableArray arrayWithCapacity:players.count];
			
			// Process the array of GKPlayer objects
			for( GKPlayer *player in players )
			{
				
				NSDictionary *playerDict = [NSDictionary dictionaryWithObjectsAndKeys:
												   player.alias, @"alias",
												   [NSNumber numberWithBool:player.isFriend], @"isFriend",
												   player.playerID, @"playerId", nil];
				
				[playerArray addObject:playerDict];
			}
			
			// send back the info
			NSString *json = [playerArray JSONString];
			UnitySendMessage( "GameCenterManager", "loadPlayerDataDidLoad", [json UTF8String] );
		}
	}];
}


- (BOOL)isPlayerAuthenticated
{
	return [GKLocalPlayer localPlayer].authenticated;
}


- (NSString*)playerAlias
{
	return [GKLocalPlayer localPlayer].alias;
}


- (NSString*)playerId
{
	return [GKLocalPlayer localPlayer].playerID;
}


- (BOOL)isUnderage
{
	return [GKLocalPlayer localPlayer].underage;
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Leaderboards

- (void)loadLeaderboardCategoryTitles
{
	[GKLeaderboard loadCategoriesWithCompletionHandler:^( NSArray *categories, NSArray *titles, NSError *error )
	{
		if( error )
		{
			UnitySendMessage( "GameCenterManager", "loadCategoryTitlesDidFail", [[error localizedDescription] UTF8String] );
			return;
		}
		
		// use category and title information
		NSDictionary *dict = [NSDictionary dictionaryWithObjects:categories forKeys:titles];
		NSString *json = [dict JSONString];
		UnitySendMessage( "GameCenterManager", "categoriesDidLoad", [json UTF8String] );
	}];
}


- (void)reportScoreOnBackgroundThread:(GKScore*)scoreReporter
{
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
	
	[scoreReporter reportScoreWithCompletionHandler:^( NSError *error )
	{
		 if( error )
		 {
			 // handle error.  If network error, save scores for later use (they are NSCoding compliant)
			 UnitySendMessage( "GameCenterManager", "reportScoreDidFail", [scoreReporter.category UTF8String] );
		 }
		 else
		 {
			 UnitySendMessage( "GameCenterManager", "reportScoreDidFinish", [scoreReporter.category UTF8String] );
		 }
	}];
	
	[pool release];
}


- (void)reportScore:(int64_t)score forLeaderboard:(NSString*)category
{
	GKScore *scoreReporter = [[[GKScore alloc] initWithCategory:category] autorelease];
	scoreReporter.value = score;
	
	[self performSelectorInBackground:@selector(reportScoreOnBackgroundThread:) withObject:scoreReporter];
}


- (void)showLeaderboardWithTimeScope:(GKLeaderboardTimeScope)timeScope
{
	[self showLeaderboardWithTimeScope:timeScope category:nil];
}


- (void)showLeaderboardWithTimeScope:(GKLeaderboardTimeScope)timeScope category:(NSString*)category
{
	GKLeaderboardViewController *leaderboardController = [[GKLeaderboardViewController alloc] init];
	leaderboardController.timeScope = timeScope;
	
	// optional category
	if( category )
		leaderboardController.category = category;
	
	if( leaderboardController )
	{
		leaderboardController.leaderboardDelegate = self;
		[self showViewControllerModallyInWrapper:leaderboardController];
	}
}


- (void)retrieveScores:(BOOL)friendsOnly timeScope:(GKLeaderboardTimeScope)timeScope range:(NSRange)range
{
	[self retrieveScores:friendsOnly timeScope:timeScope range:range category:nil];
}


- (void)retrieveScores:(BOOL)friendsOnly timeScope:(GKLeaderboardTimeScope)timeScope range:(NSRange)range category:(NSString*)category
{
	GKLeaderboard *leaderboardRequest = [[GKLeaderboard alloc] init];
	
	if( !leaderboardRequest )
		return;
	
	leaderboardRequest.playerScope = ( friendsOnly ) ? GKLeaderboardPlayerScopeFriendsOnly : GKLeaderboardPlayerScopeGlobal;
	leaderboardRequest.timeScope = timeScope;
	leaderboardRequest.range = range;
	leaderboardRequest.category = category;
	
	[leaderboardRequest loadScoresWithCompletionHandler: ^( NSArray *scores, NSError *error )
	{
		 if( error )
		 {
			 // handle the error.
			 UnitySendMessage( "GameCenterManager", "retrieveScoresDidFail", [[error localizedDescription] UTF8String] );
		 }
		 
		 if( scores )
		 {
			 // process the score information.
			 NSMutableArray *scoreArray = [NSMutableArray arrayWithCapacity:scores.count];
			 NSMutableArray *playerIdentifers = [NSMutableArray array];
			 
			 for( GKScore *score in scores )
			 {
				 // Save this for later use in getting the alias
				 [playerIdentifers addObject:score.playerID];
				 
				 NSString *category = ( score.category ) ? score.category : @"";
				 NSMutableDictionary *scoreDict = [NSMutableDictionary dictionaryWithObjectsAndKeys:category, @"category",
												   [NSNumber numberWithDouble:[score.date timeIntervalSince1970]], @"date",
												   score.formattedValue, @"formattedValue",
												   score.playerID, @"playerId",
												   [NSNumber numberWithInt:score.rank], @"rank",
												   [NSNumber numberWithInt:score.value], @"value", nil];
				 [scoreArray addObject:scoreDict];
			 }
			 
			// Now we need to grab the playerAliases
			[GKPlayer loadPlayersForIdentifiers:playerIdentifers withCompletionHandler:^( NSArray *players, NSError *error )
			{
				if( error )
					UnitySendMessage( "GameCenterManager", "retrieveScoresDidFail", [[error localizedDescription] UTF8String] );
				  
				if( players )
				{
					// Process the array of GKPlayer objects
					for( GKPlayer *player in players )
					{
						for( NSMutableDictionary *scoreDict in scoreArray )
						{
							if( [[scoreDict objectForKey:@"playerId"] isEqualToString:player.playerID] )
							{
								[scoreDict setObject:player.alias forKey:@"alias"];
								[scoreDict setObject:[NSNumber numberWithBool:player.isFriend] forKey:@"isFriend"];
								break;
							}
						}
					}
				}
				  
				// send back the info
				NSString *json = [scoreArray JSONString];
				UnitySendMessage( "GameCenterManager", "retrieveScoresDidLoad", [json UTF8String] );
			}];
		}
		else
		{
			// no scores to report
			UnitySendMessage( "GameCenterManager", "retrieveScoresDidLoad", "[]" );
		}

	 }];
}


- (void)retrieveScoresForPlayerId:(NSString*)playerId
{
	[self retrieveScoresForPlayerId:playerId category:nil];
}


- (void)retrieveScoresForPlayerId:(NSString*)playerId category:(NSString*)category
{
    GKLeaderboard *leaderboardRequest = [[GKLeaderboard alloc] initWithPlayerIDs:[NSArray arrayWithObject:playerId]];
	
	if( !leaderboardRequest )
		return;
	
	leaderboardRequest.category = category;
	
	[leaderboardRequest loadScoresWithCompletionHandler:^( NSArray *scores, NSError *error )
	{
		 if( error )
		 {
			 // handle the error.
			 UnitySendMessage( "GameCenterManager", "retrieveScoresForPlayerIdDidFail", [[error localizedDescription] UTF8String] );
		 }
		 
		 if( scores )
		 {
			 // process the score information.
			 NSMutableArray *scoreArray = [NSMutableArray arrayWithCapacity:scores.count];
			 NSMutableArray *playerIdentifers = [NSMutableArray array];
			 
			 for( GKScore *score in scores )
			 {
				 // Save this for later use in getting the alias
				 [playerIdentifers addObject:score.playerID];
				
				 NSString *category = ( score.category ) ? score.category : @"";
				 NSMutableDictionary *scoreDict = [NSMutableDictionary dictionaryWithObjectsAndKeys:category, @"category",
												   [NSNumber numberWithDouble:[score.date timeIntervalSince1970]], @"date",
												   score.formattedValue, @"formattedValue",
												   score.playerID, @"playerId",
												   [NSNumber numberWithInt:score.rank], @"rank",
												   [NSNumber numberWithInt:score.value], @"value", nil];
				 [scoreArray addObject:scoreDict];
			 }

			 // Now we need to grab the playerAliases
			 [GKPlayer loadPlayersForIdentifiers:playerIdentifers withCompletionHandler:^( NSArray *players, NSError *error )
			 {
				  if( error )
					  UnitySendMessage( "GameCenterManager", "retrieveScoresDidFail", [[error localizedDescription] UTF8String] );
				  
				  if( players )
				  {
					  // Process the array of GKPlayer objects
					  for( GKPlayer *player in players )
					  {
						  for( NSMutableDictionary *scoreDict in scoreArray )
						  {
							  if( [[scoreDict objectForKey:@"playerId"] isEqualToString:player.playerID] )
							  {
								  [scoreDict setObject:player.alias forKey:@"alias"];
								  [scoreDict setObject:[NSNumber numberWithBool:player.isFriend] forKey:@"isFriend"];
								  break;
							  }
						  }
					  }
				  }
				  
				  // send back the info
				  NSString *json = [scoreArray JSONString];
				  UnitySendMessage( "GameCenterManager", "retrieveScoresForPlayerIdDidLoad", [json UTF8String] );
			  }];
		 }
		 else
		 {
			 // no scores available
			 UnitySendMessage( "GameCenterManager", "retrieveScoresForPlayerIdDidLoad", "[]" );
		 }
	 }];
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark GKLeaderboardViewControllerDelegate

- (void)leaderboardViewControllerDidFinish:(GKLeaderboardViewController*)viewController
{
	[self dismissWrappedController];
}


// TODO: does this belong here or in multiplayer
// For later when multiplayer is added
- (void)receiveMatchBestScores:(GKMatch*)match
{
	GKLeaderboard *query = [[GKLeaderboard alloc] initWithPlayerIDs:match.playerIDs];
	
	if( !query )
		return;
	
	[query loadScoresWithCompletionHandler: ^( NSArray *scores, NSError *error )
	{
		if( error )
		{
			UnitySendMessage( "GameCenterManager", "retrieveMatchesBestScoresDidFail", [[error localizedDescription] UTF8String] );
		}
		
		if( scores )
		{
			// process the score information.
		}
	}];
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Achievements

- (void)reportAchievementIdentifier:(NSString*)identifier percentComplete:(float)percent
{
	GKAchievement *achievement = [self getAchievementForIdentifier:identifier];
	if( !achievement )
		return;
	
	// should we show a banner if complete?
	if( _showCompletionBanner )
	{
		if( [achievement respondsToSelector:@selector(setShowsCompletionBanner:)] )
			[achievement performSelector:@selector(setShowsCompletionBanner:) withObject:[NSNumber numberWithBool:YES]];
	}
	
	achievement.percentComplete = percent;
	
	// send off the request
	[achievement reportAchievementWithCompletionHandler:^( NSError *error )
	{
		if( error )
		{
			// handle error
			UnitySendMessage( "GameCenterManager", "reportAchievementDidFail", [[error localizedDescription] UTF8String] );

			// Retain the achievement object and try again later
		}
		else
		{
			UnitySendMessage( "GameCenterManager", "reportAchievementDidFinish", [achievement.identifier UTF8String] );
		}
	}];
}


// called whenever a player is authenticated
- (void)loadAchievements
{
	[GKAchievement loadAchievementsWithCompletionHandler:^( NSArray *achievements, NSError *error )
	{
		if( error )
			UnitySendMessage( "GameCenterManager", "loadAchievementsDidFail", [[error localizedDescription] UTF8String] );
		
 		if( achievements )
		{			
			for( GKAchievement *achievement in achievements )
			{
				// store these locally for easy access
				[_achievementsDictionary setObject:achievement forKey:achievement.identifier];
			}
			
			// kick off an achievementsDidLoad call back to Unity
			[self getAchievements];
		}
	}];
}


- (void)getAchievements
{
	// use the identifier property to match each GKAchievementDescription object to the corresponding GKAchievement object
	NSMutableArray *achievementArray = [NSMutableArray arrayWithCapacity:_achievementsDictionary.count];
	NSArray *allIdentifiers = [_achievementsDictionary allKeys];
	
	for( NSString *identifer in allIdentifiers )
	{
		GKAchievement *achievement = [_achievementsDictionary objectForKey:identifer];
		
		NSMutableDictionary *achievementDict = [NSMutableDictionary dictionaryWithObjectsAndKeys:
												achievement.identifier, @"identifier",
												[NSNumber numberWithBool:achievement.completed], @"completed",
												[NSNumber numberWithBool:achievement.hidden], @"hidden",
												[NSNumber numberWithDouble:[achievement.lastReportedDate timeIntervalSince1970]], @"lastReportedDate",
												[NSNumber numberWithInt:achievement.percentComplete], @"percentComplete", nil];
		[achievementArray addObject:achievementDict];
	}
	
	// send back the info
	NSString *json = [achievementArray JSONString];
	UnitySendMessage( "GameCenterManager", "achievementsDidLoad", [json UTF8String] );
}


// Never create an achievement object directly.  Always use this method.
- (GKAchievement*)getAchievementForIdentifier:(NSString*)identifier
{
	GKAchievement *achievement = [_achievementsDictionary objectForKey:identifier];
	
	if( !achievement )
	{
		achievement = [[[GKAchievement alloc] initWithIdentifier:identifier] autorelease];
		[_achievementsDictionary setObject:achievement forKey:achievement.identifier];
	}
	
	return [[achievement retain] autorelease];
}


- (void)resetAchievements
{
	// Clear all progress saved on Game Center
	[GKAchievement resetAchievementsWithCompletionHandler:^( NSError *error )
	{
		if( error )
		{
			UnitySendMessage( "GameCenterManager", "resetAchievementsDidFail", [[error localizedDescription] UTF8String] );
		}
		else
		{
			// Clear all locally saved achievement objects.
			_achievementsDictionary = [[NSMutableDictionary alloc] init];
			
			UnitySendMessage( "GameCenterManager", "resetAchievementsDidFinish", "" );
		}
	}];
}


- (void)showAchievements
{
    GKAchievementViewController *achievements = [[GKAchievementViewController alloc] init];
    if( achievements )
    {
        achievements.achievementDelegate = self;
		[self showViewControllerModallyInWrapper:achievements];
    }
    [achievements release];
}


- (void)retrieveAchievementMetadata
{
    [GKAchievementDescription loadAchievementDescriptionsWithCompletionHandler:^( NSArray *descriptions, NSError *error )
	 {
		 if( error )
			 UnitySendMessage( "GameCenterManager", "retrieveAchievementsMetaDataDidFail", [[error localizedDescription] UTF8String] );
		 
		 if( descriptions )
		 {
			 // use the achievement descriptions.
			 // use the identifier property to match each GKAchievementDescription object to the corresponding GKAchievement object
			 NSMutableArray *achievementArray = [NSMutableArray arrayWithCapacity:descriptions.count];
			 
			 for( GKAchievementDescription *achievement in descriptions )
			 {
				 NSDictionary *achievementDict = [NSDictionary dictionaryWithObjectsAndKeys:
														 achievement.achievedDescription, @"achievedDescription",
														 achievement.identifier, @"identifier",
														 [NSNumber numberWithBool:achievement.hidden], @"hidden",
														 [NSNumber numberWithInt:achievement.maximumPoints], @"maximumPoints",
														 achievement.title, @"title",
														 achievement.unachievedDescription, @"unachievedDescription", nil];
				 [achievementArray addObject:achievementDict];
			 }
			 
			 // send back the info
			 NSString *json = [achievementArray JSONString];
			 UnitySendMessage( "GameCenterManager", "achievementMetadataDidLoad", [json UTF8String] );
		 }
	 }];
}


- (void)showCompletionBannerForAchievements
{
	_showCompletionBanner = YES;
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark GKAchievementViewControllerDelegate

- (void)achievementViewControllerDidFinish:(GKAchievementViewController*)viewController
{
    [self dismissWrappedController];
}

@end
