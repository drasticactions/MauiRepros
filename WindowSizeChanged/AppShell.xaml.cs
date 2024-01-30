using Microsoft.Maui.Platform;

namespace WindowSizeChanged;

public partial class AppShell : Shell
{
	IDisposable? _frameObserver;
	private const double minPageWidth = 600;
	
	public AppShell()
	{
		InitializeComponent();
	}

	protected override void OnHandlerChanged()
	{
		base.OnHandlerChanged();
		#if IOS || MACCATALYST
		var window = this.GetParentWindow().ToPlatform(this.Handler!.MauiContext!);
		_frameObserver = window.AddObserver("frame", Foundation.NSKeyValueObservingOptions.New,
			(Foundation.NSObservedChange obj) =>
			{
				if (obj.NewValue is Foundation.NSValue value)
				{
					var rectangle = value.CGRectValue.ToRectangle();
					double currentWidth = rectangle.Width;
					if (currentWidth <= minPageWidth && this.FlyoutBehavior != FlyoutBehavior.Flyout)
					{
					    this.FlyoutBehavior = FlyoutBehavior.Flyout;
					    this.FlyoutIsPresented = false;
					}
					// FlyoutWidth is always -1
					else if (currentWidth > minPageWidth + this.FlyoutWidth)
					{
						this.FlyoutBehavior = FlyoutBehavior.Locked;
					}
				}
			});
		#endif
	}
}
