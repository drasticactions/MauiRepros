using System.Diagnostics;

namespace SourceInfoCheck;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
		var referencedLabelSource = VisualDiagnostics.GetSourceInfo(this.referencedLabel);
		Debug.WriteLine($"Referenced Label: {referencedLabelSource.LineNumber}");
		var otherLabelSource = VisualDiagnostics.GetSourceInfo(this.firstLabel);
		Debug.WriteLine($"First Label: {otherLabelSource.LineNumber}");
		var secondLabelSource = VisualDiagnostics.GetSourceInfo(this.secondLabel);
		 Debug.WriteLine($"Second Label: {secondLabelSource.LineNumber})");
		this.SourceInfoLabel.Text = $"Referenced Label: {referencedLabelSource.LineNumber}\nFirst Label: {otherLabelSource.LineNumber}\nSecond Label:{secondLabelSource.LineNumber}";
	}
}

