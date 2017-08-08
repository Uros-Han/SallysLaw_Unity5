//
//  GameCenterManager.h
//  Unity-iPhone
//
//  Created by RoughHands_Black on 8/11/14.
//
//

#import <Foundation/Foundation.h>
#import <GameKit/GameKit.h>

#define CheckAuthenticationAndReturn \
    if( NO == [self IsAuthenticated] )\
    {\
        NSLog(@"[GameCenterManager] Authenticate Before %s", __FUNCTION__);\
        return;\
    }

// NOTE :   alloc and copy memory when returns string to unity
//          returns string on Unity DLL Plugin
//          Unity deletes the memory of returned pointer internally
// http://www.mono-project.com/Interop_with_Native_Libraries#Strings_as_Return_Values
#define CopyAndReturnStringToUnity(nsString)\
    const char* cString = [nsString UTF8String];\
    if( cString == NULL )\
    {\
        return NULL;\
    }\
    char* returnString = (char*)malloc(strlen(cString)+1);\
    strcpy(returnString, cString);\
    return returnString;
    

@class GKLocalPlayer;

// Singleton Object
@interface GameCenterManager : NSObject<GKGameCenterControllerDelegate>
{
    UIViewController*           m_RootViewController;
    NSMutableDictionary*        m_AchievementsList;
};

// currentPlayerID is the value of the playerID last time GameKit authenticated.
@property (retain,readwrite) NSString * currentPlayerID;

+(GameCenterManager*)getInstance;

- (void)SetRootViewController:(UIViewController*)rootViewController;
- (void)AuthenticateLocalPlayer;
- (BOOL)IsAuthenticated;
- (void)UpdateAchievement:(NSString*)achievementID percentComplete:(float)percent;
- (void)LoadAchievements;
- (void) ResetAchievements;
- (void)ShowBanner:(NSString*)title :(NSString*)message :(void(^)(void))completionHandler;
- (void)ShowAchievementView;
- (void)OpenLeaderboard:(NSString *)identifier;
- (void)ReportScore:(int64_t)score forLeaderboardID:(NSString *)identifier;
- (void)LoadMyScore:(NSString *)identifier;

@end


extern "C"
{
    void            GameCenterManager_AuthenticateLocalPlayer();
    bool            GameCenterManager_IsAuthenticated();
    void            GameCenterManager_UpdateAchievement(char* achievementID, float percentComplete);
    void            GameCenterManager_LoadAchievements();     // Load and callback Unity with JSON of entire AchivementList
    void            GameCenterManager_ShowAchievementView();
    
    void            GameCenterManager_OpenLeaderboard(char* leaderboardID);
    void            GameCenterManager_LoadMyScore(char* leaderboardID);
    void            GameCenterManager_ReportScore(int score,char* leaderboardID);
}


