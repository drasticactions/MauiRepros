using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AndroidTapThroughIssue
{
    public partial class MainPage : ContentPage
    {
        private int _count;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            _count++;

            CounterBtn.Text = _count == 1 ? $"Clicked {_count} time" : $"Clicked {_count} times";

        }
    }
}
