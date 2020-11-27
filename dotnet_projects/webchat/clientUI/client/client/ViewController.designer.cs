// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace client
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSButton disconnect { get; set; }

		[Outlet]
		AppKit.NSTextField msgbox { get; set; }

		[Outlet]
		AppKit.NSButton sendmsg { get; set; }

		[Outlet]
		AppKit.NSTextField username { get; set; }

		[Action ("connect:")]
		partial void connect (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (username != null) {
				username.Dispose ();
				username = null;
			}

			if (disconnect != null) {
				disconnect.Dispose ();
				disconnect = null;
			}

			if (msgbox != null) {
				msgbox.Dispose ();
				msgbox = null;
			}

			if (sendmsg != null) {
				sendmsg.Dispose ();
				sendmsg = null;
			}
		}
	}
}
