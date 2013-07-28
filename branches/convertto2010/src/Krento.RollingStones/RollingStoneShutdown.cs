using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Laugris.Sage;

namespace Krento.RollingStones
{
    public class RollingStoneShutdown : RollingStoneTask
    {
        public RollingStoneShutdown(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "Shutdown.png";
                TranslationId = SR.Keys.StoneShutdown;
                TargetDescription = null;
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }

        public override void Run()
        {
            base.Run();
            WindowsOperations.Shutdown(ShutdownOption.PowerOff);
            NativeMethods.ExitKrento();
        }
    }
}
