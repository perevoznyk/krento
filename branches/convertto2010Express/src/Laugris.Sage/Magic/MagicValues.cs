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
using System.ComponentModel;
using System.Globalization;

namespace Laugris.Sage
{
    /// <summary>
    /// Class for holding the min and max values of the specific property
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MinMaxValue
    {
        private double minValue;
        private double maxValue;

        #region Events
        public event EventHandler Changed;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MinMaxValue"/> class.
        /// </summary>
        /// <param name="min">The min value.</param>
        /// <param name="max">The max value.</param>
        public MinMaxValue(double min, double max)
        {
            this.minValue = min;
            this.maxValue = max;
        }

        protected virtual void OnChanged(EventArgs e)
        {
            if (Changed != null)
            {
                Changed(this, e);
            }
        }

        protected virtual void DoChange()
        {
            OnChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Gets or sets the min value.
        /// </summary>
        /// <value>The min value.</value>
        public double MinValue
        {
            get { return minValue; }
            set
            {
                minValue = value;
                DoChange();
            }
        }

        /// <summary>
        /// Gets or sets the max value.
        /// </summary>
        /// <value>The max value.</value>
        public double MaxValue
        {
            get { return maxValue; }
            set
            {
                maxValue = value;
                DoChange();
            }
        }

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return minValue.ToString(CultureInfo.InvariantCulture) + ", " + maxValue.ToString(CultureInfo.InvariantCulture);
        }
    }


    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class Velocity
    {
        private double x;
        private double y;

        #region Events
        public event EventHandler Changed;
        #endregion

        public Velocity(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        protected virtual void OnChanged(EventArgs e)
        {
            if (Changed != null)
            {
                Changed(this, e);
            }
        }

        protected virtual void DoChange()
        {
            OnChanged(EventArgs.Empty);
        }

        public double X
        {
            get { return x; }
            set
            {
                x = value;
                DoChange();
            }
        }

        public double Y
        {
            get { return y; }
            set
            {
                y = value;
                DoChange();
            }
        }

        public override string ToString()
        {
            return x.ToString(CultureInfo.InvariantCulture) + ", " + y.ToString(CultureInfo.InvariantCulture);
        }
    }

}
