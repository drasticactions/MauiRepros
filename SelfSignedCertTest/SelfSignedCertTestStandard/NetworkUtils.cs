using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace SelfSignedCertTestStandard;

public static class NetworkUtils {
    public static IEnumerable<string> DeviceIps ()
    {
        return GoodInterfaces ()
            .SelectMany (x =>
                x.GetIPProperties ().UnicastAddresses
                    .Where (y => y.Address.AddressFamily == AddressFamily.InterNetwork)
                    .Select (y => y.Address.ToString ())).Union(new[] { "127.0.0.1" }).OrderBy(x=> x);
    }

    public static IEnumerable<NetworkInterface> GoodInterfaces ()
    {
        var allInterfaces = NetworkInterface.GetAllNetworkInterfaces ();
        return allInterfaces.Where (x => x.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                                         !x.Name.StartsWith ("pdp_ip", StringComparison.Ordinal) &&
                                         x.OperationalStatus == OperationalStatus.Up);
    }
}