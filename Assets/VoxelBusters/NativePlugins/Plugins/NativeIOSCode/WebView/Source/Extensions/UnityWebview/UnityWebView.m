//
//  UnityWebView.m
//  Cross Platform Native Plugins
//
//  Created by Ashwin kumar on 21/01/15.
//  Copyright (c) 2015 Voxel Busters Interactive LLP. All rights reserved.
//

#import "UnityWebView.h"

#define kShowing  						"WebViewDidShow"
#define kDismissed						"WebViewDidHide"
#define kDestroyed						"WebViewDidDestroy"
#define kDidStartLoad 					"WebViewDidStartLoad"
#define kDidFinishLoad 					"WebViewDidFinishLoad"
#define kDidFailLoadWithError			"WebViewDidFailLoadWithError"
#define kFinishedEvaluatingJavaScript	"WebViewDidFinishEvaluatingJS"
#define kReceivedMessage 				"WebViewDidReceiveMessage"

@implementation UnityWebView

- (id)initWithFrame:(CGRect)frame tag:(NSString *)tag
{
	self = [super initWithFrame:frame tag:tag];
	
    if (self)
	{
		// Initialisation code
		[[self webview] setOpaque:NO];
    }
	
    return self;
}

- (void)dealloc
{	
	// Notify unity
	NotifyEventListener(kDestroyed, [[self webviewTag] UTF8String]);
	
	[super dealloc];
}

#pragma mark - Override View Methods

- (void)show
{
    [super show];

	// Show
	[UnityGetGLViewController().view addSubview:self];
    
    // Notify unity
    NotifyEventListener(kShowing, [[self webviewTag] UTF8String]);
}

- (void)dismiss
{
	[super dismiss];
	
    // Notify unity
	NotifyEventListener(kDismissed, [[self webviewTag] UTF8String]);
}

#pragma mark - Override Load Methods

- (NSString *)stringByEvaluatingJavaScriptFromString:(NSString *)script
{
    return [self stringByEvaluatingJavaScriptFromString:script notifyUnity:YES];
}

- (NSString *)stringByEvaluatingJavaScriptFromString:(NSString *)script notifyUnity:(BOOL)needsToNotify
{
    NSString *result	= [super stringByEvaluatingJavaScriptFromString:script];
	
	// Notify unity
	if (needsToNotify)
	{
		NSMutableDictionary *data   = [NSMutableDictionary dictionary];
		data[@"tag"]                = [self webviewTag];
		
		if (result != NULL)
			data[@"result"]			= result;
	
		NotifyEventListener(kFinishedEvaluatingJavaScript, ToJsonCString(data));
	}
	
	return result;
}

#pragma mark - Overide URL Scheme

- (void)foundMatchingURLScheme:(NSURL *)requestURL
{
	NSMutableDictionary *messageData	= [self parseURLScheme:requestURL];
	
	// Notify unity
	NSMutableDictionary *data  		= [NSMutableDictionary dictionary];
	data[@"tag"]              		= [self webviewTag];
	data[@"message-data"]       	= messageData;
	
	// Notify unity
    NotifyEventListener(kReceivedMessage, ToJsonCString(data));
}

#pragma mark - Override Webview Callback

- (void)webViewDidStartLoad:(UIWebView *)webView
{
	// Notify unity
	NotifyEventListener(kDidStartLoad, [[self webviewTag] UTF8String]);
	
	// Base class action
	[super webViewDidStartLoad:webView];
}

- (void)webViewDidFinishLoad:(UIWebView *)webView
{
    // Notify unity
	NotifyEventListener(kDidFinishLoad, [[self webviewTag] UTF8String]);
	
	// Base class action
	[super webViewDidFinishLoad:webView];
}

- (void)webView:(UIWebView *)webView didFailLoadWithError:(NSError *)error
{
	[super webView:webView didFailLoadWithError:error];
    
    // Notify unity
    NSMutableDictionary* data   = [NSMutableDictionary dictionary];
    data[@"tag"]                = [self webviewTag];
    data[@"error"]              = error.description;
    
    NotifyEventListener(kDidFailLoadWithError, ToJsonCString(data));
}

@end