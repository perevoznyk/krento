using System;
using System.Collections.Generic;
using System.Text;
using Laugris.Sage;
using System.Drawing;

namespace Krento.RollingStones
{
    public class RollingStoneControlPanel : RollingStoneFolder
    {
        public RollingStoneControlPanel(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "control.png";
                Path = "control.exe";
                TranslationId = SR.Keys.ControlPanel;
                TargetDescription = null;
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }
    }

    public class RollingStoneApplet : RollingStoneFolder
    {
        public RollingStoneApplet(StonesManager manager)
            : base(manager)
        {
            Path = "control.exe";
            Args = "";
        }
    }

    public class RollingStoneAddRemoveProgram : RollingStoneApplet
    {
        public RollingStoneAddRemoveProgram(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "AppAddRemove.png";
                TranslationId = SR.Keys.AppletAddRemoveProgram;
                TargetDescription = null;
                Args = "appwiz.cpl";
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }
    }

    public class RollingStoneDateAndTime : RollingStoneApplet
    {
        public RollingStoneDateAndTime(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "AppDateTime.png";
                TranslationId = SR.Keys.AppletDateAndTime;
                TargetDescription = null;
                Args = "date/time";
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }
    }

    public class RollingStonePrintersAndFaxes : RollingStoneApplet
    {
        public RollingStonePrintersAndFaxes(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "AppPrinters.png";
                TranslationId = SR.Keys.AppletPrintersAndFaxes;
                TargetDescription = null;
                Args = "printers";
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }
    }

    public class RollingStoneNetworkConnections : RollingStoneApplet
    {
        public RollingStoneNetworkConnections(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "AppNetwork.png";
                TranslationId = SR.Keys.AppletNetworkConnections;
                TargetDescription = null;
                Args = "netconnections";
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }
    }

    public class RollingStoneFonts : RollingStoneApplet
    {
        public RollingStoneFonts(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "AppFonts.png";
                TranslationId = SR.Keys.AppletFonts;
                TargetDescription = null;
                Args = "fonts";
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }
    }

    public class RollingStoneDisplayBackground : RollingStoneApplet
    {
        public RollingStoneDisplayBackground(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "AppBackground.png";
                TranslationId = SR.Keys.AppletDisplayBackground;
                TargetDescription = null;
                Args = "desk.cpl,,0";
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }
    }

    public class RollingStoneAppearance : RollingStoneApplet
    {
        public RollingStoneAppearance(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "AppAppearance.png";
                TranslationId = SR.Keys.AppletAppearance;
                TargetDescription = null;
                Args = "color";
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }
    }

    public class RollingStoneUserAccounts : RollingStoneApplet
    {
        public RollingStoneUserAccounts(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "AppUsers.png";
                TranslationId = SR.Keys.AppletUserAccounts;
                TargetDescription = null;
                Args = "userpasswords";
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }
    }

    public class RollingStoneThemes : RollingStoneApplet
    {
        public RollingStoneThemes(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "AppThemes.png";
                TranslationId = SR.Keys.AppletThemes;
                TargetDescription = null;
                Args = "desk.cpl";
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }
    }

    public class RollingStoneAccessibilityOptions : RollingStoneApplet
    {
        public RollingStoneAccessibilityOptions(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "AppAccess.png";
                TranslationId = SR.Keys.AppletAccessibilityOptions;
                TargetDescription = null;
                Args = "access.cpl";
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }
    }

    public class RollingStoneMouse : RollingStoneApplet
    {
        public RollingStoneMouse(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "AppMouse.png";
                TranslationId = SR.Keys.AppletMouse;
                TargetDescription = null;
                Args = "mouse";
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }
    }

    public class RollingStoneKeyboard : RollingStoneApplet
    {
        public RollingStoneKeyboard(StonesManager manager)
            : base(manager)
        {
            try
            {
                ResourceName = "AppKeyboard.png";
                TranslationId = SR.Keys.AppletKeyboard;
                TargetDescription = null;
                Args = "keyboard";
            }
            catch (Exception ex)
            {
                throw new StoneConstructorException("Create stone error", ex);
            }
        }
    }

}
