using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Laugris.Sage
{
    class MenuAccessibleObject : AccessibleObject
    {
        private KrentoMenu menu;

        public MenuAccessibleObject(KrentoMenu menu)
        {
            this.menu = menu;
        }


        public override AccessibleRole Role
        {
            get
            {
                return AccessibleRole.MenuBar;
            }
        }

        public override int GetChildCount()
        {
            return menu.Items.Count;
        }

        public override AccessibleObject GetChild(int index)
        {
            return new MenuItemAccessibleObject(menu.Items[index]);
        }

        public override string Name
        {
            get
            {
                if (!string.IsNullOrEmpty(menu.Caption))
                    return menu.Caption;
                else
                    return "Krento Menu";
            }
            set
            {
                menu.Caption = value;
            }
        }

        //public override AccessibleObject GetSelected()
        //{
        //    return new MenuItemAccessibleObject(menu.Items[menu.SelectedIndex]);
        //}
    }
}
