//===============================================================================
// Copyright © Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Security.AccessControl;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Laugris.Sage
{
    /// <summary>
    /// This component is used to ensure that only one instance of the application is running
    /// at the same time
    /// </summary>
    public partial class OneInstance : Component, ISupportInitialize
    {
        private static string uniqueAppStr;
        private WndProcDelegate QWindow;
        private IntPtr handle;
        private bool firstInstance;
        IntPtr mutexHandle;
        // An IntPtr to hold the previous instance if one exists
        private static IntPtr prevPtr = IntPtr.Zero;
        private bool csLoading;


        /// <summary>
        /// Callback function for EnumWindows
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lParam"></param>
        /// <returns>
        /// 1 unless the window is the running copy of this application,
        /// then it sends the command line text and returns 0 to exit the EnumWindows loop
        /// </returns>
        private static bool WindowEnumProc(IntPtr hWnd, IntPtr lParam)
        {
            if (hWnd != lParam)
            {
                IntPtr propPtr = NativeMethods.GetProp(hWnd, uniqueAppStr);
                if (propPtr.ToInt32() != 0)
                {
                    prevPtr = propPtr;
                    return false;
                }
            }

            return true;
        }

        private void GetRunningInstance()
        {
            // Enumerate windows
            EnumWindowsProc enumWindowsProc = new EnumWindowsProc(WindowEnumProc);
            NativeMethods.EnumWindows(enumWindowsProc, this.handle);
        }

        public void Execute(string parameter)
        {
            if (!csLoading)
            {
                InitInstance(parameter);
            }
        }


        protected void InitInstance(string parameter)
        {
            mutexHandle = NativeMethods.OpenMutex((int)MutexRights.ReadPermissions, false, uniqueAppStr);
            if (mutexHandle == IntPtr.Zero)
                // Mutex object has not yet been created, meaning that no previous 
                // instance has been created. 
                DoFirstInstance();
            else
                BroadcastFocusMessage(parameter);

        }

        protected void DoFirstInstance()
        {
            mutexHandle = NativeMethods.CreateMutex(IntPtr.Zero, false, uniqueAppStr);
            firstInstance = true;
        }

        protected void BroadcastFocusMessage(string parameter)
        {
            firstInstance = false;
            //send message to the first instance

            GetRunningInstance();

            if (prevPtr != IntPtr.Zero)
            {
                COPYDATASTRUCT cds = new COPYDATASTRUCT();
                IntPtr ptr = IntPtr.Zero;

                if (parameter != null)
                {
                    ptr = Marshal.StringToHGlobalAnsi(parameter);
                    cds.dwData = IntPtr.Zero;
                    cds.cbData = parameter.Length;
                    cds.lpData = ptr;
                }

                NativeMethods.SendMessage(new HandleRef(null, prevPtr), NativeMethods.WM_COPYDATA, IntPtr.Zero, ref cds);

                if (ptr != IntPtr.Zero)
                {
                    // Required to free unmanaged memory otherwise a memory leak occurs
                    Marshal.FreeHGlobal(ptr);
                }
            }

            NativeMethods.ExitKrento();
        }

        public bool FirstInstance
        {
            get { return firstInstance; }
        }

        public string UniqueAppStr
        {
            get { return uniqueAppStr; }
            set 
            { 
                uniqueAppStr = value;
                NativeMethods.SetProp(handle, uniqueAppStr, handle);
            }
        }

        public OneInstance()
        {
            InitializeComponent();
            QWindow = new WndProcDelegate(WndProc);
            handle = NativeMethods.AllocateHWND(QWindow);
        }

        public OneInstance(IContainer container)
        {
            if (container != null)
                container.Add(this);

            InitializeComponent();
            QWindow = new WndProcDelegate(WndProc);
            handle = NativeMethods.AllocateHWND(QWindow);
        }

        public event EventHandler Loaded;
        public event EventHandler<OneInstanceEventArgs> InstanceMessageReceived;

        protected virtual void OnInstanceMessageReceived(string param)
        {
            if (InstanceMessageReceived != null)
            {
                InstanceMessageReceived(this, new OneInstanceEventArgs(param));
            }
        }

        protected virtual IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == NativeMethods.WM_COPYDATA)
            {
                Message m = Message.Create(hWnd, msg, wParam, lParam);
                ProcessMessage(ref m);
            }
                            
            return NativeMethods.LayeredWndProc(hWnd, msg, wParam, lParam);
        }

        public void ProcessMessage(ref Message m)
        {
            string param = null;

            COPYDATASTRUCT cds = new COPYDATASTRUCT();
            cds = (COPYDATASTRUCT)Marshal.PtrToStructure(m.LParam, typeof(COPYDATASTRUCT));

            if (cds.cbData > 0)
            {
                param = Marshal.PtrToStringAnsi(cds.lpData).Substring(0, cds.cbData);
            }

            OnInstanceMessageReceived(param);
        }


        protected virtual void OnLoaded()
        {
            if (string.IsNullOrEmpty(uniqueAppStr))
                uniqueAppStr = OneInstance.NewGuid().ToString();


            if (Loaded != null)
                Loaded(this, EventArgs.Empty);
        }

        /// <summary>
        /// Initializes a new instance of the "System.Guid" class
        /// </summary>
        /// <returns>A new System.Guid object</returns>
        private static Guid NewGuid()
        {
            Guid val = Guid.Empty;
            int hresult = 0;
            hresult = NativeMethods.CoCreateGuid(ref val);
            if (hresult != 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error(), "GUID creation error");
            }

            return val;
        }


        #region ISupportInitialize Members

        public void BeginInit()
        {
            csLoading = true;
        }

        public void EndInit()
        {
            csLoading = false;
            OnLoaded();
        }

        #endregion
    }
}
