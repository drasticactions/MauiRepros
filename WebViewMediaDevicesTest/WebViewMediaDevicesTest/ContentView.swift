//
//  ContentView.swift
//  WebViewMediaDevicesTest
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

struct WebView: UIViewRepresentable {
    let url: String
    
    func makeUIView(context: Context) -> WKWebView {
        let view = WKWebView()
        view.isInspectable = true
        return view
    }
    
    func updateUIView(_ uiView: WKWebView, context: Context) {
        guard let url = URL(string: url) else {
            return
        }
        
        let request = URLRequest(url: url)
        uiView.load(request)
    }
}
