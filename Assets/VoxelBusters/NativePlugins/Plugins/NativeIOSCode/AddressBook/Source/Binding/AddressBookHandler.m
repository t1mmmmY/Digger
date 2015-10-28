//
//  AddressBookHandler.m
//  Cross Platform Native Plugins
//
//  Created by Ashwin kumar on 10/12/14.
//  Copyright (c) 2015 Voxel Busters Interactive LLP. All rights reserved.
//

#import "AddressBookHandler.h"
#import "NSData+UIImage.h"

@implementation AddressBookHandler

#define kLastName							@"last-name"
#define kImagePath							@"image-path"
#define kFirstName							@"first-name"
#define kPhoneNumList						@"phone-number-list"
#define kEmailIDList						@"emailID-list"

#define	kContactList						@"contacts-list"
#define	kAuthStatus							@"auth-status"
#define kError								@"error"

#define kABReadContactsFinished				"ABReadContactsFinished"
#define kABRequestAccessFinished	 		"ABRequestAccessFinished"

#pragma mark - Auth Methods

- (ABAuthorizationStatus)getAuthorizationStatus
{
	return ABAddressBookGetAuthorizationStatus();
}

- (void)requestAccess
{
	if (ABAddressBookRequestAccessWithCompletion != NULL)
	{
		ABAuthorizationStatus authorisationStatus	= ABAddressBookGetAuthorizationStatus();
		
		// Request access permission from user
		if (authorisationStatus == kABAuthorizationStatusNotDetermined)
		{
			CFErrorRef *error               = nil;
			ABAddressBookRef addressBook    = ABAddressBookCreateWithOptions(nil, error);

			ABAddressBookRequestAccessWithCompletion(addressBook, ^(bool granted, CFErrorRef error) {
				
				// Invoke handler
				[self onRequesAccessFinished:[self getAuthorizationStatus] error:(NSError *)error];
			});
			
			CFRelease(addressBook);
		}
		else
		{
			// Invoke handler
			[self onRequesAccessFinished:authorisationStatus error:nil];
		}
	}
	else
	{
		// Invoke handler
		[self onRequesAccessFinished:kABAuthorizationStatusAuthorized error:nil];
	}
}

- (void)onRequesAccessFinished:(ABAuthorizationStatus)authorisationStatus error:(NSError *)error
{
	// Notify Unity
	NSMutableDictionary *dataDict	= [NSMutableDictionary dictionary];
	
	[dataDict setObject:[NSNumber numberWithLong:authorisationStatus] forKey:kAuthStatus];
	
	if (error)
	{
		[dataDict setObject:[error description] forKey:kError];
	}
	
	NotifyEventListener(kABRequestAccessFinished, ToJsonCString(dataDict));
}

#pragma mark - Read Contacts Methods

- (void)readContacts
{
    CFErrorRef *error               = nil;
    ABAddressBookRef addressBook    = ABAddressBookCreateWithOptions(nil, error);
	CFArrayRef allPeople            = ABAddressBookCopyArrayOfAllPeopleInSourceWithSortOrdering(addressBook, nil, kABPersonFirstNameProperty);
	CFIndex totalContacts           = CFArrayGetCount(allPeople);
	NSMutableArray *contactsList    = [NSMutableArray arrayWithCapacity:totalContacts];
	
	for (int iter = 0; iter < totalContacts; iter++)
	{
		ABRecordRef person                      = CFArrayGetValueAtIndex(allPeople, iter);
		NSMutableDictionary *eachContactData    = [NSMutableDictionary dictionary];

		// Get name info
		NSString *firstName 			= (NSString *)CFBridgingRelease(ABRecordCopyValue(person, kABPersonFirstNameProperty));
		NSString *lastName  			= (NSString *)CFBridgingRelease(ABRecordCopyValue(person, kABPersonLastNameProperty));

		if (firstName)
		{
			eachContactData[kFirstName]	= firstName;
		}
		
		if (lastName)
		{
			eachContactData[kLastName]	= lastName;
		}
		
		// Get image
		NSData *imageData				= (NSData *)CFBridgingRelease(ABPersonCopyImageData(person));
		
		if (imageData)
		{
			// Add image path to the contact info dictionary
			eachContactData[kImagePath]	= [imageData saveImage];
		}
		
		// Get phone numbers
		ABMultiValueRef phoneNumbersRef = CFBridgingRelease(ABRecordCopyValue(person, kABPersonPhoneProperty));
		CFIndex phoneNumberCount        = ABMultiValueGetCount(phoneNumbersRef);
		NSMutableArray *phoneNumberList	= [NSMutableArray array];

		for (CFIndex pIter = 0; pIter < phoneNumberCount; pIter++)
		{
			NSString *curNumber			= (NSString *)CFBridgingRelease(ABMultiValueCopyValueAtIndex(phoneNumbersRef, pIter));
			
			if (curNumber)
			{
				NSString *formattedNo	= [curNumber stringByReplacingOccurrencesOfString:@"[^0-9]"
																			 withString:@""
																				options:NSRegularExpressionSearch
																				  range:NSMakeRange(0, [curNumber length])];

				// Add phone no
				[phoneNumberList addObject:formattedNo];
			}
		}
		
		eachContactData[kPhoneNumList]  = phoneNumberList;
		
		// Get email address
		ABMultiValueRef emailIDsRef   	= CFBridgingRelease(ABRecordCopyValue(person, kABPersonEmailProperty));
		CFIndex emaildIdCount			= ABMultiValueGetCount(emailIDsRef);
		NSMutableArray *emailIDList		= [NSMutableArray array];

		for (CFIndex i = 0; i < emaildIdCount; i++)
		{
			NSString *curEmail     	 	= (NSString *)CFBridgingRelease(ABMultiValueCopyValueAtIndex(emailIDsRef, i));
			
			if (curEmail)
			{
				// Add email id
				[emailIDList addObject:curEmail];
			}
		}
		
		eachContactData[kEmailIDList]	= emailIDList;
		
		// Add contact info to the list
		[contactsList addObject:eachContactData];
	}
	
	// Invoke handler
	[self onReadContactsFinished:contactsList error:nil];
	
	// Release
	CFRelease(allPeople);
	CFRelease(addressBook);
}

- (void)onReadContactsFinished:(NSMutableArray *)contactList error:(NSError *)error
{
	// Notify unity
	NSMutableDictionary *dataDict	= [NSMutableDictionary dictionary];
	[dataDict setObject:[NSNumber numberWithLong:[self getAuthorizationStatus]] forKey:kAuthStatus];

	if (contactList)
	{
		[dataDict setObject:contactList forKey:kContactList];
	}
	
	if (error)
	{
		[dataDict setObject:[error description] forKey:kError];
	}
	
	NotifyEventListener(kABReadContactsFinished, ToJsonCString(dataDict));
}

@end