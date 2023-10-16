namespace ListViewTestNet8;

public partial class MainPage : ContentPage
{
	int count = 0;
	
	public MainPage()
	{
		InitializeComponent();
	}

	private void TestCellOnClicked(object sender, EventArgs e)
	{
		this.Window!.Page = new TextCellPage();
	}
	
	private void ViewCellOnClicked(object sender, EventArgs e)
	{
		this.Window!.Page = new ViewCellExample();
	}
	
	private void SwitchCellOnClicked(object sender, EventArgs e)
	{
		this.Window!.Page = new SwitchCellExample();
	}
	
	private void EntryCellOnClicked(object sender, EventArgs e)
	{
		this.Window!.Page = new EntryCellExample();
	}
	
	private void ImageCellOnClicked(object sender, EventArgs e)
	{
		this.Window!.Page = new ImageCellExample();
	}
	
	private void TestCellNavOnClicked(object sender, EventArgs e)
	{
		this.Window!.Page = new NavigationPage(new TextCellPage());
	}
}

