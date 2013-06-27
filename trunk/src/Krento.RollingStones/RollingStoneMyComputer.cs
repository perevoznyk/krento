using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Laugris.Sage;

namespace Krento.RollingStones
{
    public class RollingStoneMyComputer : RollingStoneFolder
    {
        public RollingStoneMyComputer(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "MyComputer.png";
                TranslationId = SR.Keys.StoneMyComputer;
                TargetDescription = null;
                Path = "explorer.exe";
                Args = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}";
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }
    }
}
