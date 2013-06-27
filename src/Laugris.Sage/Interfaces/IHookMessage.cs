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

namespace Laugris.Sage
{
    public interface IHookMessage
    {
        void SuppressHookMessage(bool value);
        void SaveCurrentCircle();
        void SaveManagerSettings();
        void ChangeCurrentCircle(string circleName);
        void ShowHelpPages();
        void ShowKrentoSettings();
        void CreateCircle();
        void EditCircle();
        void DeleteCircle();
    }
}
