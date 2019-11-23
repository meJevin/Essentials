﻿using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Path = System.IO.Path;

namespace Xamarin.Essentials
{
    public static partial class Screenshot
    {
        static bool PlatformCanCapture => !Platform.WindowManager.DefaultDisplay.Flags.HasFlag(DisplayFlags.Secure);

        static string GetTempFileName() => Path.Combine(FileSystem.CacheDirectory, Path.GetTempFileName());

        static async Task<ScreenshotFile> PlatformCaptureAsync()
        {
            var path = GetTempFileName();
            var view = Platform.GetCurrentActivity(false).Window.DecorView.RootView;
            using (var bitmap = Bitmap.CreateBitmap(view.Width, view.Height, Bitmap.Config.Argb8888))
            {
                var canvas = new Canvas(bitmap);
                var drawable = view.Background;
                if (drawable != null)
                {
                    drawable.Draw(canvas);
                }
                else
                {
                    canvas.DrawColor(Color.White);
                }

                view.Draw(canvas);

                using (var stream = File.Create(path))
                {
                    var success = await bitmap.CompressAsync(Bitmap.CompressFormat.Png, 100, stream);
                    if (!success)
                        throw new AndroidException("Failure to compress bitmap to file!");
                }
            }

            return new ScreenshotFile(path);
        }
    }
}
