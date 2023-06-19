using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MauiApp1
{
    public partial class ViewModel : INotifyPropertyChanged
    {
        readonly CollectionView _collectionView;

        public ViewModel(CollectionView collectionView)
        {
            // Set up the items
            Items = new ObservableCollection<string> { "one", "two", "three", "four", "five" };

            // Here we try to preselect as per https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/collectionview/selection#multiple-preselection
            SelectedItems = new ObservableCollection<object>()
            {
                Items[1], Items[3]
            };

            _collectionView = collectionView;

            /* You can comment out the above SelectedItems assignment and un-comment this and it still doesn't preselect.
             * It only works when it's triggered from the PreSelectUsingMethod command.
             * 
            var preSelectedList = new ObservableCollection<object> { Items[1], Items[3] };
            _collectionView.UpdateSelectedItems(preSelectedList);
            SelectedItems = preSelectedList;
            */
        }

        public ObservableCollection<string> Items { get; set; }

        public ObservableCollection<object> SelectedItems
        {
            get
            {
                return selectedItems;
            }
            set
            {
                if (selectedItems != value)
                {
                    selectedItems = value;
                }

                // If you directly set SelectedItems, you need to invoke the property changed event.
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedItems)));
            }
        }
        ObservableCollection<object> selectedItems;

        public event PropertyChangedEventHandler PropertyChanged;

        [RelayCommand]
        Task PreSelectUsingBinding()
        {
            SelectedItems = new ObservableCollection<object> { Items[1], Items[3] };

            return Task.CompletedTask;
        }

        [RelayCommand]
        Task ClearUsingBinding()
        {
            SelectedItems = null;

            return Task.CompletedTask;
        }

        [RelayCommand]
        Task PreSelectUsingMethod()
        {
            var preSelectedList = new ObservableCollection<object> { Items[1], Items[3] };
            _collectionView.UpdateSelectedItems(preSelectedList);
            SelectedItems = preSelectedList;

            return Task.CompletedTask;
        }
    }
}
