//===============================================================================
// Copyright (c) Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using Laugris.Sage;
using System.Threading;
using System.Drawing.Imaging;
using System.IO;

namespace Krento.RollingStones
{


    internal sealed class ManagerWindow : LayeredWindow
    {
        private StonesManager manager;

        private Bitmap backgroundImage;
        private RingSwitcher taskManager;

        private int skinMarginLeft;
        private int skinMarginRight;
        private int skinMarginTop;
        private int skinMarginBottom;

        private const int rotateTimer = 2;
        private const int memoryTimer = 3;
        private const int foregroundTimer = 4;
        private const int wheelDelayTimer = 5;

        private int rotateInterval = 40;

        private int acceleration = 1;
        private int speedCounter = 0;
        private int speedLimit = (180 / 25) * 180;
        private int accelerationLimit = 6;

        private bool rotationActive;


        private int buttonSize = 60;

        private int skinMarginDefault = 38;

        private Dictionary<Keys, KeyboardAction> actions;
        private bool virtualClick;

        private SmoothingMode smoothingMode = SmoothingMode.HighQuality;
        private CompositingQuality compositingQuality = CompositingQuality.HighQuality;
        private InterpolationMode interpolationMode = GlobalConfig.InterpolationMode;
        private CompositingMode compositingMode = CompositingMode.SourceCopy;

        private PufSmoke smoke = null;

        private VisualCollection buttons;


        public ManagerWindow(StonesManager manager)
        {
            this.manager = manager;
            StartTimer(memoryTimer, 30000);
            ClearUnusedMemory();

            if (GlobalSettings.ShowManagerButtons)
            {
                LoadButtons();
            }

            actions = new Dictionary<Keys, KeyboardAction>(200);

            PopulateKeyboardActions();

            Text = "Krento Manager";
            Name = "ManagerWindow";
        }


        public bool RotationActive
        {
            get { return rotationActive; }
        }

        /// <summary>
        /// Handles the Invalidate event of the buttons control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void ButtonsInvalidate(object sender, EventArgs e)
        {
            if (GlobalSettings.ShowManagerButtons)
            {
                manager.RedrawManagerButtons();
            }
        }


        internal void LoadButtons()
        {
            if (!GlobalSettings.ShowManagerButtons)
                return;

            if (buttons == null)
            {
                buttons = new VisualCollection();
                buttons.Invalidate += new EventHandler(ButtonsInvalidate);
            }

            UIButton button;

            buttonSize = 60;
            if (buttonSize * 5 > Width)
                buttonSize = Width / 5;

            button = (UIButton)buttons["Launcher"];
            if (button == null)
            {
                button = new UIButton();
                button.Name = "Launcher";
                buttons.Add(button);
                button.Click += new EventHandler(LauncherClick);
                button.MouseEnter += new EventHandler(LauncherMouseEnter);
            }

            if (button.NormalFace != null)
            {
                button.NormalFace.Dispose();
                button.NormalFace = null;
            }
            if (button.FocusedFace != null)
            {
                button.FocusedFace.Dispose();
                button.FocusedFace = null;
            }
            if (button.PressedFace != null)
            {
                button.PressedFace.Dispose();
                button.PressedFace = null;
            }

            button.NormalFace  = BitmapPainter.ResizeBitmap(NativeThemeManager.LoadBitmap("ButtonLauncherNormal.png"), buttonSize, buttonSize, true);
            button.FocusedFace = BitmapPainter.ResizeBitmap(NativeThemeManager.LoadBitmap("ButtonLauncherFocused.png"), buttonSize, buttonSize, true);
            button.PressedFace = BitmapPainter.ResizeBitmap(NativeThemeManager.LoadBitmap("ButtonLauncherPressed.png"), buttonSize, buttonSize, true);


            button = (UIButton)buttons["Search"];
            if (button == null)
            {
                button = new UIButton();
                button.Name = "Search";
                buttons.Add(button);
                button.Click += new EventHandler(BackClick);
                button.MouseEnter += new EventHandler(BackMouseEnter);
            }

            if (button.NormalFace != null)
            {
                button.NormalFace.Dispose();
                button.NormalFace = null;
            }
            if (button.FocusedFace != null)
            {
                button.FocusedFace.Dispose();
                button.FocusedFace = null;
            }
            if (button.PressedFace != null)
            {
                button.PressedFace.Dispose();
                button.PressedFace = null;
            }

            button.NormalFace  = BitmapPainter.ResizeBitmap(NativeThemeManager.LoadBitmap("ButtonSearchNormal.png"), buttonSize, buttonSize, true);
            button.FocusedFace = BitmapPainter.ResizeBitmap(NativeThemeManager.LoadBitmap("ButtonSearchFocused.png"), buttonSize, buttonSize, true);
            button.PressedFace = BitmapPainter.ResizeBitmap(NativeThemeManager.LoadBitmap("search_left_press.png"), buttonSize, buttonSize, true);


            button = (UIButton)buttons["Power"];
            if (button == null)
            {
                button = new UIButton();
                button.Name = "Power";
                buttons.Add(button);
                button.Click += new EventHandler(ForwardClick);
                button.MouseEnter += new EventHandler(ForwardMouseEnter);
            }

            if (button.NormalFace != null)
            {
                button.NormalFace.Dispose();
                button.NormalFace = null;
            }
            if (button.FocusedFace != null)
            {
                button.FocusedFace.Dispose();
                button.FocusedFace = null;
            }
            if (button.PressedFace != null)
            {
                button.PressedFace.Dispose();
                button.PressedFace = null;
            }

            button.NormalFace  = BitmapPainter.ResizeBitmap(NativeThemeManager.LoadBitmap("ButtonPowerNormal.png"), buttonSize, buttonSize, true);
            button.FocusedFace = BitmapPainter.ResizeBitmap(NativeThemeManager.LoadBitmap("ButtonPowerFocused.png"), buttonSize, buttonSize, true);
            button.PressedFace = BitmapPainter.ResizeBitmap(NativeThemeManager.LoadBitmap("ButtonPowerPressed.png"), buttonSize, buttonSize, true);


            button = (UIButton)buttons["Settings"];
            if (button == null)
            {
                button = new UIButton();
                button.Name = "Settings";
                buttons.Add(button);
                button.Click += new EventHandler(SettingsClick);
                button.MouseEnter += new EventHandler(SettingsMouseEnter);
            }

            if (button.NormalFace != null)
            {
                button.NormalFace.Dispose();
                button.NormalFace = null;
            }
            if (button.FocusedFace != null)
            {
                button.FocusedFace.Dispose();
                button.FocusedFace = null;
            }
            if (button.PressedFace != null)
            {
                button.PressedFace.Dispose();
                button.PressedFace = null;
            }

            button.NormalFace =  BitmapPainter.ResizeBitmap(NativeThemeManager.LoadBitmap("ButtonSettingsNormal.png"), buttonSize, buttonSize, true);
            button.FocusedFace = BitmapPainter.ResizeBitmap(NativeThemeManager.LoadBitmap("ButtonSettingsFocused.png"), buttonSize, buttonSize, true);
            button.PressedFace = BitmapPainter.ResizeBitmap(NativeThemeManager.LoadBitmap("ButtonSettingsPressed.png"), buttonSize, buttonSize, true);

            button = (UIButton)buttons["Home"];
            if (button == null)
            {
                button = new UIButton();
                button.Name = "Home";
                button.Click += new EventHandler(HomeClick);
                button.MouseEnter += new EventHandler(HomeMouseEnter);
                buttons.Add(button);
            }
            if (button.NormalFace != null)
            {
                button.NormalFace.Dispose();
                button.NormalFace = null;
            }
            if (button.FocusedFace != null)
            {
                button.FocusedFace.Dispose();
                button.FocusedFace = null;
            }
            if (button.PressedFace != null)
            {
                button.PressedFace.Dispose();
                button.PressedFace = null;
            }

            button.NormalFace = BitmapPainter.ResizeBitmap(NativeThemeManager.LoadElement("ButtonHome", NativeElementState.Normal), buttonSize, buttonSize, true);
            button.FocusedFace = BitmapPainter.ResizeBitmap(NativeThemeManager.LoadElement("ButtonHome", NativeElementState.Focused), buttonSize, buttonSize, true);
            button.PressedFace = BitmapPainter.ResizeBitmap(NativeThemeManager.LoadElement("ButtonHome", NativeElementState.Pressed), buttonSize, buttonSize, true);
        }


