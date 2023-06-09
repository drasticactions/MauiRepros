using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace BugCollectionViewScrollDemo
{
	public class MainPageViewModel : INotifyPropertyChanged
	{
        private ObservableCollection<ListContent> _demoList = new ObservableCollection<ListContent>();
        public ObservableCollection<ListContent> DemoList
        {
            get { return _demoList; }
            set
            {
                _demoList = value;
                OnPropertyChanged();
            }
        }

        public MainPageViewModel()
        {
            LoadCollection();
        }

        private void LoadCollection()
        {
            _demoList.Add(new ListContent { DemoDate = "01/22/2023", DemoName = "Item 1", DemoNumber = "00000001" });
            _demoList.Add(new ListContent { DemoDate = "01/21/2023", DemoName = "Item 2", DemoNumber = "00000002" });
            _demoList.Add(new ListContent { DemoDate = "01/20/2023", DemoName = "Item 3", DemoNumber = "00000003" });
            _demoList.Add(new ListContent { DemoDate = "01/19/2023", DemoName = "Item 4", DemoNumber = "00000004" });
            _demoList.Add(new ListContent { DemoDate = "01/18/2023", DemoName = "Item 5", DemoNumber = "00000005" });
            _demoList.Add(new ListContent { DemoDate = "01/17/2023", DemoName = "Item 6", DemoNumber = "00000006" });
            _demoList.Add(new ListContent { DemoDate = "01/16/2023", DemoName = "Item 7", DemoNumber = "00000007" });
            _demoList.Add(new ListContent { DemoDate = "01/15/2023", DemoName = "Item 8", DemoNumber = "00000008" });
            _demoList.Add(new ListContent { DemoDate = "01/14/2023", DemoName = "Item 9", DemoNumber = "00000009" });
            _demoList.Add(new ListContent { DemoDate = "01/13/2023", DemoName = "Item 10", DemoNumber = "00000010" });
            _demoList.Add(new ListContent { DemoDate = "01/12/2023", DemoName = "Item 11", DemoNumber = "00000011" });
            _demoList.Add(new ListContent { DemoDate = "01/11/2023", DemoName = "Item 12", DemoNumber = "00000012" });
            _demoList.Add(new ListContent { DemoDate = "01/10/2023", DemoName = "Item 13", DemoNumber = "00000013" });
            _demoList.Add(new ListContent { DemoDate = "01/09/2023", DemoName = "Item 14", DemoNumber = "00000014" });
            _demoList.Add(new ListContent { DemoDate = "01/08/2023", DemoName = "Item 15", DemoNumber = "00000015" });
            _demoList.Add(new ListContent { DemoDate = "01/07/2023", DemoName = "Item 16", DemoNumber = "00000016" });
            _demoList.Add(new ListContent { DemoDate = "01/06/2023", DemoName = "Item 17", DemoNumber = "00000017" });
            _demoList.Add(new ListContent { DemoDate = "01/05/2023", DemoName = "Item 18", DemoNumber = "00000018" });
            _demoList.Add(new ListContent { DemoDate = "01/04/2023", DemoName = "Item 19", DemoNumber = "00000019" });
            _demoList.Add(new ListContent { DemoDate = "01/03/2023", DemoName = "Item 20", DemoNumber = "00000020" });
            _demoList.Add(new ListContent { DemoDate = "01/02/2023", DemoName = "Item 21", DemoNumber = "00000021" });
            _demoList.Add(new ListContent { DemoDate = "01/01/2023", DemoName = "Item 22", DemoNumber = "00000022" });
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public class ListContent : BaseModel
    {
        public string DemoDate { get; set; }
        public string DemoName { get; set; }
        public string DemoNumber { get; set; }
    }

    public class BaseModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
