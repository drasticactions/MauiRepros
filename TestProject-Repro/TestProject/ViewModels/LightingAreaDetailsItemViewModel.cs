namespace TestProject.ViewModels;

public class LightingAreaDetailsItemViewModel : GenericDetailsItemViewModel
{
    public LightingAreaDetailsItemViewModel()
    {
        Init();
    }

    public int Index { get; set; }

    public int? ControlPointCount { get; set; }

    public int LightingPointCount { get; set; }

    public int LightingSpotCount { get; set; }

    public void Init()
    {
        Index = 45;
        ControlPointCount = new Random().Next(0, 5);
        LightingPointCount = new Random().Next(0, 5);
        LightingSpotCount = new Random().Next(0, 5);

      Label = "Unknown tag";

      SecondLabel = LightingPointCount switch
      {
          0 => null,
          1 => $"{LightingPointCount} spot",
          _ => $"{LightingPointCount} spots"
      };

      ThirdLabel = LightingSpotCount switch
      {
          0 => null,
          1 => $"{LightingSpotCount} spot",
          _ => $"{LightingSpotCount} spots"
      };

      FourthLabel = "A label";
      FifthLabel = ControlPointCount switch
      {
          null => null,
          0 => null,
          1 => $"{ControlPointCount} point",
          _ => $"{ControlPointCount} points"
      };
    }
}