        void HomeMouseEnter(object sender, EventArgs e)
        {
            if (GlobalSettings.ShowManagerButtons)
                manager.ShowButtonHint(SR.FileConfigDefault, buttons["Home"].Left);
        }

        void SettingsMouseEnter(object sender, EventArgs e)
        {
            if (GlobalSettings.ShowManagerButtons)
                manager.ShowButtonHint(SR.Settings, buttons["Settings"].Left);
        }

        void ForwardMouseEnter(object sender, EventArgs e)
        {
            if (GlobalSettings.ShowManagerButtons)
                manager.ShowButtonHint(SR.PowerManagement, buttons["Power"].Left);
        }

        void BackMouseEnter(object sender, EventArgs e)
        {
            if (GlobalSettings.ShowManagerButtons)
                manager.ShowButtonHint(SR.Applications, buttons["Search"].Left);
        }

        void LauncherMouseEnter(object sender, EventArgs e)
        {
            if (GlobalSettings.ShowManagerButtons)
                manager.ShowButtonHint(SR.SelectCircle, buttons["Launcher"].Left);
        }

        void HomeClick(object sender, EventArgs e)
        {
            manager.BackToDefaultCircle();
        }

        void SettingsClick(object sender, EventArgs e)
        {
            if (buttons != null) buttons.MouseLeave();
            if (manager.ShowPopupDialog())
                manager.BringToFront();
        }

        void ForwardClick(object sender, EventArgs e)
        {
            if (buttons != null) buttons.MouseLeave();
            manager.ShowPowerControlDialog();
            manager.BringToFront();
        }

        void BackClick(object sender, EventArgs e)
        {
            if (buttons != null) buttons.MouseLeave();
            manager.ShowStartMenuDialog(false);
            manager.BringToFront();
        }

        void LauncherClick(object sender, EventArgs e)
        {
            if (buttons != null) buttons.MouseLeave();
            manager.SwitchRing();
            manager.BringToFront();
        }


        protected override void WndProc(ref Message m)
        {

            switch (m.Msg)
            {

                case NativeMethods.CM_REDRAW_MANAGER:
                    {
                        manager.RedrawScreenHintInternal();
                        return;
                    }
                case NativeMethods.CM_REDRAW_BUTTONS:
                    {
                        manager.RepaintButtonsInternal();
                        return;
                    }
                case NativeMethods.CM_CHANGE_STONE:
                    {
                        manager.ChangeStoneType((int)m.WParam);
                        return;
                    }
                case NativeMethods.CN_CLOSE:
                    {
                        if (smoke != null)
                        {
                            try
                            {
                                smoke.Hide();
                                smoke.Dispose();
                            }
                            finally
                            {
                                smoke = null;
                            }

                        }
                        return;
                    }

                case NativeMethods.CM_ROTATE:
                    {
                        manager.Rotate((int)m.WParam);
                        return;
                    }

                case NativeMethods.CM_REMOVE_STONE:
                    {
                        if (smoke != null)
                        {
                            smoke.Dispose();
                            smoke = null;
                        }
                        if (Visible && (m.LParam == (IntPtr)1))
                        {
                            RollingStoneBase stone = manager.Stones[(int)m.WParam];
                            int x = stone.Left + manager.StoneSize / 2;
                            int y = stone.Top + manager.StoneSize / 2;
                            stone.Visible = false;
                            manager.HideScreenHint();

                            smoke = new PufSmoke();
                            smoke.StartSmoke(x, y);
                        }
                        manager.RemoveStone((int)m.WParam);

                        return;
                    }
                case NativeMethods.WM_MOUSEACTIVATE:
                    {
                        IntPtr topWindow = NativeMethods.GetTopWindow(IntPtr.Zero);
                        if (topWindow != Handle)
                        {
                            manager.Rotate(0);
                            //manager.Rotate(0);
                        }
                        break;
                    }
            }

            base.WndProc(ref m);
        }


