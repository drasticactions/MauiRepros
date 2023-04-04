namespace GridTest;

public partial class MainPage : ContentPage
{
	int count = 0;
	private bool isReversed = false;
	public MainPage()
	{
		InitializeComponent();
	}

    void Button_Clicked(System.Object sender, System.EventArgs e)
    {
		if (isReversed)
		{
			BaseGrid.SetRow(this.DotNetBot, 0);
            BaseGrid.SetRow(this.Label1, 1);
            BaseGrid.SetRow(this.Label2, 2);
            BaseGrid.SetRow(this.CounterBtn, 3);
        }
		else
		{
            BaseGrid.SetRow(this.DotNetBot, 3);
            BaseGrid.SetRow(this.Label1, 2);
            BaseGrid.SetRow(this.Label2, 1);
            BaseGrid.SetRow(this.CounterBtn, 0);
        }

        isReversed = !isReversed;
    }
}

