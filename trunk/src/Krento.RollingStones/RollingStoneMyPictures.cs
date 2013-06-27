using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Laugris.Sage;

namespace Krento.RollingStones
{
    public class RollingStoneMyPictures : RollingStoneFolder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RollingStoneMyPictures"/> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        public RollingStoneMyPictures(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "MyPictures.png";
                TranslationId = SR.Keys.StoneMyPictures;
                TargetDescription = null;
                Path = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

                AllowDrop = true;
                DragOver += new System.Windows.Forms.DragEventHandler(window_DragOver);
                DragDrop += new System.Windows.Forms.DragEventHandler(window_DragDrop);
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }

        private void window_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
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
                    FileOperations.ShellCopyFile(fullName, Path);
                }
            }
        }

        private void window_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
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
