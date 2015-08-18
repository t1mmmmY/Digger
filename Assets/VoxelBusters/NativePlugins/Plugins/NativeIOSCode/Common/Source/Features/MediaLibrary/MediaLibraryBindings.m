//
//  MediaLibraryBindings.m
//  NativePluginIOSWorkspace
//
//  Created by Ashwin kumar on 10/01/15.
//  Copyright (c) 2015 Voxel Busters Interactive LLP. All rights reserved.
//

#import "MediaLibraryBindings.h"
#import "MediaLibraryHandler.h"

bool isCameraSupported ()
{
    return [[MediaLibraryHandler Instance] isCameraSupported];
}

void pickImage (int source, float scaleFactor)
{
    [[MediaLibraryHandler Instance] pickImage:(eImageSource)source
								  scaleDownTo:scaleFactor];
}

void saveImageToGallery (UInt8* imgByteArray, int imgByteArrayLength)
{
    [[MediaLibraryHandler Instance] saveImageToGallery:[Utility CreateImageFromByteArray:imgByteArray
																				ofLength:imgByteArrayLength]];
}

void playVideoUsingWebView (const char* embedHTML)
{
	[[MediaLibraryHandler Instance] playVideoUsingWebView:ConvertToNSString(embedHTML)];
}

void playVideoFromURL (const char* URLString)
{
	[[MediaLibraryHandler Instance] playVideoFromURL:ConvertToNSString(URLString) fullscreen:YES];
}

void playVideoFromGallery ()
{
	[[MediaLibraryHandler Instance] playVideoFromGallery:YES];
}