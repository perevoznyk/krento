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
    /// <summary>
    /// Animation element for <see cref="MagicFire"/> class
    /// </summary>
    public class MagicSparkle : MagicShape
    {
        #region private members
        private double fireworkOpacityInc = -0.02;
        private double xVelocity = 1;
        private double yVelocity = 1;
        private double gravity = 1;
        #endregion

        public double Gravity
        {
            get { return gravity; }
            set { gravity = value; }
        }

        public double XVelocity
        {
            get { return xVelocity; }
            set { xVelocity = value; }
        }

        public double YVelocity
        {
            get { return yVelocity; }
            set { yVelocity = value; }
        }

        public double FireworkOpacityInc
        {
            get { return fireworkOpacityInc; }
            set { fireworkOpacityInc = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MagicSparkle"/> class.
        /// </summary>
        /// <param name="red">The red component value for the new Color. Valid values are 0 through 255.</param>
        /// <param name="green">The green component value for the new Color. Valid values are 0 through 255.</param>
        /// <param name="blue">The blue component value for the new Color. Valid values are 0 through 255.</param>
        /// <param name="size">The size of the shape</param>
        public MagicSparkle(byte red, byte green, byte blue, double size)
            : base(red, green, blue, size)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MagicSparkle"/> class.
        /// </summary>
        /// <param name="color">The color of the sparkle.</param>
        /// <param name="size">The size of the sparkle.</param>
        public MagicSparkle(Color color, double size)
            : base(color, size)
        {
        }

        /// <summary>
        /// Performs animation
        /// </summary>
        public override void Run()
        {
            this.Opacity += FireworkOpacityInc;
            if (this.Opacity < 0)
                this.Opacity = 0;
            else
                if (this.Opacity > 255)
                    this.Opacity = 255;
            ResetOpacity();
            YVelocity += Gravity;
            X = X + XVelocity;
            Y = Y + YVelocity;

        }
    }
}
