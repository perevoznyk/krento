using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Laugris.Sage;

namespace Krento.RollingStones
{
    public class PopupDialog : FolderView
    {
        private readonly Bitmap popupAddRing;
        private readonly Bitmap popupAddStone;
        private readonly Bitmap popupApplications;
        private readonly Bitmap popupChangeSkin;
        private readonly Bitmap popupDeleteCircle;
        private readonly Bitmap popupEditCircle;
        private readonly Bitmap popupHelp;
        private readonly Bitmap popupSettings;


        public PopupDialog(IntPtr parentWindow)
            : base(parentWindow)
        {
            popupAddRing = NativeThemeManager.LoadBitmap("PopupAddRing.png");
            popupAddStone = NativeThemeManager.LoadBitmap("PopupAddStone.png");
            popupApplications = NativeThemeManager.LoadBitmap("PopupApplications.png");
            popupChangeSkin = NativeThemeManager.LoadBitmap("PopupChangeSkin.png");
            popupDeleteCircle = NativeThemeManager.LoadBitmap("PopupDeleteRing.png");
            popupEditCircle = NativeThemeManager.LoadBitmap("PopupEditRing.png");
            popupHelp = NativeThemeManager.LoadBitmap("PopupHelp.png");
            popupSettings = NativeThemeManager.LoadBitmap("PopupSettings.png");


            HeaderText = SR.Settings;

            ItemSize = 104;
            Font = new Font("Tahoma", 10);
        }


        protected override void BuildItems()
        {
            FolderItem item;
            DisposeItems();

            item = new FolderItem(null);
            item.Logo = popupSettings;
            item.Name = SR.PulsarOptions;
            Items.Add(item);

            item = new FolderItem(null);
            item.Logo = popupAddRing;
            item.Name = SR.CreateCircle;
            Items.Add(item);

            item = new FolderItem(null);
            item.Logo = popupApplications;
            item.Name = SR.TaskManager;
            Items.Add(item);

            item = new FolderItem(null);
            item.Logo = popupChangeSkin;
            item.Name = SR.KrentoNewSkin;
            Items.Add(item);

            item = new FolderItem(null);
            item.Logo = popupEditCircle;
            item.Name = SR.Modify;
            Items.Add(item);

            item = new FolderItem(null);
            item.Logo = popupDeleteCircle;
            item.Name = SR.Delete;
            Items.Add(item);

            item = new FolderItem(null);
            item.Logo = popupAddStone;
            item.Name = SR.AddStone;
            Items.Add(item);

            item = new FolderItem(null);
            item.Logo = popupHelp;
            item.Name = SR.PulsarHelp;
            Items.Add(item);

        }

        protected override void Dispose(bool disposing)
        {
            popupAddRing.Dispose();
            popupAddStone.Dispose();
            popupApplications.Dispose();
            popupChangeSkin.Dispose();
            popupDeleteCircle.Dispose();
            popupEditCircle.Dispose();
            popupHelp.Dispose();
            popupSettings.Dispose();
            Font.Dispose();
            base.Dispose(disposing);
        }
    }
}
