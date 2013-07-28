using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Laugris.Sage
{
    partial class OneInstance 
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components;

        
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && (components != null))
                {
                    components.Dispose();
                }

                if (mutexHandle != IntPtr.Zero)
                {
                    NativeMethods.CloseHandle(mutexHandle);
                    mutexHandle = IntPtr.Zero;
                }

                NativeMethods.RemoveProp(handle, uniqueAppStr);
                NativeMethods.DeallocateHWND(handle);
                GC.KeepAlive(QWindow);

            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
        }

        #endregion
    }
}
