using System;
using System.Collections.Generic;
using System.Text;
using Laugris.Sage;
using System.Drawing;

namespace Krento.RollingStones
{
    /// <summary>
    /// Stone for closing Krento application
    /// </summary>
    public class RollingStoneCloseKrento : RollingStoneTask
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RollingStoneCloseKrento"/> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        public RollingStoneCloseKrento(StonesManager manager)
            : base(manager)
        {
            ResourceName = "CloseKrento.png";
            TranslationId = SR.Keys.PulsarClose;
            TargetDescription = null;
        }

        public override void Run()
        {
            NativeMethods.ExitKrento();
        }

    }
}
