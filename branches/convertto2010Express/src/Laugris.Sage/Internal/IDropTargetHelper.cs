using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using ComIDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace Laugris.Sage
{
    [ComVisible(true)]
    [ComImport]
    [Guid("4657278B-411B-11D2-839A-00C04FD918D0")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IDropTargetHelper
    {
        void DragEnter(
            [In] IntPtr hwndTarget,
            [In, MarshalAs(UnmanagedType.Interface)] ComIDataObject dataObject,
            [In] ref POINT pt,
            [In] int effect);

        void DragLeave();

        void DragOver(
            [In] ref POINT pt,
            [In] int effect);

        void Drop(
            [In, MarshalAs(UnmanagedType.Interface)] ComIDataObject dataObject,
            [In] ref POINT pt,
            [In] int effect);

        void Show(
            [In] bool show);
    }

    [ComImport]
    [Guid("4657278A-411B-11d2-839A-00C04FD918D0")]
    internal class DragDropHelper { }

}
