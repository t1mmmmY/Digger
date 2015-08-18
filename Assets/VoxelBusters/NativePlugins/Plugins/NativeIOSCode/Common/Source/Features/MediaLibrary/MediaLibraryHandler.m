//
//  MediaLibraryHandler.m
//  NativePluginIOSWorkspace
//
//  Created by Ashwin kumar on 10/01/15.
//  Copyright (c) 2015 Voxel Busters Interactive LLP. All rights reserved.
//

#import "MediaLibraryHandler.h"
#import "UIImage+Save.h"
#import "UIHandler.h"
#import "NSURL+Extensions.h"

@implementation MediaLibraryHandler

#define kPickImageFinished 				"PickImageFinished"
#define kSaveImageToGalleryFinished 	"SaveImageToGalleryFinished"
#define kPickVideoFinished 				"PickVideoFinished"
#define kPlayVideoFinished	 			"PlayVideoFinished"

@synthesize scaleFactor;
@synthesize allowsFullScreenVideoPlayback;
@synthesize popoverController;
@synthesize moviePlayerVC;

- (id)init
{
	self	= [super init];
	
	if (self)
	{
		[[AVAudioSession sharedInstance] setCategory:AVAudioSessionCategoryPlayback error:NULL];
		
		// Add observer to get notified when application enters foreground
		[[NSNotificationCenter defaultCenter] addObserver:self
												 selector:@selector(willEnterForegroundNotification:)
													 name:UIApplicationWillEnterForegroundNotification
												   object:nil];
	}
	
	return self;
}

- (void)dealloc
{
	// Deregister from notification
	[[NSNotificationCenter defaultCenter] removeObserver:self];
	
	 // Release
	 self.popoverController = NULL;
	 self.moviePlayerVC		= NULL;
	 
	 [super dealloc];
}

#pragma mark - Image

- (BOOL)isCameraSupported
{
	bool isSupported = [UIImagePickerController isSourceTypeAvailable:UIImagePickerControllerSourceTypeCamera];
	NSLog(@"[MediaLibraryHandler] isCameraSupported: %d", isSupported);
	
	return isSupported;
}

#define kChooseExistingPhoto	@"Choose existing photo"
#define kCapturePhoto			@"Capture photo"

- (void)pickImage:(eImageSource)source scaleDownTo:(float)factor
{
	UIImagePickerControllerSourceType albumSource	= UIImagePickerControllerSourceTypePhotoLibrary;
	NSArray *imageType								= @[(NSString *)kUTTypeImage,
														(NSString *)kUTTypeJPEG,
														(NSString *)kUTTypeJPEG2000,
														(NSString *)kUTTypePNG];
	
    // Cache scale factor
    [self setScaleFactor:factor];
    
    // Camera is not supported
    if (![self isCameraSupported])
    {
        [self presentImagePickerController:albumSource
				   withSupportedMediaTypes:imageType];
		
        return;
    }
    else
    {
        if (source == ALBUM)
        {
            [self presentImagePickerController:albumSource
					   withSupportedMediaTypes:imageType];
			
            return;
        }
        else if (source == CAMERA)
        {
            [self presentImagePickerController:UIImagePickerControllerSourceTypeCamera
					   withSupportedMediaTypes:imageType];
			
            return;
        }
        else
        {
            UIActionSheet *actionsheet  = [[[UIActionSheet alloc] initWithTitle:NULL
                                                                       delegate:self
                                                              cancelButtonTitle:NULL
                                                         destructiveButtonTitle:NULL
                                                              otherButtonTitles:kChooseExistingPhoto, kCapturePhoto, nil] autorelease];
            
            // Present it
			[actionsheet showFromRect:[self getPopOverRect]
							   inView:UnityGetGLView()
							 animated:YES];
        }
    }
}

