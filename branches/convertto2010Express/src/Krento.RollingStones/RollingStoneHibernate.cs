using System;
using System.Collections.Generic;
using System.Text;
using Laugris.Sage;
using System.Drawing;

namespace Krento.RollingStones
{
    public class RollingStoneHibernate : RollingStoneTask
    {
        public RollingStoneHibernate(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "Hibernate.png";
                TranslationId = SR.Keys.StoneHibernate;
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
            WindowsOperations.Hibernate();
        }
    }
}
