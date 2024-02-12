//
//  ContentView.swift
//  MacOSWebViewMediaTest
//
//  Created by DrasticActions on 2024/02/13.
//

import SwiftUI
import WebKit

struct ContentView: View {
    private let url = "https://www.apple.com"
        
    var body: some View {
            WebView(url: url)
     }
}

#Preview {
    ContentView()
}

struct WebView: NSViewRepresentable {
    let url: String
    
    func makeNSView(context: Context) -> WKWebView {
        let webConfiguration = WKWebViewConfiguration()
        webConfiguration.mediaTypesRequiringUserActionForPlayback = []
        let view = WKWebView()
        view.isInspectable = true
        return view
    }
    
    func updateNSView(_ uiView: WKWebView, context: Context) {
        guard let url = URL(string: url) else {
            return
        }
        
        let request = URLRequest(url: url)
        uiView.load(request)
    }
}

