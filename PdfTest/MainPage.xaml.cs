namespace PdfTest;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private async void OnCounterClicked(object sender, EventArgs e)
	{
		await Launcher.OpenAsync(new OpenFileRequest("PDF", new ReadOnlyFile( "test.pdf")));
	}
}

