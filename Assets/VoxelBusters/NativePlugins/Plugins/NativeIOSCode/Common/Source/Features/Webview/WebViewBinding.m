//
//  WebViewBinding.m
//  NativePluginIOSWorkspace
//
//  Created by Ashwin kumar on 11/01/15.
//  Copyright (c) 2015 Voxel Busters Interactive LLP. All rights reserved.
//

#import "WebViewBinding.h"
#import	"WebViewHandler.h"
#import "CustomWebView.h"

#pragma mark - Handling webviews

void webviewCreate (const char* tag, WebViewFrame frame)
{
    [[WebViewHandler Instance] createWebViewWithFrame:CGRectMake(frame.x, frame.y, frame.width, frame.height)
                                               andTag:ConvertToNSString(tag)];
}

void webviewDestroy (const char* tag)
{
    [[WebViewHandler Instance] destroyWebViewWithTag:ConvertToNSString(tag)];
}

void webviewShow (const char* tag)
{
    [[WebViewHandler Instance] showWebViewWithTag:ConvertToNSString(tag)];
}

void webviewHide (const char* tag)
{
    [[WebViewHandler Instance] dismissWebViewWithTag:ConvertToNSString(tag)];
}

#pragma mark - Loading

void webviewLoadRequest (const char* URL, const char* tag)
{
    [[WebViewHandler Instance] loadRequest:ConvertToNSString(URL)
                       usingWebViewWithTag:ConvertToNSString(tag)];
}

void webviewLoadHTMLString (const char* HTMLString, const char* baseURL, const char* tag)
{
    [[WebViewHandler Instance] loadHTMLString:ConvertToNSString(HTMLString)
                                      baseURL:ConvertToNSString(baseURL)
                          usingWebViewWithTag:ConvertToNSString(tag)];
}

void webviewLoadData (UInt8* dataArray, 	int dataArrayLength,
					  const char* MIMEType, const char* textEncodingName,
					  const char* baseURL, 	const char* tag)
{
    [[WebViewHandler Instance] loadData:[Utility CreateNSDataFromByteArray:(void*)dataArray ofLength:dataArrayLength]
							   MIMEType:ConvertToNSString(MIMEType)
					   textEncodingName:ConvertToNSString(textEncodingName)
								baseURL:ConvertToNSString(baseURL)
					usingWebViewWithTag:ConvertToNSString(tag)];
}

void webviewEvaluateJavaScriptFromString (const char* javaScript, const char* tag)
{
    [[WebViewHandler Instance] evaluateJavaScriptFromString:ConvertToNSString(javaScript)
                                        usingWebViewWithTag:ConvertToNSString(tag)];
}

void webviewReload (const char* tag)
{
    [[WebViewHandler Instance] reloadWebViewWithTag:ConvertToNSString(tag)];
}

void webviewStopLoading (const char* tag)
{
    [[WebViewHandler Instance] stopLoadingWebViewWithTag:ConvertToNSString(tag)];
}

#pragma mark - Properties

void webviewSetCanHide (bool canHide, const char* tag)
{
    [[WebViewHandler Instance] setCanDismiss:canHide
						   forWebViewWithTag:ConvertToNSString(tag)];
}

void webviewSetCanBounce (bool canBounce, const char* tag)
{
    [[WebViewHandler Instance] setCanBounce:canBounce
						  forWebViewWithTag:ConvertToNSString(tag)];
}

void webviewSetControlType (int type, const char* tag)
{
	[[WebViewHandler Instance] setControlType:(WebviewControlType)type
							forWebViewWithTag:ConvertToNSString(tag)];
	
}

void webviewSetShowSpinnerOnLoad (bool showSpinner, const char* tag)
{
    [[WebViewHandler Instance] setShowSpinnerOnLoad:showSpinner
								  forWebViewWithTag:ConvertToNSString(tag)];
}
	 
void webviewSetAutoShowOnLoadFinish (bool autoShow, const char* tag)
{
    [[WebViewHandler Instance] setAutoShowOnLoadFinish:autoShow
									 forWebViewWithTag:ConvertToNSString(tag)];
}

void webviewSetScalesPageToFit (bool scaleToFit, const char* tag)
{
    [[WebViewHandler Instance] setScalesPageToFit:scaleToFit
                                forWebViewWithTag:ConvertToNSString(tag)];
}

void webviewSetFrame (WebViewFrame frame, const char* tag)
{
	float contentScale				= [[UIScreen mainScreen] scale];
	
	// Get webview frame considering scale factor
	CGRect frameInScreenSpace;
	frameInScreenSpace.origin.x		= frame.x / contentScale;
	frameInScreenSpace.origin.y		= frame.y / contentScale;
	frameInScreenSpace.size.width	= frame.width / contentScale;
	frameInScreenSpace.size.height	= frame.height / contentScale;
	
    [[WebViewHandler Instance] setFrame:frameInScreenSpace
                      forWebViewWithTag:ConvertToNSString(tag)];
	
}

void webviewSetBackgroundColor (float r, float g, float b, float alpha, const char* tag)
{
	[[WebViewHandler Instance] setBackgroundColor:[UIColor colorWithRed:r green:g blue:b alpha:alpha]
								forWebViewWithTag:ConvertToNSString(tag)];
}

#pragma mark - URL scheme

void webviewAddNewURLScheme (const char* _newURLScheme, const char* tag)
{
    [[WebViewHandler Instance] addNewURLScheme:ConvertToNSString(_newURLScheme)
                             forWebViewWithTag:ConvertToNSString(tag)];
}

#pragma mark - Cache

void webviewClearCache ()
{
    [[WebViewHandler Instance] clearCache];
}