namespace ThemeChangeError;

public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();
        App.Current.RequestedThemeChanged += (s, a) =>
        {
            this.UpdateLabels();
        };
        this.UpdateLabels();
    }

    private void SwitchAppTheme(AppTheme theme)
    {
        switch (theme)
        {
            case AppTheme.Light:
                App.Current.UserAppTheme = AppTheme.Dark;
                break;
            case AppTheme.Dark:
                App.Current.UserAppTheme = AppTheme.Light;
                break;
        }
    }

    private void UpdateLabels()
    {
        this.AppThemeLabel.Text = $"Platform Theme: {App.Current.PlatformAppTheme}";
        this.UserThemeLabel.Text = $"User Theme: {App.Current.UserAppTheme}";
        this.RequestedThemeLabel.Text = $"Requested Theme: {App.Current.RequestedTheme}";
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        count++;

        if (count == 1)
            CounterBtn.Text = $"Clicked {count} time";
        else
            CounterBtn.Text = $"Clicked {count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        var theme = App.Current.UserAppTheme != AppTheme.Unspecified ? App.Current.UserAppTheme : App.Current.PlatformAppTheme;
        SwitchAppTheme(theme);
    }
}

