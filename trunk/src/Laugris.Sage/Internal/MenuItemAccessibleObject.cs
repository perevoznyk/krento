using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Laugris.Sage
{
    class MenuItemAccessibleObject : AccessibleObject
    {
        private KrentoMenuItem item;

        public MenuItemAccessibleObject(KrentoMenuItem item)
            : base()
        {
            this.item = item;
        }

        public override string KeyboardShortcut
        {
            get
            {
                if (item != null)
                    return item.ShortCutName;
                else
                    return base.KeyboardShortcut;
            }
        }

        public override AccessibleStates State
        {
            get
            {
                if (item != null)
                {
                    if (!item.Enabled)
                        return AccessibleStates.Focused | AccessibleStates.Unavailable;
                    return AccessibleStates.Focused;

                }
                else
                    return base.State;
            }
        }

        //public override string Value
        //{
        //    get
        //    {
        //        if (item != null)
        //            return item.Caption;
        //        else
        //            return base.Value;
        //    }
        //    set
        //    {
        //        base.Value = value;
        //    }
        //}

        public override string Name
        {
            get
            {
                if (item != null)
                    return item.Caption;
                else
                    return base.Name;
            }
            set
            {
                base.Name = value;
            }
        }

        public override string Description
        {
            get
            {
                //if (item != null)
                //    return item.Caption;
                //else
                    return base.Description;
            }
        }

        public override AccessibleRole Role
        {
            get
            {
                if (item.Caption == "-")
                    return AccessibleRole.Separator;
                else
                    return AccessibleRole.MenuItem;
            }
        }

    }
}