- (void)presentImagePickerController:(UIImagePickerControllerSourceType)pickerSource withSupportedMediaTypes:(NSArray *)mediaTypes
{
	NSLog(@"[MediaLibraryHandler] image source: %d", pickerSource);
	
    UIImagePickerController *imagePickerController  = [[[UIImagePickerController alloc] init] autorelease];
    imagePickerController.delegate                  = self;
    imagePickerController.allowsEditing             = [self isVideoType:mediaTypes] ? NO : YES;
    imagePickerController.sourceType                = pickerSource;
    imagePickerController.mediaTypes                = mediaTypes;
    
    if (IsIpadInterface() && pickerSource != UIImagePickerControllerSourceTypeCamera)
    {
        self.popoverController	= [[[UIPopoverController alloc] initWithContentViewController:imagePickerController] autorelease];

		// Set delegate
		[self.popoverController setDelegate:self];
		
        // Present it
        [self.popoverController presentPopoverFromRect:[self getPopOverRect]
                                                inView:UnityGetGLView()
                              permittedArrowDirections:UIPopoverArrowDirectionAny
                                              animated:YES];
    }
    else
    {
        [UnityGetGLViewController() presentViewController:imagePickerController
                                                 animated:YES
                                               completion:nil];
    }
}

#define kSelectedImagePath			@"image-path"
#define kPickImageFinishReason		@"finish-reason"

- (void)onDidFinishPickingImage:(UIImagePickerController *)picker withInfo:(NSDictionary *)info
{
	NSLog(@"[MediaLibraryHandler] finished picking image");
	
	// Fetch image
    UIImage *selectedImg = [info objectForKey:UIImagePickerControllerEditedImage];
    
    // If edited image is null then use original
    if (selectedImg == NULL)
        selectedImg	= [info objectForKey:UIImagePickerControllerOriginalImage];
    
    NSString *savedPath  				= [selectedImg saveImageToDocumentsDirectory:self.scaleFactor];
	ePickImageFinishReason finishReason	= ePickImageFinishReasonSelected;
	
	// Check image saved path, if its null/empty
	if (IsNullOrEmpty(savedPath))
	{
		finishReason	= ePickImageFinishReasonFailed;
	}
	
    // Notify unity
	NSMutableDictionary *resultDict		= [NSMutableDictionary dictionary];
	resultDict[kPickImageFinishReason]	= [NSNumber numberWithInt:finishReason];
	resultDict[kSelectedImagePath]		= savedPath;
	
    NotifyEventListener(kPickImageFinished, ToJsonCString(resultDict));
}

- (void)onDidCancelPickImage
{
	NSLog(@"[MediaLibraryHandler] cancelled picking image");
	
	// Notify unity
	NSMutableDictionary *resultDict		= [NSMutableDictionary dictionary];
	resultDict[kPickImageFinishReason]	= [NSNumber numberWithInt:ePickImageFinishReasonCancelled];
	resultDict[kSelectedImagePath]		= kNSStringDefault;
	
	NotifyEventListener(kPickImageFinished, ToJsonCString(resultDict));
}

- (void)saveImageToGallery:(UIImage *)image
{
	NSLog(@"[MediaLibraryHandler] saving image to gallery");
	
	// Save to album
    UIImageWriteToSavedPhotosAlbum(image,
                                   self,
                                   @selector(image:didFinishSavingWithError:contextInfo:),
                                   nil);
}

- (void)               image:(UIImage *)image
    didFinishSavingWithError:(NSError *)error
                 contextInfo:(void *)contextInfo
{
    NSString *status = NULL;
    
    if (error != NULL)
        status  = @"false";
    else
        status  = @"true";
    
    // Notify Unity
    NotifyEventListener(kSaveImageToGalleryFinished, [status UTF8String]);
	NSLog(@"[MediaLibraryHandler] did finish saving, status: %@", status);
}

#pragma mark - Video

- (void)playVideoUsingWebView:(NSString *)embedHTML
{
	WebViewEmbeddedVideoPlayer *player	= [[WebViewEmbeddedVideoPlayer alloc] initWithFrame:GetUsableScreenSpace()];
	
	// Set delegate
	[player setDelegate:self];
	
	// Play video
	[player playVideo:embedHTML];
}

- (void)playVideoFromURL:(NSString *)URLString fullscreen:(BOOL)fullscreen
{
	// Get URl
	NSURL *moviePathURL	= [NSURL createURLWithString:URLString];

	// Play video
	[self playVideoAtURL:moviePathURL fullscreen:fullscreen];
}

