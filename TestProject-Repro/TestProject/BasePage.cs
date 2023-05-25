using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestProject.ViewModels;

namespace TestProject
{
    /// <summary>
    /// Base class for our content pages.
    /// </summary>
    public class BasePage : ContentPage
    {
        private BaseViewModel ViewModel => BindingContext as BaseViewModel;

        /// <inheritdoc/>
        protected override void OnAppearing()
        {
            base.OnAppearing();
            ViewModel?.OnAppearing();
        }

        /// <inheritdoc/>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
          //  ViewModel?.OnDisappearing();
        }

        /// <inheritdoc/>
        protected override void OnNavigatingFrom(NavigatingFromEventArgs args)
        {
            base.OnNavigatingFrom(args);
          //  ViewModel?.OnNavigatingFrom(args);
        }

        /// <inheritdoc/>
        protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
        {
            base.OnNavigatedFrom(args);
          //  ViewModel?.OnNavigatedFrom(args);
        }

        /// <inheritdoc/>
        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
            ViewModel?.OnNavigatedTo(args);
        }
    }
}
