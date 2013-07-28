using System;
using System.Collections.Generic;
using System.Text;
using Laugris.Sage;
using System.Drawing;

namespace Krento.RollingStones
{
    public class RollingStoneSuspend : RollingStoneTask
    {
        public RollingStoneSuspend(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "Suspend.png";
                TranslationId = SR.Keys.StoneSuspend;
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
            WindowsOperations.Suspend();
        }
    }
}
