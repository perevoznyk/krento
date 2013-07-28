using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Laugris.Sage;

namespace KrentoDistributor
{
    class KrentoDistributor
    {
        [STAThread]
        [DebuggerNonUserCode]
        [LoaderOptimization(LoaderOptimization.SingleDomain)]
        static void Main(string[] args)
        {
            if (args == null)
                return;
            if (args.Length == 0)
                return;

            for (int i = 0; i < args.Length; i++)
            {
                string str2 = FileOperations.RemoveURI(args[i]);
                string fullName = FileOperations.StripFileName(str2);
                InstallKrentoPart(fullName);
            }

        }

        [DllImport("MoonRoad.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.StdCall)]
        private static extern void ExtractArchive(string archiveName, string destination);

        internal static void InstallKrentoPart(string fileName)
        {
            string destination = null;
            bool compressed = false;


            string ext = Path.GetExtension(fileName);
            if (TextHelper.SameText(ext, ".toy"))
            {
                destination = GlobalConfig.ToysFolder;
                compressed = true;
            }
            else
                if (TextHelper.SameText(ext, ".kmenu"))
                {
                    destination = GlobalConfig.MenusFolder;
                    compressed = true;
                }
                else
                    if (TextHelper.SameText(ext, ".kskin"))
                    {
                        destination = GlobalConfig.SkinsFolder;
                        compressed = true;
                    }
                    else
                        if (TextHelper.SameText(ext, ".stone"))
                        {
                            destination = GlobalConfig.StoneClasses;
                            compressed = true;
                        }
                        else
                            if (TextHelper.SameText(ext, ".lng"))
                            {
                                destination = GlobalConfig.LanguagesFolder;
                                compressed = false;
                            }
                            else
                                if (TextHelper.SameText(ext, ".kadd"))
                                {
                                    destination = GlobalConfig.AddInRootFolder;
                                    compressed = true;
                                }
                                else
                                    if (TextHelper.SameText(ext, ".docklet"))
                                    {
                                        destination = GlobalConfig.DockletsFolder;
                                        compressed = true;
                                    }
                                    else
                                        if (TextHelper.SameText(ext, ".circle"))
                                        {
                                            destination = GlobalConfig.RollingStonesFolder;
                                            compressed = false;
                                        }

            if (string.IsNullOrEmpty(destination))
            {
                return;
            }

            if (!compressed)
            {
                string destinationName = Path.Combine(destination, Path.GetFileName(fileName));
                try
                {
                    FileOperations.CopyFile(fileName, destinationName);
                }
                catch
                {
                    //continue...
                }
            }
            else
            {
                try
                {
                    FileOperations.CopyFile(fileName, Path.Combine(GlobalConfig.DownloadsFolder, Path.GetFileName(fileName)));
                    destination = Path.Combine(destination, Path.GetFileNameWithoutExtension(fileName));
                    ExtractArchive(fileName, destination);
                }
                catch
                {
                    //continue...
                }

            }
        }
    }
}
