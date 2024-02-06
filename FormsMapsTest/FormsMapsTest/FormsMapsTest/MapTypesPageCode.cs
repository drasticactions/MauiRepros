using Xamarin.Forms;

namespace FormsMapsTest
{
    public class MapTypesPageCode : ContentPage
    {
        public MapTypesPageCode()
        {
            Xamarin.Forms.Maps.Map map = new Xamarin.Forms.Maps.Map();
            Content = map;
        }
    }
}