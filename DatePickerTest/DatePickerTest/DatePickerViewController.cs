namespace DatePickerTest;

public class DatePickerViewController : UIViewController
{
    public DatePickerViewController(CGRect rect)
    {
        var maxDate = DateTime.Parse("2020-02-05").ToNSDate();
        var minDate = DateTime.Parse("2020-02-01").ToNSDate();
        var date = DateTime.Parse("2020-02-03").ToNSDate();
        System.Diagnostics.Debug.WriteLine(maxDate);
        System.Diagnostics.Debug.WriteLine(minDate);
        System.Diagnostics.Debug.WriteLine(date);
        View!.AddSubview (new UIDatePicker (new CGRect(100, 100, 100, 100)) {
            BackgroundColor = UIColor.SystemBackground,
            AutoresizingMask = UIViewAutoresizing.All,
            Date = date,
            MaximumDate = maxDate,
            MinimumDate = minDate,
        });
    }
}

// From https://github.com/dotnet/maui/blob/71e6bb5610dfd000cf63f48f03b259ae8dce51cd/src/Core/src/Platform/iOS/DateExtensions.cs#L15
public static class DateExtensions
{
    internal static DateTime ReferenceDate = new DateTime(2001, 1, 1, 0, 0, 0);

    public static DateTime ToDateTime(this NSDate date)
    {
        return ReferenceDate.AddSeconds(date.SecondsSinceReferenceDate);
    }

    public static NSDate ToNSDate(this DateTime date)
    {
        return NSDate.FromTimeIntervalSinceReferenceDate((date - ReferenceDate).TotalSeconds);
    }
}