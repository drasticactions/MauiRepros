using System;
using ServiceReference1;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CoreWCFApple
{
    class ProductsViewModel : INotifyPropertyChanged
    {
        readonly IList<ProductModel> source;

        public ObservableCollection<ProductModel> ProductsModel { get; private set; }

        public ProductsViewModel()
        {
            source = new List<ProductModel>();
            CreateMonkeyCollection();
        }

        void CreateMonkeyCollection()
        {


            try
            {
                var sc = new ServiceReference1.ServiceClient();
                List<Product> methodResult = sc.GetListOfProductAsync().Result.Products.ToList<Product>();

                ObservableCollection<Product> products = new ObservableCollection<Product>(methodResult);
                foreach (var p in products)
                {
                    source.Add(new ProductModel() { ProductImageString = p.ProductImageString });
                }
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
            }

            ProductsModel = new ObservableCollection<ProductModel>(source);
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    class ProductModel
    {
        public string ProductImageString { get; set; }
    }
}

