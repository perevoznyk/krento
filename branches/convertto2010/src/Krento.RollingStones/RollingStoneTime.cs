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
    /// Displays the current time
    /// </summary>
    public class RollingStoneTime : RollingStoneTask
    {

        private System.Windows.Forms.Timer timer;

        public RollingStoneTime(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "Timer.png";
                Caption = SR.CurrentTime;

                timer = new System.Windows.Forms.Timer();
                timer.Interval = 1000;
                timer.Tick += new EventHandler(timer_Tick);
                timer.Start();
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }

        public override void FixupConfiguration()
        {
            base.FixupConfiguration();
            Caption = SR.CurrentTime;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (Selected)
            {
                if (Manager.Visible)
                {
                    UpdateManagerSurface();
                }
            }
        }

        public override void DrawTargetDescription()
        {
            timer.Stop();
            Manager.DrawText(DateTime.Now.ToLongTimeString());
            MoveStoneHint();
            timer.Start();
        }

        private static void ExecuteAsync()
        {
            FileExecutor.Execute(Path.Combine(Environment.SystemDirectory, "rundll32.exe"),
                "/d \""+Environment.SystemDirectory +"\\shell32.dll\",Control_RunDLL timedate.cpl");
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
            try
            {
                if (timer != null)
                {
                    timer.Stop();
                    timer.Dispose();
                    timer = null;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}
