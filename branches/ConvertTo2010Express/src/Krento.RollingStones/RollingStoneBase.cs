//===============================================================================
// Copyright (c) Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Laugris.Sage;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing.Text;
using System.IO;
using System.Reflection;

namespace Krento.RollingStones
{
    /// <summary>
    /// Base class for all stones
    /// </summary>
    public abstract class RollingStoneBase : IDisposable
    {
        private StonesManager manager;
        private string fullName;
        private string assemblyName;
        private int order;
        private bool visible;
        private double angle;
        private double scale;
        private int tag;
        private bool acceptFiles;
        private string stoneID;
        private const int mouseTimer = 2;
        private int mouseTimerWait = 1000;
        //when user press mouse button and keep it pressed long time
        //the mouse click will be ignored
        private bool mouseIgnore;
        private bool clicked;
        private bool mouseInside;
        private bool selected;
        private double maxScale;
        private double maxSize;
        private int runCount;
        private int runLevel;
        private int rating;
        private Bitmap stoneIndicator;
        private bool isMouseDown;
        private string translationId;
        private IntPtr nativeGraphics;
        private object internalSyncObject = new object();

        private StoneWindow window;

        private bool canRemove = true;
        private bool canConfigure = true;
        private bool canChangeType = true;

        private IntPtr backgroundBitmap;
        private int backgroundBitmapWidth;
        private int backgroundBitmapHeight;

        private string translatedDescription;
        private string targetDescription;


