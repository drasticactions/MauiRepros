//
//  WebViewFactory.m
//  WebKitTools
//
//  Created by ミラー・ティモシー on 2023/04/06.
//

#import <Foundation/Foundation.h>
#import <WebKit/WebKit.h>
#import "WebKitTools.h"

@implementation WebViewFactory

+ (WKWebView *)createDebugWebViewWithFrame:(CGRect)webviewFrame configuration:(WKWebViewConfiguration *)config {
    [config.preferences setValue:@(YES) forKey:@"developerExtrasEnabled"];
    WKWebView *webView = [[WKWebView alloc] initWithFrame:webviewFrame configuration:config];
    return webView;
}

@end
