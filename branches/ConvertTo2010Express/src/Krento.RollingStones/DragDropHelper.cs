using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Laugris.Sage;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Krento.RollingStones
{
    internal static class DragDropHelper
    {
        public static void DragOverTarget(DragEventArgs e)
        {
            if (e == null)
                return;
            DragDropEffects allowed = e.AllowedEffect;

            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
                if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(ShellClipboardFormats.CFSTR_FILENAMEA) || e.Data.GetDataPresent(ShellClipboardFormats.CFSTR_FILENAMEW))
                {
                    if ((allowed & DragDropEffects.Link) == DragDropEffects.Link)
                    {

                        e.Effect = DragDropEffects.Link;
                    }
                    else
                        if ((allowed & DragDropEffects.Copy) == DragDropEffects.Copy)
                        {

                            e.Effect = DragDropEffects.Copy;
                        }
                }
                else if (e.Data.GetDataPresent(ShellClipboardFormats.CFSTR_SHELLURL))
                {
                    e.Effect = DragDropEffects.Link;
                }
                else if (e.Data.GetDataPresent(ShellClipboardFormats.CFSTR_INETURLW))
                {
                    e.Effect = DragDropEffects.Link;
                }
                else if (e.Data.GetDataPresent(ShellClipboardFormats.CFSTR_SHELLIDLIST))
                {
                    e.Effect = DragDropEffects.Link;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
        }

        public static void DragDropTarget(RollingStoneFile stone, DragEventArgs e)
        {
            DragDropTarget(stone, e, true);
            stone.Manager.FlushCurrentCircle();
        }

        public static void DragDropTarget(RollingStoneFile stone, DragEventArgs e, bool existingStone)
        {
            //Warning !!!
            //The order of the format check is very important
            if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(ShellClipboardFormats.CFSTR_FILENAMEA) || e.Data.GetDataPresent(ShellClipboardFormats.CFSTR_FILENAMEW))
            #region FileDrop
            {
                string[] strArray = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (strArray == null)
                    return;

                if (strArray.Length < 1)
                    return;

                string str2 = FileOperations.RemoveURI(strArray[0]);

                if (((e.KeyState & 4) == 4) && existingStone)
                {
                    if (stone.TargetName != null)
                    {
                        string execName = FileOperations.StripFileName(str2);
                        string fullName = FileOperations.StripFileName(stone.TargetName);
                        stone.programName = fullName;
                        if (stone.TargetName.Contains("##"))
                        {
                            stone.programName = stone.programName.Replace("##", execName);
                            stone.PerformExecution();
                            return;
                        }
                        else
                        {
                            stone.dynamicParameters = "\"" + execName + "\"";
                            stone.PerformDynamicExecution();
                            return;
                        }
                    }
                }
                else
                {
                    string fileName = str2;
                    string fullName;
                    if (FileOperations.FileIsLink(fileName))
                    {
                        fullName = Path.Combine(GlobalConfig.AppShortcuts, Path.GetFileName(fileName));
                        FileOperations.CopyFile(fileName, fullName);
                    }
                    else
                        fullName = fileName;
                    fullName = FileOperations.UnExpandPath(fullName);
                    stone.UpdateTarget(fullName);
                }
            }
            #endregion
            else
                if (e.Data.GetDataPresent(ShellClipboardFormats.CFSTR_INETURLW))
                #region InetW
                {
                    try
                    {
                        string hyperLinkUrl = null;
                        string hyperLinkText = null;
                        hyperLinkUrl = e.Data.GetData(typeof(string)) as string;

                        System.IO.Stream ioStream =
                        (System.IO.Stream)e.Data.GetData("FileGroupDescriptorW");
                        byte[] contents = new Byte[512];
                        ioStream.Read(contents, 0, 512);
                        ioStream.Close();

                        hyperLinkText = Encoding.Unicode.GetString(contents, 76, 436).Trim(new char[1] { '\0' });
                        if (!string.IsNullOrEmpty(hyperLinkText))
                        {
                            if (hyperLinkText.Length >= 4)
                            {
                                if (hyperLinkText.Substring(hyperLinkText.Length - 4, 4).ToLower().Equals(".url"))
                                {
                                    hyperLinkText = hyperLinkText.Substring(0, hyperLinkText.Length - 4);
                                    if (!string.IsNullOrEmpty(hyperLinkText))
                                        hyperLinkText = hyperLinkText.Trim(new char[1] { '\0' });
                                }
                            }
                        }

                        if (!FileOperations.IsURL(hyperLinkUrl))
                            hyperLinkUrl = FileOperations.RemoveURI(hyperLinkUrl);

                        if (!string.IsNullOrEmpty(hyperLinkText))
                            stone.UpdateTargetURL(hyperLinkUrl, hyperLinkText);
                        else
                            stone.UpdateTarget(hyperLinkUrl);

                        stone.TargetParameters = null;

                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                }
                #endregion
                else
                    if (e.Data.GetDataPresent(ShellClipboardFormats.CFSTR_SHELLURL))
                    #region Inet Ansi
                    {
                        try
                        {
                            string hyperLinkUrl = null;
                            hyperLinkUrl = e.Data.GetData(typeof(string)) as string;

                            if (!FileOperations.IsURL(hyperLinkUrl))
                                hyperLinkUrl = FileOperations.RemoveURI(hyperLinkUrl);

                            stone.UpdateTarget(hyperLinkUrl);
                            stone.TargetParameters = null;
                        }
                        catch (UnauthorizedAccessException)
                        {
                        }
                    }
                    #endregion
                    else
                        if (e.Data.GetDataPresent(ShellClipboardFormats.CFSTR_SHELLIDLIST))
                        #region Shell
                        {
                            try
                            {
                                // Copy clipboard data into unmanaged memory.
                                MemoryStream data = (MemoryStream)e.Data.GetData(ShellClipboardFormats.CFSTR_SHELLIDLIST);
                                byte[] b = data.ToArray();
                                IntPtr p = Marshal.AllocHGlobal(b.Length);
                                Marshal.Copy(b, 0, p, b.Length);

                                // Get parent folder.
                                int offset = sizeof(UInt32);
                                IntPtr parentpidl = (IntPtr)((int)p + (UInt32)Marshal.ReadInt32(p, offset));
                                offset = sizeof(UInt32) * 2;
                                IntPtr myPidl = (IntPtr)((int)p + (UInt32)Marshal.ReadInt32(p, offset));

                                IntPtr full = NativeMethods.ILCombine(parentpidl, myPidl);


                                string linkName = FileOperations.FileNameFromPidl(full);
                                string linkFile = null;
                                if (!string.IsNullOrEmpty(linkName))
                                {
                                    linkFile = Path.Combine(GlobalConfig.AppShortcuts, linkName + ".lnk");
                                    NativeMethods.CreateShellLink(full, linkFile);
                                    linkFile = FileOperations.UnExpandPath(linkFile);
                                    stone.UpdateTarget(linkFile);
                                    stone.TargetParameters = null;

                                }


                                NativeMethods.ILFree(full);
                                Marshal.FreeHGlobal(p);
                            }
                            catch
                            {
                            }

                        }
                        #endregion
                        else
                            if (e.Data.GetDataPresent(DataFormats.Text))
                            #region Text
                            {
                                string str2 = (string)e.Data.GetData(DataFormats.Text);
                                if ((stone.TargetName != null) && existingStone)
                                {
                                    string execName = str2;
                                    string fullName = FileOperations.StripFileName(stone.TargetName);
                                    stone.programName = fullName;
                                    if (stone.TargetName.Contains("##"))
                                    {
                                        stone.programName = stone.programName.Replace("##", execName);
                                        stone.PerformExecution();
                                        return;
                                    }
                                    else
                                    {
                                        stone.dynamicParameters = execName;
                                        stone.PerformDynamicExecution();
                                        return;
                                    }
                                }
                            }
                            #endregion

        }
    }
}
