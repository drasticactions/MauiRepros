
namespace App;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : Activity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Set our view from the "main" layout resource
        SetContentView(Resource.Layout.activity_main);

        // Reference the button by its ID
        Button buttonClickMe = FindViewById<Button>(Resource.Id.buttonClickMe);

        // Add a click event handler to the button
        buttonClickMe.Click += ButtonClickMe_Click;
    }

    private void ButtonClickMe_Click(object? sender, EventArgs e)
    {
        new ClassNet7.Class1();
        new ClassNet8.Class1();
    }
}