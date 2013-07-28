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
using System.Drawing;
using System.ComponentModel;

namespace Laugris.Sage
{

    public class MagicDot : MagicShape
    {
        #region Private members
        private double swingRadius;
        private int counter;
        private int swingSpeed = 5;
        private double upSpeed = 1;
        private double centerX;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MagicDot"/> class.
        /// </summary>
        /// <param name="red">The red component value for the new Color. Valid values are 0 through 255.</param>
        /// <param name="green">The green component value for the new Color. Valid values are 0 through 255.</param>
        /// <param name="blue">The blue component value for the new Color. Valid values are 0 through 255.</param>
        /// <param name="size">The size of the shape</param>
        public MagicDot(byte red, byte green, byte blue, double size)
            : base(red, green, blue, size)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MagicDot"/> class.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="size">The size.</param>
        public MagicDot(Color color, double size)
            : base(color, size)
        {
        }

        public double CenterX
        {
            get { return centerX; }
            set { centerX = value; }
        }

        public double UpSpeed
        {
            get { return upSpeed; }
            set { upSpeed = value; }
        }

        public double SwingRadius
        {
            get { return swingRadius; }
            set { swingRadius = value; }
        }

        public int SwingSpeed
        {
            get { return swingSpeed; }
            set { swingSpeed = value; }
        }

        public int Counter
        {
            get { return counter; }
            set { counter = value; }
        }


        public override void Run()
        {
            double angle = counter / 180.0 * System.Math.PI;
            Y = Y - upSpeed;
            X = CenterX + System.Math.Cos(angle) * swingRadius;
            counter += swingSpeed;
        }

    }
}
