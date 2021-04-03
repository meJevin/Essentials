using System;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Provider;
using Android.Util;
using static Android.Content.ClipboardManager;

namespace Xamarin.Essentials
{
    public static partial class Clipboard
    {
        static readonly Lazy<ClipboardChangeListener> clipboardListener
            = new Lazy<ClipboardChangeListener>(() => new ClipboardChangeListener());

        static Task PlatformSetImageAsync(string b64Img)
        {
            var imageAsBytes = Base64.Decode(b64Img, Base64Flags.Default);
            var bmp = BitmapFactory.DecodeByteArray(imageAsBytes, 0, imageAsBytes.Length);

            var sdCardPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var filePath = System.IO.Path.Combine(sdCardPath, "temp-xe-clipboard-img.png");

            using var stream = new FileStream(filePath, FileMode.Create);
            bmp.Compress(Bitmap.CompressFormat.Png, 100, stream);
            stream.Close();

            var contentValues = new ContentValues(2);
            contentValues.Put("mime_type", "image/png");
            contentValues.Put("_data", filePath);

            var uri = Application.Context.ContentResolver.Insert(MediaStore.Images.Media.ExternalContentUri, contentValues);

            Platform.ClipboardManager.PrimaryClip = ClipData.NewUri(Application.Context.ContentResolver, "XamarinImageToClipboard", uri);

            return Task.CompletedTask;
        }

        static Task PlatformSetTextAsync(string text)
        {
            Platform.ClipboardManager.PrimaryClip = ClipData.NewPlainText("Text", text);
            return Task.CompletedTask;
        }

        static bool PlatformHasText
            => Platform.ClipboardManager.HasPrimaryClip && !string.IsNullOrEmpty(Platform.ClipboardManager.PrimaryClip?.GetItemAt(0)?.Text);

        static Task<string> PlatformGetTextAsync()
            => Task.FromResult(Platform.ClipboardManager.PrimaryClip?.GetItemAt(0)?.Text);

        static void StartClipboardListeners()
            => Platform.ClipboardManager.AddPrimaryClipChangedListener(clipboardListener.Value);

        static void StopClipboardListeners()
            => Platform.ClipboardManager.RemovePrimaryClipChangedListener(clipboardListener.Value);
    }

    class ClipboardChangeListener : Java.Lang.Object, IOnPrimaryClipChangedListener
    {
        void IOnPrimaryClipChangedListener.OnPrimaryClipChanged() =>
            Clipboard.ClipboardChangedInternal();
    }
}
