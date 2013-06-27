using System;
using System.Collections.Generic;
using System.Text;

namespace Laugris.Sage
{
    public class MoveDeltaEventArgs : EventArgs
    {
        private readonly int deltaX;

        public int DeltaX
        {
            get { return deltaX; }
        }

        private readonly int deltaY;

        public int DeltaY
        {
            get { return deltaY; }
        }

        public MoveDeltaEventArgs(int deltaX, int deltaY)
        {
            this.deltaX = deltaX;
            this.deltaY = deltaY;
        }
    }
}
