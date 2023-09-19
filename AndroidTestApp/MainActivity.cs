using AndroidX.AppCompat.App;

namespace AndroidTestApp;

[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
public class MainActivity : AppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Set our view from the "main" layout resource
        SetContentView(Resource.Layout.activity_main);

        EditText doneEntry = FindViewById<EditText>(Resource.Id.DoneEntry);
        doneEntry.ImeOptions = Android.Views.InputMethods.ImeAction.Search;
        //searchEntry.ImeOptions = Android.Views.InputMethods.ImeAction.Search;
    }
}