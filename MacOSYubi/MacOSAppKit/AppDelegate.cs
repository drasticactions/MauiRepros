using Yubico.YubiKey;

namespace MacOSAppKit;

[Register ("AppDelegate")]
public class AppDelegate : NSApplicationDelegate {
	public override void DidFinishLaunching (NSNotification notification)
	{
        // Insert code here to initialize your application

        IYubiKeyDevice yubiKey = YubiKeyDevice.FindAll().FirstOrDefault();
    }

	public override void WillTerminate (NSNotification notification)
	{
		// Insert code here to tear down your application
	}
}
