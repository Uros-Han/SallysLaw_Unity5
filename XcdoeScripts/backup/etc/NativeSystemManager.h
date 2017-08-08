//
//  NativeSystemManager.h
//  Unity-iPhone
//
//  Created by BaeMinGi on 2015. 7. 15..
//
//

#ifndef Unity_iPhone_PlaybackManager_h
#define Unity_iPhone_PlaybackManager_h
 #import <Foundation/Foundation.h>

@interface NativeSystemManager : NSObject
+(NativeSystemManager*)Instance;

- (BOOL)IsAudioPlaying;
 @end

extern "C"
{
    bool            NativeSystemManager_IsAudioPlaying();
}
#endif


