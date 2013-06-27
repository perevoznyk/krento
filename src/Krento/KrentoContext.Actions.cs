using System;
using Laugris.Sage;
using System.Windows.Forms;

namespace Krento
{
	partial class KrentoContext
    {
        #region Windows message handlers
        void KrentoShowHandler(ref Message m)
        {
            ChangeVisibilityDelegate();
        }

        void KrentoHideHandler(ref Message m)
        {
            ChangeVisibilityDelegate();
        }

        void KrentoAboutHandler(ref Message m)
        {
            if (HookActive)
                return;
            ShowAboutBox();
        }

        void KrentoOptionsHandler(ref Message m)
        {
            if (HookActive)
                return;
            ShowKrentoSettings();
        }

        void KrentoCloseHandler(ref Message m)
        {
            CloseKrentoDelegate();
        }

        void PulsarHideHandler(ref Message m)
        {
            HidePulsarDelegate();
        }

        void PulsarShowHandler(ref Message m)
        {
            ShowPulsarDelegate();
        }

        void KrentoHelpHandler(ref Message m)
        {
            ShowHelpPages();
        }

        void KrentoPortableHandler(ref Message m)
        {
#if PORTABLE
            NativeMethods.PostMessage(m.LParam, m.Msg, (IntPtr)1, IntPtr.Zero);
#else
            NativeMethods.PostMessage(m.LParam, m.Msg, IntPtr.Zero, IntPtr.Zero);
#endif
        }
        #endregion

        void SaveCurrentCircleDelegate()
        {
            if (!string.IsNullOrEmpty(Manager.CurrentCircle))
            {
                Manager.SaveCircle(Manager.CurrentCircle);
                GlobalSettings.CircleName = Manager.CurrentCircle;
            }
            else
            {
                Manager.SaveCircle(GlobalConfig.HomeCircleName);
                GlobalSettings.CircleName = GlobalConfig.HomeCircleName;
            }
        }

        void SaveManagerSettingsDelegate()
        {
            manager.SaveSettings(GlobalConfig.KrentoSettingsFileName);
        }

        void HideManagerDelegate()
        {
            MainForm.HideManager();
        }

        void ShowManagerDelegate()
        {
            MainForm.ShowManager();
        }

        void HidePulsarDelegate()
        {
            HidePulsar();
        }

        void ShowPulsarDelegate()
        {
            ShowPulsar();
        }

        void ChangeVisibilityDelegate()
        {
            if (HookActive)
                return;
            MainForm.ChangeVisibility();
        }

        void CloseKrentoDelegate()
        {
            NativeMethods.PostMessage(InteropHelper.MainWindow, NativeMethods.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }

        #region IContextManager implementation
        void IContextManager.HideManager()
        {
            Invoke(new MethodInvoker(HideManagerDelegate));
        }

        void IContextManager.ShowManager()
        {
            Invoke(new MethodInvoker(ShowManagerDelegate));
        }

        void IContextManager.ChangeVisibility()
        {
            Invoke(new MethodInvoker(ChangeVisibilityDelegate));
        }

        void IContextManager.HidePulsar()
        {
            BeginInvoke(new MethodInvoker(HidePulsarDelegate));
        }

        void IContextManager.ShowPulsar()
        {
            BeginInvoke(new MethodInvoker(ShowPulsarDelegate));
        }

        void IContextManager.CloseKrento()
        {
            BeginInvoke(new MethodInvoker(CloseKrentoDelegate));
        }

        void IContextManager.ModifySettings()
        {
            this.SuppressHookMessage(true);
            try
            {
                ShowSettingsDialog();
            }
            finally
            {
                this.SuppressHookMessage(false);
            }
        }

        void IContextManager.KillKrento()
        {
            Killer.KillSelf();
        }
        #endregion
    }
}
