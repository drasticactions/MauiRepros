using Android.Graphics;

namespace AndroidLineBreak;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : Activity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        SetContentView(Resource.Layout.activity_main);
        
        TextView myLabel = FindViewById<TextView>(Resource.Id.myLabel);
        string text = @"1
2"; 
        myLabel.Text = text;
        myLabel.PaintFlags |= PaintFlags.UnderlineText;
    }
}