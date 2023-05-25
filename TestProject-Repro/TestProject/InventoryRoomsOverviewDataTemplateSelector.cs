using TestProject.ViewModels;

namespace TestProject;

public class InventoryRoomsOverviewDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate GenericSectionDataTemplate { get; set; }

        public DataTemplate GenericDetailsWithQuantityDataTemplate { get; set; }

        public DataTemplate InventoryPlateRockerImagesDetailsDataTemplate { get; set; }

        public DataTemplate GenericQuantityDataTemplate { get; set; }

        public DataTemplate InventoryDetailsDataTemplate { get; set; }

        public DataTemplate InventorySwipeToDeleteDataTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return item switch
            {
                GenericSectionItemViewModel _ => GenericSectionDataTemplate,
                GenericDetailsWithQuantityItemViewModel _ => GenericDetailsWithQuantityDataTemplate,
                GenericImageDetailsItemViewModel _ => InventoryPlateRockerImagesDetailsDataTemplate,
                GenericQuantityItemViewModel _ => GenericQuantityDataTemplate,
                WiringAccessoriesHeatingPowerItemViewModel _ => InventorySwipeToDeleteDataTemplate,
                LightingAreaDetailsItemViewModel _ => InventorySwipeToDeleteDataTemplate,
                GenericDetailsItemViewModel _ => InventoryDetailsDataTemplate,
                _ => GenericSectionDataTemplate
            };
        }
    }