        private void KeysToHash(Keys[] keys, KeyboardAction action)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                actions[keys[i]] = action;
            }
        }

        private void PopulateKeyboardActions()
        {
            Keys[] ak;
            using (ActionLoader loader = new ActionLoader(GlobalConfig.KeyMappingFileName))
            {
                //001
                ak = loader.GetActionKeys(ActionName.HideManager);
                if (ak.Length == 0)
                {
                    loader.AddActionKey(ActionName.HideManager, Keys.Escape);
                    actions[Keys.Escape] = new KeyboardAction(HideManagerAction);
                }
                else
                {
                    KeysToHash(ak, HideManagerAction);
                }


                ak = loader.GetActionKeys(ActionName.RunCurrentStone);
                if (ak.Length == 0)
                {
                    loader.AddActionKey(ActionName.RunCurrentStone, Keys.Return);
                    loader.AddActionKey(ActionName.RunCurrentStone, Keys.Space);
                    actions[Keys.Return] = new KeyboardAction(RunCurrentStoneAction);
                    actions[Keys.Space] = new KeyboardAction(RunCurrentStoneAction);
                }
                else
                {
                    KeysToHash(ak, RunCurrentStoneAction);
                }


                ak = loader.GetActionKeys(ActionName.RotateLeft);
                if (ak.Length == 0)
                {
                    loader.AddActionKey(ActionName.RotateLeft, Keys.Left);
                    loader.AddActionKey(ActionName.RotateLeft, Keys.Down);
                    loader.AddActionKey(ActionName.RotateLeft, Keys.D3);
                    loader.AddActionKey(ActionName.RotateLeft, Keys.NumPad3);
                    loader.AddActionKey(ActionName.RotateLeft, Keys.D2);
                    loader.AddActionKey(ActionName.RotateLeft, Keys.NumPad2);
                    loader.AddActionKey(ActionName.RotateLeft, Keys.D4);
                    loader.AddActionKey(ActionName.RotateLeft, Keys.NumPad4);

                    actions[Keys.Left] = new KeyboardAction(RotateLeftAction);
                    actions[Keys.Down] = new KeyboardAction(RotateLeftAction);
                    actions[Keys.D3] = new KeyboardAction(RotateLeftAction);
                    actions[Keys.NumPad3] = new KeyboardAction(RotateLeftAction);
                    actions[Keys.D2] = new KeyboardAction(RotateLeftAction);
                    actions[Keys.NumPad2] = new KeyboardAction(RotateLeftAction);
                    actions[Keys.D4] = new KeyboardAction(RotateLeftAction);
                    actions[Keys.NumPad4] = new KeyboardAction(RotateLeftAction);
                }
                else
                {
                    KeysToHash(ak, RotateLeftAction);
                }

                ak = loader.GetActionKeys(ActionName.RotateRight);
                if (ak.Length == 0)
                {
                    loader.AddActionKey(ActionName.RotateRight, Keys.Right);
                    loader.AddActionKey(ActionName.RotateRight, Keys.Up);
                    loader.AddActionKey(ActionName.RotateRight, Keys.D9);
                    loader.AddActionKey(ActionName.RotateRight, Keys.NumPad9);
                    loader.AddActionKey(ActionName.RotateRight, Keys.D8);
                    loader.AddActionKey(ActionName.RotateRight, Keys.NumPad8);
                    loader.AddActionKey(ActionName.RotateRight, Keys.NumPad6);
                    loader.AddActionKey(ActionName.RotateRight, Keys.D6);

                    actions[Keys.Right] = new KeyboardAction(RotateRightAction);
                    actions[Keys.Up] = new KeyboardAction(RotateRightAction);
                    actions[Keys.D9] = new KeyboardAction(RotateRightAction);
                    actions[Keys.NumPad9] = new KeyboardAction(RotateRightAction);
                    actions[Keys.D8] = new KeyboardAction(RotateRightAction);
                    actions[Keys.NumPad8] = new KeyboardAction(RotateRightAction);
                    actions[Keys.NumPad6] = new KeyboardAction(RotateRightAction);
                    actions[Keys.D6] = new KeyboardAction(RotateRightAction);
                }
                else
                {
                    KeysToHash(ak, RotateRightAction);
                }

                ak = loader.GetActionKeys(ActionName.RotateNextStone);
                if (ak.Length == 0)
                {
                    loader.AddActionKey(ActionName.RotateNextStone, Keys.PageUp);
                    actions[Keys.PageUp] = new KeyboardAction(RotateNextStone);
                }
                else
                {
                    for (int i = 0; i < ak.Length; i++)
                        actions[ak[i]] = new KeyboardAction(RotateNextStone);
                }

                ak = loader.GetActionKeys(ActionName.RotatePrevStone);
                if (ak.Length == 0)
                {
                    loader.AddActionKey(ActionName.RotatePrevStone, Keys.PageDown);
                    actions[Keys.PageDown] = new KeyboardAction(RotatePreviousStone);
                }
                else
                {
                    for (int i = 0; i < ak.Length; i++)
                        actions[ak[i]] = new KeyboardAction(RotatePreviousStone);
                }


                ak = loader.GetActionKeys(ActionName.SelectPrevStone);
                if (ak.Length == 0)
                {
                    loader.AddActionKey(ActionName.SelectPrevStone, Keys.Alt | Keys.Left);
                    loader.AddActionKey(ActionName.SelectPrevStone, Keys.Alt | Keys.D4);
                    loader.AddActionKey(ActionName.SelectPrevStone, Keys.Alt | Keys.NumPad4);

                    actions[Keys.Alt | Keys.Left] = new KeyboardAction(SelectPreviousStoneAction);
                    actions[Keys.Alt | Keys.D4] = new KeyboardAction(SelectPreviousStoneAction);
                    actions[Keys.Alt | Keys.NumPad4] = new KeyboardAction(SelectPreviousStoneAction);
                }
                else
                {
                    for (int i = 0; i < ak.Length; i++)
                        actions[ak[i]] = new KeyboardAction(SelectPreviousStoneAction);
                }

                ak = loader.GetActionKeys(ActionName.SelectNextStone);
                if (ak.Length == 0)
                {
                    loader.AddActionKey(ActionName.SelectNextStone, Keys.Alt | Keys.Right);
                    loader.AddActionKey(ActionName.SelectNextStone, Keys.Alt | Keys.D6);
                    loader.AddActionKey(ActionName.SelectNextStone, Keys.Alt | Keys.NumPad6);

                    actions[Keys.Alt | Keys.Right] = new KeyboardAction(SelectNextStoneAction);
                    actions[Keys.Alt | Keys.D6] = new KeyboardAction(SelectNextStoneAction);
                    actions[Keys.Alt | Keys.NumPad6] = new KeyboardAction(SelectNextStoneAction);
                }
                else
                {
                    KeysToHash(ak, SelectNextStoneAction);
                }

                ak = loader.GetActionKeys(ActionName.FocusCurrentStone);
                if (ak.Length == 0)
                {
                    loader.AddActionKey(ActionName.FocusCurrentStone, Keys.Alt | Keys.Home);
                    loader.AddActionKey(ActionName.FocusCurrentStone, Keys.Alt | Keys.D5);
                    loader.AddActionKey(ActionName.FocusCurrentStone, Keys.Alt | Keys.NumPad5);

                    actions[Keys.Alt | Keys.Home] = new KeyboardAction(FocusCurrentStoneAction);
                    actions[Keys.Alt | Keys.D5] = new KeyboardAction(FocusCurrentStoneAction);
                    actions[Keys.Alt | Keys.NumPad5] = new KeyboardAction(FocusCurrentStoneAction);
                }
                else
                {
                    KeysToHash(ak, FocusCurrentStoneAction);
                }


                ak = loader.GetActionKeys(ActionName.SaveCircle);
                if (ak.Length == 0)
                {
                    loader.AddActionKey(ActionName.SaveCircle, Keys.Control | Keys.S);

                    actions[Keys.Control | Keys.S] = new KeyboardAction(SaveCurrentCircleAction);
                }
                else
                {
                    KeysToHash(ak, SaveCurrentCircleAction);
                }

                ak = loader.GetActionKeys(ActionName.CloseKrento);
                if (ak.Length == 0)
                {
                    loader.AddActionKey(ActionName.CloseKrento, Keys.Alt | Keys.X);

                    actions[Keys.Alt | Keys.X] = new KeyboardAction(CloseKrentoAction);
                }
                else
                {
                    KeysToHash(ak, CloseKrentoAction);
                }

                ak = loader.GetActionKeys(ActionName.ChangeStoneType);
                if (ak.Length == 0)
                {
                    loader.AddActionKey(ActionName.ChangeStoneType, Keys.Alt | Keys.T);

                    actions[Keys.Alt | Keys.T] = new KeyboardAction(ChangeCurrentStoneType);
                }
                else
                {
                    KeysToHash(ak, ChangeCurrentStoneType);
                }

                ak = loader.GetActionKeys(ActionName.ConfigureStone);
                if (ak.Length == 0)
                {
                    loader.AddActionKey(ActionName.ConfigureStone, Keys.Alt | Keys.O);

                    actions[Keys.Alt | Keys.O] = new KeyboardAction(ConfigureCurrentStone);
                }
                else
                {
                    KeysToHash(ak, ConfigureCurrentStone);
                }

                ak = loader.GetActionKeys(ActionName.StonePopupMenu);
                if (ak.Length == 0)
                {
                    loader.AddActionKey(ActionName.StonePopupMenu, Keys.Alt | Keys.M);

                    actions[Keys.Alt | Keys.M] = new KeyboardAction(ShowStonePopupMenuAction);
                }
                else
                {
                    KeysToHash(ak, ShowStonePopupMenuAction);
                }

                ak = loader.GetActionKeys(ActionName.ManagerScreenTop);
                if (ak.Length == 0)
                {
                    loader.AddActionKey(ActionName.ManagerScreenTop, Keys.Control | Keys.PageUp);
                    loader.AddActionKey(ActionName.ManagerScreenTop, Keys.Control | Keys.D9);
                    loader.AddActionKey(ActionName.ManagerScreenTop, Keys.Control | Keys.NumPad9);

                    actions[Keys.Control | Keys.PageUp] = new KeyboardAction(MoveManagerScreenTopAction);
                    actions[Keys.Control | Keys.D9] = new KeyboardAction(MoveManagerScreenTopAction);
                    actions[Keys.Control | Keys.NumPad9] = new KeyboardAction(MoveManagerScreenTopAction);
                }
                else
                {
                    KeysToHash(ak, MoveManagerScreenTopAction);
                }

                ak = loader.GetActionKeys(ActionName.ManagerScreenBottom);
                if (ak.Length == 0)
                {
                    loader.AddActionKey(ActionName.ManagerScreenBottom, Keys.Control | Keys.PageDown);
                    loader.AddActionKey(ActionName.ManagerScreenBottom, Keys.Control | Keys.D3);
                    loader.AddActionKey(ActionName.ManagerScreenBottom, Keys.Control | Keys.NumPad3);

                    actions[Keys.Control | Keys.PageDown] = new KeyboardAction(MoveManagerScreenBottomAction);
                    actions[Keys.Control | Keys.D3] = new KeyboardAction(MoveManagerScreenBottomAction);
                    actions[Keys.Control | Keys.NumPad3] = new KeyboardAction(MoveManagerScreenBottomAction);
                }
                else
                {
                    KeysToHash(ak, MoveManagerScreenBottomAction);
                }

                ak = loader.GetActionKeys(ActionName.ManagerScreenCenter);
                if (ak.Length == 0)
                {
                    loader.AddActionKey(ActionName.ManagerScreenCenter, Keys.Control | Keys.Home);
                    loader.AddActionKey(ActionName.ManagerScreenCenter, Keys.Control | Keys.D7);
                    loader.AddActionKey(ActionName.ManagerScreenCenter, Keys.Control | Keys.NumPad7);

                    actions[Keys.Control | Keys.Home] = new KeyboardAction(MoveManagerCenterAction);
                    actions[Keys.Control | Keys.D7] = new KeyboardAction(MoveManagerCenterAction);
                    actions[Keys.Control | Keys.NumPad7] = new KeyboardAction(MoveManagerCenterAction);
                }
                else
                {
                    KeysToHash(ak, MoveManagerCenterAction);
                }

                ak = loader.GetActionKeys(ActionName.ManagerScreenLeft);
                if (ak.Length == 0)
                {
                    loader.AddActionKey(ActionName.ManagerScreenLeft, Keys.Control | Keys.Delete);

                    actions[Keys.Control | Keys.Delete] = new KeyboardAction(MoveManagerScreenLeftAction);
                }
                else
                {
                    KeysToHash(ak, MoveManagerScreenLeftAction);
                }

                ak = loader.GetActionKeys(ActionName.ManagerScreenRight);
                if (ak.Length == 0)
                {
                    loader.AddActionKey(ActionName.ManagerScreenRight, Keys.Control | Keys.End);

                    actions[Keys.Control | Keys.End] = new KeyboardAction(MoveManagerScreenRightAction);
                }
                else
                {
                    KeysToHash(ak, MoveManagerScreenRightAction);
                }

                ak = loader.GetActionKeys(ActionName.MoveManagerLeft);
                if (ak.Length == 0)
                {
                    loader.AddActionKey(ActionName.MoveManagerLeft, Keys.Control | Keys.Left);
                    loader.AddActionKey(ActionName.MoveManagerLeft, Keys.Control | Keys.D4);
                    loader.AddActionKey(ActionName.MoveManagerLeft, Keys.Control | Keys.NumPad4);

                    actions[Keys.Control | Keys.Left] = new KeyboardAction(MoveManagerLeftAction);
                    actions[Keys.Control | Keys.D4] = new KeyboardAction(MoveManagerLeftAction);
                    actions[Keys.Control | Keys.NumPad4] = new KeyboardAction(MoveManagerLeftAction);
                }
                else
                {
                    KeysToHash(ak, MoveManagerLeftAction);
                }

                ak = loader.GetActionKeys(ActionName.MoveManagerRight);
                if (ak.Length == 0)
                {
                    loader.AddActionKey(ActionName.MoveManagerRight, Keys.Control | Keys.Right);
                    loader.AddActionKey(ActionName.MoveManagerRight, Keys.Control | Keys.D6);
                    loader.AddActionKey(ActionName.MoveManagerRight, Keys.Control | Keys.NumPad6);

                    actions[Keys.Control | Keys.Right] = new KeyboardAction(MoveManagerRightAction);
                    actions[Keys.Control | Keys.D6] = new KeyboardAction(MoveManagerRightAction);
                    actions[Keys.Control | Keys.NumPad6] = new KeyboardAction(MoveManagerRightAction);
                }
                else
                {
                    KeysToHash(ak, MoveManagerRightAction);
                }

                ak = loader.GetActionKeys(ActionName.MoveManagerUp);
                if (ak.Length == 0)
                {
                    loader.AddActionKey(ActionName.MoveManagerUp, Keys.Control | Keys.Up);
                    loader.AddActionKey(ActionName.MoveManagerUp, Keys.Control | Keys.D8);
                    loader.AddActionKey(ActionName.MoveManagerUp, Keys.Control | Keys.NumPad8);

                    actions[Keys.Control | Keys.Up] = new KeyboardAction(MoveManagerUpAction);
                    actions[Keys.Control | Keys.D8] = new KeyboardAction(MoveManagerUpAction);
                    actions[Keys.Control | Keys.NumPad8] = new KeyboardAction(MoveManagerUpAction);
                }
                else
                {
                    KeysToHash(ak, MoveManagerUpAction);
                }

                ak = loader.GetActionKeys(ActionName.MoveManagerDown);
                if (ak.Length == 0)
                {
                    loader.AddActionKey(ActionName.MoveManagerDown, Keys.Control | Keys.Down);
                    loader.AddActionKey(ActionName.MoveManagerDown, Keys.Control | Keys.D2);
                    loader.AddActionKey(ActionName.MoveManagerDown, Keys.Control | Keys.NumPad2);

                    actions[Keys.Control | Keys.Down] = new KeyboardAction(MoveManagerDownAction);
                    actions[Keys.Control | Keys.D2] = new KeyboardAction(MoveManagerDownAction);
                    actions[Keys.Control | Keys.NumPad2] = new KeyboardAction(MoveManagerDownAction);
                }
                else
                {
                    KeysToHash(ak, MoveManagerDownAction);
                }

                ak = loader.GetActionKeys(ActionName.MoveManagerLeftFast);
                if (ak.Length == 0)
                {
                    loader.AddActionKey(ActionName.MoveManagerLeftFast, Keys.Shift | Keys.Left);
                    loader.AddActionKey(ActionName.MoveManagerLeftFast, Keys.Shift | Keys.D4);
                    loader.AddActionKey(ActionName.MoveManagerLeftFast, Keys.Shift | Keys.NumPad4);

                    actions[Keys.Shift | Keys.Left] = new KeyboardAction(MoveManagerFastLeftAction);
                    actions[Keys.Shift | Keys.D4] = new KeyboardAction(MoveManagerFastLeftAction);
                    actions[Keys.Shift | Keys.NumPad4] = new KeyboardAction(MoveManagerFastLeftAction);
                }
                else
                {
                    KeysToHash(ak, MoveManagerFastLeftAction);
                }

                ak = loader.GetActionKeys(ActionName.MoveManagerRightFast);
                if (ak.Length == 0)
                {
                    loader.AddActionKey(ActionName.MoveManagerRightFast, Keys.Shift | Keys.Right);
                    loader.AddActionKey(ActionName.MoveManagerRightFast, Keys.Shift | Keys.D6);
                    loader.AddActionKey(ActionName.MoveManagerRightFast, Keys.Shift | Keys.NumPad6);

                    actions[Keys.Shift | Keys.Right] = new KeyboardAction(MoveManagerFastRightAction);
                    actions[Keys.Shift | Keys.D6] = new KeyboardAction(MoveManagerFastRightAction);
                    actions[Keys.Shift | Keys.NumPad6] = new KeyboardAction(MoveManagerFastRightAction);
                }
                else
                {
                    KeysToHash(ak, MoveManagerFastRightAction);
                }

                ak = loader.GetActionKeys(ActionName.MoveManagerUpFast);
                if (ak.Length == 0)
                {
                    loader.AddActionKey(ActionName.MoveManagerUpFast, Keys.Shift | Keys.Up);
                    loader.AddActionKey(ActionName.MoveManagerUpFast, Keys.Shift | Keys.D8);
                    loader.AddActionKey(ActionName.MoveManagerUpFast, Keys.Shift | Keys.NumPad8);

                    actions[Keys.Shift | Keys.Up] = new KeyboardAction(MoveManagerFastUpAction);
                    actions[Keys.Shift | Keys.D8] = new KeyboardAction(MoveManagerFastUpAction);
                    actions[Keys.Shift | Keys.NumPad8] = new KeyboardAction(MoveManagerFastUpAction);
                }
                else
                {
                    KeysToHash(ak, MoveManagerFastUpAction);
                }

                ak = loader.GetActionKeys(ActionName.MoveManagerDownFast);
                if (ak.Length == 0)
                {
                    loader.AddActionKey(ActionName.MoveManagerDownFast, Keys.Shift | Keys.Down);
                    loader.AddActionKey(ActionName.MoveManagerDownFast, Keys.Shift | Keys.D2);
                    loader.AddActionKey(ActionName.MoveManagerDownFast, Keys.Shift | Keys.NumPad2);

                    actions[Keys.Shift | Keys.Down] = new KeyboardAction(MoveManagerFastDownAction);
                    actions[Keys.Shift | Keys.D2] = new KeyboardAction(MoveManagerFastDownAction);
                    actions[Keys.Shift | Keys.NumPad2] = new KeyboardAction(MoveManagerFastDownAction);
                }
                else
                {
                    KeysToHash(ak, MoveManagerFastDownAction);
                }

                ak = loader.GetActionKeys(ActionName.ShowHelp);
                if (ak.Length == 0)
                {
                    loader.AddActionKey(ActionName.ShowHelp, Keys.F1);

                    actions[Keys.F1] = new KeyboardAction(ShowHelpAction);
                }
                else
                {
                    KeysToHash(ak, ShowHelpAction);
                }

                ak = loader.GetActionKeys(ActionName.BackToDefaultCircle);
                if (ak.Length == 0)
                {
                    loader.AddActionKey(ActionName.BackToDefaultCircle, Keys.Back);

                    actions[Keys.Back] = new KeyboardAction(BackToDefaultCircleAction);

                    loader.AddActionKey(ActionName.BackToDefaultCircle, Keys.Home);

                    actions[Keys.Home] = new KeyboardAction(BackToDefaultCircleAction);

                }
                else
                {
                    KeysToHash(ak, BackToDefaultCircleAction);
                }

                ak = loader.GetActionKeys(ActionName.LoadCircle);
                if (ak.Length == 0)
                {
                    loader.AddActionKey(ActionName.LoadCircle, Keys.Control | Keys.O);

                    actions[Keys.Control | Keys.O] = new KeyboardAction(SelectCircleAction);
                }
                else
                {
                    KeysToHash(ak, SelectCircleAction);
                }

                //--skip
                actions[Keys.Apps] = new KeyboardAction(ShowPopupMenuAction);
                actions[Keys.Alt | Keys.Space] = new KeyboardAction(ShowPopupMenuAction);
                //--skip

                //--skip
                actions[Keys.Control | Keys.F1] = new KeyboardAction(ExecuteStoneAction);
                actions[Keys.Control | Keys.F2] = new KeyboardAction(ExecuteStoneAction);
                actions[Keys.Control | Keys.F3] = new KeyboardAction(ExecuteStoneAction);
                actions[Keys.Control | Keys.F4] = new KeyboardAction(ExecuteStoneAction);
                actions[Keys.Control | Keys.F5] = new KeyboardAction(ExecuteStoneAction);
                actions[Keys.Control | Keys.F6] = new KeyboardAction(ExecuteStoneAction);
                actions[Keys.Control | Keys.F7] = new KeyboardAction(ExecuteStoneAction);
                actions[Keys.Control | Keys.F8] = new KeyboardAction(ExecuteStoneAction);
                actions[Keys.Control | Keys.F9] = new KeyboardAction(ExecuteStoneAction);
                actions[Keys.Control | Keys.F10] = new KeyboardAction(ExecuteStoneAction);
                actions[Keys.Control | Keys.F11] = new KeyboardAction(ExecuteStoneAction);
                actions[Keys.Control | Keys.F12] = new KeyboardAction(ExecuteStoneAction);

                actions[Keys.Control | Keys.Alt | Keys.F1] = new KeyboardAction(ExecuteRingAction);
                actions[Keys.Control | Keys.Alt | Keys.F2] = new KeyboardAction(ExecuteRingAction);
                actions[Keys.Control | Keys.Alt | Keys.F3] = new KeyboardAction(ExecuteRingAction);
                actions[Keys.Control | Keys.Alt | Keys.F4] = new KeyboardAction(ExecuteRingAction);
                actions[Keys.Control | Keys.Alt | Keys.F5] = new KeyboardAction(ExecuteRingAction);
                actions[Keys.Control | Keys.Alt | Keys.F6] = new KeyboardAction(ExecuteRingAction);
                actions[Keys.Control | Keys.Alt | Keys.F7] = new KeyboardAction(ExecuteRingAction);
                actions[Keys.Control | Keys.Alt | Keys.F8] = new KeyboardAction(ExecuteRingAction);
                actions[Keys.Control | Keys.Alt | Keys.F9] = new KeyboardAction(ExecuteRingAction);
                actions[Keys.Control | Keys.Alt | Keys.F10] = new KeyboardAction(ExecuteRingAction);
                actions[Keys.Control | Keys.Alt | Keys.F11] = new KeyboardAction(ExecuteRingAction);
                actions[Keys.Control | Keys.Alt | Keys.F12] = new KeyboardAction(ExecuteRingAction);
                //--skip

                loader.Save();
            }
        }

        #region Actions

        void BackToDefaultCircleAction(Keys keys)
        {
            StopRotation();
            manager.BackToDefaultCircle();
        }

        void HistoryBackAction(Keys keys)
        {
            StopRotation();
            manager.HistoryBack();
        }

        void SelectCircleAction(Keys keys)
        {
            StopRotation();
            manager.SwitchRing();
        }

        void SaveCurrentCircleAction(Keys keys)
        {
            StopRotation();
            manager.FlushCurrentCircle();
        }

        void ShowPopupMenuAction(Keys keys)
        {
            StopRotation();
            if (manager.PopupMenu == null)
                return;

            if (manager.PopupMenu.Active)
                manager.PopupMenu.CloseUp();
            else
                manager.ShowPopupMenu();
        }


        public void HideManager()
        {
            StopRotation();
            if (GlobalConfig.LowMemory)
            {
                manager.HideAll();
                manager.ChangeVisibility(false);
            }
            else
            {
                manager.Fade();
            }
            manager.RestoreFocusedWindow();
        }

        /// <summary>
        /// Hides the manager action.
        /// </summary>
        /// <param name="keys">The keys.</param>
        void HideManagerAction(Keys keys)
        {
            HideManager();
        }

        internal void RotateLeftAction(Keys keys)
        {
            if (manager.RollingDirection != RollingDirection.Left)
            {
                StopRotation();
                manager.RollingDirection = RollingDirection.Left;
            }
            StartRotation();
        }

        internal void RotateRightAction(Keys keys)
        {
            if (manager.RollingDirection != RollingDirection.Right)
            {
                StopRotation();
                manager.RollingDirection = RollingDirection.Right;
            }
            StartRotation();
        }

        void RotateNextStone(Keys keys)
        {
            manager.RollingDirection = RollingDirection.Right;
            manager.Rotate(10);
            manager.Update();
            manager.Rotate(10);
            manager.Update();
            manager.Rotate(10);
        }

        void RotatePreviousStone(Keys keys)
        {
            manager.RollingDirection = RollingDirection.Left;
            manager.Rotate(10);
            manager.Update();
            manager.Rotate(10);
            manager.Update();
            manager.Rotate(10);
        }

        void RunCurrentStoneAction(Keys keys)
        {
            StopRotation();
            if (!manager.Fading)
                manager.Run();
        }

        void CloseKrentoAction(Keys keys)
        {
            StopRotation();
            NativeMethods.ExitKrento();
        }

        void ShowHelpAction(Keys keys)
        {
            manager.ShowHelpPages();
        }

        void ConfigureCurrentStone(Keys keys)
        {
            StopRotation();
            if (manager.ActiveStone != null)
            {
                manager.ActiveStone.Configure();
            }
        }

        void ChangeCurrentStoneType(Keys keys)
        {
            StopRotation();
            if (manager.ActiveStone != null)
            {
                manager.ChangeStoneType(manager.ActiveStone.Order);
            }
        }

        void ShowStonePopupMenuAction(Keys keys)
        {
            StopRotation();
            if (manager.ActiveStone != null)
            {
                manager.StoneMenu.PopupAt(manager.ActiveStone.Left + (int)(manager.ActiveStone.Size.Width / 2), manager.ActiveStone.Top + (int)(manager.ActiveStone.Size.Height / 2));
            }
        }

        void SelectNextStoneAction(Keys keys)
        {
            manager.SelectNextStone();
        }

        void SelectPreviousStoneAction(Keys keys)
        {
            manager.SelectPrevStone();
        }

        void FocusCurrentStoneAction(Keys keys)
        {
            if (manager.ActiveStone != null)
            {
                manager.RollingDirection = RollingDirection.Left;
                manager.Rotate((int)manager.ActiveStone.Angle);
            }
        }

        void MoveManagerLeftAction(Keys keys)
        {
            int moveSpeed = acceleration;
            if (manager.Left > 0)
                manager.Left -= moveSpeed;
        }

        void MoveManagerRightAction(Keys keys)
        {
            int moveSpeed = acceleration;
            if (manager.Left < PrimaryScreen.Bounds.Right)
                manager.Left += moveSpeed;
        }

        void MoveManagerUpAction(Keys keys)
        {
            int moveSpeed = acceleration;
            if (manager.Top > 0)
                manager.Top -= moveSpeed;
        }

        void MoveManagerDownAction(Keys keys)
        {
            int moveSpeed = acceleration;
            if (manager.Top < PrimaryScreen.Bounds.Bottom)
                manager.Top += moveSpeed;
        }


        void MoveManagerFastLeftAction(Keys keys)
        {
            int moveSpeed = acceleration * 6;
            if (manager.Left > 0)
                manager.Left -= moveSpeed;
        }

        void MoveManagerFastRightAction(Keys keys)
        {
            int moveSpeed = acceleration * 6;
            if (manager.Left < PrimaryScreen.Bounds.Right)
                manager.Left += moveSpeed;
        }

        void MoveManagerFastUpAction(Keys keys)
        {
            int moveSpeed = acceleration * 6;
            if (manager.Top > 0)
                manager.Top -= moveSpeed;
        }

        void MoveManagerFastDownAction(Keys keys)
        {
            int moveSpeed = acceleration * 6;
            if (manager.Top < PrimaryScreen.Bounds.Bottom)
                manager.Top += moveSpeed;
        }

        void MoveManagerCenterAction(Keys keys)
        {
            manager.Position = PrimaryScreen.Center;
        }

        void MoveManagerScreenTopAction(Keys keys)
        {
            manager.Top = 0;
        }

        void MoveManagerScreenBottomAction(Keys keys)
        {
            manager.Top = PrimaryScreen.Bounds.Bottom;
        }

        void MoveManagerScreenLeftAction(Keys keys)
        {
            manager.Left = 0;
        }

        void MoveManagerScreenRightAction(Keys keys)
        {
            manager.Left = PrimaryScreen.Bounds.Right;
        }

        void ExecuteRingAction(Keys keys)
        {
            StopRotation();
            Keys func = keys & ~Keys.Control & ~Keys.Alt;
            int ringNumber = (int)func - 112;
            string ringName = manager.HistoryRingName(ringNumber);
            if (!string.IsNullOrEmpty(ringName))
            {
                manager.ReplaceCurrentCircle(ringName);
            }
        }

        /// <summary>
        /// Execute the stone 
        /// </summary>
        /// <param name="keys">The keys.</param>
        void ExecuteStoneAction(Keys keys)
        {
            if (manager.Fading)
                return;

            Keys func = keys & ~Keys.Control;

            switch (func)
            {
                case Keys.F1:
                    manager.SetCurrentStone(0);
                    manager.Run();
                    break;
                case Keys.F2:
                    manager.SetCurrentStone(1);
                    manager.Run();
                    break;
                case Keys.F3:
                    manager.SetCurrentStone(2);
                    manager.Run();
                    break;
                case Keys.F4:
                    manager.SetCurrentStone(3);
                    manager.Run();
                    break;
                case Keys.F5:
                    manager.SetCurrentStone(4);
                    manager.Run();
                    break;
                case Keys.F6:
                    manager.SetCurrentStone(5);
                    manager.Run();
                    break;
                case Keys.F7:
                    manager.SetCurrentStone(6);
                    manager.Run();
                    break;
                case Keys.F8:
                    manager.SetCurrentStone(7);
                    manager.Run();
                    break;
                case Keys.F9:
                    manager.SetCurrentStone(8);
                    manager.Run();
                    break;
                case Keys.F10:
                    manager.SetCurrentStone(9);
                    manager.Run();
                    break;
                case Keys.F11:
                    manager.SetCurrentStone(10);
                    manager.Run();
                    break;
                case Keys.F12:
                    manager.SetCurrentStone(11);
                    manager.Run();
                    break;
            }
        }

        #endregion

        private KeyboardAction FindAction(Keys keys)
        {
            if (actions.ContainsKey(keys))
                return actions[keys];
            else
                return null;
        }


        internal void ProcessKeyDown(KeyEventArgs e)
        {
            OnKeyDown(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if (!GlobalSettings.ShowManagerButtons)
                manager.RedrawScreenHint();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (GlobalSettings.ShowManagerButtons)
            {
                if (buttons != null)
                    buttons.MouseLeave();
            }
            else
            {
                manager.RedrawScreenHint();
            }
        }

        protected override void OnMoveDelta(MoveDeltaEventArgs e)
        {
            base.OnMoveDelta(e);
            manager.MoveTo(manager.Position.X + e.DeltaX, manager.Position.Y + e.DeltaY);
        }


        internal void SimulateWheelRotation()
        {
            MouseEventArgs e = new MouseEventArgs(MouseButtons.Middle, 0, 0, 0, -120);
            OnMouseWheel(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            WheelRotation(e.Delta, accelerationLimit);
            base.OnMouseWheel(e);
        }

        internal void WheelRotation(int delta, int acc)
        {
            StopTimer(wheelDelayTimer);
            int scrollSpeed = delta;
            if (scrollSpeed < 0)
                manager.RollingDirection = RollingDirection.Left;
            else
                manager.RollingDirection = RollingDirection.Right;

            acceleration = acc;
            StartRotation();
            StartTimer(wheelDelayTimer, rotateInterval * 15);
        }

        internal void VirtualClick()
        {
            if (manager.Count > 0)
            {
                virtualClick = true;
                VirtualMouse.LeftClick();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (GlobalSettings.ShowManagerButtons)
            {
                if (this.Focused)
                {
                    if (buttons != null)
                        buttons.OnMouseMove(e);
                }
            }
            base.OnMouseMove(e);
        }


        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (GlobalSettings.ShowManagerButtons)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (buttons != null)
                        buttons.OnMouseUp(e);
                }
            }

            manager.RedrawScreenHint();
            virtualClick = false;
            base.OnMouseUp(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            StopRotation();

            if (GlobalSettings.ShowManagerButtons)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (buttons != null)
                        buttons.OnMouseDown(e);
                }
            }
            base.OnMouseDown(e);
            manager.HideButtonHint();
            virtualClick = false;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            Point clickPoint = new Point(e.X, e.Y);

            if ((!virtualClick) && (!manager.Fading))
            {
                if (GlobalSettings.ShowManagerButtons)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        if (buttons != null)
                            buttons.OnClick(e);
                    }
                }
                base.OnMouseClick(e);
            }
            else
                TraceDebug.Trace("Fading, skipped");
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            StartTimer(foregroundTimer, 500);
        }

        protected override void OnClose(EventArgs e)
        {
            base.OnClose(e);
            NativeMethods.ExitKrento();
        }

        public override void BringToFront()
        {
            if (!this.Visible)
                return;
            NativeMethods.BringWindowToFront(this.Handle);
        }
        /// <summary>
        /// Clears the unused memory.
        /// </summary>
        public void ClearUnusedMemory()
        {
            try
            {
                //NativeMethods.SetProcessWorkingSetSize(currentProcessHandle, (IntPtr)(-1), (IntPtr)(-1));

                NativeMethods.ClearUnusedMemory();
                // Never ever do GC here!!!
                //GC.GetTotalMemory(true);
            }
            catch (Exception ex)
            {
                TraceDebug.Trace(ex.Message);
                StopTimer(memoryTimer);
            }
        }

        protected override void HandleTimerTick(int timerNumber)
        {

            switch (timerNumber)
            {
                case memoryTimer:
                    {
                        ClearUnusedMemory();
                        break;
                    }
                case foregroundTimer:
                    {
                        StopTimer(foregroundTimer);
                        IntPtr foreWindow = NativeMethods.GetForegroundWindow();
                        if ((foreWindow != this.Handle) && (foreWindow != manager.PopupMenu.Handle))
                        {
                            this.BringToFront();
                        }
                        break;
                    }
                case wheelDelayTimer:
                    {
                        StopTimer(wheelDelayTimer);
                        rotationActive = false;
                        break;
                    }
                case rotateTimer:
                    {
                        int angle = speedCounter / 180 + 1;

                        if (rotationActive)
                        {
                            manager.Rotate(angle);
                            Thread.Sleep(0);
                            //for (int i = 0; i < angle; i++)
                            //{
                            //    manager.Rotate(1);

                            //    Thread.Sleep(0);
                            //}

                            if (acceleration < accelerationLimit)
                                acceleration += 1;
                            speedCounter += (acceleration * acceleration);
                            if (speedCounter >= speedLimit)
                                speedCounter = speedLimit;

                        }
                        else
                        {
                            manager.Rotate(angle);
                            Thread.Sleep(0);
                            //for (int i = 0; i < angle; i++)
                            //{
                            //    manager.Rotate(1);

                            //    Thread.Sleep(0);
                            //}

                            //if (acceleration <= accelerationLimit)
                            //    acceleration = accelerationLimit;
                            //else
                            if (speedCounter < 60)
                            {
                                if (acceleration > 1)
                                    acceleration -= 1;
                                speedCounter -= acceleration;
                            }
                            else
                                speedCounter -= (acceleration * acceleration);

                            //TraceDebug.Trace(speedCounter.ToString());

                            if (speedCounter <= 0)
                            {
                                StopRotation();
                                manager.BringToFront();
                            }


                        }

                        break;
                    }
            }
        }

        public void StopRotation()
        {
            StopRotation(true);
        }

        public void StopRotation(bool dispayStoneHint)
        {
            lock (this)
            {
                StopTimer(rotateTimer);
                rotationActive = false;
                speedCounter = 0;
                acceleration = 1;
                if (dispayStoneHint)
                    manager.ShowScreenHintIfNeeded();
            }
        }

        public int SpeedCounter
        {
            get { return speedCounter; }
        }

        public void StartRotation()
        {
            if (!rotationActive)
            {
                rotationActive = true;
                StartTimer(rotateTimer, rotateInterval);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            // StopRotation();
            rotationActive = false;
            base.OnKeyUp(e);
        }

        internal void SwitchRing()
        {
            StopRotation();
            if (taskManager == null)
                taskManager = new RingSwitcher(manager, true);

            if (taskManager.Switch(manager.Left, manager.Top))
            {
                string ringName = manager.HistoryRingName(taskManager.SelectedIndex);
                if (!string.IsNullOrEmpty(ringName))
                {
                    manager.ReplaceCurrentCircle(ringName);
                }
            }
            else
            {
                manager.BringToFront();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (FadeActive)
                return;

            if (e.Control && (e.KeyCode == Keys.Tab))
            {
                SwitchRing();
                return;
            }

            KeyboardAction action = FindAction(e.KeyData);
            if (action != null)
            {
                action(e.KeyData);
                return;
            }
            else
            {
                if (manager.PopupMenu != null)
                {
                    KrentoMenuItem item = manager.PopupMenu.FindItemByShortCut(e.KeyData);
                    if (item != null)
                    {
                        manager.PopupMenu.Revalidate();
                        manager.PopupMenu.Execute(item);
                        return;
                    }
                }
            }

            base.OnKeyDown(e);
        }



        /// <summary>
        /// Loads the core Krento background.
        /// </summary>
        /// <param name="skin">The skin.</param>
        public void LoadKrentoCoreBackground(KrentoCoreSkin skin)
        {
            if (skin == null)
            {
                if (GlobalSettings.ShowManagerButtons)
                    LoadButtons();
                LoadOptimizedDefaultBackground();
            }
            else
            {
                skinMarginLeft = skin.TotalLeft;
                skinMarginRight = skin.TotalRight;
                skinMarginTop = skin.TotalTop;
                skinMarginBottom = skin.TotalBottom;
                if (GlobalSettings.ShowManagerButtons)
                    LoadButtonsSkin(skin);
                LoadOptimizedBackgroundImage(skin.Background);
            }
        }

        internal void LoadButtonsSkin(KrentoCoreSkin skin)
        {
            if (!GlobalSettings.ShowManagerButtons)
                return;
            LoadButtons();
            
            UIButton button;
            buttonSize = skin.ButtonSize;
            if (buttonSize * 5 > Width)
                buttonSize = Width / 5;
            Bitmap skinButton;
            
            button = (UIButton)buttons["Search"];
            if (button != null)
            {
                skinButton = FastBitmap.FromFile(skin.ButtonStartMenuNormal);
                if (skinButton != null)
                {
                    if (button.NormalFace != null)
                    {
                        button.NormalFace.Dispose();
                        button.NormalFace = null;
                    }
                    button.NormalFace = BitmapPainter.ResizeBitmap(skinButton, buttonSize, buttonSize, true);
                }

                skinButton = FastBitmap.FromFile(skin.ButtonStartMenuFocused);
                if (skinButton != null)
                {
                    if (button.FocusedFace != null)
                    {
                        button.FocusedFace.Dispose();
                        button.FocusedFace = null;
                    }
                    button.FocusedFace = BitmapPainter.ResizeBitmap(skinButton, buttonSize, buttonSize, true);
                }

                skinButton = FastBitmap.FromFile(skin.ButtonStartMenuPressed);
                if (skinButton != null)
                {
                    if (button.PressedFace != null)
                    {
                        button.PressedFace.Dispose();
                        button.PressedFace = null;
                    }
                    button.PressedFace = BitmapPainter.ResizeBitmap(skinButton, buttonSize, buttonSize, true);
                }
            }

            button = (UIButton)buttons["Power"];
            if (button != null)
            {
                skinButton = FastBitmap.FromFile(skin.ButtonPowerNormal);
                if (skinButton != null)
                {
                    if (button.NormalFace != null)
                    {
                        button.NormalFace.Dispose();
                        button.NormalFace = null;
                    }
                    button.NormalFace = BitmapPainter.ResizeBitmap(skinButton, buttonSize, buttonSize, true);
                }

                skinButton = FastBitmap.FromFile(skin.ButtonPowerFocused);
                if (skinButton != null)
                {
                    if (button.FocusedFace != null)
                    {
                        button.FocusedFace.Dispose();
                        button.FocusedFace = null;
                    }
                    button.FocusedFace = BitmapPainter.ResizeBitmap(skinButton, buttonSize, buttonSize, true);
                }

                skinButton = FastBitmap.FromFile(skin.ButtonPowerPressed);
                if (skinButton != null)
                {
                    if (button.PressedFace != null)
                    {
                        button.PressedFace.Dispose();
                        button.PressedFace = null;
                    }
                    button.PressedFace = BitmapPainter.ResizeBitmap(skinButton, buttonSize, buttonSize, true);
                }

            }

            button = (UIButton)buttons["Launcher"];
            if (button != null)
            {
                skinButton = FastBitmap.FromFile(skin.ButtonLauncherNormal);
                if (skinButton != null)
                {
                    if (button.NormalFace != null)
                    {
                        button.NormalFace.Dispose();
                        button.NormalFace = null;
                    }
                    button.NormalFace = BitmapPainter.ResizeBitmap(skinButton, buttonSize, buttonSize, true);
                }

                skinButton = FastBitmap.FromFile(skin.ButtonLauncherFocused);
                if (skinButton != null)
                {
                    if (button.FocusedFace != null)
                    {
                        button.FocusedFace.Dispose();
                        button.FocusedFace = null;
                    }
                    button.FocusedFace = BitmapPainter.ResizeBitmap(skinButton, buttonSize, buttonSize, true);
                }

                skinButton = FastBitmap.FromFile(skin.ButtonLauncherPressed);
                if (skinButton != null)
                {
                    if (button.PressedFace != null)
                    {
                        button.PressedFace.Dispose();
                        button.PressedFace = null;
                    }
                    button.PressedFace = BitmapPainter.ResizeBitmap(skinButton, buttonSize, buttonSize, true);
                }
            }

            button = (UIButton)buttons["Settings"];
            if (button != null)
            {
                skinButton = FastBitmap.FromFile(skin.ButtonSettingsNormal);
                if (skinButton != null)
                {
                    if (button.NormalFace != null)
                    {
                        button.NormalFace.Dispose();
                        button.NormalFace = null;
                    }
                    button.NormalFace = BitmapPainter.ResizeBitmap(skinButton, buttonSize, buttonSize, true);
                }

                skinButton = FastBitmap.FromFile(skin.ButtonSettingsFocused);
                if (skinButton != null)
                {
                    if (button.FocusedFace != null)
                    {
                        button.FocusedFace.Dispose();
                        button.FocusedFace = null;
                    }
                    button.FocusedFace = BitmapPainter.ResizeBitmap(skinButton, buttonSize, buttonSize, true);
                }

                skinButton = FastBitmap.FromFile(skin.ButtonSettingsPressed);
                if (skinButton != null)
                {
                    if (button.PressedFace != null)
                    {
                        button.PressedFace.Dispose();
                        button.PressedFace = null;
                    }
                    button.PressedFace = BitmapPainter.ResizeBitmap(skinButton, buttonSize, buttonSize, true);
                }
            }

            button = (UIButton)buttons["Home"];
            if (button != null)
            {
                skinButton = FastBitmap.FromFile(skin.ButtonHomeNormal);
                if (skinButton != null)
                {
                    if (button.NormalFace != null)
                    {
                        button.NormalFace.Dispose();
                        button.NormalFace = null;
                    }
                    button.NormalFace = BitmapPainter.ResizeBitmap(skinButton, buttonSize, buttonSize, true);
                }

                skinButton = FastBitmap.FromFile(skin.ButtonHomeFocused);
                if (skinButton != null)
                {
                    if (button.FocusedFace != null)
                    {
                        button.FocusedFace.Dispose();
                        button.FocusedFace = null;
                    }
                    button.FocusedFace = BitmapPainter.ResizeBitmap(skinButton, buttonSize, buttonSize, true);
                }

                skinButton = FastBitmap.FromFile(skin.ButtonHomePressed);
                if (skinButton != null)
                {
                    if (button.PressedFace != null)
                    {
                        button.PressedFace.Dispose();
                        button.PressedFace = null;
                    }
                    button.PressedFace = BitmapPainter.ResizeBitmap(skinButton, buttonSize, buttonSize, true);
                }
            }
        }

        internal void LoadOptimizedDefaultBackground()
        {

            int bMargin = skinMarginDefault;
            skinMarginLeft = bMargin;
            skinMarginRight = bMargin;
            skinMarginTop = bMargin;
            skinMarginBottom = bMargin;
            buttonSize = 60;
            Bitmap skinBackground = NativeThemeManager.LoadBitmap("DefaultBackground.png");
            LoadOptimizedBackgroundImage(skinBackground);
            skinBackground.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (backgroundImage != null)
                    {
                        backgroundImage.Dispose();
                        backgroundImage = null;
                    }

                    if (taskManager != null)
                    {
                        taskManager.Dispose();
                        taskManager = null;
                    }
                }

                StopTimer(memoryTimer);
                StopRotation();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        internal void LoadOptimizedBackgroundImage(Bitmap original)
        {
            UIButton button;
            int bHeight = Height;
            int bWidth = Width;

            int spacing;
            int leftBorder;
            int btnWidth;

            if (GlobalSettings.ShowManagerButtons)
            {
                button = (UIButton)buttons["Launcher"];
                btnWidth = button.Width;
                spacing = (Width - btnWidth * 5) / 2;
                leftBorder = spacing;

                button = (UIButton)buttons["Search"];
                button.AdjustBounds(new Rectangle(leftBorder, (Height - button.Height) / 2, button.Width, button.Height));
                leftBorder = leftBorder + btnWidth;

                button = (UIButton)buttons["Settings"];
                button.AdjustBounds(new Rectangle(leftBorder, (Height - button.Height) / 2, button.Width, button.Height));
                leftBorder = leftBorder + btnWidth;

                button = (UIButton)buttons["Launcher"];
                button.AdjustBounds(new Rectangle(leftBorder, (Height - button.Height) / 2, button.Width, button.Height));
                leftBorder = leftBorder + btnWidth;

                button = (UIButton)buttons["Home"];
                button.AdjustBounds(new Rectangle(leftBorder, (Height - button.Height) / 2, button.Width, button.Height));
                leftBorder = leftBorder + btnWidth;

                button = (UIButton)buttons["Power"];
                button.AdjustBounds(new Rectangle(leftBorder, (Height - button.Height) / 2, button.Width, button.Height));
            }

            if (backgroundImage != null)
            {
                backgroundImage.Dispose();
                backgroundImage = null;
            }

            backgroundImage = new Bitmap(Width, Height, PixelFormat.Format32bppPArgb);

            using (Graphics g = Graphics.FromImage(backgroundImage))
            {
                g.SmoothingMode = smoothingMode;
                g.CompositingQuality = compositingQuality;
                g.InterpolationMode = interpolationMode;
                g.CompositingMode = compositingMode;

                //Draw left part of the image
                g.DrawImage(original, new Rectangle(0, 0, skinMarginLeft, skinMarginTop), new Rectangle(0, 0, skinMarginLeft, skinMarginTop), GraphicsUnit.Pixel);
                g.DrawImage(original, new Rectangle(0, bHeight - skinMarginBottom, skinMarginLeft, skinMarginBottom), new Rectangle(0, original.Height - skinMarginBottom, skinMarginLeft, skinMarginBottom), GraphicsUnit.Pixel);
                g.DrawImage(original, new Rectangle(0, skinMarginTop, skinMarginLeft, bHeight - skinMarginTop - skinMarginBottom), new Rectangle(0, skinMarginTop, skinMarginLeft, original.Height - skinMarginTop - skinMarginBottom), GraphicsUnit.Pixel);

                //Draw central part of the image
                g.DrawImage(original, new Rectangle(skinMarginLeft, 0, bWidth - skinMarginLeft - skinMarginRight, skinMarginTop), new Rectangle(skinMarginLeft, 0, original.Width - skinMarginLeft - skinMarginRight, skinMarginTop), GraphicsUnit.Pixel);
                g.DrawImage(original, new Rectangle(skinMarginLeft, bHeight - skinMarginBottom, bWidth - skinMarginLeft - skinMarginRight, skinMarginBottom), new Rectangle(skinMarginLeft, original.Height - skinMarginBottom, original.Width - skinMarginLeft - skinMarginRight, skinMarginBottom), GraphicsUnit.Pixel);
                g.DrawImage(original, new Rectangle(skinMarginLeft, skinMarginTop, bWidth - skinMarginLeft - skinMarginRight, bHeight - skinMarginTop - skinMarginBottom), new Rectangle(skinMarginLeft, skinMarginTop, original.Width - skinMarginLeft - skinMarginRight, original.Height - skinMarginTop - skinMarginBottom), GraphicsUnit.Pixel);

                //Draw right part of the image
                g.DrawImage(original, new Rectangle(bWidth - skinMarginRight, 0, skinMarginRight, skinMarginTop), new Rectangle(original.Width - skinMarginRight, 0, skinMarginRight, skinMarginTop), GraphicsUnit.Pixel);
                g.DrawImage(original, new Rectangle(bWidth - skinMarginRight, bHeight - skinMarginBottom, skinMarginRight, skinMarginBottom), new Rectangle(original.Width - skinMarginRight, original.Height - skinMarginBottom, skinMarginRight, skinMarginBottom), GraphicsUnit.Pixel);
                g.DrawImage(original, new Rectangle(bWidth - skinMarginRight, skinMarginTop, skinMarginRight, bHeight - skinMarginTop - skinMarginBottom), new Rectangle(original.Width - skinMarginRight, skinMarginTop, skinMarginRight, original.Height - skinMarginTop - skinMarginBottom), GraphicsUnit.Pixel);
            }
        }


        public void DrawDefaultBackground(bool update)
        {

            this.Clear();

            GraphicsContainer container = Canvas.BeginContainer();
            try
            {
                Canvas.SmoothingMode = smoothingMode;
                Canvas.CompositingQuality = compositingQuality;
                Canvas.InterpolationMode = interpolationMode;
                Canvas.CompositingMode = compositingMode;

                if (backgroundImage != null)
                {
                    Canvas.DrawImageUnscaled(backgroundImage, 0, 0);
                }
            }
            finally
            {
                Canvas.EndContainer(container);
            }

            if (GlobalSettings.ShowManagerButtons)
            {
                if (buttons != null)
                {
                    buttons.OnRender(Canvas);
                }
            }

            if (update)
                this.Update();

        }

    }
}
