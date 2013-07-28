using System;
using System.Collections.Generic;
using System.Text;
using Laugris.Sage;
using System.Drawing;

namespace Krento.RollingStones
{
    public class PowerControlDialog : FolderView
    {
        private Bitmap popupShutdown;
        private Bitmap popupRestart;
        private Bitmap popupSuspend;
        private Bitmap popupHibernate;

        public PowerControlDialog(IntPtr parentWindow)
            : base(parentWindow)
        {
            popupShutdown = NativeThemeManager.LoadBitmap("Shutdown.png");
            popupRestart = NativeThemeManager.LoadBitmap("Restart.png");
            popupSuspend = NativeThemeManager.LoadBitmap("Suspend.png");
            popupHibernate = NativeThemeManager.LoadBitmap("Hibernate.png");

            HeaderText = SR.PowerManagement;

            ItemSize = 104;
            this.Font = new Font("Tahoma", 10);
        }

        protected override void BuildItems()
        {
            FolderItem item;
            DisposeItems();

            item = new FolderItem(null);
            item.Logo = popupShutdown;
            item.Name = SR.StoneShutdown;
            Items.Add(item);

            item = new FolderItem(null);
            item.Logo = popupSuspend;
            item.Name = SR.StoneSuspend;
            Items.Add(item);

            item = new FolderItem(null);
            item.Logo = popupHibernate;
            item.Name = SR.StoneHibernate;
            Items.Add(item);

            item = new FolderItem(null);
            item.Logo = popupRestart;
            item.Name = SR.StoneRestart;
            Items.Add(item);

        }

        protected override void Dispose(bool disposing)
        {
            popupShutdown.Dispose();
            popupSuspend.Dispose();
            popupRestart.Dispose();
            popupHibernate.Dispose();
            Font.Dispose();
            base.Dispose(disposing);
        }
    }
}
