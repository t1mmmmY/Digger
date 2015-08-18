//
//  WebViewEmbeddedVideoPlayer.h
//  NativePluginIOSWorkspace
//
//  Created by Ashwin kumar on 21/01/15.
//  Copyright (c) 2015 Voxel Busters Interactive LLP. All rights reserved.
//

#import <MediaPlayer/MediaPlayer.h>
#import "CustomWebView.h"

@class WebViewEmbeddedVideoPlayer;

@protocol WebViewEmbeddedVideoPlayerDelegate <NSObject>

@required
- (void)webviewEmbeddedPlayer:(WebViewEmbeddedVideoPlayer *)player didFinishPlaying:(MPMovieFinishReason)reason;

@end

@interface WebViewEmbeddedVideoPlayer : CustomWebView

// Properties
@property(nonatomic, assign)	id <WebViewEmbeddedVideoPlayerDelegate> delegate;

// Operations
- (void)stop;
- (void)pause;
- (void)playVideo:(NSString *)embedHTML;

@end

