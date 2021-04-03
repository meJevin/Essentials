using System.Collections.Generic;
using System.Threading.Tasks;
using AppKit;
using Foundation;

namespace Xamarin.Essentials
{
    public static partial class Clipboard
    {
        static readonly string pasteboardStringType = NSPasteboard.NSPasteboardTypeString;
        static readonly string pasteboardPNGType = NSPasteboard.NSPasteboardTypePNG;

        static readonly string[] pasteboardTypes = { pasteboardStringType, pasteboardPNGType };

        static NSPasteboard Pasteboard => NSPasteboard.GeneralPasteboard;

        static Task PlatformSetImageAsync(string b64Img)
        {
            Pasteboard.DeclareTypes(pasteboardTypes, null);
            Pasteboard.ClearContents();
            Pasteboard.SetDataForType(new NSData(b64Img, NSDataBase64DecodingOptions.None), pasteboardPNGType);
            return Task.CompletedTask;
        }

        static Task PlatformSetTextAsync(string text)
        {
            Pasteboard.DeclareTypes(pasteboardTypes, null);
            Pasteboard.ClearContents();
            Pasteboard.SetStringForType(text, pasteboardStringType);

            return Task.CompletedTask;
        }

        static bool PlatformHasText =>
            !string.IsNullOrEmpty(GetPasteboardText());

        static Task<string> PlatformGetTextAsync()
            => Task.FromResult(GetPasteboardText());

        static string GetPasteboardText()
            => Pasteboard.ReadObjectsForClasses(
                new ObjCRuntime.Class[] { new ObjCRuntime.Class(typeof(NSString)) },
                null)?[0]?.ToString();

        static void StartClipboardListeners()
            => throw ExceptionUtils.NotSupportedOrImplementedException;

        static void StopClipboardListeners()
            => throw ExceptionUtils.NotSupportedOrImplementedException;
    }
}
