using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiCollectionViewResize;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
        this.BindingContext = this;
	}

    public ObservableCollection<ChatMessage> Messages { get; private set; } = new ObservableCollection<ChatMessage>();

    void SingleLineButton_Clicked(System.Object sender, System.EventArgs e)
    {
        this.Messages.Add(new ChatMessage("Short Message", Color.FromRgb(225, 0, 0)));
    }

    void MultiLineButton_Clicked(System.Object sender, System.EventArgs e)
    {
        this.Messages.Add(new ChatMessage("Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Aenean et tortor at risus viverra adipiscing. Ornare suspendisse sed nisi lacus sed viverra tellus in. Montes nascetur ridiculus mus mauris. Id cursus metus aliquam eleifend mi in nulla posuere sollicitudin. Est ultricies integer quis auctor elit sed vulputate mi. Ipsum dolor sit amet consectetur adipiscing elit pellentesque habitant morbi. Blandit massa enim nec dui nunc mattis enim ut. Facilisis sed odio morbi quis commodo odio. Proin sagittis nisl rhoncus mattis rhoncus urna. Lectus urna duis convallis convallis tellus id interdum velit laoreet.", Color.FromRgb(0, 255, 0)));
    }

    async void SingleToMultiButton_Clicked(System.Object sender, System.EventArgs e)
    {
        var chatMessage = new ChatMessage("Wait a sec...", Color.FromRgb(0, 0, 255));
        this.Messages.Add(chatMessage);
        await Task.Delay(1500);
        chatMessage.Message = "Maecenas sed enim ut sem viverra. Id diam vel quam elementum pulvinar etiam non quam lacus. Pretium lectus quam id leo. Volutpat blandit aliquam etiam erat velit scelerisque in dictum non. Iaculis eu non diam phasellus vestibulum. Feugiat in fermentum posuere urna nec tincidunt. Quam nulla porttitor massa id neque aliquam. Ut diam quam nulla porttitor massa id. Purus gravida quis blandit turpis cursus in. Risus at ultrices mi tempus imperdiet nulla malesuada pellentesque. Condimentum id venenatis a condimentum vitae sapien pellentesque habitant morbi. Quam nulla porttitor massa id neque aliquam vestibulum morbi blandit. Eu feugiat pretium nibh ipsum consequat nisl vel. Etiam dignissim diam quis enim lobortis.";
    }
}

public class ChatMessage : INotifyPropertyChanged
{
    private bool isBusy;
    private string message;

    public ChatMessage(string message, Color color)
    {
        this.Message = message;
        this.BackgroundColor = color;
    }
    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    public bool IsBusy
    {
        get { return this.isBusy; }
        set { this.SetProperty(ref this.isBusy, value); }
    }

    public string Message
    {
        get { return this.message; }
        set
        {
            this.IsBusy = string.IsNullOrEmpty(value);
            this.SetProperty(ref this.message, value);
        }
    }

    public Color BackgroundColor { get; }

    public string Text { get; }

#pragma warning disable SA1600 // Elements should be documented
    protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action? onChanged = null)
#pragma warning restore SA1600 // Elements should be documented
    {
        if (EqualityComparer<T>.Default.Equals(backingStore, value))
        {
            return false;
        }

        backingStore = value;
        onChanged?.Invoke();
        this.OnPropertyChanged(propertyName);
        return true;
    }

    /// <summary>
    /// On Property Changed.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        var changed = this.PropertyChanged;
        if (changed == null)
        {
            return;
        }

        changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}