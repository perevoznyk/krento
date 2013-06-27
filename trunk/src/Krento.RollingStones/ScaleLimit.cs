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

namespace Krento.RollingStones
{
    public class ScaleLimit
    {
        private double minValue;

        public double MinValue
        {
            get { return minValue; }
            set { minValue = value; }
        }
        private double maxValue;

        public double MaxValue
        {
            get { return maxValue; }
            set { maxValue = value; }
        }

        public ScaleLimit(double minValue, double maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }
    }
}
