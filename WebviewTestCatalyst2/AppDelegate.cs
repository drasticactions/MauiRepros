using ObjCRuntime;
using WebKit;

namespace WebviewTestCatalyst;

[Register("AppDelegate")]
public class AppDelegate : UIApplicationDelegate
{
    public override UIWindow? Window { get; set; }

    public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
    {
        // create a new window instance based on the screen size
        Window = new UIWindow(UIScreen.MainScreen.Bounds);

        // create a UIViewController with a single UILabel
        var vc = new UIViewController();
        var webview = new TestDownloadWebView(Window!.Frame, new WebKit.WKWebViewConfiguration())
        {
            AutoresizingMask = UIViewAutoresizing.All
        };
        
        webview.Cook

        vc.View!.AddSubview(webview);
        Window.RootViewController = vc;

        // make the window visible
        Window.MakeKeyAndVisible();

        webview.LoadRequest(new NSUrlRequest(
            new NSUrl("https://amazon.com")));

        return true;
    }

    public class TestDownloadWebView : WKWebView, IWKDownloadDelegate, IWKNavigationDelegate
    {
        public TestDownloadWebView(CGRect frame, WKWebViewConfiguration configuration) : base(frame, configuration)
        {
            this.NavigationDelegate = this;
        }

        public void DecideDestination(WKDownload download, NSUrlResponse response, string suggestedFilename,
            Action<NSUrl> completionHandler)
        {
            var destinationURL = GetDestinationURL();

            completionHandler?.Invoke(destinationURL);
        }

        [Export("webView:decidePolicyForNavigationResponse:decisionHandler:")]
        public void DecidePolicy(WKWebView webView, WKNavigationResponse navigationResponse, Action<WKNavigationResponsePolicy> decisionHandler)
        {
            var url = navigationResponse.Response.Url;
            var mimeType = navigationResponse.Response.MimeType;
            Console.WriteLine($"Content-Type: {mimeType}");

            // Perform any actions based on the content type
            if (mimeType == "application/pdf")
            {
                // Download the PDF file separately instead of loading it in the WKWebView
                DownloadPDF(url);

                decisionHandler?.Invoke(WKNavigationResponsePolicy.Cancel);
            }
            else
            {
                decisionHandler?.Invoke(WKNavigationResponsePolicy.Allow);
            }
           
        }

        private void DownloadPDF(NSUrl url)
        {
            var downloadTask = NSUrlSession.SharedSession.CreateDownloadTask(url, (location, _, error) =>
            {
                if (location is NSUrl sourceURL && error == null)
                {
                    var destinationURL = GetDestinationURL();

                    try
                    {
                        NSFileManager.DefaultManager.Move(sourceURL, destinationURL, out error);
                        Console.WriteLine($"PDF file downloaded and saved at: {destinationURL.Path}");

                        // Perform any additional actions with the downloaded file
                    }
                    catch (Exception ex)
                    {
                        // Handle file moving error
                    }
                }
                else
                {
                    // Handle download error
                }
            });

            downloadTask.Resume();
        }

        private NSUrl GetDestinationURL()
        {
            // Customize the destination URL as desired
            var documentsURL =
                NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User)
                    [0];
            var destinationURL = documentsURL.Append("downloaded_file.pdf", false);

            return destinationURL;
        }
    }
}