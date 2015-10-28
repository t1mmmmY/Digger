//
//  CustomActivityViewController.m
//  Cross Platform Native Plugins
//
//  Created by Ashwin kumar on 05/10/15.
//  Copyright (c) 2015 Voxel Busters Interactive LLP. All rights reserved.
//

#import "CustomActivityViewController.h"

@implementation CustomActivityViewController

#pragma mark - Methods

- (NSUInteger)supportedInterfaceOrientations
{
	return [UnityGetGLViewController() supportedInterfaceOrientations];
}

@end
