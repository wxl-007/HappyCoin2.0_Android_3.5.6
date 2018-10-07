//
//  AdMobBinding.m
//  AdMobTest
//
//  Created by Mike on 1/27/11.
//  Copyright 2011 Prime31 Studios. All rights reserved.
//

#import "AdMobManager.h"


// Converts C style string to NSString
#define GetStringParam( _x_ ) ( _x_ != NULL ) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]


// Sets the publiser Id and prepares AdMob for action.  Must be called before any other methods!
void _adMobInit( const char * publisherId, BOOL isTesting )
{
	[AdMobManager sharedManager].publisherId = GetStringParam( publisherId );
	if( isTesting )
		[AdMobManager sharedManager].isTesting = isTesting;
}


// Creates a banner of the given type with the x and y offset of the top-left corner of the ad
void _adMobCreateBanner( int bannerType, float xPos, float yPos )
{
	AdMobBannerType type = bannerType;
	
	[[AdMobManager sharedManager] createBanner:type withOrigin:CGPointMake( xPos, yPos )];
}


// Destroys the banner and removes it from view
void _adMobDestroyBanner()
{
	[[AdMobManager sharedManager] destroyBanner];
}


// Starts loading an interstitial ad
void _adMobRequestInterstitalAd( const char * interstitialUnitId )
{
	[[AdMobManager sharedManager] requestInterstitalAd:GetStringParam( interstitialUnitId )];
}


// Checks to see if the interstitial ad is loaded and ready to show
bool _adMobIsInterstitialAdReady()
{
	return [AdMobManager sharedManager].interstitialAd.isReady;
}


// If an interstitial ad is loaded this will take over the screen and show the ad
void _adMobShowInterstitialAd()
{
	[[AdMobManager sharedManager] showInterstitialAd];
}


// Reports an app download to AdMob.  Will only send the request if the app download has not yet been reported
void _adMobRegisterAppDownloadWithiTunesAppId( const char * iTunesAppId )
{
	[[AdMobManager sharedManager] registerAppDownloadWithiTunesAppId:GetStringParam( iTunesAppId )];
}


// Reports an app download to AdMob.  Will only send the request if the app download has not yet been reported
void _adMobRegisterAppDownloadWithAdMobSiteId()
{
	[[AdMobManager sharedManager] registerAppDownloadWithAdMobSiteId];
}



