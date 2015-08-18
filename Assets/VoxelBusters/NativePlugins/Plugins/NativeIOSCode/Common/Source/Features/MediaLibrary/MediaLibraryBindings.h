//
//  MediaLibraryBindings.h
//  NativePluginIOSWorkspace
//
//  Created by Ashwin kumar on 10/01/15.
//  Copyright (c) 2015 Voxel Busters Interactive LLP. All rights reserved.
//

#import <Foundation/Foundation.h>

UIKIT_EXTERN bool isCameraSupported ();
UIKIT_EXTERN void pickImage (int accessPhotoInt, float scaleFactor);
UIKIT_EXTERN void saveImageToGallery (UInt8* imgByteArray, int imgByteArrayLength);
UIKIT_EXTERN void playVideoUsingWebView (const char* embedHTML);
UIKIT_EXTERN void playVideoFromURL (const char* URLString);
UIKIT_EXTERN void playVideoFromGallery ();