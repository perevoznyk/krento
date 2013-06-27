using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Laugris.Sage
{
    public partial class HotKeyControl : Control
    {
        private const int HOTKEYF_SHIFT = 0x01;
        private const int HOTKEYF_CONTROL = 0x02;
        private const int HOTKEYF_ALT = 0x04;
        private const int WM_KEYUP = 0x0101;
        private const int WM_USER = 0x0400;
        private const int HKM_SETHOTKEY = WM_USER + 1;
        private const int HKM_GETHOTKEY = WM_USER + 2;
        private const int HKM_SETRULES = WM_USER + 3;
        private const int EN_CHANGE = 0x0300;
        private const int WM_COMMAND = 0x0111;

        public HotKeyControl()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, false);

        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(100, 25);
            }
        }

        protected override CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                CreateParams params1 = base.CreateParams;
                params1.ClassName = "msctls_hotkey32";
                return params1;
            }
        }

        private short GetCurrentHotKey()
        {
            return (short)(NativeMethods.SendMessage(new HandleRef(this, this.Handle), HKM_GETHOTKEY, IntPtr.Zero, IntPtr.Zero));
        }

        protected override void CreateHandle()
        {
            if (!base.RecreatingHandle)
            {
                INITCOMMONCONTROLSEX initcommoncontrolsex1 = new INITCOMMONCONTROLSEX();
                initcommoncontrolsex1.dwICC = 0x00000040;//ICC_HOTKEY_CLASS      
                NativeMethods.InitCommonControlsEx(initcommoncontrolsex1);
            }
            base.CreateHandle();
        }

        public Keys HotKey
        {
            get
            {
                short key_mod = this.GetCurrentHotKey();
                return (Keys)(key_mod & 0xff);
            }
        }
        public Keys HotModifierKeys
        {
            get
            {
                short key_mod = this.GetCurrentHotKey();
                byte modifier = (byte)(key_mod >> 8);
                Keys result = Keys.None;
                if ((HOTKEYF_ALT & modifier) != 0)
                    result |= Keys.Alt;
                if ((HOTKEYF_SHIFT & modifier) != 0)
                    result |= Keys.Shift;
                if ((HOTKEYF_CONTROL & modifier) != 0)
                    result |= Keys.Control;
                return result;
            }
        }


        /// <summary>
        /// Makes a 16 bit short from two bytes
        /// </summary>
        /// <param name="pValueLow">The low order value.</param>
        /// <param name="pValueHigh">The high order value.</param>
        /// <returns></returns>
        internal static short MakeWord(byte pValueLow, byte pValueHigh)
        {
           return  (short)( pValueLow | pValueHigh << 8);
        }

        public void SetHotKeyValue(Keys modifierKeys, Keys hotKey)
        {
            byte keymod = 0;
            if ((modifierKeys & Keys.Alt) != 0)
                keymod |= HOTKEYF_ALT;
            if ((modifierKeys & Keys.Shift) != 0)
                keymod |= HOTKEYF_SHIFT;
            if ((modifierKeys & Keys.Control) != 0)
                keymod |= HOTKEYF_CONTROL;

            short keyvalue = MakeWord((byte)hotKey, keymod);
            NativeMethods.SendMessage(new HandleRef(this, this.Handle), HKM_SETHOTKEY, (IntPtr)(keyvalue), IntPtr.Zero);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_KEYUP)
            {
                this.OnTextChanged(EventArgs.Empty);
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
    }
}
