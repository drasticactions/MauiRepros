using Microsoft.Maui;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.ViewModels
{
    public class SimpleListViewModel : BaseViewModel
    {
        public SimpleListViewModel()
        {
            AddItems();
        }

        private ObservableCollection<SimpleListItemViewModel> _items;

        public ObservableCollection<SimpleListItemViewModel> Items
        {
            get { return _items; }
            set { _items = value; }
        }


        private void AddItems()
        {
            List<SimpleListItemViewModel> items = new List<SimpleListItemViewModel>();

            items.Add(new SimpleListItemViewModel { Title = "Ana" , Image="button_add_application.png"});
            items.Add(new SimpleListItemViewModel { Title = "Ana PRODCUCT", Image = "button_add_product2.png" });
            items.Add(new SimpleListItemViewModel { Title = "Ana VOICE", Image = "button_add_voice2.png" });
            items.Add(new SimpleListItemViewModel { Title = "Ana", Image = "button_add_application2.png" });
            items.Add(new SimpleListItemViewModel { Title = "Ana", Image = "button_add_application2.png" });
            items.Add(new SimpleListItemViewModel { Title = "Ana" , Image = "button_add_application2.png" });
            items.Add(new SimpleListItemViewModel { Title = "Ana", Image = "button_add_application.png" });
            items.Add(new SimpleListItemViewModel { Title = "Ana" , Image = "button_add_application.png" });
            items.Add(new SimpleListItemViewModel { Title = "Ana" , Image = "button_add_application.png" });
            items.Add(new SimpleListItemViewModel { Title = "Ana" , Image = "button_add_application.png" });
            items.Add(new SimpleListItemViewModel { Title = "Ana 34", Image = "button_add_product.png" });


            items.Add(new SimpleListItemViewModel { Title = "Ana are few swef dsa dsa as as as as si da sdas asd asd as as a as si are si mere multe rosiii YEEEE Ana are few swef dsa dsa as as as as si da sdas asd asd as as a as si are si mere multe rosiii YEEEE Ana are few swef dsa dsa as as as as si da sdas asd asd as as a as si are si mere multe rosiii YEEEE Ana are few swef dsa dsa as as as as si da sdas asd asd as as a as si are si mere multe rosiii YEEEE Ana are few swef dsa dsa as as as as si da sdas asd asd as as a as si are si mere multe rosiii YEEEE",
                Image = "button_add_application.png" });
            items.Add(new SimpleListItemViewModel { Title = "Ana" , Image = "button_add_voice.png" });
            items.Add(new SimpleListItemViewModel { Title = "Ana" , Image = "button_add_product.png" });
            items.Add(new SimpleListItemViewModel { Title = "Ana" , Image = "button_add_product.png" });
            items.Add(new SimpleListItemViewModel { Title = "Ana" , Image = "button_add_product.png" });
            items.Add(new SimpleListItemViewModel { Title = "Ana" , Image = "button_add_voice.png" });
            items.Add(new SimpleListItemViewModel { Title = "Ana" , Image = "button_add_application.png" });
            items.Add(new SimpleListItemViewModel { Title = "Ana" , Image = "button_add_application.png" });
            items.Add(new SimpleListItemViewModel { Title = "Ana" , Image = "button_add_application.png" });
            items.Add(new SimpleListItemViewModel { Title = "Ana" , Image = "button_add_application.png" });
            items.Add(new SimpleListItemViewModel { Title = "Ana 40" , Image = "button_add_voice.png" });

            Items = new ObservableCollection<SimpleListItemViewModel>(items);
        }
    }
}
