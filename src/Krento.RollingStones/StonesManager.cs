//===============================================================================
// Copyright (c) Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

//History:
//
// 11.06.2010: Added DrawBackground property to stones
//

using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using Laugris.Sage;
using System.Threading;
using System.Reflection;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Collections;
using System.IO;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace Krento.RollingStones
{

    #region Extra types
    /// <summary>
    /// Paint mode for stone: painting within the manager window or inside the stone
    /// </summary>
    public enum PaintMode { Manager, Stone }

    /// <summary>
    /// Called when fading of the window is finished
    /// </summary>
    public delegate void FadeHookDelegate();

    /// <summary>
    /// Rotation direction
    /// </summary>
    public enum RollingDirection { Left, Right }

    /// <summary>
    /// Location of the Stone Manager on the screen at the moment it shows
    /// </summary>
    public enum CircleLocation { Point, ScreenCenter, Fixed }

    internal class TypeObjectEntry
    {
        internal Type Type;
        internal object Value;
        internal string CustomIcon;
        internal string Author;
        internal bool BuiltIn;
        internal string Version;
        internal string Copyright;

        internal TypeObjectEntry(Type type, object value, string customIcon)
        {
            this.Type = type;
            this.Value = value;
            this.CustomIcon = customIcon;
            this.BuiltIn = true;
            this.Author = "Serhiy Perevoznyk";
            this.Version = Application.ProductVersion;
            this.Copyright = "Copyright © Serhiy Perevoznyk";
        }

        internal TypeObjectEntry(Type type, object value, string customIcon, string author, string version, string copyright)
        {
            this.Type = type;
            this.Value = value;
            this.CustomIcon = customIcon;
            this.BuiltIn = false;
            this.Author = author;
            this.Version = version;
            this.Copyright = copyright;
        }

    }

    #endregion

    /// <summary>
    /// Manages "rolling stones"
    /// </summary>
    public sealed class StonesManager : IDisposable, IEnumerable, IAnimationListener
    {
        #region Do not change this
        private const double verticalMultiplex = 0.71;
        private const double horizontalMultiplex = 1.22;
        private const int defaultMargin = 20;
        #endregion

        private IntPtr stoneBackgroundBitmap;
        private int stoneBackgroundWidth;
        private int stoneBackgroundHeight;

        private Bitmap indicator;

        private FadeHookDelegate fadeHook;
        private int tag;
        private int updateCount;
        private RollingStoneBase activeStone;
        private ManagerWindow managerWindow;
        private CircleLocation circleLocation;

        private StartMenuDialog startMenu;
        private KrentoMenu skinsMenu;
        private KrentoMenu popupMenu;
        private KrentoMenu stoneMenu;

        private DeleteWindow deleteWindow;

        private int fadeDelay = 500;
        private int visibleWindowWidth = 300;
        private int visibleWindowHeight = 80;
        private int adjustedWindowWidth = 300;

        private const int defaultSpacing = 10;

        private int hSpacing = defaultSpacing;
        private int vSpacing = defaultSpacing;

        private int textOffset = 8;
        private int transparency = 230;

        #region Stone Margins
        private int stoneMarginLeft = defaultMargin;
        private int stoneMarginTop = defaultMargin;
        private int stoneMarginBottom = defaultMargin;
        private int stoneMarginRight = defaultMargin;
        #endregion

        private bool inRotation;
        private bool isFading;
        private bool movingStones;
        private string skinFileName;
        private int storedAngle;
        private NameObjectCollection availableStoneClasses;

        private StonesCollection stones;
        private StonesCollection realStones;
        private StonesCollection virtualStones;

        private bool visible;

        private int stoneSize = 128;
        private int stoneBorder;

        private double radiusVertical;
        private double radiusHorizontal;
        private int radius;

        private ScaleLimit scale = new ScaleLimit(0.8, 1.2);

        private int speed = 1;

        private RollingDirection rollingDirection;

        private int left;
        private int top;
        private bool mouseInsideStone;

        private IHookMessage hookMessage;

        private string currentCircle;
        private string previousCircle;

        private HistoryList history;


        private double halfPlusScale;
        private double halfMinScale;

        private Color foreColor = Color.White;

        private Font font = new Font("Tahoma", 14, FontStyle.Bold, GraphicsUnit.Pixel);

        private KrentoHint managerHintWindow;
        private KrentoHint buttonHintWindow;

        private RingSwitcher taskManager;

        private IntPtr focusedWindow;

        private ScreenHint screenHint;

        private Type[] parameterList = new Type[] { typeof(StonesManager) };

        private int messageID;

        /// <summary>
        /// Initializes a new instance of the <see cref="StonesManager"/> class.
        /// </summary>
        public StonesManager()
        {
            messageID = NativeMethods.RegisterWindowMessage("KrentoSkin"); //Do not localize!!!
            NativeMethods.AddRemoveMessageFilter(messageID, ChangeWindowMessageFilterFlags.Add);

            radius = 200;

            if (GlobalSettings.FlatRing)
            {
                radiusHorizontal = radius;
                radiusVertical = radius;
            }
            else
            {
                radiusHorizontal = radius * horizontalMultiplex;
                radiusVertical = radius * verticalMultiplex;
            }

            this.deleteWindow = new DeleteWindow();
            this.managerWindow = new ManagerWindow(this);
            this.managerHintWindow = new KrentoHint();

            this.buttonHintWindow = new KrentoHint();
            this.screenHint = new ScreenHint();

            if (InteropHelper.IsTabletPC)
                screenHint.MoveDelta += new EventHandler<MoveDeltaEventArgs>(screenHint_MoveDelta);

            screenHint.ButtonAbout.MouseClick += new MouseEventHandler(ButtonScreenHintMouseClick);
            screenHint.ButtonChangeType.MouseClick += new MouseEventHandler(ButtonScreenHintMouseClick);
            screenHint.ButtonConfigureStone.MouseClick += new MouseEventHandler(ButtonScreenHintMouseClick);
            screenHint.ButtonRemoveStone.MouseClick += new MouseEventHandler(ButtonScreenHintMouseClick);
            managerWindow.Text = SR.ManagerWindowText;

            taskManager = new RingSwitcher(this, false);

            managerWindow.TopMostWindow = true;
            managerWindow.FadeFinished += new EventHandler<FadeEventArgs>(FadeFinishedInternalHelper);
            managerWindow.MouseClick += new MouseEventHandler(MouseClickInternalHelper);
            managerWindow.AllowDrop = true;
            managerWindow.MouseEnter += new EventHandler(managerWindow_MouseEnter);
            managerWindow.MouseLeave += new EventHandler(managerWindow_MouseLeave);

            managerWindow.DragEnter += new DragEventHandler(managerWindow_DragEnter);
            managerWindow.DragDrop += new DragEventHandler(managerWindow_DragDrop);

            skinsMenu = new KrentoMenu();
            stoneMenu = new KrentoMenu();

            skinsMenu.MenuClick += new EventHandler<KrentoMenuArgs>(SkinsMenuClick);
            skinsMenu.BeforePopup += new EventHandler(SkinsMenuBeforePopup);

            stoneMenu.MenuClick += new EventHandler<KrentoMenuArgs>(StoneMenuClick);
            stoneMenu.BeforePopup += new EventHandler(StoneMenuBeforePopup);

            stoneMenu.AddItem(KrentoMenuTag.About, (Keys.Control | Keys.F1)).Caption = SR.StoneAbout;
            stoneMenu.AddItem(KrentoMenuTag.ChangeType, (Keys.Alt | Keys.T)).Caption = SR.StoneChangeType;
            stoneMenu.AddItem(KrentoMenuTag.Configure, (Keys.Alt | Keys.O)).Caption = SR.StoneConfigure;
            stoneMenu.AddItem(KrentoMenuTag.RemoveStone, (Keys.Control | Keys.R)).Caption = SR.RemoveStone;

            this.realStones = new StonesCollection();
            this.stones = realStones;

            this.virtualStones = new StonesCollection();

            this.availableStoneClasses = new NameObjectCollection();

            history = new HistoryList();
            HistoryReload();
            UpdateScaleValue();

            startMenu = new StartMenuDialog(this.Handle) { Limit = 15, FixedItemsNumber = true, FixedRowSize = 5 };
        }


        void ButtonScreenHintMouseClick(object sender, MouseEventArgs e)
        {
            VisualButton button = (VisualButton)sender;
            TranslateAnimation animation = new TranslateAnimation(button.Left, button.Left + 2, button.Top, button.Top + 2);
            animation.SetAnimationListener(this);
            animation.Duration = 100;
            animation.RepeatCount = 1;
            animation.RepeatMode = RepeatMode.Reverse;
            animation.FillAfter = false;
            button.StartAnimation(animation);
        }


        void managerWindow_DragEnter(object sender, DragEventArgs e)
        {
            DragDropHelper.DragOverTarget(e);
        }

        void screenHint_MoveDelta(object sender, MoveDeltaEventArgs e)
        {
            if (!managerWindow.RotationActive)
            {
                if (screenHint.Top < managerWindow.Top)
                    managerWindow.WheelRotation(-e.DeltaX, 1);
                else
                    managerWindow.WheelRotation(e.DeltaX, 1);
            }
        }


        public ScreenHint ScreenHint
        {
            get { return screenHint; }
        }

        public HistoryList History
        {
            get { return history; }
        }

        public void SimulateWheelRotation()
        {
            managerWindow.SimulateWheelRotation();
        }

        private void StoneMenuBeforePopup(object sender, EventArgs e)
        {
            bool noConfig = true;
            KrentoMenuItem item;

            if (activeStone != null)
            {
                if (activeStone.HasConfigurationEvent)
                    noConfig = false;
                else
                    noConfig = true;


                item = stoneMenu.FindItemByTag(KrentoMenuTag.Configure);
                if (item != null)
                {
                    item.Enabled = !noConfig;
                }

                item = stoneMenu.FindItemByTag(KrentoMenuTag.ChangeType);
                if (item != null)
                {
                    item.Enabled = activeStone.CanChangeType;
                }

                item = stoneMenu.FindItemByTag(KrentoMenuTag.RemoveStone);
                if (item != null)
                {
                    item.Enabled = activeStone.CanRemove;
                }
            }
        }

        private void StoneMenuClick(object sender, KrentoMenuArgs e)
        {
            if (activeStone != null)
            {
                switch (e.MenuItem.Tag)
                {
                    case KrentoMenuTag.About:
                        ShowAboutBox(activeStone);
                        break;
                    case KrentoMenuTag.ChangeType:
                        ChangeTypeRequest(activeStone.Order);
                        break;
                    case KrentoMenuTag.RemoveStone:
                        RemoveStoneRequest(activeStone.Order, 1);
                        break;
                    case KrentoMenuTag.Configure:
                        activeStone.Configure();
                        break;

                }
            }
        }


        private void managerWindow_DragDrop(object sender, DragEventArgs e)
        {
            RollingStoneFile stone = CreateNewStone();
            DragDropHelper.DragDropTarget(stone, e, false);

            UpdateAngles();
            Rotate(0);

            int stoneNumber = stones.Count - 1;

            this.SetCurrentStone(stoneNumber);

            if (visible)
                stone.Visible = true;
            else
                stone.Visible = false;

            FlushCurrentCircle();
            BringToFront();

        }


        private void managerWindow_MouseLeave(object sender, EventArgs e)
        {
            HideRingHint();
            if (GlobalSettings.ShowManagerButtons)
            {
                buttonHintWindow.Visible = false;
            }
        }

        private void managerWindow_MouseEnter(object sender, EventArgs e)
        {
            managerHintWindow.Left = managerWindow.Left + ((managerWindow.Width - managerHintWindow.Width) / 2);
            managerHintWindow.Top = managerWindow.Top - managerHintWindow.Height - 2 + vSpacing;
            managerHintWindow.Update();
            managerHintWindow.Show(false);
        }


        public bool ActivateMouse { get; set; }

        #region Events
        public event EventHandler AboutBoxShow;
        public event EventHandler CircleLoaded;
        public event EventHandler VisibilityChanged;
        #endregion


        internal void OnVisibilityChanged(EventArgs e)
        {
            EventHandler handler = VisibilityChanged;
            if (handler != null)
                handler(this, e);
        }


        public bool LiveReflection { get; set; }
        public bool ShowStoneHint { get; set; }


        public void ShowButtonHint(string caption, int left)
        {
            if (GlobalSettings.ShowManagerButtons)
            {
                if (buttonHintWindow != null)
                {
                    buttonHintWindow.PaintCaption(caption);
                    buttonHintWindow.Left = managerWindow.Left + left - (buttonHintWindow.Width - 60) / 2;
                    buttonHintWindow.Top = managerWindow.Top + managerWindow.Height + 2 - vSpacing;
                    buttonHintWindow.Update();
                    if (visible)
                    {
                        buttonHintWindow.Show(false);
                        buttonHintWindow.MoveAndPlaceOnTop();
                    }
                }
            }
        }

        public void HideButtonHint()
        {
            if (GlobalSettings.ShowManagerButtons)
            {
                if (buttonHintWindow != null)
                {
                    buttonHintWindow.Visible = false;
                }
            }
        }

        public void HideScreenHint()
        {
            if (GlobalSettings.ShowManagerButtons)
            {
                if (screenHint != null)
                {
                    screenHint.Visible = false;
                }
            }
        }

        public void ShowScreenHint()
        {
            if (GlobalSettings.ShowManagerButtons && ShowStoneHint)
            {
                if (screenHint != null)
                {
                    if (Count > 0)
                        screenHint.Visible = true;
                    else
                        screenHint.Visible = false;
                }
            }
        }

        public bool ButtonHintVisible
        {
            get
            {
                if (buttonHintWindow == null)
                {
                    return false;
                }
                else
                {
                    if (GlobalSettings.ShowManagerButtons)
                        return buttonHintWindow.Visible;
                    else
                        return false;
                }
            }
        }

        public void ShowRingHint()
        {
            if (visible && (managerHintWindow != null))
                managerHintWindow.Show(false);
        }

        public void HideRingHint()
        {
            if (managerHintWindow != null)
                managerHintWindow.Visible = false;
        }

        private static Font CreateDefaultFont()
        {
            return new Font("Tahoma", 14, FontStyle.Bold, GraphicsUnit.Pixel);
        }

        public bool RotateOnClick
        { get; set; }

        public int DefaultStonesNumber { get; set; }
        /// <summary>
        /// Gets or sets the font of the stones manager.
        /// </summary>
        /// <value>The font.</value>
        public Font Font
        {
            get
            {
                return font;
            }
            set { font = value; }
        }

        public Color ForeColor
        {
            get { return foreColor; }
            set { foreColor = value; }
        }


        public int WindowWidth
        {
            get { return visibleWindowWidth; }
            set
            {
                visibleWindowWidth = value;
                AdjustManagerWindowSizeForRing();
                RepaintButtonsInternal();
            }
        }

        public int WindowHeight
        {
            get { return visibleWindowHeight; }
            set
            {
                visibleWindowHeight = value;
                AdjustManagerWindowSizeForRing();
                RepaintButtonsInternal();
            }
        }

        public int Transparency
        {
            get { return transparency; }
            set
            {
                transparency = value;
                ReplaceAlpha((byte)value);
            }
        }


        /// <summary>
        /// Clears the history.
        /// </summary>
        public void HistoryClear()
        {
            history.Clear();
            if (!string.IsNullOrEmpty(currentCircle))
                history.Add(currentCircle);
        }

        public void BackToDefaultCircle()
        {
            if (IsVirtual)
            {
                BackToReality();
                return;
            }

            if (FileOperations.FileExists(GlobalConfig.DefaultCircleName))
                ReplaceCurrentCircle(GlobalConfig.DefaultCircleName);
            else
                HistoryBack();
        }

        public void BackToRealityQuiet()
        {
            if (IsVirtual)
            {
                IsVirtual = false;

                DisposeVirtualStones();
                virtualStones.Clear();
                SetCurrentStone(null);
                managerHintWindow.PaintCaption(CircleCaption);
                stones = realStones;
                Rotate(0);
            }
        }


        public string CircleCaption
        {
            get
            {
                string result = string.Empty;
                HistoryEntry entry = history[currentCircle];
                if (entry != null)
                {
                    result = entry.Caption;
                }
                else
                    result = Path.GetFileNameWithoutExtension(currentCircle);
                return result;
            }
        }

        public void RepaintHintWindow()
        {
            managerHintWindow.PaintCaption(CircleCaption);
        }

        public void BackToReality()
        {
            if (IsVirtual)
            {
                IsVirtual = false;

                if (Visible)
                    Visible = false;

                DisposeVirtualStones();
                virtualStones.Clear();
                SetCurrentStone(null);
                managerHintWindow.PaintCaption(CircleCaption);
                stones = realStones;
                Rotate(0);
                Fade();
            }
        }

        public void HistoryBack()
        {
            if (IsVirtual)
            {
                BackToReality();
                return;
            }

            if (history.Count < 2)
                return;

            if (string.IsNullOrEmpty(currentCircle))
                return;

            int idx = history.IndexOf(currentCircle);
            idx--;

            if (idx < 0)
                idx = history.Count - 1;

            ReplaceCurrentCircle(history[idx].FileName);
        }

        public void HistoryForward()
        {
            if (IsVirtual)
            {
                BackToReality();
                return;
            }

            if (history.Count < 2)
                return;

            if (string.IsNullOrEmpty(currentCircle))
                return;

            int idx = history.IndexOf(currentCircle);
            idx++;
            if (idx >= history.Count)
                idx = 0;

            ReplaceCurrentCircle(history[idx].FileName);
        }

        /// <summary>
        /// Add circle to the navigation history list
        /// </summary>
        /// <param name="circleName">Name of the circle.</param>
        public HistoryEntry HistoryAdd(string circleName)
        {
            if (string.IsNullOrEmpty(circleName))
                return null;

            if (!FileOperations.FileExists(circleName))
                return null;

            int idx = history.IndexOf(circleName);

            if (idx < 0)
            {
                return history.Add(circleName);
            }
            else
                return history[idx];
        }

        /// <summary>
        /// Remove circle from the navigation history list
        /// </summary>
        /// <param name="circleName">Name of the circle.</param>
        public void HistoryRemove(string circleName)
        {
            if (string.IsNullOrEmpty(circleName))
                return;

            if (history.IndexOf(circleName) >= 0)
                history.Remove(circleName);
        }

        public bool HistoryContains(string circleName)
        {
            if (string.IsNullOrEmpty(circleName))
                return false;

            return history.IndexOf(circleName) >= 0;
        }

        public string HistoryRingName(int index)
        {
            if (index < 0)
                return null;
            if (index >= history.Count)
                return null;
            return history[index].FileName;
        }

        /// <summary>
        /// Gets the history count.
        /// </summary>
        /// <value>The history count.</value>
        public int HistoryCount
        {
            get { return history.Count; }
        }

        /// <summary>
        /// Gets the current index of the history 
        /// </summary>
        /// <value>The index of the history current.</value>
        public int HistoryCurrentIndex
        {
            get { return history.IndexOf(currentCircle); }
        }


        /// <summary>
        /// Adjusts the manager window size for ring configuration.
        /// </summary>
        private void AdjustManagerWindowSizeForRing()
        {
            int maxSize;
            adjustedWindowWidth = visibleWindowWidth;
            int newSize = visibleWindowWidth + hSpacing * 2;
            if (GlobalSettings.FlatRing)
                maxSize = radius * 2 - stoneSize;
            else
                maxSize = radius * 2 - (stoneSize / 2);

            if (newSize > maxSize)
            {
                newSize = maxSize;
                adjustedWindowWidth = newSize - (hSpacing * 2);
            }
            if (adjustedWindowWidth / visibleWindowHeight > 3.75f)
                visibleWindowHeight = (int)(adjustedWindowWidth / 3.75f);
            managerWindow.Size(newSize, visibleWindowHeight + vSpacing * 2);
        }



        private void LoadStoneIndicator()
        {
            if (indicator != null)
            {
                indicator.Dispose();
                indicator = null;
            }

            indicator = NativeThemeManager.LoadBitmap("indicator.png");
            IndicatorLeft = 0;
            IndicatorBottom = 0;
        }


        internal Bitmap Indicator
        {
            get { return indicator; }
        }


        private void LoadOptimizedDefaultStoneImage()
        {
            LoadStoneIndicator();

            //temporary bitmap for loading stone background
            Bitmap tempStone;

            tempStone = NativeThemeManager.LoadBitmap("DefaultBackground.png");
            try
            {
                stoneMarginBottom = 20;
                stoneMarginRight = 20;
                stoneMarginTop = 20;
                stoneMarginLeft = 20;
                stoneBorder = 0;
                LoadOptimizedStoneImage(tempStone);
            }
            finally
            {
                tempStone.Dispose();
                tempStone = null;
            }
        }

        private void SkinsMenuBeforePopup(object sender, EventArgs e)
        {

            //if (skinsMode == SkinsMode.ManagerSkin)
            // {
            if (!string.IsNullOrEmpty(skinFileName))
            {
                KrentoMenuItem item = skinsMenu.FindItemByData(skinFileName);
                if (item != null)
                    skinsMenu.SelectItem(skinsMenu.Items.IndexOf(item));
            }
            //}
        }

        /// <summary>
        /// Clears the unused memory.
        /// </summary>
        public void ClearUnusedMemory()
        {
            if (managerWindow != null)
            {
                try
                {
                    managerWindow.ClearUnusedMemory();
                }
                catch (Exception ex)
                {
                    TraceDebug.Trace(ex.Message);
                }
            }
        }

        private void SkinsMenuClick(object sender, KrentoMenuArgs e)
        {
            Visible = false;
            if (e.MenuItem.Caption[0] == '<')
                ReloadSkin(null);
            else
                ReloadSkin(e.MenuItem.Data);

            if (hookMessage != null)
                hookMessage.SaveManagerSettings();

            Rotate(0);
            if (GlobalConfig.LowMemory)
                Visible = true;
            else
                Fade();
        }

        public void ShowHelpPages()
        {
            if (hookMessage != null)
                hookMessage.ShowHelpPages();
        }


        private void LoadOptimizedStoneImage(Bitmap original)
        {
            int bHeight = stoneSize;
            int bWidth = stoneSize;


            Bitmap defaultStone = new Bitmap(stoneSize, stoneSize, PixelFormat.Format32bppPArgb);
            using (Graphics g = Graphics.FromImage(defaultStone))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.InterpolationMode = GlobalConfig.InterpolationMode;

                //Draw left part of the image
                g.DrawImage(original, new Rectangle(0, 0, stoneMarginLeft, stoneMarginTop), new Rectangle(0, 0, stoneMarginLeft, stoneMarginTop), GraphicsUnit.Pixel);
                g.DrawImage(original, new Rectangle(0, bHeight - stoneMarginBottom, stoneMarginLeft, stoneMarginBottom), new Rectangle(0, original.Height - stoneMarginBottom, stoneMarginLeft, stoneMarginBottom), GraphicsUnit.Pixel);
                g.DrawImage(original, new Rectangle(0, stoneMarginTop, stoneMarginLeft, bHeight - stoneMarginTop - stoneMarginBottom), new Rectangle(0, stoneMarginTop, stoneMarginLeft, original.Height - stoneMarginTop - stoneMarginBottom), GraphicsUnit.Pixel);

                //Draw central part of the image
                g.DrawImage(original, new Rectangle(stoneMarginLeft, 0, bWidth - stoneMarginLeft - stoneMarginRight, stoneMarginTop), new Rectangle(stoneMarginLeft, 0, original.Width - stoneMarginLeft - stoneMarginRight, stoneMarginTop), GraphicsUnit.Pixel);
                g.DrawImage(original, new Rectangle(stoneMarginLeft, bHeight - stoneMarginBottom, bWidth - stoneMarginLeft - stoneMarginRight, stoneMarginBottom), new Rectangle(stoneMarginLeft, original.Height - stoneMarginBottom, original.Width - stoneMarginLeft - stoneMarginRight, stoneMarginBottom), GraphicsUnit.Pixel);
                g.DrawImage(original, new Rectangle(stoneMarginLeft, stoneMarginTop, bWidth - stoneMarginLeft - stoneMarginRight, bHeight - stoneMarginTop - stoneMarginBottom), new Rectangle(stoneMarginLeft, stoneMarginTop, original.Width - stoneMarginLeft - stoneMarginRight, original.Height - stoneMarginTop - stoneMarginBottom), GraphicsUnit.Pixel);

                //Draw right part of the image
                g.DrawImage(original, new Rectangle(bWidth - stoneMarginRight, 0, stoneMarginRight, stoneMarginTop), new Rectangle(original.Width - stoneMarginRight, 0, stoneMarginRight, stoneMarginTop), GraphicsUnit.Pixel);
                g.DrawImage(original, new Rectangle(bWidth - stoneMarginRight, bHeight - stoneMarginBottom, stoneMarginRight, stoneMarginBottom), new Rectangle(original.Width - stoneMarginRight, original.Height - stoneMarginBottom, stoneMarginRight, stoneMarginBottom), GraphicsUnit.Pixel);
                g.DrawImage(original, new Rectangle(bWidth - stoneMarginRight, stoneMarginTop, stoneMarginRight, bHeight - stoneMarginTop - stoneMarginBottom), new Rectangle(original.Width - stoneMarginRight, stoneMarginTop, stoneMarginRight, original.Height - stoneMarginTop - stoneMarginBottom), GraphicsUnit.Pixel);
            }

            if (stoneBackgroundBitmap != IntPtr.Zero)
            {
                NativeMethods.DeleteObject(stoneBackgroundBitmap);
                stoneBackgroundBitmap = IntPtr.Zero;
            }

            stoneBackgroundBitmap = defaultStone.GetHbitmap(Color.FromArgb(0));
            stoneBackgroundWidth = defaultStone.Width;
            stoneBackgroundHeight = defaultStone.Height;
            defaultStone.Dispose();
        }

        public void SetHookMessage(IHookMessage hookMessage)
        {
            this.hookMessage = hookMessage;
        }


        void MouseClickInternalHelper(object sender, MouseEventArgs e)
        {
            if (isFading)
                return;

            if (e.Button == MouseButtons.Right)
            {
                if (popupMenu != null)
                {
                    POINT pt = new POINT();
                    pt.x = e.X;
                    pt.y = e.Y;
                    NativeMethods.ClientToScreen(this.managerWindow.Handle, ref pt);
                    popupMenu.PopupAt(pt.x, pt.y);
                }
            }
        }


        internal void DoCircleLoaded(EventArgs e)
        {
            if (CircleLoaded != null)
                CircleLoaded(this, e);
        }

        public void ClearStatistics()
        {
            for (int i = 0; i < stones.Count; i++)
            {
                stones[i].RunCount = 0;
                stones[i].RunLevel = 0;
            }
        }


        /// <summary>
        /// Gets or sets the popup menu.
        /// </summary>
        /// <value>The popup menu.</value>
        public KrentoMenu PopupMenu
        {
            get { return popupMenu; }
            set { popupMenu = value; }
        }

        /// <summary>
        /// Gets or sets the stone menu.
        /// </summary>
        /// <value>The stone menu.</value>
        public KrentoMenu StoneMenu
        {
            get { return stoneMenu; }
            set { stoneMenu = value; }
        }

        /// <summary>
        /// Creates the default circle with the set of predefined stones and saves it to the file.
        /// <see cref="CreateDefaultCircle"/> calls private method CreateDefaultStones and then
        /// <see cref="SaveCircle"/> method to flush the new circle to the file.
        /// </summary>
        public void CreateDefaultCircle()
        {
            CreateDefaultStones();
            SaveCircle(GlobalConfig.HomeCircleName);
            DisposeStones();
            realStones.Clear();
        }

        public void CreateDesktopCircle()
        {
            try
            {
                CreateDesktopStones();
                DisposeStones();
                realStones.Clear();
            }
            catch (Exception ex)
            {
                TraceDebug.Trace(ex.Message);
            }
        }

        private string[] MergeArrays(string[] array1, string[] array2)
        {
            if (array1 == null)
                return array2;
            if (array2 == null)
                return array1;
            string[] result = new string[array1.Length + array2.Length];
            Array.Copy(array1, result, array1.Length);
            Array.Copy(array2, 0, result, array1.Length, array2.Length);
            return result;
        }

        public void CreateDesktopStones()
        {
            bool exception = false;
            bool alreadyExists = false;

            int numberOfRings;
            int reminder;
            int stonesNumber;
            RollingStoneFile stone;
            int indexer = 0;
            string ringFileName;

            string folderName;
            string[] files1;
            string[] files2;
            string[] files;

            string singleCircleName = Path.Combine(GlobalConfig.RollingStonesFolder, SR.DesktopRing + ".circle");

            files1 = Directory.GetFiles(GlobalConfig.RollingStonesFolder, SR.DesktopRing + "*.circle", SearchOption.AllDirectories);
            if (files1 != null)
            {
                if (files1.Length > 0)
                    alreadyExists = true;
            }

            if (alreadyExists)
                return;

            try
            {
                folderName = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                files1 = Directory.GetFiles(folderName, "*.lnk", SearchOption.AllDirectories);

                folderName = NativeMethods.GetFolderPath(CSIDL.COMMON_DESKTOPDIRECTORY);
                files2 = Directory.GetFiles(folderName, "*.lnk", SearchOption.AllDirectories);

                files = MergeArrays(files1, files2);
            }
            catch
            {
                files = null;
            }

            if (files != null)
            {
                if (files.Length > 0)
                {

                    numberOfRings = files.Length / 12;
                    reminder = files.Length % 12;

                    if (reminder > 0)
                        numberOfRings += 1;

                    for (int i = 0; i < numberOfRings; i++)
                    {
                        try
                        {
                            stonesNumber = 12;
                            if (i == (numberOfRings - 1))
                                stonesNumber = reminder;


                            string[] ringStones = new string[stonesNumber];
                            Array.Copy(files, indexer, ringStones, 0, stonesNumber);
                            indexer += stonesNumber;

                            DisposeStones();
                            stones.Clear();

                            foreach (string fileName in ringStones)
                            {
                                stone = this.CreateNewStone();
                                string fullName;
                                if (FileOperations.FileIsLink(fileName))
                                {
                                    fullName = Path.Combine(GlobalConfig.AppShortcuts, Path.GetFileName(fileName));
                                    FileOperations.CopyFile(fileName, fullName);
                                }
                                else
                                    fullName = fileName;

                                fullName = FileOperations.UnExpandPath(fileName);
                                stone.UpdateTarget(fullName);
                                stone.FixupConfiguration();
                            }
                        }
                        catch
                        {
                            exception = true;
                        }

                        if (!exception)
                        {
                            if (Count > 0)
                            {
                                if (numberOfRings == 1)
                                    ringFileName = singleCircleName;
                                else
                                    ringFileName = Path.Combine(GlobalConfig.RollingStonesFolder, SR.DesktopRing + " " + (i + 1).ToString() + ".circle");
                                SaveCircle(ringFileName);
                                NativeMethods.WriteString(ringFileName, "Settings", "TranslationId", SR.Keys.DesktopRing);
                                HistoryAdd(ringFileName);
                            }
                        }
                    }
                }
            }
        }



        /// <summary>
        /// Creates the default stones.
        /// </summary>
        private void CreateDefaultStones()
        {
            DisposeStones();
            stones.Clear();

            RollingStoneTask stone;
            RollingStoneFile fileStone;

            fileStone = new RollingStoneFile(this);
            Stones.Add(fileStone);
            fileStone.Order = stones.Count - 1;
            fileStone.SetStoneBackground(stoneBackgroundBitmap, stoneBackgroundWidth, stoneBackgroundHeight);
            fileStone.StoneIndicator = this.indicator;
            fileStone.StoneID = GuidCreator.NewGuid().ToString();
            fileStone.TranslationId = SR.Keys.Google;
            fileStone.TargetName = "http://www.google.com/search?hl=en&q=##";
            fileStone.CustomIcon = Path.Combine(GlobalConfig.IconsFolder, "google.png");
            fileStone.ArgumentDescription = SR.SearchPhrase;
            fileStone.FixupConfiguration();

            fileStone = new RollingStoneFile(this);
            Stones.Add(fileStone);
            fileStone.Order = stones.Count - 1;
            fileStone.SetStoneBackground(stoneBackgroundBitmap, stoneBackgroundWidth, stoneBackgroundHeight);
            fileStone.StoneIndicator = this.indicator;
            fileStone.StoneID = GuidCreator.NewGuid().ToString();
            fileStone.TranslationId = SR.Keys.Wikipedia;
            fileStone.TargetName = "http://en.wikipedia.org/wiki/Special:Search?search=##&go=Go";
            fileStone.CustomIcon = Path.Combine(GlobalConfig.IconsFolder, "wiki.gif");
            fileStone.ArgumentDescription = SR.SearchText;
            fileStone.FixupConfiguration();

            fileStone = new RollingStoneFile(this);
            Stones.Add(fileStone);
            fileStone.Order = stones.Count - 1;
            fileStone.SetStoneBackground(stoneBackgroundBitmap, stoneBackgroundWidth, stoneBackgroundHeight);
            fileStone.StoneIndicator = this.indicator;
            fileStone.StoneID = GuidCreator.NewGuid().ToString();
            fileStone.TranslationId = SR.Keys.InternetExplorer;
            fileStone.TargetName = "IExplore.exe";
            fileStone.CustomIcon = Path.Combine(GlobalConfig.IconsFolder, "explorer.png");
            fileStone.FixupConfiguration();

            stone = new RollingStoneMyDocuments(this);
            Stones.Add(stone);
            stone.Order = stones.Count - 1;
            stone.SetStoneBackground(stoneBackgroundBitmap, stoneBackgroundWidth, stoneBackgroundHeight);
            stone.StoneIndicator = this.indicator;
            stone.StoneID = GuidCreator.NewGuid().ToString();
            stone.FixupConfiguration();

            stone = new RollingStoneMyMusic(this);
            Stones.Add(stone);
            stone.Order = stones.Count - 1;
            stone.SetStoneBackground(stoneBackgroundBitmap, stoneBackgroundWidth, stoneBackgroundHeight);
            stone.StoneIndicator = this.indicator;
            stone.StoneID = GuidCreator.NewGuid().ToString();
            stone.FixupConfiguration();

            stone = new RollingStoneMyPictures(this);
            Stones.Add(stone);
            stone.Order = stones.Count - 1;
            stone.SetStoneBackground(stoneBackgroundBitmap, stoneBackgroundWidth, stoneBackgroundHeight);
            stone.StoneIndicator = this.indicator;
            stone.StoneID = GuidCreator.NewGuid().ToString();
            stone.FixupConfiguration();

            stone = new RollingStoneMyComputer(this);
            Stones.Add(stone);
            stone.Order = stones.Count - 1;
            stone.SetStoneBackground(stoneBackgroundBitmap, stoneBackgroundWidth, stoneBackgroundHeight);
            stone.StoneIndicator = this.indicator;
            stone.StoneID = GuidCreator.NewGuid().ToString();
            stone.FixupConfiguration();

            stone = new RollingStoneRecycleBin(this);
            Stones.Add(stone);
            stone.Order = stones.Count - 1;
            stone.SetStoneBackground(stoneBackgroundBitmap, stoneBackgroundWidth, stoneBackgroundHeight);
            stone.StoneIndicator = this.indicator;
            stone.StoneID = GuidCreator.NewGuid().ToString();
            stone.FixupConfiguration();

            UpdateItems();
        }

        public void ShowAll()
        {
            for (int i = 0; i < Count; i++)
            {
                stones[i].Visible = true;
            }
            managerWindow.Visible = true;
            visible = true;
        }

        public void HideAll()
        {
            HideRingHint();

            if (GlobalSettings.ShowManagerButtons)
            {
                if (buttonHintWindow != null)
                    buttonHintWindow.Visible = false;
                HideScreenHint();
            }



            HideAllMenus();

            for (int i = 0; i < Count; i++)
            {
                stones[i].Visible = false;
            }
            managerWindow.Visible = false;

            visible = false;
        }

        public void VirtualClick()
        {
            managerWindow.VirtualClick();
        }

        public void ChangeVisibility(bool show)
        {
            isFading = false;

            if (show)
            {
                visible = true;

                if (!popupMenu.Active)
                {
                    ShowAll();


                    Rearrange();
                    managerWindow.BringToFront();

                    managerWindow.Activate();
                    if (managerWindow.CanFocus)
                        managerWindow.Focus();


                    managerWindow.Cursor = Cursors.Hand;

                    this.Position = this.Position;

                    if (ActivateMouse)
                    {
                        if (PrimaryScreen.WindowFromMouse != this.Handle)
                        {
                            NativeMethods.SetCursorPos(managerWindow.Left + (managerWindow.Width / 2),
                                managerWindow.Top + (managerWindow.Height / 2));
                        }
                        managerWindow.VirtualClick();
                    }


                    NativeMethods.LockForegroundWindow();
                }
            }
            else
            {

                NativeMethods.UnlockForegroundWindow();
                //Fading is finished and manager is invisible now
                visible = false;

                HideScreenHint();
                HideAllMenus();
                HideRingHint();
                BackToRealityQuiet();

                //Do not call HideAll() here

                if (RotateOnClick)
                {
                    //Put active stone to bottom
                    if (activeStone != null)
                    {
                        rollingDirection = RollingDirection.Left;
                        Rotate((int)activeStone.Angle);
                    }
                }
                else
                {
                    Rotate(0);
                }

            }
            OnVisibilityChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Hides the skins menu.
        /// </summary>
        public void HideSkinsMenu()
        {
            if (skinsMenu != null)
                skinsMenu.CloseUp();
        }


        /// <summary>
        /// Hides the popup menu.
        /// </summary>
        public void HidePopupMenu()
        {
            if (popupMenu != null)
            {
                if (PopupMenu.Active)
                    PopupMenu.CloseUp();
            }
        }

        /// <summary>
        /// Hides the stone menu.
        /// </summary>
        public void HideStoneMenu()
        {
            if (StoneMenu != null)
            {
                if (StoneMenu.Active)
                    StoneMenu.CloseUp();
            }
        }

        /// <summary>
        /// Hides all menus.
        /// </summary>
        public void HideAllMenus()
        {
            HideSkinsMenu();
            HidePopupMenu();
            HideStoneMenu();
        }

        private void FadeFinishedInternalHelper(object sender, FadeEventArgs e)
        {
            ChangeVisibility(e.FadeUp);

            if (fadeHook != null)
            {
                try
                {
                    fadeHook();
                }
                finally
                {
                    fadeHook = null;
                }
            }
        }


        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="StonesManager"/> is reclaimed by garbage collection.
        /// </summary>
        ~StonesManager()
        {
            Dispose(false);
        }

        private void DisposeStones()
        {
            for (int i = 0; i < stones.Count; i++)
                stones[i].Dispose();
        }

        private void DisposeVirtualStones()
        {
            for (int i = 0; i < virtualStones.Count; i++)
                virtualStones[i].Dispose();
        }

        private void DisposeRealStones()
        {
            for (int i = 0; i < realStones.Count; i++)
                realStones[i].Dispose();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        internal void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.popupMenu = null;
                HideAll();

                deleteWindow.Dispose();
                managerWindow.Dispose();
                managerHintWindow.Dispose();

                if (buttonHintWindow != null)
                {
                    buttonHintWindow.Dispose();
                    buttonHintWindow = null;
                }

                if (screenHint != null)
                {
                    screenHint.Dispose();
                    screenHint = null;
                }

                DisposeRealStones();
                realStones.Clear();

                DisposeVirtualStones();
                virtualStones.Clear();

                if (stoneMenu != null)
                {
                    stoneMenu.Dispose();
                    stoneMenu = null;
                }

                if (skinsMenu != null)
                {
                    skinsMenu.Dispose();
                    skinsMenu = null;
                }


                if (stoneBackgroundBitmap != IntPtr.Zero)
                {
                    NativeMethods.DeleteObject(stoneBackgroundBitmap);
                    stoneBackgroundBitmap = IntPtr.Zero;
                }


                if (indicator != null)
                {
                    indicator.Dispose();
                    indicator = null;
                }


                if (font != null)
                {
                    font.Dispose();
                    font = null;
                }

                if (taskManager != null)
                {
                    taskManager.Dispose();
                    taskManager = null;
                }

                for (int i = 0; i < history.Count; i++)
                    history[i].Dispose();
                history.Clear();

                startMenu.Dispose();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [mouse inside stone].
        /// </summary>
        /// <value><c>true</c> if [mouse inside stone]; otherwise, <c>false</c>.</value>
        internal bool MouseInsideStone
        {
            set
            {
                this.mouseInsideStone = value;
            }
        }

        public void SuppressHookMessage(bool value)
        {
            if (hookMessage != null)
                hookMessage.SuppressHookMessage(value);
        }

        public DeleteWindow DeleteWindow
        {
            get { return deleteWindow; }
        }

        /// <summary>
        /// Gets or sets the fade delay.
        /// </summary>
        /// <value>The fade delay.</value>
        public int FadeDelay
        {
            get { return fadeDelay; }
            set { fadeDelay = value; }
        }

        /// <summary>
        /// Gets the stones collection.
        /// </summary>
        /// <value>The stones collection.</value>
        public StonesCollection Stones
        {
            get { return this.stones; }
        }

        public ScaleLimit Scale
        {
            get { return scale; }
        }

        /// <summary>
        /// Draws the text on the manager window. This method can be used by stones for implementing the 
        /// unified text drawing on manager window.
        /// </summary>
        /// <param name="text">The text to draw.</param>
        public void DrawText(string text)
        {
            if (GlobalSettings.ShowManagerButtons)
            {
                if (ShowStoneHint)
                {
                    if (screenHint != null)
                    {
                        Bitmap glyph = null;
                        bool mustDispose = false;
                        if (activeStone != null)
                        {
                            if (activeStone is RollingStoneTask)
                                glyph = ((RollingStoneTask)activeStone).Logo;
                        }
                        if (glyph == null)
                        {
                            glyph = NativeThemeManager.LoadBitmap("BigKrento.png");
                            mustDispose = true;
                        }
                        screenHint.PaintCaption(text, glyph);
                        if (mustDispose && (glyph != null))
                            glyph.Dispose();
                    }
                }
            }
            else
            {
                this.managerWindow.Clear();
                this.managerWindow.DrawDefaultBackground(false);
                if (!string.IsNullOrEmpty(text))
                {
                    TextPainter.DrawString(this.managerWindow.Canvas, text, this.font, this.hSpacing + this.textOffset, this.vSpacing, this.adjustedWindowWidth - this.textOffset * 2, this.visibleWindowHeight, this.foreColor, true);
                }
                this.managerWindow.Update(true);
            }
        }

        /// <summary>
        /// Redraws the main window by the active stone. This method sends CM_REDRAW_MANAGER message to the manager window
        /// and manager window calls RedrawScreenHintInternal method
        /// </summary>
        public void RedrawScreenHint()
        {
            managerWindow.Perform(NativeMethods.CM_REDRAW_MANAGER, IntPtr.Zero, IntPtr.Zero);
        }

        /// <summary>
        /// Redraws the manager buttons. This methods call the RepaintButtonsInternal using manager window for synchronization
        /// </summary>
        public void RedrawManagerButtons()
        {
            managerWindow.Perform(NativeMethods.CM_REDRAW_BUTTONS, IntPtr.Zero, IntPtr.Zero);
        }

        internal void RedrawScreenHintInternal()
        {
            if (activeStone != null)
            {
                activeStone.DrawTargetDescription();
            }
            else
            {
                DrawText(null);
            }
        }
        /// <summary>
        /// Gets the active stone.
        /// </summary>
        /// <value>The active stone.</value>
        public RollingStoneBase ActiveStone
        {
            get { return activeStone; }
        }

        public int IndicatorLeft { get; set; }
        public int IndicatorBottom { get; set; }



        private void LoadKrentoCoreBackground(string themeName)
        {
            Bitmap tmp;

            KrentoCoreSkin skin = new KrentoCoreSkin(themeName);
            try
            {
                if (skin.LoadFromFile())
                {

                    vSpacing = skin.OutsideTopMargin;
                    if (vSpacing < 0)
                        vSpacing = 0;

                    hSpacing = skin.OutsideLeftMargin;
                    if (hSpacing < 0)
                        hSpacing = 0;

                    textOffset = skin.TextOffset;

                    stoneMarginBottom = skin.StoneMarginBottom;
                    stoneMarginTop = skin.StoneMarginTop;
                    stoneMarginLeft = skin.StoneMarginLeft;
                    stoneMarginRight = skin.StoneMarginRight;
                    stoneBorder = skin.StoneBorder;

                    if (font != null)
                    {
                        font.Dispose();
                        font = null;
                    }
                    font = new Font(skin.FontName, skin.FontSize, FontStyle.Bold, GraphicsUnit.Pixel);

                    foreColor = skin.ForeColor;
                    if (screenHint != null)
                    {
                        screenHint.ForeColor = foreColor;
                        screenHint.HintBodyColor = skin.HintBodyColor;
                        screenHint.HintOutlineColor = skin.HintOutlineColor;
                        screenHint.HintBorderColor = skin.HintBorderColor;
                        screenHint.SelectNewFont(skin.FontName, skin.FontSize, FontStyle.Bold);
                        screenHint.ButtonSize = skin.ButtonStoneSize;
                        screenHint.ButtonSpace = skin.ButtonStoneSpace;
                        screenHint.ReloadButtonAbout(skin.ButtonStoneAbout);
                        screenHint.ReloadButtonDelete(skin.ButtonStoneDelete);
                        screenHint.ReloadButtonEdit(skin.ButtonStoneEdit);
                        screenHint.ReloadButtonSelect(skin.ButtonStoneSelect);
                    }

                    if (managerHintWindow != null)
                    {
                        managerHintWindow.ForeColor = foreColor;
                        managerHintWindow.HintBodyColor = skin.HintBodyColor;
                        managerHintWindow.HintOutlineColor = skin.HintOutlineColor;
                        managerHintWindow.HintBorderColor = skin.HintBorderColor;
                        managerHintWindow.RepaintAll();
                    }

                    if (buttonHintWindow != null)
                    {
                        buttonHintWindow.ForeColor = foreColor;
                        buttonHintWindow.HintBodyColor = skin.HintBodyColor;
                        buttonHintWindow.HintOutlineColor = skin.HintOutlineColor;
                        buttonHintWindow.HintBorderColor = skin.HintBorderColor;
                        buttonHintWindow.RepaintAll();
                    }


                    if (indicator != null)
                    {
                        indicator.Dispose();
                        indicator = null;
                    }


                    if (string.IsNullOrEmpty(skin.IndicatorImageName))
                    {
                        LoadStoneIndicator();
                    }
                    else
                    {
                        try
                        {
                            tmp = FastBitmap.FromFile(skin.IndicatorImageName);
                        }
                        catch
                        {
                            tmp = null;
                        }
                        if (tmp == null)
                        {
                            LoadStoneIndicator();
                        }
                        else
                        {
                            indicator = BitmapPainter.ResizeBitmap(tmp, skin.IndicatorWidth, skin.IndicatorHeight, true);
                            IndicatorLeft = skin.IndicatorLeft;
                            IndicatorBottom = skin.IndicatorBottom;
                        }
                    }

                    WindowWidth = skin.ManagerWidth;
                    WindowHeight = skin.ManagerHeight;
                    Radius = skin.Radius;

                    LoadOptimizedStoneImage(skin.Stone);

                    AdjustManagerWindowSizeForRing();
                    managerWindow.LoadKrentoCoreBackground(skin);
                }
                else //not skin file
                {
                    if (font != null)
                    {
                        font.Dispose();
                        font = null;
                    }

                    font = CreateDefaultFont();
                    foreColor = Color.White;
                    stoneBorder = 0;
                    textOffset = 8;

                    hSpacing = defaultSpacing;
                    vSpacing = defaultSpacing;


                    WindowWidth = GlobalSettings.WindowWidth;
                    WindowHeight = GlobalSettings.WindowHeight;
                    Radius = GlobalSettings.Radius;

                    AdjustManagerWindowSizeForRing();
                    LoadOptimizedDefaultStoneImage();
                    managerWindow.LoadKrentoCoreBackground(null);

                    if (screenHint != null)
                    {
                        screenHint.ForeColor = foreColor;
                        screenHint.HintBodyColor = Color.Black;
                        screenHint.HintOutlineColor = Color.Black;
                        screenHint.HintBorderColor = Color.Gainsboro;
                        screenHint.SelectNewFont("Tahoma", 14, FontStyle.Bold);
                        screenHint.ButtonSize = skin.ButtonStoneSize;
                        screenHint.ButtonSpace = skin.ButtonStoneSpace;
                        screenHint.ReloadButtonResources();
                    }

                    if (managerHintWindow != null)
                    {
                        managerHintWindow.ForeColor = foreColor;
                        managerHintWindow.HintBodyColor = Color.Black;
                        managerHintWindow.HintOutlineColor = Color.Black;
                        managerHintWindow.HintBorderColor = Color.Gainsboro;
                        managerHintWindow.RepaintAll();
                    }

                    if (buttonHintWindow != null)
                    {
                        buttonHintWindow.ForeColor = foreColor;
                        buttonHintWindow.HintBodyColor = Color.Black;
                        buttonHintWindow.HintOutlineColor = Color.Black;
                        buttonHintWindow.HintBorderColor = Color.Gainsboro;
                        buttonHintWindow.RepaintAll();
                    }


                }

                MenuSkin = skin.MenuSkin;
                KrentoMenuColors menuColors = new KrentoMenuColors();
                menuColors.BodyColor = skin.MenuBodyColor;
                menuColors.BorderColor = skin.MenuBorderColor;
                menuColors.OutlineColor = skin.MenuOutlineColor;
                menuColors.HighlightColor = skin.MenuHighlightColor;
                menuColors.FontName = skin.MenuFontName;
                menuColors.FontSize = skin.MenuFontSize;
                menuColors.NormalTextColor = skin.MenuNormalTextColor;
                menuColors.SelectedTextColor = skin.MenuSelectedTextColor;
                menuColors.DisabledTextColor = skin.MenuDisabledTextColor;

                KrentoMenu.MenuColors = menuColors;
                ChangeMenuSkin(MenuSkin);
            }
            finally
            {
                skin.Dispose();
            }

            if (screenHint != null)
                RedrawScreenHint();

        }

        public void ChangeMenuSkin(string skinFileName)
        {
            //GlobalSettings.MenuSkin = skinFileName;
            //KrentoMenu.SkinFileName = FileOperations.StripFileName(GlobalSettings.MenuSkin);
            if (string.IsNullOrEmpty(skinFileName))
                KrentoMenu.SkinFileName = FileOperations.StripFileName(GlobalSettings.MenuSkin);
            else
                KrentoMenu.SkinFileName = Path.Combine(GlobalConfig.MenusFolder, skinFileName + @"\background.ini");

            InteropHelper.BroadcastApplicationMessage(messageID);
        }

        public string MenuSkin { get; set; }
        /// <summary>
        /// Gets the current circle configuration file name.
        /// </summary>
        /// <value>The current circle.</value>
        public string CurrentCircle
        {
            get { return currentCircle; }
            set { currentCircle = value; }
        }

        /// <summary>
        /// Loads the circle file.
        /// </summary>
        /// <param name="configurationFile">The configuration file.</param>
        /// <param name="savePrevious">if set to <c>true</c> save previous circle file modifications 
        /// before loading new circle.</param>
        public void LoadCircle(string configurationFile, bool savePrevious)
        {
            if (!savePrevious)
                currentCircle = null;
            LoadCircle(configurationFile);
        }

        /// <summary>
        /// Loads the new circle. If new circle has no stones, the empty circle
        /// is created.
        /// </summary>
        /// <param name="configurationFile">The configuration file.</param>
        public void LoadCircle(string configurationFile)
        {

            if (string.IsNullOrEmpty(configurationFile))
                return;

            if (!string.IsNullOrEmpty(currentCircle))
            {
                MemIniFile iniFile = null;
                HistoryEntry entry;
                entry = history[currentCircle];
                if (entry != null)
                    iniFile = entry.IniFile;
                if (iniFile != null)
                {
                    if (Count > 0)
                    {
                        iniFile.WriteInteger("Settings", "Angle", (int)(stones[0].Angle));
                        NativeMethods.WriteString(currentCircle, "Settings", "Angle", ((int)(stones[0].Angle)).ToString());
                    }
                }

            }

            LoadCircleInternal(configurationFile);

            if (ActiveStone == null)
            {
                SelectFirstStone();
            }


        }

        public void CreateVirtualCircle()
        {
            Visible = false;
            IsVirtual = true;
            stones = virtualStones;
            DisposeVirtualStones();
            virtualStones.Clear();
            SetCurrentStone(null);
        }

        public void ShowRunningApplications()
        {
            string appPath;
            IntPtr[] running = DesktopHelper.GetDesktopWindows();
            if (running == null)
                return;
            if (running.Length == 0)
                return;
            RollingStoneRunning rStone;
            Visible = false;
            CreateVirtualCircle();
            for (int i = 0; i < running.Length; i++)
            {

                appPath = DesktopHelper.GetApplicationFromWindow(running[i]);

                rStone = new RollingStoneRunning(this);
                rStone.IsVirtual = true;
                rStone.StoneID = GuidCreator.NewGuid().ToString();

                if (!string.IsNullOrEmpty(appPath))
                {
                    rStone.Path = appPath;
                    rStone.Logo = BitmapPainter.ResizeBitmap(FileImage.FileNameImage(rStone.Path), FileImage.ImageSize, FileImage.ImageSize, true);
                }
                else
                {
                    IntPtr hIcon = NativeMethods.GetAppicationIcon(running[i]);
                    if (hIcon != IntPtr.Zero)
                    {
                        Icon ico = Icon.FromHandle(hIcon);
                        Bitmap tmp = (Bitmap)FileImage.ConvertIconToBitmap(ico);
                        rStone.Logo = BitmapPainter.ResizeBitmap(tmp, FileImage.ImageSize, FileImage.ImageSize, true);
                        ico.Dispose();
                    }
                    else
                        rStone.Logo = NativeThemeManager.LoadBitmap("ButtonLauncherFocused.png");
                }

                rStone.SetStoneBackground(stoneBackgroundBitmap, stoneBackgroundWidth, stoneBackgroundHeight);
                rStone.StoneIndicator = this.indicator;
                rStone.MainWindow = running[i];
                rStone.TargetDescription = DesktopHelper.GetWindowText(running[i]);
                if (string.IsNullOrEmpty(rStone.TargetDescription))
                    rStone.TargetDescription = Path.GetFileNameWithoutExtension(rStone.Path);
                rStone.FixupConfiguration();
                stones.Add(rStone);

            }
            managerHintWindow.PaintCaption(SR.TaskManager);
            UpdateAngles();
            Rotate(0);
            Fade();
        }

        internal Type LoadTypeFromClassAssembly(string fileName, string typeName)
        {
            Assembly assembly;

            try
            {
                string[] names = Directory.GetFiles(GlobalConfig.StoneClasses, fileName, SearchOption.AllDirectories);
                if (names == null)
                    return null;
                if (names.Length != 1)
                    return null;
                string fullName = names[0];

                if (FileOperations.FileExists(fullName))
                {
                    if (NativeMethods.IsAssembly(fullName))
                        assembly = Assembly.LoadFile(fullName);
                    else
                        assembly = null;

                    if (assembly == null)
                        return null;
                    Type t = assembly.GetType(typeName, false, true);
                    return t;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }

        }


        /// <summary>
        /// Creates the empty circle.
        /// </summary>
        public void CreateEmptyCircle()
        {
            RollingStoneBase stone;

            DisposeStones();
            stones.Clear();

            if (Count == 0)
            {
                BeginUpdate();
                //create default stones set
                for (int i = 0; i < DefaultStonesNumber; i++)
                {
                    stone = CreateNewStone();
                    stone.FixupConfiguration();
                }
                EndUpdate();
                UpdateItems();
            }
        }

        /// <summary>
        /// Reload navigation history. 
        /// </summary>
        public void HistoryReload()
        {
            HistoryClear();
            string[] rings = Directory.GetFiles(GlobalConfig.RollingStonesFolder, "*.circle", SearchOption.TopDirectoryOnly);
            foreach (string ring in rings)
            {
                HistoryAdd(ring);
            }
        }


        public void LoadPreviousCircle()
        {
            if (!string.IsNullOrEmpty(previousCircle))
            {
                if (FileOperations.FileExists(previousCircle))
                    ReplaceCurrentCircle(previousCircle);
                else
                    HistoryBack();
            }
        }

        public string PreviousCircle
        {
            get { return previousCircle; }
        }


        /// <summary>
        /// Gets or sets the executing circle. This is a storage for the ring stone to know what is the current circle name to go back
        /// </summary>
        /// <value>
        /// The executing circle.
        /// </value>
        public string ExecutingCircle { get; set; }

        private void LoadCircleInternal(string configurationFile)
        {
            string[] stonesKeys;
            string stoneID;
            RollingStoneBase stone;
            bool knownType;
            Type type;
            TypeObjectEntry typeEntry;

            if (string.IsNullOrEmpty(configurationFile))
                return;

            if (Visible)
                Visible = false;

            if (IsVirtual)
            {
                IsVirtual = false;
                DisposeVirtualStones();
                virtualStones.Clear();
                SetCurrentStone(null);
                stones = realStones;
            }


            previousCircle = currentCircle;

            string fullConfigurationFile = FileOperations.StripFileName(configurationFile);
            MemIniFile iniFile = HistoryAdd(fullConfigurationFile).IniFile;
            currentCircle = fullConfigurationFile;


            DisposeStones();
            stones.Clear();
            SetCurrentStone(null);

            managerHintWindow.PaintCaption(CircleCaption);
            string defaultAssemblyName = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
            storedAngle = iniFile.ReadInteger("Settings", "Angle", 0);

            //get the names of the keys
            //Stone0..Stone11
            //Usually it must be sorted
            stonesKeys = iniFile.ReadSection("Stones");

            if (stonesKeys == null)
            {
                HideScreenHint();
                DrawText(SR.KrentoWelcome);
                return;
            }

            if (stonesKeys.Length == 0)
            {
                HideScreenHint();
                DrawText(SR.KrentoWelcome);
                return;
            }


            if (stonesKeys == null)
            {
                HideScreenHint();
                DrawText(SR.KrentoWelcome);
                return;
            }

            if (stonesKeys.Length == 0)
            {
                HideScreenHint();
                DrawText(SR.KrentoWelcome);
                return;
            }

            //!!!???!!!
            //Array.Sort(stonesKeys, CompareStonesByNumber);

            //Now we have sorted array of right items

            BeginUpdate();
            try
            {
                //Check: The stones must be loaded in the right order

                for (int i = 0; i < stonesKeys.Length; i++)
                {
                    stoneID = iniFile.ReadString("Stones", stonesKeys[i], null);
                    stone = null;
                    if (!string.IsNullOrEmpty(stoneID))
                    {
                        string stoneTypeName = iniFile.ReadString(stoneID, "StoneClass", string.Empty);
                        string stoneAssemblyName = iniFile.ReadString(stoneID, "Assembly", defaultAssemblyName);
                        type = null;
                        typeEntry = (TypeObjectEntry)availableStoneClasses[stoneTypeName];
                        if ((!string.IsNullOrEmpty(stoneTypeName)) && (typeEntry != null))
                        {

                            if (typeEntry.BuiltIn)
                                knownType = true;
                            else
                            {
                                knownType = false;
                            }

                            if (!knownType)
                            {
                                type = typeEntry.Type;
                                if (type == null)
                                    type = Type.GetType(stoneTypeName, false, true);
                                if (type == null)
                                {
                                    type = LoadTypeFromClassAssembly(stoneAssemblyName, stoneTypeName);
                                }
                            }

                            if ((type != null) || (knownType))
                            {
                                if (knownType)
                                {
                                    switch (stoneTypeName)
                                    {
                                        case StoneClass.RollingStoneFile:
                                            stone = new RollingStoneFile(this);
                                            break;
                                        case StoneClass.RollingStoneRing:
                                            stone = new RollingStoneRing(this);
                                            break;
                                        case StoneClass.RollingStoneMyIP:
                                            stone = new RollingStoneMyIP(this);
                                            break;
                                        case StoneClass.RollingStoneMyDocuments:
                                            stone = new RollingStoneMyDocuments(this);
                                            break;
                                        case StoneClass.RollingStoneMyPictures:
                                            stone = new RollingStoneMyPictures(this);
                                            break;
                                        case StoneClass.RollingStoneMyMusic:
                                            stone = new RollingStoneMyMusic(this);
                                            break;
                                        case StoneClass.RollingStoneMyComputer:
                                            stone = new RollingStoneMyComputer(this);
                                            break;
                                        case StoneClass.RollingStoneRecycleBin:
                                            stone = new RollingStoneRecycleBin(this);
                                            break;
                                        case StoneClass.RollingStoneShutdown:
                                            stone = new RollingStoneShutdown(this);
                                            break;
                                        case StoneClass.RollingStoneRestart:
                                            stone = new RollingStoneRestart(this);
                                            break;
                                        case StoneClass.RollingStoneSuspend:
                                            stone = new RollingStoneSuspend(this);
                                            break;
                                        case StoneClass.RollingStoneHibernate:
                                            stone = new RollingStoneHibernate(this);
                                            break;
                                        case StoneClass.RollingStoneTime:
                                            stone = new RollingStoneTime(this);
                                            break;
                                        case StoneClass.RollingStoneDate:
                                            stone = new RollingStoneDate(this);
                                            break;
                                        case StoneClass.RollingStoneCloseKrento:
                                            stone = new RollingStoneCloseKrento(this);
                                            break;
                                        case StoneClass.RollingStoneControlPanel:
                                            stone = new RollingStoneControlPanel(this);
                                            break;
                                        case StoneClass.RollingStoneAddRemoveProgram:
                                            stone = new RollingStoneAddRemoveProgram(this);
                                            break;
                                        case StoneClass.RollingStoneDateAndTime:
                                            stone = new RollingStoneDateAndTime(this);
                                            break;
                                        case StoneClass.RollingStonePrintersAndFaxes:
                                            stone = new RollingStonePrintersAndFaxes(this);
                                            break;
                                        case StoneClass.RollingStoneNetworkConnections:
                                            stone = new RollingStoneNetworkConnections(this);
                                            break;
                                        case StoneClass.RollingStoneFonts:
                                            stone = new RollingStoneFonts(this);
                                            break;
                                        case StoneClass.RollingStoneDisplayBackground:
                                            stone = new RollingStoneDisplayBackground(this);
                                            break;
                                        case StoneClass.RollingStoneAppearance:
                                            stone = new RollingStoneAppearance(this);
                                            break;
                                        case StoneClass.RollingStoneUserAccounts:
                                            stone = new RollingStoneUserAccounts(this);
                                            break;
                                        case StoneClass.RollingStoneThemes:
                                            stone = new RollingStoneThemes(this);
                                            break;
                                        case StoneClass.RollingStoneAccessibilityOptions:
                                            stone = new RollingStoneAccessibilityOptions(this);
                                            break;
                                        case StoneClass.RollingStoneMouse:
                                            stone = new RollingStoneMouse(this);
                                            break;
                                        case StoneClass.RollingStoneKeyboard:
                                            stone = new RollingStoneKeyboard(this);
                                            break;
                                        case StoneClass.RollingStoneWikipedia:
                                            stone = new RollingStoneWikipedia(this);
                                            break;
                                        case StoneClass.RollingStoneGoogle:
                                            stone = new RollingStoneGoogle(this);
                                            break;
                                        case StoneClass.RollingStoneGMail:
                                            stone = new RollingStoneGMail(this);
                                            break;
                                        case StoneClass.RollingStoneTwitter:
                                            stone = new RollingStoneTwitter(this);
                                            break;
                                        case StoneClass.RollingStoneBlogger:
                                            stone = new RollingStoneBlogger(this);
                                            break;
                                        case StoneClass.RollingStoneFaceBook:
                                            stone = new RollingStoneFaceBook(this);
                                            break;
                                        case StoneClass.RollingStoneYouTube:
                                            stone = new RollingStoneYouTube(this);
                                            break;
                                        case StoneClass.RollingStoneWordpress:
                                            stone = new RollingStoneWordpress(this);
                                            break;
                                        case StoneClass.RollingStoneYahoo:
                                            stone = new RollingStoneYahoo(this);
                                            break;
                                        case StoneClass.RollingStoneReddit:
                                            stone = new RollingStoneReddit(this);
                                            break;
                                        case StoneClass.RollingStoneFlickr:
                                            stone = new RollingStoneFlickr(this);
                                            break;
                                        case StoneClass.RollingStoneDelicious:
                                            stone = new RollingStoneDelicious(this);
                                            break;
                                        default:
                                            stone = new RollingStoneFile(this);
                                            break;
                                    }

                                }
                                else
                                {
                                    ConstructorInfo constructor = type.GetConstructor(parameterList);
                                    if (constructor != null)
                                    {
                                        try
                                        {
                                            stone = (RollingStoneBase)constructor.Invoke(new object[] { this });
                                        }
                                        catch (Exception ex)
                                        {
                                            TraceDebug.Trace(ex.Message);
                                            stone = null;
                                        }
                                    }
                                }
                                if (stone != null)
                                {
                                    AddStone<RollingStoneBase>((RollingStoneBase)stone);
                                    stone.StoneID = stoneID;

                                    //iniFile.ReadConfiguration(stoneID, stone);
                                    stone.ReadConfiguration(iniFile);

                                    stone.FixupConfiguration();
                                }
                            }
                        }
                    }
                    else //stoneID for one of the stones is empty
                    {
                        //We have the stone record without stone id GUID,
                        //something like Stone0=
                        //For keeping the stones order, we replace this stone with default one
                        //because in this case is impossible to find back the right stone
                        stone = CreateNewStone();
                        //Check: stone number id = new stone id
                        //The new stone must be inserted at the same position as an old stone
                        //with missing id number
                    }
                } //for

                if (!string.IsNullOrEmpty(ExecutingCircle))
                {
                    stone = new RollingStoneRing(this);
                    Stones.Add(stone);
                    stone.Order = stones.Count - 1;
                    stone.SetStoneBackground(stoneBackgroundBitmap, stoneBackgroundWidth, stoneBackgroundHeight);
                    stone.StoneIndicator = this.indicator;
                    stone.StoneID = GuidCreator.NewGuid().ToString();
                    stone.CanChangeType = false;
                    stone.CanConfigure = false;
                    stone.CanRemove = false;
                    stone.IsVirtual = true;
                    ((RollingStoneRing)stone).ResourceName = "PreviousRing.png";
                    ((RollingStoneRing)stone).TargetName = ExecutingCircle;
                    string desc = string.Empty;
                    HistoryEntry entry = history[ExecutingCircle];
                    if (entry != null)
                    {
                        desc = entry.Caption;
                    }
                    else
                        desc = Path.GetFileNameWithoutExtension(ExecutingCircle);

                    ((RollingStoneRing)stone).TargetDescription = desc;
                    ((RollingStoneRing)stone).FixupResourceLogo();
                    ExecutingCircle = null;

                }
            }
            finally
            {
                EndUpdate();
            }


            stonesKeys = null;

            DoCircleLoaded(EventArgs.Empty);

            if (storedAngle > 0)
            {
                rollingDirection = RollingDirection.Right;
                Rotate(storedAngle);
            }
        }

        public bool IsVirtual { get; set; }

        /// <summary>
        /// Saves the circle file flusing all changes to the disk.
        /// </summary>
        /// <param name="configurationFile">The configuration file.</param>
        public void SaveCircle(string configurationFile)
        {
            if (IsVirtual)
                return;

            MemIniFile iniFile;
            HistoryEntry entry;
            string fullConfigurationFile = FileOperations.StripFileName(configurationFile);

            entry = history[fullConfigurationFile];
            if (entry == null)
                iniFile = new MemIniFile(fullConfigurationFile, true, false);
            else
                iniFile = entry.IniFile;
            if (iniFile == null)
                return;
            iniFile.Clear();
            if (Count > 0)
            {
                iniFile.WriteInteger("Settings", "Angle", (int)(stones[0].Angle));
            }

            if (entry != null)
            {
                iniFile.WriteString("Settings", "Logo", entry.LogoFile);
                iniFile.WriteString("Settings", "Description", entry.Description);
            }

            for (int i = 0; i < Count; i++)
            {
                if (!stones[i].IsVirtual)
                {
                    iniFile.WriteString("Stones", "Stone" + i.ToString(), stones[i].StoneID);
                    iniFile.WriteString(stones[i].StoneID, "StoneClass", stones[i].FullName);
                    iniFile.WriteString(stones[i].StoneID, "Assembly", stones[i].AssemblyName);
                    stones[i].SaveConfiguration(iniFile);
                }
            }

            iniFile.Save();
        }


        internal static T TypeAttribute<T>(Type t)
        {
            object[] attributes = t.GetCustomAttributes(typeof(T), false);
            if (attributes != null)
            {
                if (attributes.Length > 0)
                    return (T)attributes[0];
            }
            return default(T);
        }

        internal Bitmap StoneIcon(RollingStoneBase stone)
        {
            if (stone == null)
                return BitmapPainter.ResizeBitmap(NativeThemeManager.Load("SmallKrento.png"), 64, 64, true);

            TypeObjectEntry entry = (TypeObjectEntry)availableStoneClasses[stone.FullName];
            if (entry == null)
                return BitmapPainter.ResizeBitmap(NativeThemeManager.Load("SmallKrento.png"), 64, 64, true);

            if (entry.BuiltIn)
                return BitmapPainter.ResizeBitmap(NativeThemeManager.Load(entry.CustomIcon), 64, 64, true);
            else
            {
                bool found = false;
                string fileName = entry.CustomIcon;
                if (!string.IsNullOrEmpty(fileName))
                {
                    if (FileOperations.FileExists(fileName))
                        found = true;
                }

                if (found)
                {
                    Bitmap tmp = FastBitmap.FromFile(fileName);
                    return BitmapPainter.ResizeBitmap(tmp, 64, 64, true);
                }
                else
                    return BitmapPainter.ResizeBitmap(NativeThemeManager.Load("SmallKrento.png"), 64, 64, true);

            }
        }

        internal string StoneDescription(RollingStoneBase stone)
        {
            if (stone == null)
                return null;

            TypeObjectEntry entry = (TypeObjectEntry)availableStoneClasses[stone.FullName];
            if (entry == null)
                return null;

            return (string)entry.Value;
        }

        internal string StoneAuthor(RollingStoneBase stone)
        {
            if (stone == null)
                return null;

            TypeObjectEntry entry = (TypeObjectEntry)availableStoneClasses[stone.FullName];
            if (entry == null)
                return null;

            return (string)entry.Author;
        }

        internal bool StoneBuiltIn(RollingStoneBase stone)
        {
            if (stone == null)
                return false;

            TypeObjectEntry entry = (TypeObjectEntry)availableStoneClasses[stone.FullName];
            if (entry == null)
                return false;

            return (bool)entry.BuiltIn;
        }

        internal string StoneVersion(RollingStoneBase stone)
        {
            if (stone == null)
                return null;

            TypeObjectEntry entry = (TypeObjectEntry)availableStoneClasses[stone.FullName];
            if (entry == null)
                return null;

            return (string)entry.Version;
        }

        internal string StoneCopyright(RollingStoneBase stone)
        {
            if (stone == null)
                return null;

            TypeObjectEntry entry = (TypeObjectEntry)availableStoneClasses[stone.FullName];
            if (entry == null)
                return null;

            return (string)entry.Copyright;
        }

        /// <summary>
        /// Loads the stone classes.
        /// </summary>
        public void LoadStoneClasses()
        {
            Assembly assembly;
            Type[] types;
            string settingsFileName;
            MemIniFile iniFile;
            string customIcon = null;
            string description = null;
            string author;
            string version;
            string copyright;
            string section;

            availableStoneClasses.Clear();

            availableStoneClasses.Add(StoneClass.RollingStoneFile, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneFile), SR.StoneFile, "Launcher.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneRing, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneRing), SR.StoneRing, "DefaultRing.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneMyIP, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneMyIP), SR.StoneMyIP, "MyIP.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneMyDocuments, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneMyDocuments), SR.StoneMyDocuments, "MyDocuments.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneMyPictures, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneMyPictures), SR.StoneMyPictures, "MyPictures.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneMyMusic, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneMyMusic), SR.StoneMyMusic, "MyMusic.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneMyComputer, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneMyComputer), SR.StoneMyComputer, "MyComputer.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneRecycleBin, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneRecycleBin), SR.StoneRecycleBin, "RecycleBin.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneShutdown, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneShutdown), SR.StoneShutdown, "Shutdown.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneRestart, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneRestart), SR.StoneRestart, "Restart.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneSuspend, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneSuspend), SR.StoneSuspend, "Suspend.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneHibernate, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneHibernate), SR.StoneHibernate, "Hibernate.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneTime, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneTime), SR.CurrentTime, "Timer.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneDate, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneDate), SR.CurrentDate, "Calendar.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneCloseKrento, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneCloseKrento), SR.PulsarClose, "CloseKrento.png"));


            availableStoneClasses.Add(StoneClass.RollingStoneControlPanel, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneControlPanel), SR.ControlPanel, "control.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneAddRemoveProgram, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneAddRemoveProgram), SR.AppletAddRemoveProgram, "AppAddRemove.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneDateAndTime, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneDateAndTime), SR.AppletDateAndTime, "AppDateTime.png"));
            availableStoneClasses.Add(StoneClass.RollingStonePrintersAndFaxes, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStonePrintersAndFaxes), SR.AppletPrintersAndFaxes, "AppPrinters.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneNetworkConnections, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneNetworkConnections), SR.AppletNetworkConnections, "AppNetwork.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneFonts, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneFonts), SR.AppletFonts, "AppFonts.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneDisplayBackground, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneDisplayBackground), SR.AppletDisplayBackground, "AppBackground.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneAppearance, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneAppearance), SR.AppletAppearance, "AppAppearance.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneUserAccounts, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneUserAccounts), SR.AppletUserAccounts, "AppUsers.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneThemes, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneThemes), SR.AppletThemes, "AppThemes.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneAccessibilityOptions, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneAccessibilityOptions), SR.AppletAccessibilityOptions, "AppAccess.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneMouse, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneMouse), SR.AppletMouse, "AppMouse.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneKeyboard, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneKeyboard), SR.AppletKeyboard, "AppKeyboard.png"));

            availableStoneClasses.Add(StoneClass.RollingStoneWikipedia, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneWikipedia), SR.Wikipedia, "WebWiki.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneGoogle, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneGoogle), SR.Google, "WebGoogle.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneGMail, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneGMail), SR.GMail, "WebGmail.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneTwitter, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneTwitter), SR.Twitter, "WebTwitter.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneBlogger, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneBlogger), SR.Blogger, "WebBlogger.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneFaceBook, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneFaceBook), SR.Facebook, "WebFacebook.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneYouTube, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneYouTube), SR.YouTube, "WebYoutube.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneWordpress, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneWordpress), SR.Wordpress, "WebWordpress.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneYahoo, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneYahoo), SR.Yahoo, "WebYahoo.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneReddit, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneReddit), SR.Reddit, "WebReggit.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneFlickr, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneFlickr), SR.Flickr, "WebFlickr.png"));
            availableStoneClasses.Add(StoneClass.RollingStoneDelicious, new TypeObjectEntry(typeof(Krento.RollingStones.RollingStoneDelicious), SR.Delicious, "WebDelicious.png"));

            string folder = GlobalConfig.StoneClasses;

            if (!Directory.Exists(folder))
                return;

            string[] files = Directory.GetFiles(folder, "*.dll", SearchOption.AllDirectories);
            foreach (string fileName in files)
            {
                try
                {
                    if (NativeMethods.IsAssembly(fileName))
                        assembly = Assembly.LoadFile(fileName);
                    else
                        assembly = null;
                }
                catch (BadImageFormatException)
                {
                    //this is not a .NET assembly, skip it
                    continue;
                }

                if (assembly == null)
                    continue;

                settingsFileName = Path.Combine(Path.GetDirectoryName(fileName), "config.ini");
                if (FileOperations.FileExists(settingsFileName))
                {
                    iniFile = new MemIniFile(settingsFileName);
                    iniFile.Load();
                }
                else
                    iniFile = null;

                try
                {
                    types = assembly.GetExportedTypes();
                }
                catch (Exception ex)
                {
                    TraceDebug.Trace(ex.Message);
                    continue;
                }

                Type baseType = typeof(Krento.RollingStones.RollingStoneBase);

                foreach (Type t in types)
                {
                    if (GenericHelper.IsSubclassOfRawGeneric(baseType, t))
                    {
                        if (iniFile != null)
                        {
                            section = t.Name;
                            if (iniFile.SectionExists(section))
                            {
                                description = iniFile.ReadString(section, "Description");
                                customIcon = iniFile.ReadString(section, "Icon");
                            }
                            if (string.IsNullOrEmpty(description))
                                description = iniFile.ReadString("Stone", "Description", SR.GetString(Path.GetFileNameWithoutExtension(fileName)));

                            if (string.IsNullOrEmpty(customIcon))
                                customIcon = iniFile.ReadString("Stone", "Icon", string.Empty);

                            author = iniFile.ReadString("Stone", "Author", string.Empty);
                            version = iniFile.ReadString("Stone", "Version", "1.0.0.0");
                            copyright = iniFile.ReadString("Stone", "Copyright", string.Empty);
                            if (!string.IsNullOrEmpty(customIcon))
                                customIcon = Path.Combine(Path.GetDirectoryName(fileName), customIcon);
                        }
                        else
                        {
                            description = SR.GetString(Path.GetFileNameWithoutExtension(fileName));
                            customIcon = string.Empty;
                            author = string.Empty;
                            version = "1.0.0.0";
                            copyright = string.Empty;
                        }
                        if (string.IsNullOrEmpty(description))
                        {
                            StoneDescriptionAttribute sdesc = TypeAttribute<StoneDescriptionAttribute>(t);
                            if (sdesc != null)
                                description = sdesc.Description;
                            if (string.IsNullOrEmpty(description))
                                description = t.FullName;
                        }

                        availableStoneClasses.Add(t.FullName, new TypeObjectEntry(t, description, customIcon, author, version, copyright));
                    }
                }

                if (iniFile != null)
                {
                    iniFile.Dispose();
                    iniFile = null;
                }
            }
        }

        /// <summary>
        /// Loads the settings.
        /// </summary>
        /// <param name="configurationFile">The configuration file.</param>
        public void LoadSettings(string configurationFile)
        {

            MemIniFile iniFile = new MemIniFile(configurationFile);

            iniFile.Load();


            int x;
            int y;

            x = GlobalSettings.ManagerLeft;
            y = GlobalSettings.ManagerTop;

            try
            {
                this.CircleLocation = (CircleLocation)Enum.Parse(typeof(CircleLocation), iniFile.ReadString("Position", "Location", "Point"), true);
            }
            catch (Exception ex)
            {
                TraceDebug.Trace(ex.Message);
                this.CircleLocation = CircleLocation.Point;
            }

            try
            {
                switch (CircleLocation)
                {
                    case CircleLocation.Point:
                        if ((x == -1) && (y == -1))
                        {
                            Position = PrimaryScreen.Center;
                        }
                        else
                        {
                            Left = x;
                            Top = y;
                        }
                        break;
                    case CircleLocation.ScreenCenter:
                        Position = PrimaryScreen.Center;
                        break;
                    case CircleLocation.Fixed:
                        Left = x;
                        Top = y;
                        break;
                    default:
                        Position = PrimaryScreen.Center;
                        break;
                }
            }
            catch (Exception ex)
            {
                TraceDebug.Trace("SetLeftTopPosition: " + ex.Message);
            }


            //Radius = iniFile.ReadInteger("Options", "Radius", 200);

            Speed = iniFile.ReadInteger("Options", "Speed", 3);

            string themeName = iniFile.ReadString("Background", "Theme", string.Empty);

            ReloadSkin(themeName);


            iniFile.Dispose();
        }


        /// <summary>
        /// Repaints the buttons internal.
        /// </summary>
        internal void RepaintButtonsInternal()
        {
            if (GlobalSettings.ShowManagerButtons)
            {
                managerWindow.Clear();
                managerWindow.DrawDefaultBackground(false);
                managerWindow.Update(true);
            }
        }

        /// <summary>
        /// Reloads the skin.
        /// </summary>
        /// <param name="skinFileName">Name of the skin file.</param>
        public void ReloadSkin(string skinFileName)
        {
            //if (!string.IsNullOrEmpty(skinFileName))
            //{
            skinFileName = FileOperations.StripFileName(skinFileName);
            this.skinFileName = skinFileName;

            if (!string.IsNullOrEmpty(skinFileName))
            {
                string fullSkinFileName = FileOperations.UnExpandPath(skinFileName);
                NativeMethods.WriteString(GlobalConfig.KrentoSettingsFileName, "Background", "Theme", fullSkinFileName);
            }

            LoadKrentoCoreBackground(skinFileName);

            for (int i = 0; i < stones.Count; i++)
            {
                stones[i].SetStoneBackground(stoneBackgroundBitmap, stoneBackgroundWidth, stoneBackgroundHeight);
                stones[i].StoneIndicator = indicator;
            }


            //not used because if no buttons, no new background
            //RepaintButtonsInternal();
            managerWindow.Clear();
            managerWindow.DrawDefaultBackground(false);
            managerWindow.Update(true);

            UpdateMainWindowPosition();


        }

        /// <summary>
        /// Saves the manager settings.
        /// </summary>
        public void SaveSettings(string configurationFile)
        {
            string fullConfigurationFile = FileOperations.StripFileName(configurationFile);
            MemIniFile iniFile = new MemIniFile(fullConfigurationFile);
            iniFile.Load();

            try
            {
                iniFile.WriteString("Position", "Location", CircleLocation.ToString());
                //iniFile.WriteInteger("Options", "Radius", Radius);
                iniFile.WriteInteger("Options", "Speed", Speed);

                if (!string.IsNullOrEmpty(skinFileName))
                {
                    string fullSkinFileName = FileOperations.UnExpandPath(skinFileName);
                    iniFile.WriteString("Background", "Theme", fullSkinFileName);
                }
                else
                {
                    iniFile.DeleteKey("Background", "Theme");
                }

                iniFile.Save();
            }

            finally
            {
                iniFile.Dispose();
            }
        }

        /// <summary>
        /// Gets or sets the size of the stone.
        /// </summary>
        /// <value>The size of the stone.</value>
        [DefaultValue(128)]
        public int StoneSize
        {
            get { return stoneSize; }
            set { SetStoneSize(value); }
        }

        private void SetStoneSize(int value)
        {
            if (stoneSize != value)
            {
                stoneSize = value;
                for (int cnt = 0; cnt < stones.Count; cnt++)
                {
                    stones[cnt].Size = new Size(value, value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the stone border width.
        /// </summary>
        /// <value>The stone border width.</value>
        public int StoneBorder
        {
            get { return stoneBorder; }
            set { stoneBorder = value; }
        }

        /// <summary>
        /// Gets or sets the left.
        /// </summary>
        /// <value>The left.</value>
        public int Left
        {
            get { return left; }
            set
            {
                if (left != value)
                {
                    left = value;
                    UpdateMainWindowPosition();
                    Rotate(0);
                }
            }
        }

        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        /// <value>The top.</value>
        public int Top
        {
            get { return top; }
            set
            {
                if (top != value)
                {
                    top = value;
                    UpdateMainWindowPosition();
                    Rotate(0);
                }
            }
        }

        private void MoveStones(int deltaX, int deltaY)
        {
            if (movingStones)
                return;

            IntPtr Defer;

            movingStones = true;
            try
            {
                Defer = NativeMethods.BeginDeferWindowPos(Count);

                for (int i = 0; i < Count; i++)
                {
                    stones[i].MoveDelta(Defer, deltaX, deltaY);
                }

                NativeMethods.EndDeferWindowPos(Defer);
                Thread.Sleep(0);

                if (GlobalSettings.ShowManagerButtons && ShowStoneHint)
                {
                    screenHint.FollowMovement(deltaX, deltaY);
                }
            }
            finally
            {
                movingStones = false;
            }
        }

        private void SetPosition(int left, int top)
        {
            this.left = left;
            this.top = top;
            UpdateMainWindowPosition();
            Rotate(0);
        }

        /// <summary>
        /// Gets or sets the circle location.
        /// </summary>
        /// <value>The circle location.</value>
        public CircleLocation CircleLocation
        {
            get { return circleLocation; }
            set { circleLocation = value; }
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public Point Position
        {
            get { return new Point(left, top); }
            set { SetPosition(value.X, value.Y); }
        }

        internal void MoveTo(int x, int y)
        {
            int deltaX = x - this.left;
            int deltaY = y - this.top;
            this.left = x;
            this.top = y;
            managerWindow.StopRotation();
            if (!((deltaX == 0) && (deltaY == 0)))
                MoveStones(deltaX, deltaY);
            managerHintWindow.MoveWindowDelta(deltaX, deltaY);
            if (GlobalSettings.ShowManagerButtons)
            {
                if (buttonHintWindow != null)
                {
                    buttonHintWindow.MoveWindowDelta(deltaX, deltaY);
                }
            }
        }

        /// <summary>
        /// Gets or sets the rolling direction.
        /// </summary>
        /// <value>The rolling direction.</value>
        public RollingDirection RollingDirection
        {
            get { return rollingDirection; }
            set { rollingDirection = value; }
        }

        /// <summary>
        /// Gets or sets the radius.
        /// </summary>
        /// <value>The radius.</value>
        [DefaultValue(200)]
        public int Radius
        {
            get { return radius; }
            set
            {
                if (radius != value)
                {
                    radius = value;
                    if (GlobalSettings.FlatRing)
                    {
                        radiusHorizontal = radius;
                        radiusVertical = radius;
                    }
                    else
                    {
                        radiusHorizontal = radius * horizontalMultiplex;
                        radiusVertical = radius * verticalMultiplex;
                    }
                    UpdateCoordinates();
                }
            }
        }



        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="StonesManager"/> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        public bool Visible
        {
            get { return visible; }
            set { SetVisible(value); }
        }

        /// <summary>
        /// Replaces the alpha value of the manager and stones windows.
        /// </summary>
        /// <param name="newAlpha">The new alpha.</param>
        public void ReplaceAlpha(byte newAlpha)
        {
            managerWindow.ReplaceAlpha(newAlpha);
            for (int i = 0; i < stones.Count; i++)
            {
                stones[i].ReplaceAlpha(newAlpha);
            }
        }

        private void SetVisible(bool value)
        {
            if (visible != value)
            {

                if (Fading)
                    StopFade();

                HideRingHint();

                if (GlobalSettings.ShowManagerButtons)
                {
                    if (buttonHintWindow != null)
                        buttonHintWindow.Visible = false;
                    HideScreenHint();
                }


                this.visible = value;

                if (managerWindow.Alpha == 0)
                    managerWindow.RestoreAlpha();

                if (StoneMenu != null)
                    StoneMenu.CloseUp();


                IntPtr Defer = NativeMethods.BeginDeferWindowPos(Count + 1);
                int deferFlags = (int)(SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_NOMOVE | SetWindowPosFlags.SWP_NOZORDER | SetWindowPosFlags.SWP_NOREDRAW);
                if (!value)
                    deferFlags = deferFlags | (int)SetWindowPosFlags.SWP_HIDEWINDOW;
                else
                    deferFlags = deferFlags | (int)SetWindowPosFlags.SWP_SHOWWINDOW;

                NativeMethods.DeferWindowPos(Defer, managerHintWindow.Handle, IntPtr.Zero, 0, 0, 0, 0, deferFlags);
                for (int i = 0; i < stones.Count; i++)
                {
                    NativeMethods.DeferWindowPos(Defer, stones[i].Handle, IntPtr.Zero, 0, 0, 0, 0, deferFlags);
                }
                NativeMethods.EndDeferWindowPos(Defer);

                managerWindow.Visible = value;

                for (int i = 0; i < stones.Count; i++)
                {
                    stones[i].Visible = value;
                }

                if (visible)
                {
                    Rotate(0);
                    managerWindow.BringToFront();
                    managerWindow.Activate();
                    if (managerWindow.CanFocus)
                        managerWindow.Focus();
                }
                else
                {
                    BackToRealityQuiet();
                }

                OnVisibilityChanged(EventArgs.Empty);
            }
        }


        /// <summary>
        /// Gets or sets the rotation speed.
        /// </summary>
        /// <value>The rotation speed.</value>
        public int Speed
        {
            get { return speed; }
            set { speed = value; }
        }


        /// <summary>
        /// Creates the new stone.
        /// </summary>
        /// <returns></returns>
        public RollingStoneFile CreateNewStone()
        {
            RollingStoneFile stone = new RollingStoneFile(this);
            Stones.Add(stone);
            stone.Order = stones.Count - 1;
            stone.SetStoneBackground(stoneBackgroundBitmap, stoneBackgroundWidth, stoneBackgroundHeight);
            stone.StoneIndicator = this.indicator;
            stone.StoneID = GuidCreator.NewGuid().ToString();

            return stone;
        }

        public T AddStone<T>(T stone) where T : RollingStoneBase
        {
            Stones.Add(stone);
            stone.Order = stones.Count - 1;
            stone.SetStoneBackground(stoneBackgroundBitmap, stoneBackgroundWidth, stoneBackgroundHeight);
            stone.StoneIndicator = this.indicator;
            UpdateItems();
            return stone;
        }

        public void BeginUpdate()
        {
            updateCount++;
        }

        public void EndUpdate()
        {
            updateCount--;
            if (updateCount < 0)
                updateCount = 0;
            if (updateCount == 0)
            {
                UpdateItems();
            }
        }

        private void UpdateItems()
        {
            if (updateCount != 0)
                return;

            UpdateCoordinates();
            Rotate(0);
        }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public int Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        /// <summary>
        /// Updates stones windows
        /// </summary>
        public void Update()
        {
            for (int i = 0; i < stones.Count; i++)
                stones[i].Update();
        }

        private void UpdateMainWindowPosition()
        {

            int managerOffsetLeft = (int)(managerWindow.Width / 2);
            int managerOffsetTop = (int)(managerWindow.Height / 2);

            managerWindow.Left = left - managerOffsetLeft;
            managerWindow.Top = top - managerOffsetTop;
        }

        private void UpdateCoordinates()
        {

            UpdateMainWindowPosition();

            int Total = stones.Count;
            if (Total > 0)
            {
                double TurnAngle = (double)(360.0d / Total);
                double Angle = 0;
                for (int cnt = 0; cnt < Total; cnt++)
                {
                    stones[cnt].Angle = Angle;
                    stones[cnt].Left = 0;
                    stones[cnt].Top = 0;
                    stones[cnt].Scale = 1.0;
                    stones[cnt].Order = cnt;
                    Angle += TurnAngle;
                }
            }

        }

        internal void RotateRequest(int speed)
        {
            managerWindow.Perform(NativeMethods.CM_ROTATE, (IntPtr)speed, IntPtr.Zero);
        }

        internal void UpdateAngles()
        {
            int Total = stones.Count;
            if (Total > 0)
            {
                double TurnAngle = (double)(360.0d / Total);
                double Angle = 0;
                for (int cnt = 0; cnt < Total; cnt++)
                {
                    stones[cnt].Angle = Angle;
                    stones[cnt].Scale = 1.0;
                    stones[cnt].Order = cnt;
                    Angle += TurnAngle;
                }
            }
        }

        /// <summary>
        /// Gets the count of the stones. Normally the value must be equal to 12
        /// </summary>
        /// <value>The number of the stones.</value>
        public int Count
        {
            get { return stones.Count; }
        }

        public void Rotate()
        {
            Rotate(speed);
        }

        public RollingStoneBase FindNearestStone(RollingStoneBase baseStone, int x, int y)
        {
            int deltaX = radius;
            int deltaY = radius;
            RollingStoneBase result = null;

            for (int i = 0; i < Count; i++)
            {
                if (stones[i] != baseStone)
                {
                    int kx = Math.Abs(baseStone.Left - stones[i].Left);
                    int ky = Math.Abs(baseStone.Top - stones[i].Top);
                    if ((kx < deltaX) && (ky < deltaY))
                    {
                        deltaX = kx;
                        deltaY = ky;
                        result = stones[i];
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Updates the scale value. When changing the values of min or max scale, update scale value
        /// for correct stones painting
        /// </summary>
        public void UpdateScaleValue()
        {
            halfPlusScale = (scale.MaxValue + scale.MinValue) / 2;
            halfMinScale = (scale.MaxValue - scale.MinValue) / 2;
        }

        public void Rotate(int rotateSpeed)
        {
            lock (this)
            {
                if (inRotation)
                    return;
                try
                {
                    inRotation = true;
                    int stoneLeft;
                    int stoneTop;
                    int stoneHalf = stoneSize / 2;

                    int Total = Count;

                    if (Total == 0)
                        return;

                    for (int cnt = Total - 1; cnt > -1; cnt--)
                    {
                        double angle = stones[cnt].Angle;


                        if (rotateSpeed > 0)
                        {
                            if (rollingDirection == RollingDirection.Left)
                            {
                                angle -= rotateSpeed;
                                if (angle < 0)
                                    angle = 360 + angle;
                            }
                            else
                            {
                                angle += rotateSpeed;
                                if (angle > 359)
                                    angle = angle - 360;
                            }
                        }

                        stones[cnt].Angle = angle;
                        double angleRadians = (double)(angle * MathUtils.PiDiv180);
                        double stoneScale;
                        if (GlobalSettings.FlatRing)
                            stoneScale = 1.0;
                        else
                            stoneScale = halfPlusScale + halfMinScale * Math.Cos(angleRadians);

                        stoneLeft = (int)(left + Math.Sin(angleRadians) * radiusHorizontal) - stoneHalf;
                        stoneTop = (int)(top + Math.Cos(angleRadians) * radiusVertical) - stoneHalf;

                        stones[cnt].NewPosition(stoneLeft, stoneTop);
                        stones[cnt].Scale = stoneScale;

                    }


                    Rearrange();
                }
                finally
                {
                    inRotation = false;
                }
            }
        }

        public bool InRotation
        {
            get { return inRotation; }
        }
        /// <summary>
        /// Selects the first stone.
        /// </summary>
        public void SelectFirstStone()
        {
            if (stones.Count > 0)
                SetCurrentStone(0);
            else
                SetCurrentStone(-1);
        }

        /// <summary>
        /// Selects the next stone.
        /// </summary>
        public void SelectNextStone()
        {
            if (activeStone == null)
            {
                SelectFirstStone();
                return;
            }

            ICircleEnumerator enumerator = (ICircleEnumerator)GetEnumerator();
            enumerator.JumpTo(stones.IndexOf(activeStone));
            enumerator.MoveNext();
            SetCurrentStone((RollingStoneBase)enumerator.Current);
        }

        /// <summary>
        /// Selects the previous stone. This methods selects the stone with the smaller number than
        /// current selected stone
        /// </summary>
        public void SelectPrevStone()
        {
            if (activeStone == null)
            {
                SelectFirstStone();
                return;
            }

            ICircleEnumerator enumerator = (ICircleEnumerator)GetEnumerator();
            enumerator.JumpTo(stones.IndexOf(activeStone));
            enumerator.MovePrevious();
            SetCurrentStone((RollingStoneBase)enumerator.Current);

        }

        /// <summary>
        /// Rearranges the stones. The most heavy method of this class
        /// </summary>
        private void Rearrange()
        {
            if (stones == null)
                return;

            lock (this)
            {
                int w;
                int hightest = -1;
                int lowest = -1;
                int idx;
                int cnt;

                ICircleEnumerator enumerator;
                RollingStoneBase currentStone;
                RollingStoneBase previousStone;

                cnt = stones.Count;

                if (cnt == 0)
                    return;

                enumerator = (ICircleEnumerator)GetEnumerator();

                if (cnt > 1)
                {
                    w = 0;
                    for (int i = 0; i < cnt; i++)
                    {
                        if (stones[i].Top > w)
                        {
                            w = stones[i].Top;
                            lowest = i;
                        }
                    }

                    if (lowest < 0)
                        return;

                    for (int i = 0; i < cnt; i++)
                    {
                        if (stones[i].Top < w)
                        {
                            w = stones[i].Top;
                            hightest = i;
                        }
                    }

                    if (hightest < 0)
                        return;



                    StoneWindow.BeginDeferWindowPos(cnt);

                    idx = lowest;
                    enumerator.JumpTo(idx);
                    while (idx != hightest)
                    {
                        previousStone = (RollingStoneBase)enumerator.Current;
                        enumerator.MovePrevious();
                        currentStone = (RollingStoneBase)enumerator.Current;
                        idx = currentStone.Order;

                        if (!GlobalConfig.LowMemory)
                        {
                            currentStone.UpdateRescale();
                        }

                        if (idx != hightest)
                        {
                            currentStone.DeferAfter(previousStone);
                        }
                    }

                    idx = lowest;
                    enumerator.JumpTo(idx);
                    while (idx != hightest)
                    {
                        previousStone = (RollingStoneBase)enumerator.Current;
                        enumerator.MoveNext();
                        currentStone = (RollingStoneBase)enumerator.Current;
                        idx = currentStone.Order;

                        if (!GlobalConfig.LowMemory)
                        {
                            currentStone.UpdateRescale();
                        }

                        currentStone.DeferAfter(previousStone);

                    }


                    StoneWindow.EndDeferWindowPos();

                }
                else
                {
                    lowest = 0;
                }
                enumerator.JumpTo(lowest);
                currentStone = (RollingStoneBase)enumerator.Current;
                if (!GlobalConfig.LowMemory)
                {
                    if (!GlobalSettings.FlatRing)
                        currentStone.UpdateRescale();
                }
                currentStone.MoveOnTop();

                if (!mouseInsideStone)
                {
                    if (activeStone != currentStone)
                    {
                        SetCurrentStone(currentStone);
                    }
                }

                enumerator.JumpTo(hightest);
                previousStone = (RollingStoneBase)enumerator.Current;
                enumerator.MoveNext();
                currentStone = (RollingStoneBase)enumerator.Current;
                previousStone.ShowAfter(currentStone);

                HideScreenHint();
                if (activeStone != null)
                {
                    activeStone.MoveStoneHint();
                }

                BringToFront();

            }
        }


        /// <summary>
        /// Gets the handle of the manager window.
        /// </summary>
        /// <value>The handle of the manager window.</value>
        public IntPtr Handle
        {
            get { return managerWindow.Handle; }
        }

        public void ShowScreenHintIfNeeded()
        {
            if (managerWindow.Visible)
            {
                if (GlobalSettings.ShowManagerButtons)
                {
                    if (ShowStoneHint)
                    {
                        if (Count > 0)
                        {
                            if ((!managerWindow.RotationActive) && (managerWindow.SpeedCounter == 0) && (!isFading))
                            {
                                ShowScreenHint();
                            }
                        }
                    }
                }
            }

        }

        /// <summary>
        /// Brings the Krento manager window to front.
        /// </summary>
        public void BringToFront()
        {

            if (managerWindow.Visible)
            {
                ShowScreenHintIfNeeded();
                managerWindow.BringToFront();
                managerWindow.Activate();
                if (managerWindow.CanFocus)
                    managerWindow.Focus();
                managerWindow.Cursor = Cursors.Hand;
            }
        }

        internal void ProcessKeyDown(KeyEventArgs e)
        {
            managerWindow.ProcessKeyDown(e);
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region ICircleEnumerable Members

        /// <summary>
        /// Returns an enumerator that iterates through a collection
        /// </summary>
        /// <returns>An ICircleEnumerator object that can be used to iterate through the collection.</returns>
        public IEnumerator GetEnumerator()
        {
            return new StonesCircleEnumerator(this);
        }

        #endregion

        #region IStonesManager Members

        /// <summary>
        /// Gets the drawing canvas of the manager
        /// </summary>
        /// <value>The drawing canvas of the manager.</value>
        public Graphics Canvas
        {
            get { return managerWindow.Canvas; }
        }

        /// <summary>
        /// Sets the current active stone.
        /// </summary>
        /// <param name="stone">The stone.</param>
        public void SetCurrentStone(RollingStoneBase stone)
        {
            if (activeStone != null)
            {
                activeStone.DeselectStone();
            }

            activeStone = stone;
            if (activeStone != null)
            {
                activeStone.SelectStone();
                RedrawScreenHint();
            }
        }

        /// <summary>
        /// Sets the current active stone.
        /// </summary>
        /// <param name="index">The stone index.</param>
        public void SetCurrentStone(int index)
        {
            if (index < 0)
                SetCurrentStone(null);
            else
                if (index >= stones.Count)
                    SetCurrentStone(null);
                else
                    SetCurrentStone(stones[index]);
        }

        /// <summary>
        /// If there is selected stone, it will be executed.
        /// The execution action depends from the stone type
        /// </summary>
        public void Run()
        {
            if (activeStone != null)
                activeStone.Run();
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="StonesManager"/> is fading.
        /// </summary>
        /// <value><c>true</c> if fading; otherwise, <c>false</c>.</value>
        public bool Fading
        {
            get { return isFading; }
        }

        public void StoreFocusedWindow()
        {
            focusedWindow = NativeMethods.GetForegroundWindow();
        }

        public void RestoreFocusedWindow()
        {
            if (focusedWindow != IntPtr.Zero)
                NativeMethods.BringWindowToFront(focusedWindow);
        }

        /// <summary>
        /// Hide the stones and when the fading is finished, execute delegate
        /// </summary>
        /// <param name="execCode">The method to execute.</param>
        public void HideAndExecute(FadeHookDelegate execCode)
        {
            if (execCode == null)
                return;

            if (isFading)
                return;

            if (!Visible)
            {
                execCode();
                return;
            }
            else
            {
                fadeHook = execCode;
                Fade();
            }
        }


        public void ShowAndExecute(FadeHookDelegate execCode)
        {
            if (execCode == null)
                return;

            if (isFading)
                return;

            if (Visible)
            {
                return;
            }
            else
            {
                fadeHook = execCode;
                Fade();
            }
        }

        public void Fade()
        {
            #region Fade activity check

            if (isFading)
                return;

            for (int cnt = 0; cnt < Count; cnt++)
            {
                if (stones[cnt].FadeActive)
                    return;
            }

            if (managerWindow.FadeActive)
                return;

            #endregion

            HideRingHint();
            deleteWindow.Hide();

            if (GlobalSettings.ShowManagerButtons)
            {
                buttonHintWindow.Visible = false;
            }

            isFading = true;
            managerWindow.StopRotation();
            for (int i = 0; i < Count; i++)
            {
                stones[i].Fade(fadeDelay);
            }

            managerWindow.StartFade(fadeDelay);
        }

        #endregion

        public void StopFade()
        {
            isFading = false;
            managerWindow.StopFade();
            for (int i = 0; i < Count; i++)
            {
                stones[i].StopFade();
            }
        }

        internal void ShowAboutBox(RollingStoneBase rollingStoneBase)
        {
            if (AboutBoxShow != null)
                AboutBoxShow(rollingStoneBase, EventArgs.Empty);
        }


        internal void ChangeTypeRequest(int stoneNumber)
        {
            managerWindow.Perform(NativeMethods.CM_CHANGE_STONE, (IntPtr)stoneNumber, IntPtr.Zero);
        }

        internal void RemoveStoneRequest(int stoneNumber, int param)
        {
            managerWindow.Perform(NativeMethods.CM_REMOVE_STONE, (IntPtr)stoneNumber, (IntPtr)param);
        }

        public void RemoveStone(int stoneNumber)
        {
            RollingStoneBase oldStone = stones[stoneNumber];
            stones.RemoveAt(stoneNumber);
            oldStone.Visible = false;
            SetCurrentStone(null);
            oldStone.Dispose();

            UpdateAngles();
            Rotate(0);
            FlushCurrentCircle();

            if (stones.Count == 0)
            {
                HideScreenHint();
                DrawText(SR.KrentoWelcome);
            }
        }

        /// <summary>
        /// Adds the new stone using stone selection dialog.
        /// </summary>
        public void AddNewStone()
        {
            TypeObjectEntry typeEntry;
            bool knownType;
            RollingStoneBase stone = null;
            string stoneClassName = null;

            SuppressHookMessage(true);
            try
            {
                StoneTypeSelector sts = new StoneTypeSelector();
                sts.AddStandardIcons();


                object[] entries = availableStoneClasses.AllValues;
                for (int i = 0; i < entries.Length; i++)
                {
                    sts.StonesSelector.Items.Add(((TypeObjectEntry)entries[i]).Value);
                    if (!((TypeObjectEntry)entries[i]).BuiltIn)
                    {
                        sts.AddCustomIcon(((TypeObjectEntry)entries[i]).CustomIcon);
                    }
                }

                sts.StonesSelector.SelectedIndex = 0;
                sts.ShowDialog();
                if ((sts.DialogResult == DialogResult.OK) && (sts.StonesSelector.SelectedIndex > -1))
                {
                    typeEntry = (TypeObjectEntry)availableStoneClasses[sts.StonesSelector.SelectedIndex];
                    Type type = typeEntry.Type;

                    if (typeEntry.BuiltIn)
                        knownType = true;
                    else
                    {
                        knownType = false;
                    }


                    if (type != null)
                    {

                        if (knownType)
                        {
                            stoneClassName = availableStoneClasses.Keys[sts.StonesSelector.SelectedIndex];
                            switch (stoneClassName)
                            {
                                case StoneClass.RollingStoneFile:
                                    stone = new RollingStoneFile(this);
                                    break;
                                case StoneClass.RollingStoneRing:
                                    stone = new RollingStoneRing(this);
                                    break;
                                case StoneClass.RollingStoneMyIP:
                                    stone = new RollingStoneMyIP(this);
                                    break;
                                case StoneClass.RollingStoneMyDocuments:
                                    stone = new RollingStoneMyDocuments(this);
                                    break;
                                case StoneClass.RollingStoneMyPictures:
                                    stone = new RollingStoneMyPictures(this);
                                    break;
                                case StoneClass.RollingStoneMyMusic:
                                    stone = new RollingStoneMyMusic(this);
                                    break;
                                case StoneClass.RollingStoneMyComputer:
                                    stone = new RollingStoneMyComputer(this);
                                    break;
                                case StoneClass.RollingStoneRecycleBin:
                                    stone = new RollingStoneRecycleBin(this);
                                    break;
                                case StoneClass.RollingStoneShutdown:
                                    stone = new RollingStoneShutdown(this);
                                    break;
                                case StoneClass.RollingStoneRestart:
                                    stone = new RollingStoneRestart(this);
                                    break;
                                case StoneClass.RollingStoneSuspend:
                                    stone = new RollingStoneSuspend(this);
                                    break;
                                case StoneClass.RollingStoneHibernate:
                                    stone = new RollingStoneHibernate(this);
                                    break;
                                case StoneClass.RollingStoneTime:
                                    stone = new RollingStoneTime(this);
                                    break;
                                case StoneClass.RollingStoneDate:
                                    stone = new RollingStoneDate(this);
                                    break;
                                case StoneClass.RollingStoneCloseKrento:
                                    stone = new RollingStoneCloseKrento(this);
                                    break;
                                case StoneClass.RollingStoneControlPanel:
                                    stone = new RollingStoneControlPanel(this);
                                    break;
                                case StoneClass.RollingStoneAddRemoveProgram:
                                    stone = new RollingStoneAddRemoveProgram(this);
                                    break;
                                case StoneClass.RollingStoneDateAndTime:
                                    stone = new RollingStoneDateAndTime(this);
                                    break;
                                case StoneClass.RollingStonePrintersAndFaxes:
                                    stone = new RollingStonePrintersAndFaxes(this);
                                    break;
                                case StoneClass.RollingStoneNetworkConnections:
                                    stone = new RollingStoneNetworkConnections(this);
                                    break;
                                case StoneClass.RollingStoneFonts:
                                    stone = new RollingStoneFonts(this);
                                    break;
                                case StoneClass.RollingStoneDisplayBackground:
                                    stone = new RollingStoneDisplayBackground(this);
                                    break;
                                case StoneClass.RollingStoneAppearance:
                                    stone = new RollingStoneAppearance(this);
                                    break;
                                case StoneClass.RollingStoneUserAccounts:
                                    stone = new RollingStoneUserAccounts(this);
                                    break;
                                case StoneClass.RollingStoneThemes:
                                    stone = new RollingStoneThemes(this);
                                    break;
                                case StoneClass.RollingStoneAccessibilityOptions:
                                    stone = new RollingStoneAccessibilityOptions(this);
                                    break;
                                case StoneClass.RollingStoneMouse:
                                    stone = new RollingStoneMouse(this);
                                    break;
                                case StoneClass.RollingStoneKeyboard:
                                    stone = new RollingStoneKeyboard(this);
                                    break;
                                case StoneClass.RollingStoneWikipedia:
                                    stone = new RollingStoneWikipedia(this);
                                    break;
                                case StoneClass.RollingStoneGoogle:
                                    stone = new RollingStoneGoogle(this);
                                    break;
                                case StoneClass.RollingStoneGMail:
                                    stone = new RollingStoneGMail(this);
                                    break;
                                case StoneClass.RollingStoneTwitter:
                                    stone = new RollingStoneTwitter(this);
                                    break;
                                case StoneClass.RollingStoneBlogger:
                                    stone = new RollingStoneBlogger(this);
                                    break;
                                case StoneClass.RollingStoneFaceBook:
                                    stone = new RollingStoneFaceBook(this);
                                    break;
                                case StoneClass.RollingStoneYouTube:
                                    stone = new RollingStoneYouTube(this);
                                    break;
                                case StoneClass.RollingStoneWordpress:
                                    stone = new RollingStoneWordpress(this);
                                    break;
                                case StoneClass.RollingStoneYahoo:
                                    stone = new RollingStoneYahoo(this);
                                    break;
                                case StoneClass.RollingStoneReddit:
                                    stone = new RollingStoneReddit(this);
                                    break;
                                case StoneClass.RollingStoneFlickr:
                                    stone = new RollingStoneFlickr(this);
                                    break;
                                case StoneClass.RollingStoneDelicious:
                                    stone = new RollingStoneDelicious(this);
                                    break;
                                default:
                                    stone = new RollingStoneFile(this);
                                    break;
                            }

                        }
                        else
                        {
                            ConstructorInfo constructor = type.GetConstructor(parameterList);
                            if (constructor != null)
                            {
                                stone = (RollingStoneBase)constructor.Invoke(new object[] { this });
                            }
                        }

                        if (stone != null)
                        {
                            stones.Add(stone);
                            stone.StoneID = GuidCreator.NewGuid().ToString();
                            stone.Order = stones.Count - 1;
                            stone.SetStoneBackground(stoneBackgroundBitmap, stoneBackgroundWidth, stoneBackgroundHeight);
                            stone.StoneIndicator = this.indicator;
                            stone.FixupConfiguration();
                            UpdateAngles();
                            Rotate(0);
                            int stoneNumber = stones.Count - 1;

                            this.SetCurrentStone(stoneNumber);


                            if (visible)
                                stone.Visible = true;
                            else
                                stone.Visible = false;

                            if (!string.IsNullOrEmpty(this.currentCircle))
                            {
                                FlushCurrentCircle();
                            }
                        }
                    }
                }

                sts.ClearIcons();
                sts.Dispose();

                BringToFront();
            }
            finally
            {
                SuppressHookMessage(false);
            }

        }

        /// <summary>
        /// Changes the type of the stone.
        /// </summary>
        /// <param name="stoneNumber">The stone number.</param>
        internal void ChangeStoneType(int stoneNumber)
        {
            SuppressHookMessage(true);
            try
            {
                StoneTypeSelector sts = new StoneTypeSelector();
                sts.AddStandardIcons();

                RollingStoneBase stone;

                object[] entries = availableStoneClasses.AllValues;
                for (int i = 0; i < entries.Length; i++)
                {
                    sts.StonesSelector.Items.Add(((TypeObjectEntry)entries[i]).Value);
                    if (!((TypeObjectEntry)entries[i]).BuiltIn)
                    {
                        sts.AddCustomIcon(((TypeObjectEntry)entries[i]).CustomIcon);
                    }
                }

                sts.StonesSelector.Text = (string)((TypeObjectEntry)availableStoneClasses[stones[stoneNumber].FullName]).Value;

                sts.ShowDialog();
                if ((sts.DialogResult == DialogResult.OK) && (sts.StonesSelector.SelectedIndex > -1))
                {
                    Type type = ((TypeObjectEntry)availableStoneClasses[sts.StonesSelector.SelectedIndex]).Type;


                    if (type != null)
                    {
                        ConstructorInfo constructor = type.GetConstructor(parameterList);
                        if (constructor != null)
                        {
                            stone = (RollingStoneBase)constructor.Invoke(new object[] { this });
                            if (stone != null)
                            {
                                RollingStoneBase oldStone = stones[stoneNumber];
                                stones.RemoveAt(stoneNumber);
                                stones.Insert(stoneNumber, stone);
                                stone.StoneID = GuidCreator.NewGuid().ToString();

                                stone.Order = oldStone.Order;
                                stone.Left = oldStone.Left;
                                stone.Top = oldStone.Top;
                                stone.Angle = oldStone.Angle;
                                stone.Scale = oldStone.Scale;
                                stone.SetStoneBackground(stoneBackgroundBitmap, stoneBackgroundWidth, stoneBackgroundHeight);
                                stone.FixupConfiguration();
                                stone.Update(true);

                                this.SetCurrentStone(stoneNumber);

                                if (visible)
                                    stone.Visible = true;
                                else
                                    stone.Visible = false;

                                oldStone.Visible = false;


                                if (!string.IsNullOrEmpty(this.currentCircle))
                                {
                                    //old stone was removed, but new stone is not saved yet
                                    FlushCurrentCircle();
                                }
                                oldStone.Dispose();
                                FileOperations.DeleteFile(Path.Combine(GlobalConfig.RollingStonesCache, oldStone.StoneID) + ".png");
                            }
                        }
                    }
                }

                sts.ClearIcons();
                sts.Dispose();

                BringToFront();
            }
            finally
            {
                SuppressHookMessage(false);
            }
        }


        /// <summary>
        /// Flushes the current circle to the circle ini file.
        /// When some changes was performed this method save changes to the hard disk
        /// via the hook message (see IHookMessage interface for details).
        /// </summary>
        public void FlushCurrentCircle()
        {
            if (hookMessage != null)
                hookMessage.SaveCurrentCircle();
        }

        /// <summary>
        /// Replaces the current circle with the new circle.
        /// This method is used for async circle loading
        /// </summary>
        /// <param name="newCircleName">New name of the circle.</param>
        public void ReplaceCurrentCircle(string newCircleName)
        {
            if (hookMessage != null)
                hookMessage.ChangeCurrentCircle(newCircleName);
        }


        public void ShowSkinsMenu()
        {
            PublishSkins(2, 2);
        }

        public void ShowPopupMenu()
        {
            if (popupMenu != null)
            {
                POINT pt = new POINT();
                pt.x = 2;
                pt.y = 2;
                NativeMethods.ClientToScreen(this.managerWindow.Handle, ref pt);
                popupMenu.PopupAt(pt.x, pt.y, false);
            }
        }


        public void SwitchRingInplace()
        {
            managerWindow.SwitchRing();
        }

        public void ShowStartMenuDialog(bool centered)
        {
            int idx = -1;

            this.SuppressHookMessage(true);
            try
            {
                if (startMenu.Executed)
                {
                    if ((DateTime.Now - startMenu.ScanTime).TotalMinutes > 10)
                        startMenu.RebuildCatalog();
                    if (!string.IsNullOrEmpty(startMenu.SearchText))
                        startMenu.Research();
                    startMenu.SearchText = string.Empty;
                }
                Point mousePoint = PrimaryScreen.CursorPosition;
                if (startMenu.Execute(centered ? -1 : mousePoint.X, centered ? -1 : mousePoint.Y))
                {

                    idx = startMenu.SelectedItem;
                    startMenu.HistoryAdd();
                    if (startMenu.Items.Count > idx)
                    {
                        var fileName = startMenu.Items[idx].FileName;
                        if (fileName != null)
                        {
                            if (visible)
                                Visible = false;
                            AsyncShellExecute ase = new AsyncShellExecute(fileName);
                            ase.Run();
                        }
                    }
                }
            }
            finally
            {
                this.SuppressHookMessage(false);
            }
        }

        public void ShowPowerControlDialog()
        {
            int idx = -1;

            this.SuppressHookMessage(true);
            try
            {
                PowerControlDialog ppd = new PowerControlDialog(this.Handle);
                ppd.FixedRowSize = 4;
                Point mousePoint = PrimaryScreen.CursorPosition;
                if (ppd.Execute(mousePoint.X, mousePoint.Y))
                {
                    idx = ppd.SelectedItem;
                }
                ppd.Dispose();
            }
            finally
            {
                this.SuppressHookMessage(false);
            }

            switch (idx)
            {
                case 0:
                    WindowsOperations.Shutdown(ShutdownOption.PowerOff);
                    NativeMethods.ExitKrento();
                    break;
                case 1:
                    WindowsOperations.Suspend();
                    break;
                case 2:
                    WindowsOperations.Hibernate();
                    break;
                case 3:
                    WindowsOperations.Shutdown(ShutdownOption.Reboot);
                    NativeMethods.ExitKrento();
                    break;
                default:
                    break;
            }
        }


        public bool ShowPopupDialog()
        {
            int idx = -1;
            bool result = true;

            this.SuppressHookMessage(true);
            try
            {
                PopupDialog ppd = new PopupDialog(this.Handle);
                Point mousePoint = PrimaryScreen.CursorPosition;
                if (ppd.Execute(mousePoint.X, mousePoint.Y))
                {
                    idx = ppd.SelectedItem;
                }
                ppd.Dispose();
            }
            finally
            {
                this.SuppressHookMessage(false);
            }

            switch (idx)
            {
                case 0:
                    if (hookMessage != null)
                        hookMessage.ShowKrentoSettings();
                    break;
                case 1:
                    if (hookMessage != null)
                        hookMessage.CreateCircle();
                    break;
                case 2:
                    ShowRunningApplications();
                    break;
                case 3:
                    ShowSkinsMenu();
                    result = false;
                    break;
                case 4:
                    if (hookMessage != null)
                        hookMessage.EditCircle();
                    break;
                case 5:
                    if (hookMessage != null)
                        hookMessage.DeleteCircle();
                    break;
                case 6:
                    AddNewStone();
                    break;
                case 7:
                    if (hookMessage != null)
                        hookMessage.ShowHelpPages();
                    break;
                case 8:
                    WindowsOperations.Shutdown(ShutdownOption.PowerOff);
                    NativeMethods.ExitKrento();
                    break;
                case 9:
                    WindowsOperations.Suspend();
                    break;
                case 10:
                    WindowsOperations.Hibernate();
                    break;
                case 11:
                    WindowsOperations.Shutdown(ShutdownOption.Reboot);
                    NativeMethods.ExitKrento();
                    break;
                default:
                    break;
            }
            return result;
        }

        public void SwitchRing()
        {
            this.SuppressHookMessage(true);
            try
            {
                if (taskManager.Switch(left, top))
                {

                    if (taskManager.SelectedIndex != HistoryCurrentIndex)
                    {
                        if (taskManager.SelectedIndex == HistoryCount)
                        {
                            KrentoMenuItem mi = PopupMenu.FindItemByTag(KrentoMenuTag.CreateCircle);
                            if (mi != null)
                                PopupMenu.Execute(mi);
                        }
                        else
                        {

                            string ringName = HistoryRingName(taskManager.SelectedIndex);
                            if (!string.IsNullOrEmpty(ringName))
                            {
                                ReplaceCurrentCircle(ringName);
                            }
                        }
                    }
                    else
                    {

                        if (IsVirtual)
                            BackToReality();

                    }
                }

            }
            finally
            {
                this.SuppressHookMessage(false);
            }
        }


        internal void PublishSkins(int menuLeft, int menuTop)
        {

            KrentoMenuItem menuItem;
            skinsMenu.Clear();


            string[] iniFiles = iniFiles = Directory.GetFiles(GlobalConfig.SkinsFolder, @"background*.ini", SearchOption.AllDirectories);

            skinsMenu.AddItem().Caption = SR.ManagerDefaultBackground;

            Array.Sort(iniFiles);

            for (int i = 0; i < iniFiles.Length; i++)
            {

                menuItem = skinsMenu.AddItem();
                menuItem.Caption = KrentoCoreSkin.GetSkinCaption(iniFiles[i]);
                menuItem.Data = iniFiles[i];
            }

            POINT pt = new POINT();
            pt.x = menuLeft;
            pt.y = menuTop;
            NativeMethods.ClientToScreen(this.managerWindow.Handle, ref pt);
            skinsMenu.PopupAt(pt.x, pt.y, false);

        }

        internal void StopRotation()
        {
            managerWindow.StopRotation();
        }

        internal void StopRotation(bool displayStoneHint)
        {
            managerWindow.StopRotation(displayStoneHint);
        }

        #region IAnimationListener Members

        public void OnAnimationStart(Animation animation)
        {

        }

        public void OnAnimationEnd(Animation animation)
        {
            VisualButton button = null;

            if (animation.Target != null)
            {
                if (animation.Target is VisualButton)
                {
                    button = (VisualButton)animation.Target;
                    switch (button.Tag)
                    {
                        case 1:
                            if (activeStone != null)
                            {
                                if (activeStone.CanRemove)
                                {
                                    RemoveStoneRequest(activeStone.Order, 1);
                                }
                            }
                            break;

                        case 2:
                            if (activeStone != null)
                            {
                                if (activeStone.CanConfigure)
                                {
                                    activeStone.Configure();
                                }
                            }
                            break;

                        case 3:
                            if (activeStone != null)
                            {
                                if (activeStone.CanChangeType)
                                {
                                    ChangeTypeRequest(activeStone.Order);
                                }
                            }
                            break;

                        case 4:
                            if (activeStone != null)
                            {
                                ShowAboutBox(activeStone);
                            }
                            break;

                        default:
                            break;
                    }

                }
            }
        }

        public void OnAnimationRepeat(Animation animation)
        {

        }

        #endregion
    }
}
