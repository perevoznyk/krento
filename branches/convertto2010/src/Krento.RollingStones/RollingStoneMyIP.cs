using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Net;
using System.Net.NetworkInformation;
using Laugris.Sage;
using System.Threading;
using System.IO;

namespace Krento.RollingStones
{
    /// <summary>
    /// This stone shows IP address of the computer
    /// </summary>
    public class RollingStoneMyIP : RollingStoneTask
    {
        string ipAddress;

        public RollingStoneMyIP(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "MyIP.png";
                ipAddress = InteropHelper.CurrentIPAddress;
                Caption = ipAddress;
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }

        public override void DrawTargetDescription()
        {
            Manager.DrawText(ipAddress);
            MoveStoneHint();
        }

        private static void ExecuteAsync()
        {
            FileExecutor.Execute(Path.Combine(Environment.SystemDirectory, "rundll32.exe"),
                "/d \"" + Environment.SystemDirectory + "\\shell32.dll\",Control_RunDLL inetcpl.cpl");
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
    }
}
