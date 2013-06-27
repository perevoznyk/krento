//===============================================================================
// Copyright © Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================


using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Laugris.Sage
{

    public struct GlassMargins
    {
        private int leftWidth;
        private int rightWidth;
        private int bottomHeight;
        private int topHeight;

        public int LeftWidth
        {
            get { return leftWidth; }
            set { leftWidth = value; }
        }

        public int RightWidth
        {
            get { return rightWidth; }
            set { rightWidth = value; }
        }

        public int TopHeight
        {
            get { return topHeight; }
            set { topHeight = value; }
        }

        public int BottomHeight
        {
            get { return bottomHeight; }
            set { bottomHeight = value; }
        }

        public void SheetOfGlass()
        {
            this.LeftWidth = -1;
            this.RightWidth = -1;
            this.TopHeight = -1;
            this.BottomHeight = -1;
        }

        public void None()
        {
            this.LeftWidth = 0;
            this.RightWidth = 0;
            this.TopHeight = 0;
            this.BottomHeight = 0;
        }
    }

    public static class GlassHelper
    {


        public static bool GlassAvailable
        {
            get
            {
                Version vOs = Environment.OSVersion.Version;
                // DwmApi is safe to use with this library on Windows Vista build 5384 and later only.
                bool fRes = ((vOs.Major >= 6) && (vOs.Minor >= 0) && (vOs.Build >= 5384));
                return fRes;
            }
        }

        internal static bool IsCompositionEnabled()
        {
            bool enabled = false;
            if (!GlassAvailable)
                return false;
            try
            {
                int res = NativeMethods.DwmIsCompositionEnabled(ref enabled);
                if (res != 0)
                    enabled = false;
            }
            catch (DllNotFoundException)
            {
                TraceDebug.Trace("DWMAPI DLL not found");
                return false;
            }
            return enabled;
        }

        internal static void SetCompositionEnabled(bool value)
        {
            if (!GlassAvailable)
                return;
            try
            {
                NativeMethods.DwmEnableComposition(value);
            }
            catch (DllNotFoundException)
            {
                TraceDebug.Trace("DWMAPI DLL not found");
            }
        }

        public static bool CompositionEnabled
        {
            get { return IsCompositionEnabled(); }
            set { SetCompositionEnabled(value); }
        }


        public static bool Capable
        {
            get
            {
                bool fCapable1 = false;
                bool fCapable2 = false;

                if (!GlassAvailable)
                    return false;

                try
                {
                    NativeMethods.DwmpIsCompositionCapable(ref fCapable1, ref fCapable2);
                }
                catch (DllNotFoundException)
                {
                    TraceDebug.Trace("DWMAPI DLL not found");
                    return false;
                }
                return fCapable2;
            }
        }

        public static void Extend(Form form, GlassMargins margins)
        {
            if (form == null)
                return;

            if (!GlassAvailable)
                return;


            try
            {
                NativeMethods.MARGINS LMargins = new NativeMethods.MARGINS();
                LMargins.cxLeftWidth = margins.LeftWidth;
                LMargins.cxRightWidth = margins.RightWidth;
                LMargins.cyBottomHeight = margins.BottomHeight;
                LMargins.cyTopHeight = margins.TopHeight;

                NativeMethods.DwmExtendFrameIntoClientArea(form.Handle, ref LMargins);
            }
            catch (DllNotFoundException)
            {
                TraceDebug.Trace("DWMAPI DLL not found");
            }
        }
    }
}
