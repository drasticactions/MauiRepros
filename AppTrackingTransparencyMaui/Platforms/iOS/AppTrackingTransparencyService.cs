using AppTrackingTransparency;

namespace AppTrackingTransparencyMaui;

public class AppTrackingTransparencyService
{
    protected Func<IEnumerable<string>> RequiredInfoPlistKeys
        => () => new string[] { "NSUserTrackingUsageDescription" };

    public async Task<PermissionStatus> RequestAsync()
    {
        var result = await ATTrackingManager.RequestTrackingAuthorizationAsync();
        return ConvertStatus(result);
    }

    public Task<PermissionStatus> CheckStatusAsync()
    {
        return Task.FromResult(
            ConvertStatus(ATTrackingManager.TrackingAuthorizationStatus));
    }

    private PermissionStatus ConvertStatus(
        ATTrackingManagerAuthorizationStatus status)
    {
        switch (status)
        {
            case ATTrackingManagerAuthorizationStatus.NotDetermined:
                return PermissionStatus.Disabled;
            case ATTrackingManagerAuthorizationStatus.Restricted:
                return PermissionStatus.Restricted;
            case ATTrackingManagerAuthorizationStatus.Denied:
                return PermissionStatus.Denied;
            case ATTrackingManagerAuthorizationStatus.Authorized:
                return PermissionStatus.Granted;
            default:
                return PermissionStatus.Unknown;
        }
    }
}