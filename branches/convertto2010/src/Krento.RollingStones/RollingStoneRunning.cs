using System;
using System.Collections.Generic;
using System.Text;
using Laugris.Sage;
using System.Threading;

namespace Krento.RollingStones
{
    internal class RollingStoneRunning : RollingStoneTask
    {
        private string path;
        private IntPtr mainWindow;


        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public IntPtr MainWindow
        {
            get { return mainWindow; }
            set { mainWindow = value; }
        }

        public RollingStoneRunning(StonesManager manager)
            : base(manager)
        {
            CanDrag = false;
            CanRemove = false;
            CanConfigure = false;
            CanChangeType = false;
        }

        public override string StoneAuthor
        {
            get
            {
                return "Serhiy Perevoznyk";
            }
        }

        public override string StoneDescription
        {
            get
            {
                return "Switch to the program";
            }
        }

        public override bool StoneBuiltIn
        {
            get
            {
                return true;
            }
        }

        public override string StoneVersion
        {
            get
            {
                return "2.0";
            }
        }
        private void ExecuteAsync()
        {

            NativeMethods.BringWindowToFront(mainWindow);
        }

        public override void Run()
        {
            if (mainWindow == IntPtr.Zero)
                return;

            base.Run();
            Thread t = new Thread(new ThreadStart(ExecuteAsync));
            t.Name = "KrentoExecutor";
            t.IsBackground = true;
            t.Start();
            Thread.Sleep(0);
        }

    }
}
