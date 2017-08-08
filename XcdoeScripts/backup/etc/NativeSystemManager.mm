//
//  NativeSystemManager.mm
//  Unity-iPhone
//
//  Created by BaeMinGi on 2015. 7. 15..
//
//

#import "NativeSystemManager.h"
#import "AVFoundation/AVFoundation.h"

@implementation NativeSystemManager

+(NativeSystemManager*)Instance
{
    static NativeSystemManager* _instance = nil;
    
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        _instance = [[self alloc] init];
        [_instance retain];
    });
    
    return _instance;
}

-(BOOL)IsAudioPlaying
{
    NSError *error;
    [[AVAudioSession sharedInstance] setCategory:AVAudioSessionCategoryAmbient error:&error];
    
    bool inUse = [[AVAudioSession sharedInstance] isOtherAudioPlaying];
    return inUse;
}
@end


extern "C"
{
    bool NativeSystemManager_IsAudioPlaying()
    {
        return [[NativeSystemManager Instance] IsAudioPlaying];
    }
}
