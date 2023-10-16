using UIKit;
using RectangleF = CoreGraphics.CGRect;

namespace NativePickerTest;

[Register ("AppDelegate")]
public class AppDelegate : UIApplicationDelegate {
	public override UIWindow? Window {
		get;
		set;
	}

	public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
	{
		DrasticForbiddenControls.CatalystControls.AllowsUnsupportedMacIdiomBehavior();
		
		// create a new window instance based on the screen size
		Window = new UIWindow (UIScreen.MainScreen.Bounds);

		// create a UIViewController with a single UILabel
		var vc = new MyViewController ();
		Window.RootViewController = vc;

		// make the window visible
		Window.MakeKeyAndVisible ();

		return true;
	}


}

public class MyViewController : UIViewController, IUIPickerViewDelegate, IUIPickerViewDataSource
    {
        private UIPickerView pickerView1;
        private UIPickerView pickerView2;
        private UIPickerView pickerView3;
        private string[] data1 = { "Option 1", "Option 2", "Option 3", "Option 4", "Option 5" };
        private string[] data2 = { "Red", "Green", "Blue", "Yellow", "Orange" };
        private string[] data3 = { "Item A", "Item B", "Item C", "Item D", "Item E" };

        public MyViewController()
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Create UIPickerView 1
            pickerView1 = new UIPickerView();
            pickerView1.Frame = new CoreGraphics.CGRect(20, 50, 200, 150);
            pickerView1.Model = new PickerModel(data1);
            View.AddSubview(pickerView1);
			using var action = new UIAlertController();
            // Create UIPickerView 2
            pickerView2 = new UIPickerView();
            pickerView2.Frame = new CoreGraphics.CGRect(20, 250, 200, 150);
            pickerView2.Model = new PickerModel(data2);
            View.AddSubview(pickerView2);

            // Create UIPickerView 3
            pickerView3 = new UIPickerView();
            pickerView3.Frame = new CoreGraphics.CGRect(20, 450, 200, 150);
            pickerView3.Model = new PickerModel(data3);
            View.AddSubview(pickerView3);
        }

        // UIPickerViewDataSource methods (required)
        public nint GetComponentCount(UIPickerView pickerView)
        {
            return 1; // We have only one column in each UIPickerView
        }

        public nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            if (pickerView == pickerView1)
                return data1.Length;
            else if (pickerView == pickerView2)
                return data2.Length;
            else if (pickerView == pickerView3)
                return data3.Length;
            return 0;
        }

        // UIPickerViewDelegate methods (optional)
        public string GetTitle(UIPickerView pickerView, nint row, nint component)
        {
            if (pickerView == pickerView1)
                return data1[(int)row];
            else if (pickerView == pickerView2)
                return data2[(int)row];
            else if (pickerView == pickerView3)
                return data3[(int)row];
            return "";
        }
    }

    // Custom UIPickerView model
    public class PickerModel : UIPickerViewModel
    {
        private string[] data;

        public PickerModel(string[] data)
        {
            this.data = data;
        }

        public override nint GetComponentCount(UIPickerView pickerView)
        {
            return 1;
        }

        public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
        {
            return data.Length;
        }

        public override string GetTitle(UIPickerView pickerView, nint row, nint component)
        {
            return data[(int)row];
        }
    }

	public class PickerViewController : UIViewController
    {
        private UIPickerView pickerView;
        private string[] data = { "Option 1", "Option 2", "Option 3", "Option 4", "Option 5" };

        public PickerViewController()
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = "PickerView Example";

            // Create a "Select" button
            var selectButton = new UIButton(UIButtonType.System);
            selectButton.Frame = new CoreGraphics.CGRect(20, 220, View.Bounds.Width - 40, 40);
            selectButton.SetTitle("Select", UIControlState.Normal);
            selectButton.TouchUpInside += SelectButtonTapped;
            View.AddSubview(selectButton);
        }

        private void SelectButtonTapped(object sender, EventArgs e)
        {
            // Display the selected option
            var alertController = UIAlertController.Create("Selected Option", "", UIAlertControllerStyle.ActionSheet);
            alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
			pickerView = new UIPickerView();
			var frame = new RectangleF(0, 0, 269, 240);
            pickerView.Frame = frame;
            pickerView.Model = new PickerModel(data);
            alertController.View!.AddSubview(pickerView);
			var doneButtonHeight = 90;
			var height = NSLayoutConstraint.Create(alertController.View, NSLayoutAttribute.Height, NSLayoutRelation.Equal, null, NSLayoutAttribute.NoAttribute, 1, 240 + doneButtonHeight);
			alertController.View.AddConstraint(height);
            PresentViewController(alertController, true, null);
        }
    }

	public class PickerAlertViewController : UIViewController
    {
        private string[] data = { "Option 1", "Option 2", "Option 3", "Option 4", "Option 5" };

        public PickerAlertViewController()
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = "Picker Alert Example";

            var selectButton = new UIButton(UIButtonType.System);
            selectButton.Frame = new CoreGraphics.CGRect(20, 100, View.Bounds.Width - 40, 40);
            selectButton.SetTitle("Select", UIControlState.Normal);
            selectButton.TouchUpInside += ShowPickerAlert;
            View.AddSubview(selectButton);
        }

        private void ShowPickerAlert(object sender, EventArgs e)
        {
            UIAlertController alertController = UIAlertController.Create("Select an Option", "", UIAlertControllerStyle.ActionSheet);

            UIPickerView pickerView = new UIPickerView();
            pickerView.Model = new PickerModel(data);

            UITextField pickerTextField = new UITextField
            {
                InputView = pickerView,
                BorderStyle = UITextBorderStyle.RoundedRect
            };

            alertController.AddTextField((textField) =>
            {
                textField.Placeholder = "Select an option";
                textField.InputView = pickerTextField.InputView;
            });

            alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (action) =>
            {
                int selectedRow = (int)pickerView.SelectedRowInComponent(0);
                string selectedOption = data[selectedRow];

                UIAlertController resultAlertController = UIAlertController.Create("Selected Option", selectedOption, UIAlertControllerStyle.Alert);
                resultAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

                PresentViewController(resultAlertController, true, null);
            }));

            alertController.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));

            PresentViewController(alertController, true, null);
        }
    }

	public class ActionSheetViewController : UIViewController
    {
        public ActionSheetViewController()
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = "Action Sheet Example";

            var showActionSheetButton = new UIButton(UIButtonType.System);
            showActionSheetButton.Frame = new CoreGraphics.CGRect(20, 100, View.Bounds.Width - 40, 40);
            showActionSheetButton.SetTitle("Show Action Sheet", UIControlState.Normal);
            showActionSheetButton.TouchUpInside += ShowActionSheetButtonTapped;
            View.AddSubview(showActionSheetButton);
        }

        private void ShowActionSheetButtonTapped(object sender, EventArgs e)
        {
            UIAlertController alertController = UIAlertController.Create("Action Sheet Title", null, UIAlertControllerStyle.ActionSheet);

            // Add actions to the action sheet
            alertController.AddAction(UIAlertAction.Create("Option 1", UIAlertActionStyle.Default, (action) =>
            {
                // Handle Option 1
                ShowMessage("Option 1 Selected");
            }));

            alertController.AddAction(UIAlertAction.Create("Option 2", UIAlertActionStyle.Default, (action) =>
            {
                // Handle Option 2
                ShowMessage("Option 2 Selected");
            }));

            alertController.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, null));
            // Present the action sheet
            PresentViewController(alertController, true, null);
        }

        private void ShowMessage(string message)
        {
            UIAlertController messageAlertController = UIAlertController.Create("Message", message, UIAlertControllerStyle.Alert);
            messageAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            PresentViewController(messageAlertController, true, null);
        }
    }