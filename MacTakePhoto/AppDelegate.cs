namespace MacTakePhoto;

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

		// create a UIViewController with a single UILabel
		var vc = new ViewController ();

		Window.RootViewController = vc;

		// make the window visible
		Window.MakeKeyAndVisible ();

		return true;
	}
}

public partial class ViewController : UIViewController
    {
        private UIImageView imageView;
        private UIButton photoButton;
        private UIImagePickerController imagePicker;
        
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Create the UIImageView to display the captured photo
            imageView = new UIImageView
            {
                Frame = new CoreGraphics.CGRect(50, 100, 300, 300),
                ContentMode = UIViewContentMode.ScaleAspectFit
            };

            // Create the UIButton to trigger the image picker
            photoButton = new UIButton
            {
                Frame = new CoreGraphics.CGRect(150, 450, 100, 40),
                BackgroundColor = UIColor.Blue
            };
            photoButton.SetTitle("Take Photo", UIControlState.Normal);
            photoButton.TouchUpInside += PhotoButton_TouchUpInside;

            // Add the UIImageView and UIButton to the view
            View.AddSubview(imageView);
            View.AddSubview(photoButton);
        }

        private void PhotoButton_TouchUpInside(object sender, EventArgs e)
        {
            // Check if the device supports a camera
            if (UIImagePickerController.IsSourceTypeAvailable(UIImagePickerControllerSourceType.Camera))
            {
                imagePicker = new UIImagePickerController
                {
                    SourceType = UIImagePickerControllerSourceType.Camera,
                    MediaTypes = UIImagePickerController.AvailableMediaTypes(UIImagePickerControllerSourceType.Camera),
                    AllowsEditing = false
                };

                imagePicker.FinishedPickingMedia += ImagePicker_FinishedPickingMedia;
                imagePicker.Canceled += ImagePicker_Canceled;

                PresentViewController(imagePicker, true, null);
            }
            else
            {
                // Device doesn't support a camera
                // Handle this case accordingly
                Console.WriteLine("Camera not available");
            }
        }

        private void ImagePicker_FinishedPickingMedia(object sender, UIImagePickerMediaPickedEventArgs e)
        {
            UIImage image = e.OriginalImage;

            if (image != null)
            {
                // Display the captured image in the UIImageView
                imageView.Image = image;
            }

            imagePicker.DismissViewController(true, null);
        }

        private void ImagePicker_Canceled(object sender, EventArgs e)
        {
            imagePicker.DismissViewController(true, null);
        }
    }
