//
//  WebViewEmbeddedVideoPlayer.m
//  NativePluginIOSWorkspace
//
//  Created by Ashwin kumar on 21/01/15.
//  Copyright (c) 2015 Voxel Busters Interactive LLP. All rights reserved.
//

#import "WebViewEmbeddedVideoPlayer.h"

#define kEmbeddedPlayerURLScheme	@"embeddedplayer"
#define kOnReadyEvent				@"OnReady"
#define kOnStateChangedEvent		@"OnStateChange"
#define kOnErrorEvent				@"OnError"

@implementation WebViewEmbeddedVideoPlayer

@synthesize delegate;

- (id)initWithFrame:(CGRect)frame
{
	return [self initWithFrame:frame tag:@"video-player"];
}

- (id)initWithFrame:(CGRect)frame tag:(NSString *)tag
{
	self = [super initWithFrame:frame tag:tag];
	
    if (self)
	{
        // Initialization code
		[[[self webview] scrollView] setScrollEnabled:NO];
		[self setCanBounce:NO];
		[self setAutoShowOnLoadFinish:NO];
		[self setControlType:WebviewControlTypeCloseButton];
		
		// Add schema
		[self addNewURLScheme:kEmbeddedPlayerURLScheme];
	}
	
    return self;
}

#pragma mark - Playback Operations

- (void)stop
{
	// Remove from view
	[self dismiss];
	
	// Stop play loading
	[self stringByEvaluatingJavaScriptFromString:@"stopPlayer()"];
	
	// Notidy delegate
	[self notifyDidFinishPlaying:MPMovieFinishReasonUserExited];
}

- (void)pause
{
	[self stringByEvaluatingJavaScriptFromString:@"pausePlayer()"];
}

- (void)playVideo:(NSString *)embedHTML
{
	// Load embedded string
	[self loadHTMLString:embedHTML baseURL:[[NSBundle mainBundle] resourceURL]];
	
	// Show
	[self show];
}

#pragma mark - Override URL Scheme

- (void)foundMatchingURLScheme:(NSURL *)requestURL
{
	NSMutableDictionary *parsedDict	= [self parseURLScheme:requestURL];
	NSMutableDictionary *argsDict	= [parsedDict objectForKey:@"arguments"];
	
	// Notify unity
	NSString *host	= [parsedDict objectForKey:@"host"];
	NSString *value	= [argsDict objectForKey:@"value"];
	
	if (IsNullOrEmpty(value))
		value	= kNSStringDefault;
	
	if ([host isEqualToString:kOnReadyEvent])
	{
		[self onReady];
	}
    else if ([host isEqualToString:kOnStateChangedEvent])
	{
		[self onStateChange:value];
	}
	else if ([host isEqualToString:kOnErrorEvent])
	{
		[self onError:value];
	}
}

#pragma mark - Override Button Action

- (void)onPressingCloseButton:(id)sender
{
	NSLog(@"[VideoEmbeddedView] pressed close");
	
	// Stop player
	[self stop];
}

#pragma mark - Event Handling

#define kPlayerStateEnded		@"ENDED"
#define kPlayerStatePlaying		@"PLAYING"
#define kPlayerStatePaused		@"PAUSED"
#define kPlayerStateBuffering	@"BUFFERING"
#define kPlayerStateCued		@"CUED"
#define kPlayerStateUnstarted	@"UNSTARTED"

- (void)onReady
{
	[self stringByEvaluatingJavaScriptFromString:@"playVideo()"];
}

- (void)onStateChange:(NSString *)state
{
	if ([state isEqualToString:kPlayerStateEnded])
	{
		// Notify
		[self notifyDidFinishPlaying:MPMovieFinishReasonPlaybackEnded];
	}
}

- (void)onError:(NSString *)errorType
{
	// Notify
	[self notifyDidFinishPlaying:MPMovieFinishReasonPlaybackError];
}

- (void)notifyDidFinishPlaying:(MPMovieFinishReason)reason
{
	// Trigger DidFinish event
	if ([[self delegate] conformsToProtocol:@protocol(WebViewEmbeddedVideoPlayerDelegate)])
		[[self delegate] webviewEmbeddedPlayer:self didFinishPlaying:reason];
}

@end