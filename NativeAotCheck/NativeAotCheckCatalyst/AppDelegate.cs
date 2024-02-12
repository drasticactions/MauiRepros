using Drastic.PureLayout;
using Microsoft.Data.Sqlite;
using Nanorm;
using SQLitePCL;

namespace NativeAotCheckCatalyst;

[Register ("AppDelegate")]
public class AppDelegate : UIApplicationDelegate {
	public override UIWindow? Window {
		get;
		set;
	}

	public override bool FinishedLaunching (UIApplication application, NSDictionary launchOptions)
	{
		// create a new window instance based on the screen size
		Window = new UIWindow (UIScreen.MainScreen.Bounds);

		// create a UIViewController with a single UILabel
		var vc = new TestViewController (new SqliteConnectionTest());
		Window.RootViewController = vc;

		// make the window visible
		Window.MakeKeyAndVisible ();

		return true;
	}
}

public class TestViewController : UIViewController
{
	private UIButton button;
	
	public TestViewController(IDatabaseTest database)
	{
		this.button = new UIButton(UIButtonType.System);
		this.button.SetTitle("Click me", UIControlState.Normal);
		this.button.TouchUpInside += (sender, args) =>
		{
			database.RunTest();
		};
		this.View!.AddSubview(this.button);
		this.button.AutoCenterInSuperview();
	}
}

public interface IDatabaseTest
{
	Task RunTest();
}

[DataRecordMapper]
public partial class Todo
{
	public int Id { get; set; }

	public required string Title { get; set; }

	public bool IsComplete { get; set; }
}

public class SqliteConnectionTest : IDatabaseTest
{
	public Task RunTest()
	{
		SqliteConnection conn = new SqliteConnection("Data Source=:memory:");
		SQLitePCL.raw.SetProvider(new SQLite3Provider_sqlite3());
		conn.Open();
		conn.
		var cmd = conn.CreateCommand();
		cmd.CommandText = "SELECT sqlite_version()";
		var version = cmd.ExecuteScalar();
        
		Console.WriteLine($"Hello from sqlite {version}");
		return Task.CompletedTask;
	}
}
