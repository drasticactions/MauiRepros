//
//  ViewController.swift
//  WebViewChartTest
//
//  Created by ミラー・ティモシー on 2023/04/06.
//

import UIKit
import WebKit
class ViewController: UIViewController {

    var webView: WKWebView!
    var button: UIButton!
    
    override func viewDidLoad() {
        super.viewDidLoad()
        let configuration = WKWebViewConfiguration()
#if DEBUG
        configuration.preferences.setValue(true, forKey: "developerExtrasEnabled")
#endif
        
        // https://stackoverflow.com/questions/37159648/how-to-read-console-logs-of-wkwebview-programmatically
        // We should not need this, but I can't get the Safari tools running in Catalyst.
        let overrideConsole = """
            function log(emoji, type, args) {
              window.webkit.messageHandlers.logging.postMessage(
                `${emoji} JS ${type}: ${Object.values(args)
                  .map(v => typeof(v) === "undefined" ? "undefined" : typeof(v) === "object" ? JSON.stringify(v) : v.toString())
                  .map(v => v.substring(0, 3000)) // Limit msg to 3000 chars
                  .join(", ")}`
              )
            }

            let originalLog = console.log
            let originalWarn = console.warn
            let originalError = console.error
            let originalDebug = console.debug

            console.log = function() { log("📗", "log", arguments); originalLog.apply(null, arguments) }
            console.warn = function() { log("📙", "warning", arguments); originalWarn.apply(null, arguments) }
            console.error = function() { log("📕", "error", arguments); originalError.apply(null, arguments) }
            console.debug = function() { log("📘", "debug", arguments); originalDebug.apply(null, arguments) }

            window.addEventListener("error", function(e) {
               log("💥", "Uncaught", [`${e.message} at ${e.filename}:${e.lineno}:${e.colno}`])
            })
        """
        
        class LoggingMessageHandler: NSObject, WKScriptMessageHandler {
            func userContentController(_ userContentController: WKUserContentController, didReceive message: WKScriptMessage) {
                print(message.body)
            }
        }
        
        let userContentController = WKUserContentController()
        userContentController.add(LoggingMessageHandler(), name: "logging")
        userContentController.addUserScript(WKUserScript(source: overrideConsole, injectionTime: .atDocumentStart, forMainFrameOnly: true))
        configuration.userContentController = userContentController
        
        webView = WKWebView(frame: self.view.bounds, configuration: configuration)
               
        webView.translatesAutoresizingMaskIntoConstraints = false // disable autoresizing mask
        self.view.addSubview(webView)
        
        button = UIButton(type: .system)
        button.translatesAutoresizingMaskIntoConstraints = false
        button.setTitle("Execute JS in WebView", for: .normal)
        button.addTarget(self, action: #selector(buttonPressed), for: .touchUpInside)
        view.addSubview(button)
                       
        NSLayoutConstraint.activate([
                    webView.topAnchor.constraint(equalTo: view.topAnchor),
                    webView.leadingAnchor.constraint(equalTo: view.leadingAnchor),
                    webView.trailingAnchor.constraint(equalTo: view.trailingAnchor),
                    webView.bottomAnchor.constraint(equalTo: button.topAnchor),

                    button.bottomAnchor.constraint(equalTo: view.bottomAnchor),
                    button.leadingAnchor.constraint(equalTo: view.leadingAnchor),
                    button.trailingAnchor.constraint(equalTo: view.trailingAnchor),
                    button.heightAnchor.constraint(equalToConstant: 50)
                ])
    
        // Load the web page
        let url = URL(string: "https://digital.fidelity.com/stgw/digital/fidchart/consumers/atp/0.0.4/atp-chart.html")!
        let request = URLRequest(url: url)
        webView.load(request)
    }

    // Button action method
    @objc func buttonPressed() {
        let chartOptions = """
        { "header":false,"isAuthenticated":true,"symbol":"AAPL","isPartial":false,"displays":{ "default":"candle" },"timeframe":{ "default":"TODAY","items":["TODAY","2D","5D","10D","1M","3M","6M","YTD","1Y","2Y","5Y","10Y","MAX"] },"frequency":{ "default":"5m","items":["1m","5m","10m","15m","30m","1H","4H","1D","1W","1M","1Q","1Y"] },"study":false,"toggle":{ "savedChartsMenu":true,"patternEventsMenu":false,"openOrders":false }}
        """
        let datasourceOptions = """
        { "env":"dev", "productid":"chartplatform", "realtime":false, "uuid":"d94167ee-1b27-11dc-ab78-ac196951aa77" }
        """

        do {
            webView.evaluateJavaScript("(displayInitialChart(\(chartOptions),\(datasourceOptions)))")
        } catch {
            // Handle the error
        }

    }
}

