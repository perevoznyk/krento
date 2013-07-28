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
using System.Drawing;
using System.Threading;
using Laugris.Sage;

namespace Krento.RollingStones
{
    /// <summary>
    /// Base class for Krento stones that open one of the special folders
    /// </summary>
    public class RollingStoneFolder : RollingStoneTask
    {
        private string path;
        private string args;
        internal string programName;


        public string Args
        {
            get { return args; }
            set { args = value; }
        }

        public string Path
        {
            get { return path; }
            set { path = value; }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="RollingStoneFolder"/> class.
        /// </summary>
        /// <param name="manager">The Krento Stones Manager.</param>
        public RollingStoneFolder(StonesManager manager)
            : base(manager)
        {
            args = string.Empty;
        }

        private void ExecuteAsync()
        {

            try
            {
                if (string.IsNullOrEmpty(programName))
                    FileExecutor.Execute(path, args);
                else
                    FileExecutor.Execute(programName);
            }
            finally
            {
                programName = string.Empty;
            }
        }

        public override void Run()
        {
            if (string.IsNullOrEmpty(path))
                return;


            bool perform = true;
            programName = string.Empty;

            if (FileOperations.DirectoryExists(path) && (FileOperations.GetFilesCount(path) > 0))
            {
                Manager.SuppressHookMessage(true);
                try
                {
                    LiveFolder liveFolder = new LiveFolder(this.Manager.Handle, this.path);
                    try
                    {
                        perform = liveFolder.Execute();
                        if (perform)
                            programName = liveFolder.Items[liveFolder.SelectedItem].FileName;
                    }
                    finally
                    {
                        liveFolder.Dispose();
                    }
                }
                finally
                {
                    Manager.SuppressHookMessage(false);
                }
            }


            if (perform)
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
}
