namespace XmlApple.Maui;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        var xmlSerializer = new System.Xml.Serialization.XmlSerializer(typeof(Test));
    }
}

public class Test
{
}
