using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiSceneBug;

public partial class NewPage : ContentPage
{
    public NewPage()
    {
        InitializeComponent();
    }

    private void Button_OnClicked(object sender, EventArgs e)
    {
        Application.Current.OpenWindow(new Window(new NewPage()));
    }

    private void Button_OnClickedCrash(object sender, EventArgs e)
    {
        throw new Exception();
    }
}