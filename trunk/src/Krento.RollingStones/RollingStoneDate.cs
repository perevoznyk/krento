using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Laugris.Sage;
using System.Threading;
using System.IO;

namespace Krento.RollingStones
{
    /// <summary>
    /// Displays current date
    /// </summary>
    public class RollingStoneDate : RollingStoneTask
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RollingStoneDate"/> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        public RollingStoneDate(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "Calendar.png";
                Caption = SR.CurrentDate;
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }

        public override void FixupConfiguration()
        {
            base.FixupConfiguration();
            Caption = SR.CurrentDate;
        }

        public override void DrawTargetDescription()
        {
            Manager.DrawText(DateTime.Now.ToLongDateString());
            MoveStoneHint();
        }

        private static void ExecuteAsync()
        {
            FileExecutor.Execute(Path.Combine(Environment.SystemDirectory, "rundll32.exe"),
                "/d \"" + Environment.SystemDirectory + "\\shell32.dll\",Control_RunDLL timedate.cpl");
        }

        public override void Run()
        {
            base.Run();

            Thread t = new Thread(new ThreadStart(ExecuteAsync));
            t.Name = "KrentoExecutor";
            t.IsBackground = true;
            t.Start();
            Thread.Sleep(0);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
