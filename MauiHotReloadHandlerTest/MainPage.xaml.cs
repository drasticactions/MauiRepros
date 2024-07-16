namespace MauiHotReloadHandlerTest;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"dd {count} time";
		else
			CounterBtn.Text = $"test {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}

	[Microsoft.Maui.HotReload.OnHotReload]
	static void OnHotReload()
	{
		Microsoft.Maui.Controls.Application.Current!.Dispatcher.DispatchAsync(async () => {

            await Microsoft.Maui.Controls.Application.Current.MainPage!.DisplayAlert("Hot Reload", "Hot Reload was triggered", "OK");
        });
	}
}

