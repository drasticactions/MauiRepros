//
//  WebKitTools.h
//  WebKitTools
//
//  Created by ミラー・ティモシー on 2023/04/06.
//

#import <Foundation/Foundation.h>

//! Project version number for WebKitTools.
FOUNDATION_EXPORT double WebKitToolsVersionNumber;

//! Project version string for WebKitTools.
FOUNDATION_EXPORT const unsigned char WebKitToolsVersionString[];

// In this header, you should import all the public headers of your framework using statements like #import <WebKitTools/PublicHeader.h>

@interface WebViewFactory : NSObject

+ (WKWebView *)createDebugWebViewWithFrame:(CGRect)webviewFrame configuration:(WKWebViewConfiguration *)config;

@end

