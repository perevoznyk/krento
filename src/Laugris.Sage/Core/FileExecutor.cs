//===============================================================================
// Copyright © Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Security.Permissions;
using System.Threading;

namespace Laugris.Sage
{
    /// <summary>
    /// Executes applications
    /// </summary>
    public static class FileExecutor
    {
        [SecurityPermission(SecurityAction.LinkDemand)]
        public static void ProcessExecute(string command, string argument, string workingFolder)
        {
            if (string.IsNullOrEmpty(command))
                return;

            Process process;
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.FileName = command;
            startInfo.Arguments = argument;
            startInfo.WorkingDirectory = workingFolder;
            startInfo.ErrorDialog = true;

            try
            {
                process = Process.Start(startInfo);
            }
            catch
            {
                process = null;
            }
            if (process != null)
            {
                try
                {
                    if (!process.HasExited)
                    {
                        process.WaitForInputIdle(0x7d0);
                        process.Close();
                    }
                }
                catch (InvalidOperationException)
                {
                }
            }
            NativeMethods.Sleep(0x5dc);
        }


        public static void Execute(string command)
        {
            Execute(command, string.Empty);
        }


        [SecurityPermission(SecurityAction.LinkDemand)]
        public static void Execute(string command, string argument)
        {
            if (string.IsNullOrEmpty(command))
                return;

            Process process;
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.FileName = command;
            startInfo.Arguments = argument;
            startInfo.ErrorDialog = true;

            try
            {
                process = Process.Start(startInfo);
            }
            catch
            {
                process = null;
            }
            if (process != null)
            {
                try
                {
                    if (!process.HasExited)
                    {
                        process.WaitForInputIdle(2000);
                        process.Close();
                    }
                }
                catch (InvalidOperationException)
                {
                }
            }
            NativeMethods.Sleep(1500);
        }

    }
}
