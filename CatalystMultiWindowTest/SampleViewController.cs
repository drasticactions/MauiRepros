using Drastic.PureLayout;


/// <summary>
/// Sample View Controller.
/// </summary>
public class SampleViewController : UIViewController
{
    private UIButton label = new UIButton();

    /// <summary>
    /// Initializes a new instance of the <see cref="SampleViewController"/> class.
    /// </summary>
    /// <param name="title">Sets the text on the label.</param>
    public SampleViewController()
    {
        this.label = new UIButton()
        {
            
            BackgroundColor = UIColor.Clear,
            AutoresizingMask = UIViewAutoresizing.All,
        };
        label.SetTitle("Open Window", UIControlState.Normal);
        label.TouchUpInside += (sender, e) =>
        {
            UIApplication.SharedApplication.RequestSceneSessionActivation(null, new NSUserActivity("SceneDelegate"), null, null);
        };

        this.View!.AddSubview(this.label);
        this.label.AutoCenterInSuperview();
    }
}