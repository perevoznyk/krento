using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Laugris.Sage;
using System.Windows.Forms;

namespace Krento.RollingStones
{
    /// <summary>
    /// My Documents folder stone
    /// </summary>
    public class RollingStoneMyDocuments : RollingStoneFolder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RollingStoneMyDocuments"/> class.
        /// </summary>
        /// <param name="manager">The manager.</param>
        public RollingStoneMyDocuments(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "MyDocuments.png";
                TranslationId = SR.Keys.StoneMyDocuments;
                TargetDescription = null;
                Path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

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
