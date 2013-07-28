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
using System.Collections;

namespace Krento.RollingStones
{
    public interface ICircleEnumerator : IEnumerator
    {
        bool MovePrevious();
        bool JumpTo(int stoneIndex);
    }
}
