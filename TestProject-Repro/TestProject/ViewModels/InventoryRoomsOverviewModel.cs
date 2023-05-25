using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TestProject.ViewModels;

/// <summary>
/// InventoryRoomsOverviewViewModel.
/// </summary>
public class InventoryRoomsOverviewViewModel : BaseViewModel
{
    public ObservableCollection<INotifyPropertyChanged> Items { get; private set; } =
        new ObservableCollection<INotifyPropertyChanged>();

    private Random _random = new Random();

    public InventoryRoomsOverviewViewModel()
    {
        AddItems();
    }

    private void AddItems()
    {
        AddWiringAccessoriesRange();

        Items.Add(new GenericSectionItemViewModel { Title = "Heating power item" });

        for (var i=0; i<5; i++)
        {
            Items.Add(new WiringAccessoriesHeatingPowerItemViewModel
            {
                Index = _random.Next(i, 100),
                PowerLevel = _random.Next(i, 1500),
                Label = "Label",
                ThirdLabel = "ThirdLabel",
            });
        }

        Items.Add(new GenericSectionItemViewModel { Title = "Generic Details With Quantity" });

        for (var i=0; i<3; i++)
        {
            Items.Add(new GenericDetailsWithQuantityItemViewModel
            {
                Quantity = _random.Next(i, 100),
                Label = "Label",
                SecondLabel = "SecondLabel",
                 ThirdLabel = "ThirdLabel",
                 FourthLabel = "Very veryy loooooooooooooong loooooooooooooong loooooooooooooong ForthLabel"
            });
        }

        Items.Add(new GenericSectionItemViewModel { Title = "PlateRocker:" });

        for (var i=0; i<2; i++)
        {
            Items.Add(new GenericImageDetailsItemViewModel
            {
                SecondLabel = "SecondLabel" + i,
            });
        }

        Items.Add(new GenericSectionItemViewModel { Title = "Quantity:" });

        for (var i=0; i<2; i++)
        {
            Items.Add(new GenericQuantityItemViewModel
            {
                Label = "Label" + i,
                Quantity = _random.Next(1, 100),
                IsInformationVisible = false,
                MaximumQuantity = 101,
                MinimumQuantity = 0,
            });
        }

        Items.Add(new GenericSectionItemViewModel { Title = "Lighthing Area:" });

        for (var i=0; i<5; i++)
        {
            Items.Add(new LightingAreaDetailsItemViewModel());
        }

        Items.Add(new GenericSectionItemViewModel { Title = "Generic Details:" });

        for (var i=0; i<1; i++)
        {
            Items.Add(new GenericDetailsItemViewModel
            {
                Label = "Label",
                SecondLabel = "SecondLabel",
                ThirdLabel = "ThirdLabel",
                FourthLabel = "Very veryy loooooooooooooong loooooooooooooong loooooooooooooong ForthLabel"
            });
        }
    }
    
    protected void AddWiringAccessoriesRange()
    {
        Items.Add(new GenericImageDetailsItemViewModel
        {
            FirstLabel = "First GenericImageDetailsItemViewModel FirstLabel",
            SecondLabel = "First GenericImageDetailsItemViewModel SecondLabel",
            FirstImageFilePath = "dotnet_bot.png",
            SecondImageFilePath = "dotnet_bot.png",
            HasInfoSign = true,
        });
        Items.Add(new GenericImageDetailsItemViewModel
        {
            FirstLabel = "Second GenericImageDetailsItemViewModel FirstLabel",
            SecondLabel = "Second GenericImageDetailsItemViewModel SecondLabel",
            FirstImageFilePath = "dotnet_bot.png",
            SecondImageFilePath = "dotnet_bot.png",
            HasInfoSign = false,
        });
    }
}