- (void)playVideoFromGallery:(BOOL)fullscreen
{
	NSLog(@"[MediaLibraryHandler] pick video from gallery");
	
	// Cache video playback mode
	[self setAllowsFullScreenVideoPlayback:fullscreen];
	
	// Present controller for selecting video
	[self presentImagePickerController:UIImagePickerControllerSourceTypePhotoLibrary
			   withSupportedMediaTypes:@[(NSString *)kUTTypeMovie]];
}

- (void)onDidFinishPickingVideo:(UIImagePickerController *)picker withInfo:(NSDictionary *)info
{
	NSLog(@"[MediaLibraryHandler] finished picking video");
	NSURL *moviePathURL = [info objectForKey:UIImagePickerControllerMediaURL];
	
	// Notify unity
	NSString *pickVideoFinishReason	= [NSString stringWithFormat:@"%d", ePickVideoFinishReasonSelected];
    NotifyEventListener(kPickVideoFinished, [pickVideoFinishReason UTF8String]);
	
	// Play video
	[self playVideoAtURL:moviePathURL fullscreen:self.allowsFullScreenVideoPlayback];
}

- (void)onDidCancelPickVideo
{
	NSLog(@"[MediaLibraryHandler] cancelled picking video");
	
	// Notify unity
	NSString *pickVideoFinishReason	= [NSString stringWithFormat:@"%d", ePickVideoFinishReasonCancelled];
    NotifyEventListener(kPickVideoFinished, [pickVideoFinishReason UTF8String]);
}

- (BOOL)isVideoType:(NSArray *)mediaTypes
{
	NSString *mediaType = [mediaTypes objectAtIndex:0];
	
    return (CFStringCompare((CFStringRef) mediaType, kUTTypeMovie, 0) == kCFCompareEqualTo);
}

- (void)playVideoAtURL:(NSURL *)URL fullscreen:(BOOL)fullscreen
{
	NSLog(@"[MediaLibraryHandler] play video, URL=%@", [URL absoluteString]);
	
	if (self.moviePlayerVC != NULL)
	{
		// Stop
		[[self.moviePlayerVC moviePlayer] stop];
		
		// Remove
		[UnityGetGLViewController() dismissMoviePlayerViewControllerAnimated];
		self.moviePlayerVC = NULL;
	}
	
	self.moviePlayerVC 						= [[[MPMoviePlayerViewController alloc] initWithContentURL:URL] autorelease];
	MPMoviePlayerController *moviePlayer	= [moviePlayerVC moviePlayer];
	
	// Register for the movie player notification
	[[NSNotificationCenter defaultCenter] addObserver:self
											 selector:@selector(moviePlayerDidFinishReason:)
												 name:MPMoviePlayerPlaybackDidFinishNotification
											   object:nil];
	
	// Unregister to avoid stopping playback once in background
	[[NSNotificationCenter defaultCenter] removeObserver:self.moviePlayerVC
													name:UIApplicationDidEnterBackgroundNotification
												  object:nil];
		
	// Set properties
	[moviePlayer setControlStyle:MPMovieControlStyleFullscreen];
	[moviePlayer setScalingMode:MPMovieScalingModeAspectFit];
	[moviePlayer setFullscreen:fullscreen animated:NO];
	[moviePlayer setShouldAutoplay:YES];
	[moviePlayer setUseApplicationAudioSession:NO];
	[moviePlayer prepareToPlay];
	
	// Present it
	[UnityGetGLViewController() presentMoviePlayerViewControllerAnimated:moviePlayerVC];
}

#pragma mark - UIImagePickerControllerDelegate

- (void)imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary *)info
{
    NSLog(@"[MediaLibraryHandler] did finish picking");
	BOOL isVideoType	= [self isVideoType:[picker mediaTypes]];
	
	// Retain to own ImagePicker
	[picker retain];
	
	// Dismiss image picker controller
	[self dismissImagePicker:picker animated:!isVideoType];
	
    // Handle video type
    if (isVideoType)
		[self onDidFinishPickingVideo:picker withInfo:info];
	// Handle audio type
	else
		[self onDidFinishPickingImage:picker withInfo:info];
	
	// Release ownership
	[picker release];
}

