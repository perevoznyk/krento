using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using ComIDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace Laugris.Sage
{
    [ComVisible(true)]
    [ComImport]
    [Guid("DE5BF786-477A-11D2-839D-00C04FD918D0")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IDragSourceHelper
    {
        void InitializeFromBitmap(
            [In, MarshalAs(UnmanagedType.Struct)] ref ShDragImage dragImage,
            [In, MarshalAs(UnmanagedType.Interface)] ComIDataObject dataObject);

        void InitializeFromWindow(
            [In] IntPtr hwnd,
            [In] ref POINT pt,
            [In, MarshalAs(UnmanagedType.Interface)] ComIDataObject dataObject);
    }
}
