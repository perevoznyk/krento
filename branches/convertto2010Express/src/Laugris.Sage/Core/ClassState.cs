using System;
using System.Collections.Generic;
using System.Text;

namespace Laugris.Sage
{
    [Flags]
    public enum ClassState
    {
        Normal = 0x00,
        Loading = 0x01,
        Reading = 0x02,
        Writing = 0x04,
        Destroying = 0x08,
        Updating = 0x10,
        Fixups = 0x20,
        FreeNotification = 0x40
    }
}
