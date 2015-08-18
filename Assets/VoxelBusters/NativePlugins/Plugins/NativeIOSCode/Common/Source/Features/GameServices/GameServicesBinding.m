//
//  GameServicesBinding.m
//  Unity-iPhone
//
//  Created by Ashwin kumar on 27/05/15.
//
//

#import "GameServicesBinding.h"
#import "GameServicesHandler.h"

bool isGameCenterAvailable ()
{
	return [[GameServicesHandler Instance] isGameCenterAvailable];
}