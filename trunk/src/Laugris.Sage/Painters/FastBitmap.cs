using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Laugris.Sage
{

    public sealed class FastBitmap
    {
        private static IntPtr gdipToken = IntPtr.Zero;

        private static IntPtr themeLibrary;

        public static IntPtr ThemeLibrary { get { return themeLibrary; } }

        public static IntPtr LoadThemeLibrary(string fileName)
        {
            themeLibrary = NativeMethods.LoadLibraryEx(fileName, LoadLibraryExFlags.LOAD_LIBRARY_AS_DATAFILE);
            return themeLibrary;
        }

        private FastBitmap()
        {
        }


        static FastBitmap()
        {
            if (gdipToken == IntPtr.Zero)
            {
                StartupInput input = StartupInput.GetDefaultStartupInput();
                StartupOutput output;

                int status = NativeMethods.GdiplusStartup(out gdipToken, ref input, out output);
                if (status == 0)
                    AppDomain.CurrentDomain.ProcessExit += new EventHandler(Cleanup_Gdiplus);
            }
        }

        private static void Cleanup_Gdiplus(object sender, EventArgs e)
        {
            if (gdipToken != IntPtr.Zero)
                NativeMethods.GdiplusShutdown(gdipToken);

        }

        public static Bitmap FromNativeResource(IntPtr handle, string resource)
        {
            if (string.IsNullOrEmpty(resource) || handle == IntPtr.Zero)
                return null;

            Bitmap result;
            int w = 0;
            int h = 0;
            IntPtr ppvBits;
            IntPtr hBitmap = NativeMethods.LoadPNGResource(handle, resource, ref w, ref h, out ppvBits);
            if (hBitmap != IntPtr.Zero)
            {
                try
                {
#if PRESSURE
                    long pressure = InteropHelper.AlignToPage(w * h * 4);
                    if (pressure != 0L)
                    {
                        GC.AddMemoryPressure(pressure);
                    }
#endif

                    result = new Bitmap(w, h, PixelFormat.Format32bppArgb);
                    try
                    {
                        BitmapData data = result.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                        NativeMethods.RtlMoveMemory(data.Scan0, ppvBits, h * data.Stride); // copies the bitmap
                        result.UnlockBits(data);
                    }
                    catch (Exception ex)
                    {
                        TraceDebug.Trace("FromNativeResource: " + ex.Message + " Resource name " + resource);
                        result = null;
                    }
                }
                finally
                {
                    NativeMethods.DeleteObject(hBitmap);
#if PRESSURE
                    long pressure = InteropHelper.AlignToPage(w * h * 4);
                    if (pressure != 0L)
                    {
                        GC.RemoveMemoryPressure(pressure);
                    }
#endif

                }
                return result;
            }
            else
                return null;
        }

        public static Bitmap FromNativeResource(string resource)
        {
            return FromNativeResource(themeLibrary, resource);
        }

        public static Bitmap FromFile(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return null;

            if (FileOperations.FileExists(filename))
            {
                if (FileOperations.FileExtensionIs(filename, ".png"))
                {
                    int w = 0;
                    int h = 0;
                    IntPtr ppvBits;
                    IntPtr hBitmap = NativeMethods.LoadBitmapPNG(filename, ref w, ref h, out ppvBits);
                    if (hBitmap != IntPtr.Zero)
                    {
                        Bitmap result = new Bitmap(w, h, PixelFormat.Format32bppArgb);
                        BitmapData data = result.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                        NativeMethods.RtlMoveMemory(data.Scan0, ppvBits, h * data.Stride); // copies the bitmap
                        result.UnlockBits(data);
                        NativeMethods.DeleteObject(hBitmap);
                        result = BitmapPainter.ConvertTo32Bit(result, true);
                        return result;
                    }
                }

                if (FileOperations.FileExtensionIs(filename, ".jpg") || FileOperations.FileExtensionIs(filename, ".jpeg"))
                {
                    int w = 0;
                    int h = 0;
                    IntPtr ppvBits;
                    IntPtr hBitmap = NativeMethods.LoadBitmapJPG(filename, ref w, ref h, out ppvBits);
                    if (hBitmap != IntPtr.Zero)
                    {
                        Bitmap result = new Bitmap(w, h, PixelFormat.Format24bppRgb);
                        BitmapData data = result.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                        NativeMethods.RtlMoveMemory(data.Scan0, ppvBits, h * data.Stride); // copies the bitmap
                        result.UnlockBits(data);
                        NativeMethods.DeleteObject(hBitmap);
                        Bitmap realResult = BitmapPainter.ConvertTo32Bit(result, true);
                        return realResult;
                    }
                }

                if (FileOperations.FileExtensionIs(filename, ".ico"))
                {
                    Icon icon = new Icon(FileOperations.StripFileName(filename));
                    Bitmap result = icon.ToBitmap();
                    icon.Dispose();
                    return result;
                }

                Bitmap image = null;
                IntPtr zero = IntPtr.Zero;
                int imageType;

                if (NativeMethods.GdipLoadImageFromFile(FileOperations.StripFileName(filename), out zero) != 0)
                    return null;

                if (NativeMethods.GdipGetImageType(zero, out imageType) != 0)
                {
                    return null;
                }

                if (imageType == 1)
                {
                     image = (Bitmap)typeof(System.Drawing.Bitmap).InvokeMember("FromGDIplus", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod, null, null, new object[] { zero });
                }

                if (!NativeMethods.IsAnimatedGIF(filename))
                    image = BitmapPainter.ConvertTo32Bit(image, true);

                return image;

            }
            else
                return null;
        }
    }
}
