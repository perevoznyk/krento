using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Laugris.Sage;

namespace Krento.RollingStones
{
    public class RollingStoneRestart : RollingStoneTask
    {
        public RollingStoneRestart(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "Restart.png";
                TranslationId = SR.Keys.StoneRestart;
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
            WindowsOperations.Shutdown(ShutdownOption.Reboot);
            NativeMethods.ExitKrento();
        }
    }
}
