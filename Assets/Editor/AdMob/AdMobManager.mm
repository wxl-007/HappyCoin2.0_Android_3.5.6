//
//  AdMobManager.mm
//  AdMobTest
//
//  Created by Mike on 1/27/11.
//  Copyright 2011 Prime31 Studios. All rights reserved.
//

#import "AdMobManager.h"
#import <CommonCrypto/CommonDigest.h>


void UnityPause( bool pause );

UIViewController *UnityGetGLViewController();


@implementation AdMobManager

@synthesize adView = _adView, interstitialAd = _interstitialAd, controller = _controller, publisherId = _publisherId, isTesting;

///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark NSObject

+ (AdMobManager*)sharedManager
{
	static AdMobManager *sharedSingleton;
	
	if( !sharedSingleton )
		sharedSingleton = [[AdMobManager alloc] init];
	
	return sharedSingleton;
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Private

// This method requires adding #import <CommonCrypto/CommonDigest.h> to your source file.
- (NSString*)hashedISU
{
	NSString *result = nil;
	NSString *isu = [UIDevice currentDevice].uniqueIdentifier;
	
	if( isu )
	{
		unsigned char digest[16];
		NSData *data = [isu dataUsingEncoding:NSASCIIStringEncoding];
		CC_MD5( data.bytes, data.length, digest );
		
		result = [NSString stringWithFormat: @"%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x%02x",
				  digest[0], digest[1], 
				  digest[2], digest[3],
				  digest[4], digest[5],
				  digest[6], digest[7],
				  digest[8], digest[9],
				  digest[10], digest[11],
				  digest[12], digest[13],
				  digest[14], digest[15]];
		result = [result uppercaseString];
	}
	return result;
}


- (void)reportAppOpenWithAppId:(NSString*)iTunesAppId
{
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
	
	// Have we already reported an app open?
	NSString *adMobKey = @"admobAppOpen";
	
	if( ![[NSUserDefaults standardUserDefaults] boolForKey:adMobKey] )
	{
		// Not yet reported -- report now
		NSString *appOpenEndpoint = [NSString stringWithFormat:@"http://a.admob.com/f0?isu=%@&md5=1&app_id=%@",
									 [self hashedISU], iTunesAppId];
		NSURLRequest *request = [NSURLRequest requestWithURL:[NSURL URLWithString:appOpenEndpoint]];
		NSURLResponse *response;
		NSError *error = nil;
		NSData *responseData = [NSURLConnection sendSynchronousRequest:request returningResponse:&response error:&error];
		
		if( !error && [(NSHTTPURLResponse*)response statusCode] == 200 && responseData.length > 0 )
		{
			[[NSUserDefaults standardUserDefaults] setBool:YES forKey:adMobKey];
			NSLog( @"successfully reported app download" );
		}
	}
	
	[pool release];
}


- (void)reportAppOpenWithAdMobSiteId
{
	NSAutoreleasePool *pool = [[NSAutoreleasePool alloc] init];
	
	// Have we already reported an app open?
	NSString *adMobKey = @"admobAppOpenWithSiteId";
	
	if( ![[NSUserDefaults standardUserDefaults] boolForKey:adMobKey] )
	{
		// Not yet reported -- report now
		NSString *appOpenEndpoint = [NSString stringWithFormat:@"http://a.admob.com/f0?isu=%@&md5=1&site_id=%@",
		                                 [self hashedISU], _publisherId];
		NSURLRequest *request = [NSURLRequest requestWithURL:[NSURL URLWithString:appOpenEndpoint]];
		NSURLResponse *response;
		NSError *error = nil;
		NSData *responseData = [NSURLConnection sendSynchronousRequest:request returningResponse:&response error:&error];
		
		if( !error && [(NSHTTPURLResponse*)response statusCode] == 200 && responseData.length > 0 )
		{
			[[NSUserDefaults standardUserDefaults] setBool:YES forKey:adMobKey];
			NSLog( @"successfully reported app download using ad mob site ID" );
		}
	}
	
	[pool release];
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark GADBannerViewDelegate

- (void)adViewDidReceiveAd:(GADBannerView*)bannerView
{
	_controller.view.hidden = NO;

	UnitySendMessage( "AdMobManager", "adViewDidReceiveAd", "" );
}


- (void)adView:(GADBannerView*)bannerView didFailToReceiveAdWithError:(GADRequestError*)error
{
	_controller.view.hidden = YES;
	
	UnitySendMessage( "AdMobManager", "adViewFailedToReceiveAd", [error localizedDescription].UTF8String );
}


- (void)adViewWillPresentScreen:(GADBannerView*)bannerView
{
	_controller.view.hidden = YES;
	UnityPause( true );
}


- (void)adViewDidDismissScreen:(GADBannerView*)bannerView
{
	UnityPause( false );
}


- (void)adViewWillLeaveApplication:(GADBannerView*)bannerView
{
	NSLog( @"adViewWillLeaveApplication" );
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark GADInterstitialDelegate

- (void)interstitialDidReceiveAd:(GADInterstitial*)interstitial
{
	UnitySendMessage( "AdMobManager", "interstitialDidReceiveAd", "" );
}


// Sent when an interstitial ad request completed without an interstitial to show.  This is
// common since interstitials are shown sparingly to users.
- (void)interstitial:(GADInterstitial*)interstitial didFailToReceiveAdWithError:(GADRequestError*)error
{
	UnitySendMessage( "AdMobManager", "interstitialFailedToReceiveAd", [error localizedDescription].UTF8String );
	
	if( _interstitialAd )
	{
		_interstitialAd.delegate = nil;
		self.interstitialAd = nil;
	}
}


- (void)interstitialWillPresentScreen:(GADInterstitial*)interstitial
{
	UnityPause( true );
}


- (void)interstitialWillDismissScreen:(GADInterstitial*)interstitial
{
	UnityPause( false );
}


- (void)interstitialDidDismissScreen:(GADInterstitial*)interstitial
{
	// clean up the interstitial.  It can only be used once.
	_interstitialAd.delegate = nil;
	self.interstitialAd = nil;
}


- (void)interstitialWillLeaveApplication:(GADInterstitial*)interstitial
{
	NSLog( @"interstitialWillLeaveApplication" );
}


///////////////////////////////////////////////////////////////////////////////////////////////////
#pragma mark Public

- (void)createBanner:(AdMobBannerType)bannerType withOrigin:(CGPoint)originPoint
{
	// kill the current adView if we have one
	if( _adView )
		[self destroyBanner];

	_originPoint = originPoint;
	
	switch( bannerType )
	{
		case AdMobBannerTypeiPhone_320x50:
		{
			_adView = [[GADBannerView alloc] initWithAdSize:kGADAdSizeBanner];
			break;
		}
		case AdMobBannerTypeiPad_320x250:
		{
			_adView = [[GADBannerView alloc] initWithAdSize:kGADAdSizeMediumRectangle];
			break;
		}
		case AdMobBannerTypeiPad_468x60:
		{
			_adView = [[GADBannerView alloc] initWithAdSize:kGADAdSizeFullBanner];
			break;
		}
		case AdMobBannerTypeiPad_728x90:
		{
			_adView = [[GADBannerView alloc] initWithAdSize:kGADAdSizeLeaderboard];
			break;
		}
		case AdMobBannerTypeSmartPortrait:
		{
			_adView = [[GADBannerView alloc] initWithAdSize:kGADAdSizeSmartBannerPortrait];
			break;
		}
		case AdMobBannerTypeSmartLandscape:
		{
			_adView = [[GADBannerView alloc] initWithAdSize:kGADAdSizeSmartBannerLandscape];
		}
	}
	
	// finish setting up the banner
	_adView.adUnitID = _publisherId;
	_adView.delegate = self;
	_adView.rootViewController = UnityGetGLViewController();
	
	// setup the request
	GADRequest *request = [GADRequest request];
	request.testing = isTesting;
	[_adView loadRequest:request];
	
	CGRect frame = _adView.frame;
	frame.origin = originPoint;
	_adView.frame = frame;
	[UnityGetGLViewController().view addSubview:_adView];
}


- (void)destroyBanner
{
	_controller.view.hidden = YES;
	
	[_adView removeFromSuperview];
	_adView.delegate = nil;
	[_adView release];
	_adView = nil;
}


- (void)requestInterstitalAd:(NSString*)interstitialUnitId
{
	// only kill the ad if it is already loaded
	if( _interstitialAd && _interstitialAd.isReady )
	{
		_interstitialAd.delegate = nil;
		self.interstitialAd = nil;
	}
	
	// this will return nil if there is already a load in progress
	_interstitialAd = [[GADInterstitial alloc] init];
	_interstitialAd.adUnitID = interstitialUnitId;
	_interstitialAd.delegate = self;
	
	GADRequest *request = [GADRequest request];
	request.testing = isTesting;
	[_interstitialAd loadRequest:request];
}


- (void)showInterstitialAd
{
	if( !_interstitialAd.isReady )
	{
		NSLog( @"interstitial ad is not yet loaded" );
		return;
	}

	[_interstitialAd presentFromRootViewController:UnityGetGLViewController()];
}


- (void)registerAppDownloadWithiTunesAppId:(NSString*)appId
{
	[self performSelectorInBackground:@selector(reportAppOpenWithAppId:) withObject:appId];
}


- (void)registerAppDownloadWithAdMobSiteId
{
	if( !_publisherId )
		return;

	[self performSelectorInBackground:@selector(reportAppOpenWithAdMobSiteId) withObject:nil];
}


@end
