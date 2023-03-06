using System.Collections.ObjectModel;

namespace MauiImageLayout;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
        this.BindingContext = this;

        this.ImageLinks.Add("https://placehold.jp/150x150.png");
        this.ImageLinks.Add("https://placehold.jp/150x150.png");
        this.ImageLinks.Add("https://placehold.jp/150x150.png");
        this.ImageLinks.Add("https://placehold.jp/150x150.png");
        this.ImageLinks.Add("https://placehold.jp/150x150.png");
        this.ImageLinks.Add("https://placehold.jp/150x150.png");
        this.ImageLinks.Add("https://placehold.jp/150x150.png");
        this.ImageLinks.Add("https://placehold.jp/150x150.png");
        this.ImageLinks.Add("https://placehold.jp/150x150.png");
        this.ImageLinks.Add("https://placehold.jp/150x150.png");
        this.ImageLinks.Add("https://placehold.jp/150x150.png");
    }
    public ObservableCollection<string> ImageLinks { get; private set; } = new ObservableCollection<string>();
}

