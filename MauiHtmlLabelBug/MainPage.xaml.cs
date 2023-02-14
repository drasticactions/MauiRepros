namespace MauiHtmlLabelBug;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
		this.BindingContext = this;
		this.TestLabel.Text = this.TestString;
	}

	public string TestString = @"<p>A la une: Objets volants aux Etats-Unis&nbsp;: ce que l’on sait, et ce qui reste encore mystérieux <a href=""https://www.lexpress.fr/monde/objets-volants-aux-etats-unis-ce-que-lon-sait-et-ce-qui-reste-encore-mysterieux-7MHFTTEKCVDMJGHLJXFJXRZ3D4/"" rel=""nofollow noopener noreferrer"" target=""_blank""><span class=""invisible"">https://www.</span><span class=""ellipsis"">lexpress.fr/monde/objets-volan</span><span class=""invisible"">ts-aux-etats-unis-ce-que-lon-sait-et-ce-qui-reste-encore-mysterieux-7MHFTTEKCVDMJGHLJXFJXRZ3D4/</span></a> <a href=""https://mastodon.roitsystems.ca/tags/nouvelles"" class=""mention hashtag"" rel=""nofollow noopener noreferrer"" target=""_blank"">#<span>nouvelles</span></a> <a href=""https://mastodon.roitsystems.ca/tags/lexpress"" class=""mention hashtag"" rel=""nofollow noopener noreferrer"" target=""_blank"">#<span>lexpress</span></a> <a href=""https://mastodon.roitsystems.ca/tags/express"" class=""mention hashtag"" rel=""nofollow noopener noreferrer"" target=""_blank"">#<span>express</span></a> <a href=""https://mastodon.roitsystems.ca/tags/francais"" class=""mention hashtag"" rel=""nofollow noopener noreferrer"" target=""_blank"">#<span>francais</span></a></p>";

}

