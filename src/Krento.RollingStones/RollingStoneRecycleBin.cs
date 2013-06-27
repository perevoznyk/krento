using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Laugris.Sage;
using System.Windows.Forms;

namespace Krento.RollingStones
{
    public class RollingStoneRecycleBin : RollingStoneFolder
    {
        public RollingStoneRecycleBin(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "RecycleBin.png";
                TranslationId = SR.Keys.StoneRecycleBin;
                TargetDescription = null;
                Path = "explorer.exe";
                Args = "::{645FF040-5081-101B-9F08-00AA002F954E}";

                AllowDrop = true;
                DragOver += new System.Windows.Forms.DragEventHandler(window_DragOver);
                DragDrop += new System.Windows.Forms.DragEventHandler(window_DragDrop);
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }

        void window_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] strArray = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (strArray == null)
                    return;

                if (strArray.Length < 1)
                    return;

                for (int i = 0; i < strArray.Length; i++)
                {
                    string str2 = FileOperations.RemoveURI(strArray[i]);
                    string fullName = FileOperations.StripFileName(str2);
                    FileOperations.DeleteToRecycleBin(fullName);
                }
            }
        }

        void window_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e == null)
                return;
            DragDropEffects allowed = e.AllowedEffect;

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
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
            else
            {
                e.Effect = DragDropEffects.None;
            }

        }
    }
}