        /// <summary>
        /// Initializes a new instance of the <see cref="RollingStoneBase"/> class.
        /// </summary>
        /// <param name="manager">The Krento Stones Manager.</param>
        protected RollingStoneBase(StonesManager manager)
        {
            try
            {
                if (manager == null)
                    throw new ArgumentNullException("manager");

                this.manager = manager;
                this.window = new StoneWindow(this);
                nativeGraphics = GetWindowNativeGraphics();
                window.BufferCreated += new EventHandler(window_BufferCreated);


                window.Width = manager.StoneSize;
                window.Height = manager.StoneSize;
                window.Text = SR.WindowText;
                //window.DrawDefaultBackground();
                window.Visible = false;
                window.TopMostWindow = true;
                window.CanDrag = true;
                window.MouseDown += new MouseEventHandler(MouseDownInternalHandler);
                window.MouseUp += new MouseEventHandler(MouseUpInternalHandler);
                window.MouseClick += new MouseEventHandler(MouseClickInternalHandler);
                window.MouseEnter += new EventHandler(MouseEnterInternalHandler);
                window.MouseLeave += new EventHandler(MouseLeaveInternalHandler);
                window.MoveDelta += new EventHandler<MoveDeltaEventArgs>(window_MoveDelta);
                window.FadeFinished += new EventHandler<FadeEventArgs>(FadeFinishedInternalHandler);
                window.Shown += new EventHandler(window_Shown);
                window.Hides += new EventHandler(window_Hides);
                window.AllowDrop = false;
                window.AcceptFiles = false;
                window.ReplaceAlpha((byte)manager.Transparency);
                this.scale = 1.0;
                maxScale = 1.2;
                maxSize = (manager.StoneSize / maxScale) - (manager.StoneBorder / 2);

            }
            catch (Exception ex)
            {
                throw new StoneConstructorException(SR.CreateStoneError, ex);
            }

        }

        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(fullName))
                    fullName = GetType().FullName;
                return fullName;
            }
        }

        public string AssemblyName
        {
            get
            {
                if (string.IsNullOrEmpty(assemblyName))
                {
                    Assembly assembly;
                    assembly = GetType().Assembly;
                    if (assembly != null)
                    {
                        string assemblyFileName = assembly.Location;
                        assemblyName = Path.GetFileName(assemblyFileName);
                    }
                }
                return assemblyName;
            }
        }

        public string Caption
        {
            get
            {
                if (window == null)
                    return null;
                else
                    return window.Text;
            }
            set
            {
                if (window != null)
                {
                    window.Text = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the target description.
        /// </summary>
        /// <value>The target description.</value>
        public string TargetDescription
        {
            get { return targetDescription; }
            set { targetDescription = value; }
        }

        public bool CanRemove
        {
            get { return canRemove; }
            set { canRemove = value; }
        }

        public bool CanConfigure
        {
            get { return canConfigure; }
            set { canConfigure = value; }
        }

        void window_MoveDelta(object sender, MoveDeltaEventArgs e)
        {
            manager.StopRotation(false);
            if (manager.StoneMenu != null)
                manager.StoneMenu.CloseUp();
            manager.HideScreenHint();

            ///

            int realDeltaX = manager.Position.X - Left;
            int realDeltaY = manager.Position.Y - Top;

            int maxDistance = (int)(Math.Sqrt(Math.Pow(manager.Radius * 1.22, 2) + Math.Pow(manager.Radius * 0.71, 2)));

            ///

            int offset = (int)(manager.StoneSize - maxSize * scale) + 4;

            if (realDeltaX < 0)
            {
                manager.DeleteWindow.Left = manager.Position.X + maxDistance;
            }
            else
            {
                manager.DeleteWindow.Left = manager.Position.X - maxDistance;
            }

            if (realDeltaY < 0)
            {
                manager.DeleteWindow.Top = manager.Position.Y + maxDistance;
            }
            else
            {
                manager.DeleteWindow.Top = manager.Position.Y - maxDistance;
            }

            Rectangle dangerousBounds = this.window.Bounds;
            int hOffset = offset;
            int vOffset = offset;

            if (Left < manager.DeleteWindow.Left)
                hOffset = -offset;

            if (Top < manager.DeleteWindow.Top)
                vOffset = -offset;

            dangerousBounds.Offset(hOffset, vOffset);
            dangerousBounds.Size = new Size((int)MaxSize, (int)MaxSize);

            if ((manager.DeleteWindow.RecycleArea.IntersectsWith(dangerousBounds)) && (manager.DeleteWindow.Visible))
                manager.DeleteWindow.DrawSeleted();
            else
                manager.DeleteWindow.DrawNormal();

            if (!manager.DeleteWindow.Visible)
            {
                manager.DeleteWindow.Show();
                manager.DeleteWindow.BringToFront();
            }

        }

        public bool CanDrag
        {
            get { return window.CanDrag; }
            set { window.CanDrag = value; }
        }

        public bool CanChangeType
        {
            get { return canChangeType; }
            set { canChangeType = value; }
        }

        protected virtual void WindowHides()
        {
        }

        void window_Hides(object sender, EventArgs e)
        {
            WindowHides();
        }

        protected virtual void WindowShown()
        {
        }

        protected void UpdateFrames()
        {
            if (window != null)
                window.Perform(NativeMethods.CM_UPDATE_FRAMES, IntPtr.Zero, IntPtr.Zero);
        }

        void window_Shown(object sender, EventArgs e)
        {
            WindowShown();
        }


        void window_BufferCreated(object sender, EventArgs e)
        {
            nativeGraphics = GetWindowNativeGraphics();
        }

        #region Events
        public event KeyEventHandler KeyDown;
        public event KeyEventHandler KeyUp;
        public event EventHandler ConfigureStone;
        public event EventHandler<TimerEventArgs> TimerTick;
        public event DragEventHandler DragDrop;
        public event DragEventHandler DragEnter;
        public event EventHandler DragLeave;
        public event DragEventHandler DragOver;
        #endregion

        internal bool HasConfigurationEvent
        {
            get
            {
                if (canConfigure)
                {
                    return (ConfigureStone != null);
                }
                else
                    return false;
            }
        }


        protected IntPtr NativeGraphics
        {
            get
            {
                return nativeGraphics;
            }
        }

        protected IntPtr GetWindowNativeGraphics()
        {
            return (IntPtr)typeof(System.Drawing.Graphics).InvokeMember("NativeGraphics",
                BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, this.window.Canvas, null);

        }

        protected int BeginContainer()
        {
            int state;
            NativeMethods.GdipBeginContainer2(NativeGraphics, out state);
            return state;
        }

        protected void EndContainer(int state)
        {
            NativeMethods.GdipEndContainer(NativeGraphics, state);
        }

        public bool AllowDrop
        {
            get { return window.AllowDrop; }
            set { window.AllowDrop = value; }
        }

        #region IDropTarget Members

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.DragDrop"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.DragEventArgs"/> that contains the event data.</param>
        protected internal virtual void OnDragDrop(DragEventArgs e)
        {
            DragEventHandler handler = DragDrop;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.DragEnter"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.DragEventArgs"/> that contains the event data.</param>
        protected internal virtual void OnDragEnter(DragEventArgs e)
        {
            DragEventHandler handler = DragEnter;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.DragLeave"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected internal virtual void OnDragLeave(EventArgs e)
        {
            EventHandler handler = DragLeave;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.DragOver"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.DragEventArgs"/> that contains the event data.</param>
        protected internal virtual void OnDragOver(DragEventArgs e)
        {
            DragEventHandler handler = DragOver;
            if (handler != null)
                handler(this, e);
        }

        #endregion

        protected virtual void OnTimerTick(TimerEventArgs e)
        {
            EventHandler<TimerEventArgs> handler = TimerTick;
            if (handler != null)
            {
                handler(this, e);
            }
        }


        /// <summary>
        /// Gets or sets the max scaling limit for the stone.
        /// </summary>
        /// <value>The max scaling limit</value>
        public double MaxScale
        {
            get { return maxScale; }
            set
            {
                maxScale = value;
                maxSize = (manager.StoneSize / maxScale) - (manager.StoneBorder / 2);
            }
        }


        /// <summary>
        /// Gets the stone icon.
        /// </summary>
        /// <value>The stone icon.</value>
        public Bitmap StoneIcon
        {
            get { return manager.StoneIcon(this); }
        }

        /// <summary>
        /// Gets the stone description.
        /// </summary>
        /// <value>The stone description.</value>
        public virtual string StoneDescription
        {
            get { return manager.StoneDescription(this); }
        }

        /// <summary>
        /// Gets the stone author.
        /// </summary>
        /// <value>The stone author.</value>
        public virtual string StoneAuthor
        {
            get { return manager.StoneAuthor(this); }
        }

        public virtual bool StoneBuiltIn
        {
            get { return manager.StoneBuiltIn(this); }
        }

        public virtual string StoneVersion
        {
            get { return manager.StoneVersion(this); }
        }

        public string StoneCopyright
        {
            get { return manager.StoneCopyright(this); }
        }

        private void FadeFinishedInternalHandler(object sender, FadeEventArgs e)
        {
            if (e.FadeUp)
                visible = true;
            else
                visible = false;
        }

        public void MoveStoneHint()
        {
            if ((!GlobalSettings.ShowManagerButtons) || (manager.ScreenHint == null))
                return;

            int x;
            int y;


            if (((angle >= 345) && (angle <= 360)) || ((angle >= 0) && (angle <= 15)))
            {
                x = (window.Width / 2) + Left - (manager.ScreenHint.Width / 2);
                y = (int)(this.Top + this.Size.Height) + (int)PaintOffset;
            }
            else
                if ((angle < 345) && (angle >= 315))
                {
                    x = Left - manager.ScreenHint.Width + (int)PaintOffset;
                    y = (int)(this.Top + this.Size.Height - manager.ScreenHint.Height) + (int)PaintOffset;
                }
                else
                    if ((angle < 315) && (angle >= 255))
                    {
                        x = Left - manager.ScreenHint.Width + (int)PaintOffset;
                        y = (int)(this.Top + (this.Size.Height / 2) - (manager.ScreenHint.Height / 2));
                    }
                    else
                        if ((angle < 255) && (angle >= 195))
                        {
                            x = Left - manager.ScreenHint.Width + (int)PaintOffset;
                            y = this.Top + (int)PaintOffset;
                        }
                        else
                            if ((angle < 195) && (angle >= 165))
                            {
                                x = (window.Width / 2) + Left - (manager.ScreenHint.Width / 2);
                                y = Top - manager.ScreenHint.Height + (int)PaintOffset;
                            }
                            else
                                if ((angle < 165) && (angle >= 105))
                                {
                                    x = Left + window.Width - (int)PaintOffset;
                                    y = this.Top + (int)PaintOffset;
                                }
                                else
                                    if ((angle < 105) && (angle >= 75))
                                    {
                                        x = Left + window.Width - (int)PaintOffset;
                                        y = (int)(this.Top + (this.Size.Height / 2) - (manager.ScreenHint.Height / 2));
                                    }
                                    else
                                        if ((angle < 75) && (angle > 15))
                                        {
                                            x = Left + window.Width - (int)PaintOffset;
                                            y = (int)(this.Top + this.Size.Height - manager.ScreenHint.Height) - (int)PaintOffset;
                                        }
                                        else
                                        {
                                            x = this.Left + (int)PaintOffset;
                                            y = (int)(this.Top + this.Size.Height) + (int)PaintOffset;
                                        }

            manager.ScreenHint.Activate(x, y);
        }

        /// <summary>
        /// Handles the mouse enter to the stone area.
        /// </summary>
        protected virtual void HandleMouseEnter()
        {
            mouseInside = true;
            manager.SetCurrentStone(this);
        }

        /// <summary>
        /// Handles the mouse leave from the stone area.
        /// </summary>
        protected virtual void HandleMouseLeave()
        {
            mouseInside = false;
            manager.MouseInsideStone = false;
            Update(true);
        }

        /// <summary>
        /// Selects the current stone. The stone becomes an active stone.
        /// </summary>
        /// <remarks>
        /// This method does not check if another stone is already active. For changing the active stone
        /// use <see cref="StonesManager.SetCurrentStone"/> method.
        /// </remarks>
        public virtual void SelectStone()
        {
            selected = true;
            Update(true);
            UpdateManagerSurface();
            MoveStoneHint();
        }

        /// <summary>
        /// Deselects the current stone.
        /// </summary>
        public virtual void DeselectStone()
        {
            selected = false;
            manager.StoneMenu.CloseUp();
            Update(true);
        }

        /// <summary>
        /// Updates the manager surface.
        /// </summary>
        public void UpdateManagerSurface()
        {
            if (selected)
                manager.RedrawScreenHint();
        }

        /// <summary>
        /// Gets the max size of the stone
        /// </summary>
        /// <value>The max size of the stone</value>
        public double MaxSize
        {
            get { return maxSize; }
        }

        /// <summary>
        /// Gets or sets the run count on the current level. If the value reach max int value,
        /// it must change to 0 again and increase run level +1
        /// </summary>
        /// <value>The run count.</value>
        public int RunCount
        {
            get { return runCount; }
            set { runCount = value; }
        }

        public string TranslationId
        {
            get { return translationId; }
            set
            {
                translationId = value;
                if (!string.IsNullOrEmpty(translationId))
                    translatedDescription = SR.Keys.GetString(translationId);

            }
        }

        public string TranslatedDescription
        {
            get { return translatedDescription; }
        }

        /// <summary>
        /// Gets or sets the run level. The levels introduced for running statistics.
        /// If run count is max int then stone goes to the next level.
        /// </summary>
        /// <value>The run level.</value>
        public int RunLevel
        {
            get { return runLevel; }
            set { runLevel = value; }
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        /// <param name="timerId">The timer id.</param>
        /// <param name="interval">The interval.</param>
        public void StartTimer(int timerId, int interval)
        {
            window.StartTimer(timerId, interval);
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        /// <param name="timerId">The timer id.</param>
        public void StopTimer(int timerId)
        {
            window.StopTimer(timerId);
        }

        /// <summary>
        /// Gets or sets the stone's rating (5 star). Rated stones have the priority 
        /// </summary>
        /// <value>The rating.</value>
        public int Rating
        {
            get { return rating; }
            set { rating = value; }
        }

        private void MouseLeaveInternalHandler(object sender, EventArgs e)
        {
            manager.DeleteWindow.Hide();
            HandleMouseLeave();
        }

        /// <summary>
        /// internal handler for mouse enter event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void MouseEnterInternalHandler(object sender, EventArgs e)
        {
            manager.MouseInsideStone = true;
            HandleMouseEnter();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RollingStoneBase"/> is selected.
        /// </summary>
        /// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
        public bool Selected
        {
            get { return selected; }
            set
            {
                if (value)
                    SelectStone();
                else
                    DeselectStone();
            }
        }

        /// <summary>
        /// Gets a value indicating whether [mouse inside].
        /// </summary>
        /// <value><c>true</c> if [mouse inside]; otherwise, <c>false</c>.</value>
        protected bool MouseInside
        {
            get { return mouseInside; }
        }

        private void MouseClickInternalHandler(object sender, MouseEventArgs e)
        {
            if (FadeActive)
                return;
            if (e.Button == MouseButtons.Right)
            {
                if (manager.StoneMenu != null)
                {
                    POINT pt = new POINT();
                    pt.x = e.X;
                    pt.y = e.Y;
                    NativeMethods.ClientToScreen(this.window.Handle, ref pt);
                    manager.StoneMenu.PopupAt(pt.x, pt.y);
                }
            }
            else
                clicked = true;
        }

        /// <summary>
        /// Handles the timer tick.
        /// </summary>
        /// <param name="timerId">The timer id.</param>
        protected internal virtual void HandleTimerTick(int timerId)
        {
            switch (timerId)
            {
                case mouseTimer:
                    {
                        window.StopTimer(mouseTimer);
                        if (!window.Dragged)
                        {
                            mouseIgnore = true;
                            if (manager.StoneMenu != null)
                            {
                                manager.StoneMenu.PopupAtCursor();
                            }
                        }
                        break;
                    }
                default:
                    TimerEventArgs eventArgs = new TimerEventArgs(timerId);
                    try
                    {
                        OnTimerTick(eventArgs);
                    }
                    finally
                    {
                        eventArgs.Dispose();
                    }
                    break;
            }
        }

        /// <summary>
        /// Delegate for mouse up event
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void MouseUpInternalHandler(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
            window.StopTimer(mouseTimer);
            bool ignore = mouseIgnore;

            if (FadeActive)
                ignore = true;

            mouseIgnore = false;
            Update(true);

            if (window.Dragged)
            {

                manager.DeleteWindow.Hide();
                int offset = (int)(manager.StoneSize - maxSize * scale) + 4;
                Rectangle dangerousBounds = this.window.Bounds;
                int hOffset = offset;
                int vOffset = offset;

                if (Left < manager.DeleteWindow.Left)
                    hOffset = -offset;

                if (Top < manager.DeleteWindow.Top)
                    vOffset = -offset;

                dangerousBounds.Offset(hOffset, vOffset);
                dangerousBounds.Size = new Size((int)MaxSize, (int)MaxSize);

                if ((manager.DeleteWindow.RecycleArea.IntersectsWith(dangerousBounds)) && (canRemove))
                {
                    manager.RemoveStoneRequest(this.Order, 1);
                }
                else
                {
                    RollingStoneBase nearest = manager.FindNearestStone(this, this.Left, this.Top);
                    if (nearest != null)
                    {
                        manager.Stones.Remove(this);
                        int newIndex = nearest.Order;
                        manager.Stones.Insert(newIndex, this);
                        manager.UpdateAngles();
                        manager.RotateRequest(0);
                        manager.FlushCurrentCircle();
                    }
                    else
                    {
                        manager.UpdateAngles();
                        manager.RotateRequest(0);
                        manager.FlushCurrentCircle();
                    }
                }
            }

            if ((!ignore) && (clicked) && (e.Button == MouseButtons.Left))
            {
                clicked = false;
                if (KeyboardInfo.ModifierKeys == Keys.None)
                {
                    Run();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the mouse button is pressed and holded down.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the mouse button is down; otherwise, <c>false</c>.
        /// </value>
        public bool IsMouseDown
        {
            get { return isMouseDown; }
        }


        private void MouseDownInternalHandler(object sender, MouseEventArgs e)
        {

            manager.StopRotation();

            clicked = false;
            window.StopTimer(mouseTimer);

            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = true;
                Update(true);

                if (manager.StoneMenu != null)
                {
                    if (manager.StoneMenu.Active)
                    {
                        mouseIgnore = true;
                        manager.StoneMenu.CloseUp();
                        return;
                    }
                }

                if (KeyboardInfo.ModifierKeys == Keys.None)
                {
                    window.StartTimer(mouseTimer, mouseTimerWait);
                }
            }
        }


        /// <summary>
        /// Raises the <see cref="E:KeyUp"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        protected internal virtual void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                if (!FadeActive)
                    Run();
            }
            else
            {
                if (KeyUp != null)
                    KeyUp(this, e);
            }
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="RollingStoneBase"/> is reclaimed by garbage collection.
        /// </summary>
        ~RollingStoneBase()
        {
            Dispose(false);
        }

        public void SetStoneBackground(IntPtr newBackground, int width, int height)
        {
            backgroundBitmap = newBackground;
            backgroundBitmapWidth = width;
            backgroundBitmapHeight = height;
        }


        public Bitmap StoneIndicator
        {
            get { return stoneIndicator; }
            set { stoneIndicator = value; }
        }

        internal float PaintOffset
        {
            get { return (float)((manager.StoneSize - maxSize * scale) / 2); }
        }

        protected virtual void DoPaintBackground()
        {
            Rectangle workingArea;
            float offset = 0;
            int imageSize = 0;

            if (GlobalConfig.LowMemory)
            {
                workingArea = new Rectangle(0, 0, (int)maxSize, (int)maxSize);
            }
            else
            {
                imageSize = (int)(maxSize * scale);
                offset = (float)((manager.StoneSize - maxSize * scale) / 2);
                workingArea = new Rectangle((int)offset, (int)offset, imageSize, imageSize);
            }

            if ((backgroundBitmap != IntPtr.Zero))
            {
                if (isMouseDown && (!MouseOverSettingsButton))
                    BitmapPainter.DrawImageScaled(backgroundBitmap, window.Buffer.Handle, (int)offset, (int)offset,
                        backgroundBitmapWidth, backgroundBitmapHeight, imageSize, imageSize, 180);
                else
                {
                    BitmapPainter.DrawImageScaled(backgroundBitmap, window.Buffer.Handle, (int)offset, (int)offset,
                        backgroundBitmapWidth, backgroundBitmapHeight, imageSize, imageSize);
                }
            }
        }

        protected virtual void DoPaint(Graphics canvas)
        {
            if (scale <= 0)
                return;

            window.Clear();

            float offset = (float)((manager.StoneSize - maxSize * scale) / 2);

            if (GlobalConfig.LowMemory)
            {
                Rectangle workingArea = new Rectangle(0, 0, (int)maxSize, (int)maxSize);
                DoPaintBackground();
                Paint(canvas, workingArea, PaintMode.Stone);

            }
            else
            {
                DoPaintBackground();
                int container = this.BeginContainer();
                try
                {
                    canvas.TranslateTransform(offset, offset);
                    canvas.ScaleTransform((float)scale, (float)scale);
                    Rectangle workingArea = new Rectangle(0, 0, (int)maxSize, (int)maxSize);
                    Paint(canvas, workingArea, PaintMode.Stone);

                }
                finally
                {
                    this.EndContainer(container);
                }
            }
        }

        internal bool MouseOverSettingsButton { get; set; }

        public virtual void DrawTargetDescription()
        {
            if (string.IsNullOrEmpty(targetDescription))
                Manager.DrawText(translatedDescription);
            else
                Manager.DrawText(targetDescription);
            MoveStoneHint();
        }

        public virtual void Paint(Graphics canvas, Rectangle workingArea, PaintMode paintMode)
        {
            if (canvas == null)
                return;

            int indLeft;
            int indTop;

            if (paintMode == PaintMode.Stone)
            {
                //testing
                //!!!
                //window.DrawDefaultBackground();
                //!!!

                if ((backgroundBitmap != IntPtr.Zero))
                {
                    if ((this.selected) && (stoneIndicator != null))
                    {
                        if (manager.IndicatorLeft > 0)
                            indLeft = manager.IndicatorLeft;
                        else
                            indLeft = (workingArea.Width - stoneIndicator.Width) / 2;

                        indTop = (workingArea.Height - stoneIndicator.Height) / 2;
                        if (manager.IndicatorBottom > 0)
                            indTop -= manager.IndicatorBottom;


                        canvas.DrawImage(stoneIndicator, new Rectangle(indLeft, indTop, stoneIndicator.Width, stoneIndicator.Height));
                    }

                }
            }
        }

        /// <summary>
        /// Gets the manager.
        /// </summary>
        /// <value>The manager.</value>
        internal StonesManager Manager
        {
            get { return this.manager; }
        }

        /// <summary>
        /// This method is called after loading the stone by stones manager.
        /// At this moment all properties are assigned from .circle file and
        /// possible to adjust it before showing the stone
        /// </summary>
        public virtual void FixupConfiguration()
        {
        }

        /// <summary>
        /// Prepares to save.
        /// </summary>
        public virtual void PrepareToSave()
        {
        }

        internal void Fade(int fadeDelay)
        {
            if (manager.StoneMenu != null)
                manager.StoneMenu.CloseUp();

            window.StartFade(fadeDelay);
        }

        /// <summary>
        /// Gets a value indicating whether fade is active. This occurs when
        /// manager shows or hides stones
        /// </summary>
        /// <value><c>true</c> if fade is active; otherwise, <c>false</c>.</value>
        public bool FadeActive
        {
            get { return window.FadeActive; }
        }

        public double Angle
        {
            get { return angle; }
            set { angle = value; }
        }


        /// <summary>
        /// Gets or sets the left coordinate.
        /// </summary>
        /// <value>The left.</value>
        public int Left
        {
            get { return window.Left; }
            set { window.Left = value; }
        }

        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <value>The location.</value>
        public Point Location
        {
            get { return window.Position; }
        }

        public bool AcceptFiles
        {
            get { return acceptFiles; }
            set
            {
                acceptFiles = value;
                window.AcceptFiles = value;
            }
        }

        public int Top
        {
            get { return window.Top; }
            set { window.Top = value; }
        }

        public int Order
        {
            get { return order; }
            set { order = value; }
        }


        protected virtual void OnConfigure(EventArgs e)
        {
            if (canConfigure)
            {
                manager.SuppressHookMessage(true);
                try
                {
                    if (ConfigureStone != null)
                        ConfigureStone(this, e);
                }
                finally
                {
                    manager.SuppressHookMessage(false);
                }
            }
        }

        public virtual void Configure()
        {
            OnConfigure(EventArgs.Empty);
        }

        public IntPtr Handle
        {
            get { return window.Handle; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RollingStoneBase"/> is visible.
        /// </summary>
        /// <value><c>true</c> if visible; otherwise, <c>false</c>.</value>
        public bool Visible
        {
            get { return this.visible; }
            set
            {
                if (visible != value)
                {
                    this.visible = value;
                    if (window.Alpha == 0)
                        window.RestoreAlpha();
                    window.Visible = this.visible;

                }
            }
        }

        public void ReplaceAlpha(byte newAlpha)
        {
            window.ReplaceAlpha(newAlpha);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (window != null)
                {
                    window.StopTimer(mouseTimer);
                    window.MouseDown -= MouseDownInternalHandler;
                    window.MouseUp -= MouseUpInternalHandler;
                    window.MouseClick -= MouseClickInternalHandler;
                    window.MouseEnter -= MouseEnterInternalHandler;
                    window.MouseLeave -= MouseLeaveInternalHandler;
                    window.FadeFinished -= FadeFinishedInternalHandler;

                    window.Dispose();
                    window = null;
                }

                if (backgroundBitmap != IntPtr.Zero)
                {
                    backgroundBitmap = IntPtr.Zero;
                }


            }

            if (mouseInside)
            {
                mouseInside = false;
                manager.MouseInsideStone = false;
            }
        }

        /// <summary>
        /// Gets or sets the stone ID - GUID
        /// </summary>
        /// <value>The stone ID GUID.</value>
        public string StoneID
        {
            get { return stoneID; }
            set { stoneID = value; }
        }

        /// <summary>
        /// Gets or sets the size of the stone window.
        /// </summary>
        /// <value>The size of the stone window.</value>
        public Size Size
        {
            get { return new Size(window.Width, window.Height); }
            set
            {
                window.Size(value.Width, value.Height);
                maxSize = (manager.StoneSize / maxScale) - (manager.StoneBorder / 2);
            }
        }

        public double Scale
        {
            get { return scale; }
            set
            {
                double roundValue = Math.Round(value, 2);
                scale = roundValue;
            }
        }

        /// <summary>
        /// Update stone's surface and position
        /// </summary>
        public void Update()
        {
            Update(false);
        }

        /// <summary>
        /// Update the stone surface and position only in case if the stone size was changed
        /// </summary>
        public void UpdateRescale()
        {
            Update(true);
        }

        /// <summary>
        /// Updates stone's surface keeping the position
        /// </summary>
        /// <param name="redrawOnly">if set to <c>true</c> [redraw only].</param>
        public void Update(bool redrawOnly)
        {
            if (window != null)
            {
                window.Perform(redrawOnly ? NativeMethods.CM_UPDATE_REDRAW : NativeMethods.CM_UPDATE_ALL, IntPtr.Zero, IntPtr.Zero);
            }
        }

        internal void UpdateInternal(bool redrawOnly)
        {
            lock (this.internalSyncObject)
            {
                if (window != null)
                {
                    DoPaint(window.Canvas);
                    window.Update(redrawOnly);
                }
            }
        }


        public int Tag
        {
            get { return tag; }
            set { tag = value; }
        }


        internal void ShowAfter(RollingStoneBase stone)
        {
            if (stone == null)
                return;

            this.window.MoveAndPlaceAfter(stone.window);
        }

        internal void DeferAfter(RollingStoneBase stone)
        {
            if (stone == null)
                return;

            this.window.DeferWindowPos(stone.window);
        }

        internal void MoveOnTop()
        {
            window.MoveAndPlaceOnTop();
        }

        internal void NewPosition(int left, int top)
        {
            window.Position = new Point(left, top);
        }


        protected internal virtual void OnKeyDown(KeyEventArgs e)
        {
            if (KeyDown != null)
                KeyDown(this, e);
        }


        /// <summary>
        /// Runs this instance.
        /// </summary>
        public virtual void Run()
        {
            if (runCount < int.MaxValue)
                runCount++;
            else
            {
                runCount = 0;
                if (runLevel < int.MaxValue)
                    runLevel++;
                else
                {
                    runLevel = 0;
                    //Total runs = int.MaxValue * runLevel + runCount 
                    //The number is big enough, normally this situation never happens
                    manager.ClearStatistics();
                }
            }

            if (!IsVirtual)
            {
                NativeMethods.WriteInterger(manager.CurrentCircle, this.stoneID, "RunCount", runCount);
                NativeMethods.WriteInterger(manager.CurrentCircle, this.stoneID, "RunLevel", runLevel);
            }

            NativeThemeManager.MakeSound("#107");
            if (GlobalConfig.LowMemory)
            {
                manager.HideAll();
                manager.ChangeVisibility(false);
            }
            else
            {
                manager.HideRingHint();
                manager.HideButtonHint();
                manager.HideScreenHint();

                manager.HideAllMenus();

                manager.Fade();
            }
        }

        /// <summary>
        /// Execute stone only after the manager is totally hidden
        /// </summary>
        /// <param name="execCode">The exec code.</param>
        public virtual void Run(FadeHookDelegate execCode)
        {
            if (runCount < int.MaxValue)
                runCount++;
            else
            {
                runCount = 0;
                if (runLevel < int.MaxValue)
                    runLevel++;
                else
                {
                    runLevel = 0;
                    //Total runs = runCount * runLevel 
                    //The number is big enough, normally this situation never happens
                    manager.ClearStatistics();
                }
            }

            NativeThemeManager.MakeSound("#107");
            if (GlobalConfig.LowMemory)
            {
                manager.HideAll();
                manager.ChangeVisibility(false);
                if (execCode != null)
                    execCode();
            }
            else
            {
                manager.HideRingHint();
                manager.HideButtonHint();
                manager.HideScreenHint();

                manager.HideAllMenus();

                manager.HideAndExecute(execCode);
            }
        }


        public bool IsVirtual { get; set; }

        public virtual void SaveConfiguration(MemIniFile ini)
        {
            if (!IsVirtual)
            {
                try
                {
                    ini.WriteInteger(this.stoneID, "RunCount", RunCount);
                    ini.WriteInteger(this.stoneID, "Rating", Rating);
                    ini.WriteInteger(this.stoneID, "RunLevel", RunLevel);
                    if (!string.IsNullOrEmpty(translationId))
                        ini.WriteString(this.stoneID, "TranslationId", translationId);
                }
                catch (Exception ex)
                {
                    throw new StoneSettingsException(SR.WriteSettingsError, ex);
                }
            }
        }

        public virtual void ReadConfiguration(MemIniFile ini)
        {
            if (!IsVirtual)
            {
                try
                {
                    string stringValue;

                    RunCount = ini.ReadInteger(this.stoneID, "RunCount", 0);
                    Rating = ini.ReadInteger(this.stoneID, "Rating", 0);
                    RunLevel = ini.ReadInteger(this.stoneID, "RunLevel", 0);
                    stringValue = ini.ReadString(this.stoneID, "TranslationId", null);
                    if (!string.IsNullOrEmpty(stringValue))
                        TranslationId = stringValue;
                    TargetDescription = ini.ReadString(this.StoneID, "Description", null);

                }
                catch (Exception ex)
                {
                    throw new StoneSettingsException("Read stone settings error", ex);
                }
            }
        }

        /// <summary>
        /// Moves the stone's window.
        /// </summary>
        /// <param name="deltaX">The delta X.</param>
        /// <param name="deltaY">The delta Y.</param>
        internal void MoveDelta(int deltaX, int deltaY)
        {
            this.window.UpdatePosition(this.window.Left + deltaX, this.window.Top + deltaY);
        }

        internal void MoveDelta(IntPtr hWinPosInfo, int deltaX, int deltaY)
        {
            this.NewPosition(this.window.Left + deltaX, this.window.Top + deltaY);
            NativeMethods.DeferWindowPos(hWinPosInfo, this.window.Handle, IntPtr.Zero, window.Left, window.Top, 0, 0,
                (int)(SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_NOREDRAW | SetWindowPosFlags.SWP_NOZORDER));
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        internal void StopFade()
        {
            this.window.StopFade();
        }
    }
}
