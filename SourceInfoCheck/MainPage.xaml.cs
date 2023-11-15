using System.Diagnostics;

namespace SourceInfoCheck;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
		var referencedLabelSource = VisualDiagnostics.GetSourceInfo(this.referencedLabel);
		Debug.WriteLine($"Referenced Label: {referencedLabelSource.LinePosition}");
		// var otherLabelSource = VisualDiagnostics.GetSourceInfo(this.firstLabel);
		// Debug.WriteLine($"First Label: {otherLabelSource.LinePosition}");
		// var secondLabelSource = VisualDiagnostics.GetSourceInfo(this.secondLabel);
		// Debug.WriteLine($"Second Label: {secondLabelSource.LinePosition})");
		//this.SourceInfoLabel.Text = $"Referenced Label: {referencedLabelSource.LinePosition}\nFirst Label: {otherLabelSource.LinePosition}\nSecond Label:{secondLabelSource.LinePosition}";
	}
}

