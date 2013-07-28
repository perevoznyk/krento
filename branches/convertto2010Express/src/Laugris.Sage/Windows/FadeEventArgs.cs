using System;
using System.Collections.Generic;
using System.Text;

namespace Laugris.Sage
{
    public class FadeEventArgs : EventArgs
    {
        private readonly bool fadeUp;

        public FadeEventArgs(bool fadeUp)
        {
            this.fadeUp = fadeUp;
        }

        public bool FadeUp
        {
            get { return fadeUp; }
        } 

    }
}
