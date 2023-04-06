//
//  ViewController.swift
//  WebKitToolsTest
//
//  Created by ミラー・ティモシー on 2023/04/06.
//

import UIKit
import WebKit

class ViewController: UIViewController {

    override func viewDidLoad() {
        super.viewDidLoad()
        let configuration = WKWebViewConfiguration()
        #if DEBUG
        configuration.preferences.setValue(true, forKey: "developerExtrasEnabled")
        #endif
        let webView = WKWebView(frame: self.view.bounds, configuration: configuration)
        
        webView.translatesAutoresizingMaskIntoConstraints = false // disable autoresizing mask
                self.view.addSubview(webView)
                
                // Set up constraints to fill the view
                NSLayoutConstraint.activate([
                    webView.topAnchor.constraint(equalTo: view.topAnchor),
                    webView.leadingAnchor.constraint(equalTo: view.leadingAnchor),
                    webView.bottomAnchor.constraint(equalTo: view.bottomAnchor),
                    webView.trailingAnchor.constraint(equalTo: view.trailingAnchor)
                ])
                
                // Load the web page
                let url = URL(string: "http://www.google.com")!
                let request = URLRequest(url: url)
                webView.load(request)
    }
}

