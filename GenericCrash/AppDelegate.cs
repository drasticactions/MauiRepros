namespace GenericCrash;

[Register ("AppDelegate")]
public class AppDelegate : UIApplicationDelegate {
	public override UIWindow? Window {
		get;
		set;
	}

	public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
	{
		// create a new window instance based on the screen size
		Window = new UIWindow (UIScreen.MainScreen.Bounds);
		InitializeList();

		// create a UIViewController with a single UILabel
		var vc = new UIViewController ();
		var button = new UIButton (Window!.Frame) {
			AutoresizingMask = UIViewAutoresizing.All,
		};
		button.SetTitle ("Click me for average!", UIControlState.Normal);
		button.TouchUpInside += (sender, e) => {
			try
			{
				var average = ListItems.Average(x => x.Cost);
				var alert = UIAlertController.Create("Average", $"The average is {average}",
					UIAlertControllerStyle.Alert);
				alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
				vc.PresentViewController(alert, true, null);
			}
			catch (Exception exception)
			{
				var alert = UIAlertController.Create("Error", exception.Message,
					UIAlertControllerStyle.Alert);
				alert.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
				vc.PresentViewController(alert, true, null);
			}
		};
		vc.View!.AddSubview (button);
		Window.RootViewController = vc;

		// make the window visible
		Window.MakeKeyAndVisible ();

		return true;
	}

	 private void InitializeList()
        {
            List<CustomModel> models = new List<CustomModel>();

            for (int i = 0; i < 50000; i++)
            {
                models.Add(new CustomModel
                {
                    Title = "Test " + i,
                    Cost = i
                });
            }
            ListItems = models;
        }

        private List<CustomModel> listItems = [];

        public List<CustomModel> ListItems
        {
            get { return listItems; }
            set
            {
                listItems = value;
            }
        }
}



    public class CustomModel
    {
        public string Title { get; set; }
        public decimal Cost { get; set; }
    }
