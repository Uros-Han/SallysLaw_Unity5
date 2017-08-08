//
//  GameCenterManager.m
//  Unity-iPhone
//
//  Created by RoughHands_Black on 8/11/14.
//
//

#import "GameCenterManager.h"
#import "UnityAppController.h"


@implementation GameCenterManager


+(GameCenterManager*)getInstance
{
    static GameCenterManager* _gameCenterManagerSingletonInstance = nil;
    
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        _gameCenterManagerSingletonInstance = [[self alloc] init];
        [_gameCenterManagerSingletonInstance retain];
    });
    
    return _gameCenterManagerSingletonInstance;
}

-(id) init
{
    if( self = [super init] )
    {
        m_RootViewController = nil;
        m_AchievementsList = [[NSMutableDictionary alloc] init];
        [m_AchievementsList retain];
        [m_AchievementsList removeAllObjects];
    }
    
    return self;
}

-(void) dealloc
{
    NSAssert(false, @"[GameCenterManager] dealloc Should Never Called. Singleton Instance");

    if( m_RootViewController != nil )
    {
        [m_RootViewController release];
        m_RootViewController = nil;
    }
    
    if( m_AchievementsList != nil )
    {
        [m_AchievementsList release];
        m_AchievementsList = nil;
    }
    
    [super dealloc];
}

- (void)SetRootViewController:(UIViewController*)rootViewController
{
    if( m_RootViewController != nil )
    {
        [m_RootViewController release];
        m_RootViewController = nil;
    }
    
    m_RootViewController = rootViewController;
    [m_RootViewController retain];
}

-(void) AuthenticateLocalPlayer
{
    if ([GKLocalPlayer localPlayer].authenticated == NO) {
        GKLocalPlayer *localPlayer = [GKLocalPlayer localPlayer];
        // Available in iOS 6.0 and later.
        [localPlayer setAuthenticateHandler:(^(UIViewController* viewcontroller, NSError *error) {
            if(localPlayer.isAuthenticated)
            {
                NSLog(@"[GameCenterManager] Authentication Completed");
                UnitySendMessage("GameCenterManager", "Callback_GameCenter_AuthenticationSuccess", "");
            }else{
                if (viewcontroller) {
                    NSLog(@"[GameCenterManager] Need to Authenticate Local Player");
                    if (m_RootViewController == nil) {
                        UIViewController* rootViewController = [UIApplication sharedApplication].keyWindow.rootViewController;
                        [self SetRootViewController:rootViewController];
                    }
                    [m_RootViewController presentViewController: viewcontroller animated: YES completion:nil];
                }
                else {
                    NSLog(@"[GameCenterManager] Authentication Failed");
                }
            }
        })];
    }
}

- (void)gameCenterViewControllerDidFinish:(GKGameCenterViewController *)gameCenterViewController
{
    [m_RootViewController dismissViewControllerAnimated:YES completion:nil];
}

-(BOOL)IsAuthenticated
{
    GKLocalPlayer* localPlayer = [GKLocalPlayer localPlayer];
    return localPlayer.isAuthenticated;
}

-(void)UpdateAchievement:(NSString*)achievementID percentComplete:(float)percent
{
    CheckAuthenticationAndReturn;

    GKAchievement* achievement = [[GKAchievement alloc] initWithIdentifier:achievementID];
    [achievement setPercentComplete:percent];
    [achievement setShowsCompletionBanner:YES];

    NSLog(@"GetAchievement: %@",achievement.identifier);

    NSAssert1( (achievement!=nil), @"[GameCenterManager] Error in Initialize Achievement Object:%@", achievementID);

    NSArray *achievementsToComplete = [NSArray arrayWithObjects:achievement, nil];
    [GKAchievement reportAchievements:achievementsToComplete withCompletionHandler:^(NSError *error)
    {
        if (error != nil)
        {
            NSLog(@"[GameCenterManager] Error in reporting achievements: %@", error);
        }
        else {
            NSLog(@"[GameCenterManager] Complete Update Achievement: %@ , %f", achievementID, percent);
        }
    }];
}

- (void)ShowBanner:(NSString*)title :(NSString*)message :(void(^)(void))completionHandler
{
    [GKNotificationBanner showBannerWithTitle: title message: message completionHandler:completionHandler];
}

- (void)LoadAchievements
{
    CheckAuthenticationAndReturn;
    
    [GKAchievement loadAchievementsWithCompletionHandler:^(NSArray *achievements, NSError *error)
    {
        NSMutableDictionary* achievementJSONDictionary = [[NSMutableDictionary alloc] init];
        
        if (error != nil)
        {
            NSLog(@"[GameCenterManager] Failed Loading Achievements");
        }
        if (achievements != nil)
        {
            [m_AchievementsList removeAllObjects];
            
            for (GKAchievement* achievement in achievements)
            {
                NSLog(@"[GameCenterManager] %@, %f, %d, %d, %@", achievement.identifier, achievement.percentComplete, achievement.completed, [achievement isHidden], achievement.description);
                [m_AchievementsList setObject:achievement forKey:achievement.identifier];
                
                NSMutableDictionary* achievementInfo = [[NSMutableDictionary alloc] init];
                [achievementInfo setObject:achievement.identifier forKey:@"achievementID"];
                [achievementInfo setObject:[NSNumber numberWithFloat:achievement.percentComplete] forKey:@"percentComplete"];
                [achievementInfo setObject:[NSNumber numberWithBool:achievement.isCompleted] forKey:@"isCompleted"];
                [achievementInfo setObject:[NSNumber numberWithBool:[achievement isHidden]] forKey:@"isHidden"];

                [achievementJSONDictionary setValue:achievementInfo forKey:achievement.identifier];
            }
        }
        
        NSString* achievementListJSON = [[NSString alloc] init];
        if( achievementJSONDictionary.count == 0 )
        {
            achievementListJSON = @"{}";
        }
        else
        {
            NSError* nsError = nil;
            NSData* achievementJSONListData = [NSJSONSerialization dataWithJSONObject:achievementJSONDictionary options:NSJSONWritingPrettyPrinted error:&nsError];

            if( !achievementJSONListData )
            {
                NSLog(@"JSON Serialization Error : %@", nsError.localizedDescription);
                achievementListJSON = @"{}";
            }
            else
            {
                achievementListJSON = [[NSString alloc] initWithData:achievementJSONListData encoding:NSUTF8StringEncoding];
            }
    
    
            NSLog(@"[GameCenterManager] Achievement Loaded JSON \n %@", achievementListJSON);
        }
        
        UnitySendMessage("GameCenterManager", "Callback_GameCenter_CompleteLoadingAchievements", [achievementListJSON UTF8String]);// cStringUsingEncoding:NSUTF8StringEncoding]);
    }];
}

