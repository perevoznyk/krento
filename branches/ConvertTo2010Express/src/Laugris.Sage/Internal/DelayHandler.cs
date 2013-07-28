using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Laugris.Sage
{
    internal class DelayHandler : BackgroundHandler
    {
        private MethodInvoker notifier;

        public DelayHandler(MethodInvoker notifier) : base()
        {
            this.notifier = notifier;
        }

        protected override void HandleTimer()
        {
            StopInterval();
            if (notifier != null)
                notifier();
            Dispose();
        }
    }
}
