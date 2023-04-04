namespace LabelTest;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		// var test = this.HelloWorld.HorizontalOptions;
		//this.HelloWorld.HorizontalOptions = LayoutOptions.Start;
	}

    void Button_Clicked(System.Object sender, System.EventArgs e)
    {
        this.HelloWorld.HorizontalOptions = LayoutOptions.End;
    }
}