-(void) ResetAchievements
{
    [GKAchievement resetAchievementsWithCompletionHandler:^(NSError *error) {
        if (error != nil)
        {
            NSLog(@"[GameCenterManager] Failed Reset Achievements");
        }
        else {
            NSLog(@"[GameCenterManager] Success Reset Achievements");
        }
    }];
}

- (void)ShowAchievementView
{
    GKGameCenterViewController *gameCenterController = [[GKGameCenterViewController alloc] init];
    if (gameCenterController != nil)
    {
        gameCenterController.gameCenterDelegate = self;
        gameCenterController.viewState = GKGameCenterViewControllerStateAchievements;

        if (m_RootViewController == nil) {
            UIViewController* rootViewController = [UIApplication sharedApplication].keyWindow.rootViewController;
            [self SetRootViewController:rootViewController];
        }
        [m_RootViewController presentViewController: gameCenterController animated: YES completion:nil];
    }
}





-(void)OpenLeaderboard:(NSString *)identifier
{
    GKGameCenterViewController *view = [[[GKGameCenterViewController alloc] init] autorelease];
    if(view != nil)
    {
        view.gameCenterDelegate = self;
        view.viewState = GKGameCenterViewControllerStateLeaderboards;
        view.leaderboardIdentifier = identifier;
        
        if (m_RootViewController == nil) {
            UIViewController* rootViewController = [UIApplication sharedApplication].keyWindow.rootViewController;
            [self SetRootViewController:rootViewController];
        }
        
        [m_RootViewController presentViewController:view animated:YES completion:nil];
    }
}

-(void)ReportScore:(int64_t)score forLeaderboardID:(NSString *)identifier
{
    GKScore *scoreReporter = [[GKScore alloc] initWithLeaderboardIdentifier:identifier];
    scoreReporter.value = score;
    
    [GKScore reportScores:@[scoreReporter] withCompletionHandler:^(NSError *error) {}];
}

- (void) LoadMyScore:(NSString *)identifier
{
    GKLeaderboard *board = [[GKLeaderboard alloc] init];
    board.identifier = identifier;
    [board loadScoresWithCompletionHandler:^(NSArray *scores, NSError *error)
    {
        if( error != nil )
        {
            return;
        }

        UnitySendMessage("GameCenterManager", "Callback_GameCenter_LoadMyScore", [[NSString stringWithFormat:@"%lld",board.localPlayerScore.value] UTF8String]);
    }];
}

@end


extern "C"
{
    void GameCenterManager_AuthenticateLocalPlayer()
    {
        [[GameCenterManager getInstance] AuthenticateLocalPlayer];
    }

    bool GameCenterManager_IsAuthenticated()
    {
        return (YES == [[GameCenterManager getInstance] IsAuthenticated]);
    }

    void GameCenterManager_UpdateAchievement(char* achievementID_, float percentComplete)
    {
        NSString* achievementID = [NSString stringWithUTF8String:achievementID_];

        [[GameCenterManager getInstance] UpdateAchievement:achievementID percentComplete:percentComplete];
    }

    // Load and callback Unity with JSON of entire AchivementList
    void GameCenterManager_LoadAchievements()
    {
        [[GameCenterManager getInstance] LoadAchievements];
    }
    
    // Load and callback Unity with JSON of entire AchivementList
    void GameCenterManager_ResetAchievements()
    {
        [[GameCenterManager getInstance] ResetAchievements];
    }

    void GameCenterManager_ShowAchievementView()
    {
        [[GameCenterManager getInstance] ShowAchievementView];
    }
    
    
    
    //leader board.
    void GameCenterManager_OpenLeaderboard(char* leaderboardID)
    {
         NSString* identifier = [NSString stringWithUTF8String:leaderboardID];
        [[GameCenterManager getInstance] OpenLeaderboard:identifier];
    }
    
    void GameCenterManager_LoadMyScore(char* leaderboardID)
    {
        NSString* identifier = [NSString stringWithUTF8String:leaderboardID];
        [[GameCenterManager getInstance] LoadMyScore:identifier];
    }
    
    void GameCenterManager_ReportScore(int score,char* leaderboardID)
    {
        NSString* identifier = [NSString stringWithUTF8String:leaderboardID];
        
        [[GameCenterManager getInstance] ReportScore:score forLeaderboardID:identifier];
    }
}
