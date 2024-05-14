using CoreFoundation;
using CoreLocation;

namespace CLLocationUpdateSample;

[Register ("AppDelegate")]
public class AppDelegate : UIApplicationDelegate {
	public override UIWindow? Window {
		get;
		set;
	}

	public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
	{
		Window = new UIWindow (UIScreen.MainScreen.Bounds);

		var vc = new ViewController (Window);
		Window.RootViewController = vc;

		Window.MakeKeyAndVisible ();

		return true;
	}
}

public class ViewController : UIViewController
    {
        UILabel locationLabel;
        private DispatchQueue queue;
        private CLLocationUpdater? updater;
        CLLocationManager locationManager;
        
        public ViewController(UIWindow window)
		{
			locationLabel = new UILabel(window.Frame)
			{
				BackgroundColor = UIColor.SystemBackground,
				TextAlignment = UITextAlignment.Center,
				Text = "Location: Unknown",
				AutoresizingMask = UIViewAutoresizing.All,
			};

			View.AddSubview(locationLabel);
		}
        
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            this.locationManager = new CLLocationManager
            {
	            DesiredAccuracy = CLLocation.AccuracyBest
            };
            this.queue = DispatchQueue.GetGlobalQueue(DispatchQueuePriority.Default);
            this.updater = CLLocationUpdater.CreateLiveUpdates(queue, (location) =>
            {
	            InvokeOnMainThread(() =>
	            {
		            locationLabel.Text = $"Location: {location.Location}";
	            });
            });
            
            // You need to allow location permissions in the Info.plist file
            // And you need to request permissions in the code
            this.locationManager.AuthorizationChanged += LocationManager_AuthorizationChanged;
            this.locationManager.RequestWhenInUseAuthorization();
            this.updater!.Resume();
        }
        
        void LocationManager_AuthorizationChanged(object sender, CLAuthorizationChangedEventArgs e)
        {
	        if (e.Status == CLAuthorizationStatus.AuthorizedWhenInUse || e.Status == CLAuthorizationStatus.AuthorizedAlways)
	        {
		        this.updater!.Resume();
	        }
	        else
	        {
		        InvokeOnMainThread(() =>
		        {
			        locationLabel.Text = "Location permissions denied";
		        });
	        }
        }

        void LocationManager_Failed(object sender, NSErrorEventArgs e)
        {
	        Console.WriteLine($"Failed to find user's location: {e.Error.LocalizedDescription}");
	        locationLabel.Text = "Failed to get location";
        }
    }