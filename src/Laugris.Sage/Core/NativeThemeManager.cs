using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Laugris.Sage
{
    public enum NativeElementState
    {
        Normal,
        Focused,
        Pressed,
        Disabled
    }

    public sealed class NativeThemeManager
    {
        private NativeThemeManager()
        {
        }

        public static void MakeSound(string soundName)
        {
            if (GlobalSettings.UseSound)
                NativeMethods.MakeSoundFromResource(FastBitmap.ThemeLibrary, soundName);
        }

        public static void LoadThemeLibrary(string fileName)
        {
            FastBitmap.LoadThemeLibrary(fileName);
        }

        public static Image Load(string resourceName)
        {
            try
            {
                return FastBitmap.FromNativeResource(resourceName);
            }
            catch (Exception ex)
            {
                throw new ThemeException("Resource " + resourceName + " was not found", ex);
            }
        }

        public static Image Load(string resourceName, int width, int height)
        {
            Bitmap temp = null;
            try
            {
                temp = FastBitmap.FromNativeResource(resourceName);
            }
            catch
            {
                temp = null;
            }

            if (temp != null)
            {
                return BitmapPainter.ResizeBitmap(temp, width, height, true);
            }
            else
                return null;
        }

        public static Bitmap LoadElement(string resourceName, NativeElementState state)
        {
            try
            {
                string stateName;
                switch (state)
                {
                    case NativeElementState.Normal: stateName = "Normal";
                        break;
                    case NativeElementState.Focused: stateName = "Focused";
                        break;
                    case NativeElementState.Pressed: stateName = "Pressed";
                        break;
                    case NativeElementState.Disabled: stateName = "Disabled";
                        break;
                    default:
                        stateName = "Normal";
                        break;
                }

                return FastBitmap.FromNativeResource(resourceName + stateName + ".png");
            }
            catch (Exception ex)
            {
                throw new ThemeException("Resource " + resourceName + " was not found", ex);
            }
        }


        public static Bitmap LoadBitmap(string resourceName)
        {
            try
            {
                return FastBitmap.FromNativeResource(resourceName);
            }
            catch (Exception ex)
            {
                throw new ThemeException("Resource " + resourceName + " was not found", ex);
            }
        }

    }
}
