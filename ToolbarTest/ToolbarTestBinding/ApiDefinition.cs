using AppKit;
using UIKit;
using Foundation;
using ObjCRuntime;

namespace AppKit {

	[BaseType (typeof (NSToolbarItem))]
	[DisableDefaultCtor]
	interface NSUIViewToolbarItem {
		[DesignatedInitializer]
		[Export ("initWithItemIdentifier:uiView:")]
		NativeHandle Constructor (NSString identifier, UIView uiView);

		[Export ("uiView", ArgumentSemantic.Strong)]
		UIView UIView { get; [Bind ("setUIView:")] set; }
	}

}
