using System;
using System.Collections.Generic;
using System.Text;

namespace Laugris.Sage
{
    [Flags]
    public enum TransformationType
    {
        None    = 0x0,
        Bounds  = 0x1,
        Offset  = 0x2,
        Angle   = 0x4,
        Alpha   = 0x8
    }
}
