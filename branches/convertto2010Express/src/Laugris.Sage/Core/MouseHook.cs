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
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Reflection;

namespace Laugris.Sage
{
    /// <summary>
    /// This enumeration is needed for storing the mouse hook settings to ini file
    /// </summary>
    public enum MouseHookButton
    {
        /// <summary>
        /// Mouse hook is not in use, activation only by keyboard
        /// </summary>
        None,
        /// <summary>
        /// Wheel click activates application
        /// </summary>
        Wheel,
        /// <summary>
        /// XButton1 click activates application
        /// </summary>
        XButton1,
        /// <summary>
        /// XButton2 click activates application
        /// </summary>
        XButton2
    }

    /// <summary>
    /// Mouse Hook for application. When hook is installed
    /// the target form will receive a messages from CM_ group
    /// See NativeMethods for details
    /// </summary>
    public sealed class MouseHook : IDisposable
    {

        private bool disposed;
        private IntPtr handle;
        private bool registered;


        /// <summary>
        /// Initializes a new instance of the <see cref="MouseHook"/> class.
        /// </summary>
        /// <param name="form">The target form. This form will receive the messages
        /// from the hook callback procedure. The WndProc of the form must be overwritten for it.
        /// Another solution is to use message filtering</param>
        public MouseHook(IntPtr handle)
        {
            this.handle = handle;
            this.Register();
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="MouseHook"/> is reclaimed by garbage collection.
        /// </summary>
        ~MouseHook()
        {
            this.Dispose(false);
        }

        public static bool TrackMove
        {
            get { return NativeMethods.GetMoveTracking(); }
            set { NativeMethods.SetMoveTracking(value); }
        }

        public static bool TrackWheel
        {
            get { return NativeMethods.GetWheelTracking(); }
            set { NativeMethods.SetWheelTracking(value); }
        }

        public static bool TrackClick
        {
            get { return NativeMethods.GetClickTracking(); }
            set { NativeMethods.SetClickTracking(value); }
        }

        public static bool TrackWheelClick
        {
            get { return NativeMethods.GetClickTrackingWheel(); }
            set { NativeMethods.SetClickTrackingWheel(value); }
        }

        public static bool TrackXButtonClick
        {
            get { return NativeMethods.GetClickTrackingXButton(); }
            set { NativeMethods.SetClickTrackingXButton(value); }
        }

        public static bool InterceptClick
        {
            get { return NativeMethods.GetInterceptClick(); }
            set { NativeMethods.SetInterceptClick(value); }
        }

        public static bool DesktopClick
        {
            get { return NativeMethods.GetDesktopClick(); }
            set { NativeMethods.SetDesktopClick(value); }
        }
        
        /// <summary>
        /// Install mouse hook
        /// </summary>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        internal void Register()
        {

            this.registered = NativeMethods.InstallMouseHook(this.handle);
        }

        /// <summary>
        /// Remove mouse hook
        /// </summary>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        internal void Unregister()
        {
            if (this.registered)
            {
                NativeMethods.RemoveMouseHook();
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public static bool HookPaused
        {
            get { return NativeMethods.GetHookPaused(); }
        }

        /// <summary>
        /// Pauses mouse hook messaging
        /// </summary>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void Pause()
        {
            NativeMethods.PauseMouseHook();
        }

        /// <summary>
        /// Resumes mouse hook messaging
        /// </summary>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void Resume()
        {
            NativeMethods.ResumeMouseHook();
        }


        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void PauseIntercept()
        {
            NativeMethods.PauseIntercept();
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void ResumeIntercept()
        {
            NativeMethods.ResumeIntercept();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        internal void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                try
                {
                    this.Unregister();
                }
                finally
                {
                    this.disposed = true;
                }
            }

        }

        #endregion
    }
}