- (void)imagePickerControllerDidCancel:(UIImagePickerController *)picker
{
    NSLog(@"[MediaLibraryHandler] did cancel");
	
	// Handle video type
	if ([self isVideoType:[picker mediaTypes]])
		[self onDidCancelPickVideo];
	// Handle audio type
	else
		[self onDidCancelPickImage];
	
    // Dismiss image picker controller
	if (picker)
		[self dismissImagePicker:picker animated:YES];
}

- (void)dismissImagePicker:(UIImagePickerController *)picker animated:(BOOL)animated
{
    NSLog(@"[MediaLibraryHandler] dismissed picker controller");
	
    // Dismiss popover controller
    if (self.popoverController)
    {
        [self.popoverController dismissPopoverAnimated:animated];
        self.popoverController = NULL;
    }
    else
    {
        [picker dismissViewControllerAnimated:animated completion:NULL];
    }
}

#pragma mark - UIPopoverControllerDelegate

- (void)popoverControllerDidDismissPopover:(UIPopoverController *)popover
{
	NSLog(@"[MediaLibraryHandler] DidDismissPopover");
	[self imagePickerControllerDidCancel:(UIImagePickerController *)[popover contentViewController]];
}

#pragma mark - UIActionSheetDelegate

- (void)actionSheet:(UIActionSheet *)actionSheet clickedButtonAtIndex:(NSInteger)buttonIndex;
{
	if (buttonIndex >= 0)
	{
		NSString *buttonPressed	= [actionSheet buttonTitleAtIndex:buttonIndex];
		
		if ([buttonPressed isEqualToString:kChooseExistingPhoto])
		{
			[self pickImage:ALBUM scaleDownTo:self.scaleFactor];
		}
		else if ([buttonPressed isEqualToString:kCapturePhoto])
		{
			[self pickImage:CAMERA scaleDownTo:self.scaleFactor];
		}
	}
	else
	{
		[self onDidCancelPickImage];
	}
}

#pragma mark - UIApplication Notification

- (void)willEnterForegroundNotification:(NSNotification *)notification
{
	// Resume video
	if (self.moviePlayerVC != NULL)
	{
		if ([[self.moviePlayerVC moviePlayer] playbackState] == MPMoviePlaybackStatePaused)
		{
			[[self.moviePlayerVC moviePlayer] play];
		}
	}
}

#pragma mark - MVMoviePlayer Callback

- (void)moviePlayerDidFinishReason:(NSNotification *)notification
{
    NSLog(@"[MediaLibraryHandler] did finish playing video");
    MPMovieFinishReason reason	= [[[notification userInfo] valueForKey:MPMoviePlayerPlaybackDidFinishReasonUserInfoKey] intValue];
	
	// Notify unity
	NSString *reasonStr	= [NSString stringWithFormat:@"%d", reason];
	NotifyEventListener(kPlayVideoFinished, [reasonStr UTF8String]);
	
	// Remove as observer
    [[NSNotificationCenter defaultCenter] removeObserver:self
													name:MPMoviePlayerPlaybackDidFinishNotification
												  object:nil];
	
	// Release
	self.moviePlayerVC = NULL;
}

#pragma mark - WebViewEmbeddedPlayerDelegate

- (void)webviewEmbeddedPlayer:(WebViewEmbeddedVideoPlayer *)player didFinishPlaying:(MPMovieFinishReason)reason
{
	// Notify unity
	NSString *reasonStr	= [NSString stringWithFormat:@"%d", reason];
	NotifyEventListener(kPlayVideoFinished, [reasonStr UTF8String]);
	
	// Release
	[player dismiss];
	[player release];
}

#pragma mark - Misc

- (CGRect)getPopOverRect
{
	CGRect popoverRect;
	popoverRect.origin		= [[UIHandler Instance] popoverPoint];
	popoverRect.size		= CGSizeMake(1, 1);
	
	return popoverRect;
}

@end