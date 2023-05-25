namespace TestProject.ViewModels;

public class WiringAccessoriesHeatingPowerItemViewModel : HeatingPowerItemViewModel
{
    public static readonly int DefaultUnderfloorPowerLevel = 1000;

    public static readonly int DefaultConvectorPowerLevel = 500;

    /// <summary>
    /// Gets or sets index.
    /// </summary>
    public int Index { get; set; }
}