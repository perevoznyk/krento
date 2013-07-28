//===============================================================================
// Copyright c Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================


using System;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Laugris.Sage
{

    /// <summary>
    /// Hotkey combination modifiers
    /// </summary>
    [Flags]
    public enum KeyModifiers
    {
        /// <summary>
        /// No modifiers
        /// </summary>
        None = 0,
        /// <summary>
        /// Alt key
        /// </summary>
        Alt = 1,
        /// <summary>
        /// Control key
        /// </summary>
        Control = 2,
        /// <summary>
        /// Shift key
        /// </summary>
        Shift = 4,
        /// <summary>
        /// Windows key
        /// </summary>
        Windows = 8
    }

    /// <summary>
    /// HotKey support
    /// </summary>
    public sealed class Hotkeys :  IDisposable
    {
        private bool disposed;
        private WeakReference formRef;
        private int id;
        private Keys key;
        private KeyModifiers modifier;
        private bool registered;



        /// <summary>
        /// Initializes a new instance of the <see cref="Hotkeys"/> class.
        /// </summary>
        /// <param name="modifier">The modifier.</param>
        /// <param name="key">The key.</param>
        /// <param name="form">The form.</param>
        public Hotkeys(KeyModifiers modifier, Keys key, Form form)
        {
            this.key = key;
            this.modifier = modifier;
            this.formRef = new WeakReference(form, false);
            this.Register();
        }

        public int Id
        {
            get { return this.id; }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        internal void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                this.Unregister();
                this.disposed = true;
            }
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="Hotkeys"/> is reclaimed by garbage collection.
        /// </summary>
        ~Hotkeys()
        {
            this.Dispose(false);
        }


        /// <summary>
        /// Registers hot key combination
        /// </summary>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        internal void Register()
        {
            try
            {
                if (formRef.Target != null)
                this.id = NativeMethods.InstallKeyboardHook( ((Form)this.formRef.Target).Handle, (int)this.modifier, this.key);
            }
            catch (DllNotFoundException)
            {
                this.id = 0;
                throw new HookException();
            }

            this.registered = (this.id > 0);
            if (!this.registered)
            {
                throw new ArgumentException("Key registration failed");
            }
        }

        /// <summary>
        /// Unregisters hot key combination
        /// </summary>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        internal void Unregister()
        {
            if (this.registered)
            {
                try
                {
                    if (formRef.Target != null)
                    NativeMethods.RemoveKeyboardHook( ((Form)this.formRef.Target).Handle, this.id);
                }
                catch (DllNotFoundException ex)
                {
                    throw new HookException("Error unregistering hotkey", ex);
                }
            }
        }


    }
}

