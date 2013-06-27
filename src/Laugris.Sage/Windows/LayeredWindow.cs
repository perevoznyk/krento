// Copyright (c) Serhiy Perevoznyk.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using ComIDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;
using System.Collections;
using System.Collections.Generic;

namespace Laugris.Sage
{

    /// <summary>
    /// Basic class for all predefined layered windows
    /// </summary>
    public class LayeredWindow : LightWindow, IDropTarget, ISupportOleDropSource
    {

        #region Private Fields
        private bool clicked;
        private Size size;
        private bool mouseInside;
        private TRACKMOUSEEVENT trackMouseEvent;

        private bool dragged;
        private bool acceptFiles;
        private bool allowDrop;
        private bool dropTarget;
        private BufferedCanvas buffer;
        private int wheelAccumulator;

        #endregion

        internal static IntPtr MouseWindow = IntPtr.Zero;

        private Point position;
        private Graphics canvas;
        private byte alpha;
        private int colorKey;
        private Color foreColor;
        private Font font;
        private bool canDrag;
        private int dragTreshold;
        private bool visible;

        private InvokeProcessor processor;
        private Dictionary<int, MethodInvoker> timerHandlers = new Dictionary<int, MethodInvoker>();
        private Dictionary<int, MethodInvoker> timeouts = new Dictionary<int, MethodInvoker>();
        private const int baseTimer = 100;

        private int currentTimer = baseTimer;


        private Color colorKeyTranslated;
        private const int fadeTimer = 1;
        private bool fadeUp = true;
        private double fadeAmount;
        private double fadeIncreaseBy;
        private byte initialAlpha;
        private byte storedAlpha = 230;
        private bool fadeActive;

        private Point ClickPoint;
        private DropTarget OLEDropTarget;
        private IDropTargetHelper dropHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayeredWindow"/> class.
        /// </summary>
        public LayeredWindow()
        {
            position.X = 0;
            position.Y = 0;
            size.Height = 10;
            size.Width = 10;
            colorKeyTranslated = Color.FromArgb(0);
            colorKey = ColorTranslator.ToWin32(colorKeyTranslated);
            alpha = 230;
            foreColor = Color.White;
            dragTreshold = 1;
            canDrag = true;

            buffer = new BufferedCanvas(10, 10);
            canvas = Graphics.FromHdc(buffer.Handle);
            CreateHandle();

            //processor needs handle!
            processor = new InvokeProcessor(this.Handle);

        }

        /// <summary>
        /// Gets a value indicating whether [fade active].
        /// </summary>
        /// <value><c>true</c> if [fade active]; otherwise, <c>false</c>.</value>
        public bool FadeActive
        {
            get { return fadeActive; }
        }

        protected void StartFadeTimer(int interval)
        {
            try
            {
                if (this.Handle != IntPtr.Zero)
                    NativeMethods.SetTimer(Handle, (IntPtr)fadeTimer, interval, IntPtr.Zero);
            }
            finally
            {
                fadeActive = true;
            }
        }

        protected void StopFadeTimer()
        {
            try
            {
                if (this.Handle != IntPtr.Zero)
                    NativeMethods.KillTimer(Handle, (IntPtr)fadeTimer);
            }
            finally
            {
                fadeActive = false;
            }
        }

        public void StartTimer(int timerId, int interval)
        {
            try
            {
                if (timerId > 1)
                {
                    if (this.Handle != IntPtr.Zero)
                        NativeMethods.SetTimer(Handle, (IntPtr)timerId, interval, IntPtr.Zero);
                }
            }
            catch (Exception ex)
            {
                TraceDebug.Trace("LayeredWindow.StartTimer:", ex);
            }
        }

        public void StopTimer(int timerId)
        {
            try
            {
                if (timerId > 1)
                {
                    if (this.Handle != IntPtr.Zero)
                        NativeMethods.KillTimer(Handle, (IntPtr)timerId);
                }
            }
            catch (Exception ex)
            {
                TraceDebug.Trace("LayeredWindow.StopTimer:",  ex);
            }
        }

        public int SetInterval(MethodInvoker target, int frequency)
        {
            currentTimer++;
            int idx = currentTimer;
            timerHandlers.Add(idx, target);
            StartTimer(idx, frequency);
            return idx;
        }

        public void ResetInterval(MethodInvoker target, int interval, int frequency)
        {
            StopTimer(interval);
            timerHandlers.Remove(interval);
            timerHandlers.Add(interval, target);
            StartTimer(interval, frequency);
        }

        public void StopInterval(int interval)
        {
            StopTimer(interval);
        }

        public void RestartInterval(int interval, int frequency)
        {
            StartTimer(interval, frequency);
        }

        #region BeginInvoke
        public void BeginInvoke(WaitCallback method)
        {
            processor.BeginInvoke(method);
        }

        public void BeginInvoke(WaitCallback method, object parameter)
        {
            processor.BeginInvoke(method, parameter);
        }

        public void BeginInvoke(WaitCallback method, object parameter, string errorMessage)
        {
            processor.BeginInvoke(method, parameter, errorMessage);
        }

        public void BeginInvoke(WaitCallback method, string errorMessage)
        {
            processor.BeginInvoke(method, errorMessage);
        }
        #endregion

        #region Invoke
        public void Invoke(WaitCallback method)
        {
            processor.Invoke(method);
        }

        public void Invoke(WaitCallback method, object parameter)
        {
            processor.Invoke(method, parameter);
        }

        public void Invoke(WaitCallback method, object parameter, string errorMessage)
        {
            processor.Invoke(method, parameter, errorMessage);
        }

        public void Invoke(WaitCallback method, string errorMessage)
        {
            processor.Invoke(method, errorMessage);
        }
        #endregion

        public void ClearInterval(int interval)
        {

            if (timerHandlers.ContainsKey(interval))
            {
                MethodInvoker target = timerHandlers[interval];
                if (target != null)
                {
                    StopTimer(interval);
                    timerHandlers.Remove(interval);
                }
            }
        }

        public int SetTimeout(MethodInvoker target, int delay)
        {
            currentTimer++;
            int idx = currentTimer;
            timeouts.Add(idx, target);
            StartTimer(idx, delay);
            return idx;
        }

        public void ClearTimeoutTable()
        {
            ICollection keys = timeouts.Keys;
            foreach (int key in keys)
            {
                StopTimer(key);
            }
            timeouts.Clear();
        }

        public void ClearIntervalsTable()
        {
            ICollection keys = timerHandlers.Keys;
            foreach (int key in keys)
            {
                StopTimer(key);
            }
            timerHandlers.Clear();
        }

        public void StartFade()
        {
            StartFade(2000);
        }

        public bool MouseInside
        {
            get { return mouseInside; }
        }

        public void StartFade(int length)
        {
            if (fadeActive)
                return;

            clicked = false;
            fadeActive = true;

            double fadeSpeed;
            initialAlpha = 0;


            if (Visible)
            {
                storedAlpha = this.Alpha;
                fadeAmount = this.Alpha;
                fadeUp = false;
            }
            else
            {
                initialAlpha = storedAlpha;
                fadeAmount = 0;
                Alpha = 0;
                Update(true);
                Show(false);
                fadeUp = true;
            }

            if (initialAlpha == 0)
                initialAlpha = storedAlpha;

            fadeSpeed = fadeUp ? initialAlpha : this.Alpha;
            int precision = 5;
            int TimesToShow = length / precision;

            fadeIncreaseBy = fadeSpeed / TimesToShow;
            StartFadeTimer(precision);
        }


        public string Name { get; set; }

        public void StopFade()
        {
            StopFadeTimer();
            OnFadeFinished(new FadeEventArgs(fadeUp));
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                //To avoid call to GC delegate
                UnregisterWindowProc();
                ClearIntervalsTable();
                ClearTimeoutTable();
                if (disposing)
                {
                    StopFadeTimer();

                    // Code to cleanup managed resources held by the class.
                    if (canvas != null)
                    {
                        canvas.Dispose();
                        canvas = null;
                    }

                    if (buffer != null)
                    {
                        buffer.Dispose();
                        buffer = null;
                    }

                    if (processor != null)
                    {
                        processor.Dispose();
                        processor = null;
                    }

                    trackMouseEvent = null;
                }

                // Code to cleanup unmanaged resources held by the class.

            }
            finally
            {
                base.Dispose(disposing);
            }
        }


        public DragDropEffects DoDragDrop(Bitmap dragImage, DragDropEffects allowedEffects)
        {
            ComDataObject data = new ComDataObject();
            ShDragImage shdi = new ShDragImage();
            SIZE size = new SIZE();
            size.cx = dragImage.Width;
            size.cy = dragImage.Height;
            shdi.sizeDragImage = size;
            shdi.hbmpDragImage = BitmapPainter.GetCompatibleBitmap(dragImage);
            shdi.crColorKey = ColorTranslator.ToWin32(Color.FromArgb(0xFF, 0, 0x55));
            POINT pt = new POINT();
            pt.x = (dragImage.Width / 2);
            pt.y = (dragImage.Height / 2);

            shdi.ptOffset = pt;

            IDragSourceHelper sourceHelper = (IDragSourceHelper)new DragDropHelper();
            sourceHelper.InitializeFromBitmap(ref shdi, data);
            return DoDragDrop(data, allowedEffects);

        }

        public DragDropEffects DoDragDrop(object data, DragDropEffects allowedEffects)
        {
            int[] finalEffect = new int[1];
            IOleDropSource dropSource = new DropSource(this);
            System.Runtime.InteropServices.ComTypes.IDataObject dataObject = null;
            if (data is System.Runtime.InteropServices.ComTypes.IDataObject)
            {
                dataObject = (System.Runtime.InteropServices.ComTypes.IDataObject)data;
            }
            else
            {
                DataObject obj3 = null;
                if (data is System.Windows.Forms.IDataObject)
                {
                    obj3 = new DataObject((System.Windows.Forms.IDataObject)data);
                }
                else
                {
                    obj3 = new DataObject();
                    obj3.SetData(data);
                }
                dataObject = obj3;
            }
            try
            {
                NativeMethods.DoDragDrop(dataObject, dropSource, (int)allowedEffects, finalEffect);
            }
            catch (Exception exception)
            {
                if (IsSecurityOrCriticalException(exception))
                {
                    throw;
                }
            }
            return (DragDropEffects)finalEffect[0];
        }


        [DefaultValue(true)]
        public bool CanDrag
        {
            get { return canDrag; }
            set { canDrag = value; }
        }

        [DefaultValue(1)]
        public int DragThreshold
        {
            get { return dragTreshold; }
            set { dragTreshold = value; }
        }


        public bool AcceptFiles
        {
            get { return acceptFiles; }
            set
            {
                acceptFiles = value;
                NativeMethods.DragAcceptFiles(this.Handle, value);
            }
        }

        /// <summary>
        /// Gets or sets the position of the window without actually moving the window
        /// </summary>
        /// <value>The position.</value>
        public Point Position
        {
            get { return new Point(position.X, position.Y); }
            set
            {
                position.X = value.X;
                position.Y = value.Y;
            }
        }

        public Point ScreenPointToClient(Point p)
        {
            POINT pt;
            pt.x = p.X;
            pt.y = p.Y;
            NativeMethods.ScreenToClient(this.Handle, ref pt);
            Point result = new Point(pt.x, pt.y);
            return result;
        }

        public Point ClientToScreenPoint(Point p)
        {
            POINT pt = new POINT();
            pt.x = p.X;
            pt.y = p.Y;
            NativeMethods.ClientToScreen(this.Handle, ref pt);
            Point result = new Point(pt.x, pt.y);
            return result;
        }

        public Graphics Canvas
        {
            get { return canvas; }
        }

        public BufferedCanvas Buffer
        {
            get { return buffer; }
        }

        public byte Alpha
        {
            get { return alpha; }
            set
            {
                alpha = value;
            }
        }

        public void RestoreAlpha()
        {
            alpha = storedAlpha;
            Update(true);
        }

        public void ReplaceAlpha(byte newAlpha)
        {
            storedAlpha = newAlpha;
            alpha = storedAlpha;
        }

        public int ColorKey
        {
            get { return colorKey; }
            set
            {
                colorKey = value;
                colorKeyTranslated = Color.FromArgb(colorKey);
            }
        }

        public void UpdatePosition(int left, int top)
        {
            position.X = left;
            position.Y = top;
            UpdatePosition();
        }


        public void UpdatePosition()
        {
            NativeMethods.UpdateWindowPosition(this.Handle, position.X, position.Y);
        }


        public void Update()
        {
            SendMessageToSelf(NativeMethods.CN_UPDATE, IntPtr.Zero, IntPtr.Zero);
        }

        public void Update(bool redrawOnly)
        {
            SendMessageToSelf(NativeMethods.CN_UPDATE, IntPtr.Zero, redrawOnly ? (IntPtr)1 : IntPtr.Zero);
        }

        protected void UpdateInternal(bool redrawOnly)
        {

            if (this.Handle == IntPtr.Zero)
                return;

            NativeMethods.DrawLayeredWindow(Handle, position.X, position.Y, size.Width, size.Height, buffer.Handle, colorKey, alpha, redrawOnly);
            return;
        }

        /// <summary>
        /// Shows this window.
        /// </summary>
        public virtual void Show()
        {
            Show(true);
        }

        /// <summary>
        /// Shows this window and optionally makes it active
        /// </summary>
        /// <param name="activate">if set to <c>true</c> then activate the window.</param>
        public void Show(bool activate)
        {
            visible = true;
            short showStyle = activate ? (short)ShowWindowStyles.SW_SHOW : (short)ShowWindowStyles.SW_SHOWNOACTIVATE;
            if (this.Handle != IntPtr.Zero)
                NativeMethods.ShowWindow(this.Handle, showStyle);
            OnShown(EventArgs.Empty);
        }

        public void ShowDialog()
        {
            if (this.Handle == IntPtr.Zero)
                return;

            if (NativeMethods.GetCapture() != IntPtr.Zero)
                NativeMethods.SendMessage(NativeMethods.GetCapture(), NativeMethods.WM_CANCELMODE, IntPtr.Zero, IntPtr.Zero);
            NativeMethods.ReleaseCapture();

            ThreadWindows threadWindows = DisableTaskWindows();
            try
            {
                Show(true);
                BringToFront();
                LocalModalMessageLoop(this);
            }
            finally
            {
                EnableTaskWindows(threadWindows);
            }
        }

        /// <summary>
        /// Hides this window.
        /// </summary>
        public virtual void Hide()
        {
            visible = false;
            if (this.Handle != IntPtr.Zero)
                NativeMethods.ShowWindow(this.Handle, (short)ShowWindowStyles.SW_HIDE);
            dragged = false;
            this.clicked = false;
            NativeMethods.ReleaseCapture();
            OnHides(EventArgs.Empty);
        }

        public virtual bool Visible
        {
            get { return visible; }
            set
            {
                if (visible != value)
                {
                    visible = value;
                    if (visible)
                        Show();
                    else
                        Hide();
                }
            }
        }

        public bool Focused
        {
            get { return NativeMethods.GetFocus() == Handle; }
        }

        public void Clear()
        {
            canvas.Clear(colorKeyTranslated);
        }

        public void Clear(Color color)
        {
            canvas.Clear(color);
        }


        public Rectangle ClientRect
        {
            get { return new Rectangle(0, 0, size.Width, size.Height); }
        }

        public Rectangle Bounds
        {
            get { return new Rectangle(Left, Top, Width, Height); }
        }

        /// <summary>
        /// Move the window to the new position and place it at the top of the specified window
        /// </summary>
        /// <param name="window">The window.</param>
        public void MoveAndPlaceAfter(LightWindow window)
        {
            if (window == null)
                return;

            if (this.Handle == IntPtr.Zero)
                return;

            if (window.Handle == IntPtr.Zero)
                return;

            NativeMethods.SetWindowPos(Handle, window.Handle, position.X, position.Y, 0, 0, (int)(SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOACTIVATE));
        }

        /// <summary>
        /// Move the window to the new position and place it on the top of all other windows
        /// </summary>
        public void MoveAndPlaceOnTop()
        {
            if (this.Handle == IntPtr.Zero)
                return;

            NativeMethods.SetWindowPos(Handle, NativeMethods.HWND_TOPMOST, position.X, position.Y, 0, 0, (int)(SetWindowPosFlags.SWP_NOSIZE | SetWindowPosFlags.SWP_NOACTIVATE));
        }

        /// <summary>
        /// Recreates the painting buffer. This is needed when the size of the window is changed
        /// </summary>
        protected void RecreateBuffer()
        {
            SilentRecreateBuffer();
            OnSizeChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Recreates the painting buffer. This is needed when the size of the window is changed.
        /// This method is not triggering SizeChanged event
        /// </summary>
        protected virtual void SilentRecreateBuffer()
        {
            this.buffer.Dispose();
            this.buffer = null;
            canvas.Dispose();
            canvas = null;

            buffer = new BufferedCanvas(size.Width, size.Height);
            canvas = Graphics.FromHdc(buffer.Handle);
            OnBufferCreated(EventArgs.Empty);
        }

        protected virtual void OnBufferCreated(EventArgs e)
        {
            if (BufferCreated != null)
                BufferCreated(this, e);
        }

        public void Size(int width, int height)
        {
            if ((size.Width == width) && (size.Height == height))
                return;

            size.Width = width;
            size.Height = height;

            RecreateBuffer();

        }


        protected void SilentResize(int width, int height)
        {
            if ((size.Width == width) && (size.Height == height))
                return;

            size.Width = width;
            size.Height = height;

            SilentRecreateBuffer();

        }

        /// <summary>
        /// Gets or sets the width of the window represented by the <see cref="LayeredWindow"/> object, measured in points. 
        /// </summary>
        /// <value>The width of the window represented by the <see cref="LayeredWindow"/> object, measured in points.</value>
        public int Width
        {
            get { return this.size.Width; }
            set
            {
                if (size.Width != value)
                {
                    this.size.Width = value;
                    RecreateBuffer();
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the window represented by the <see cref="LayeredWindow"/> object, measured in points. 
        /// </summary>
        /// <value>The height of the window represented by the <see cref="LayeredWindow"/> object, measured in points.</value>
        public int Height
        {
            get { return this.size.Height; }
            set
            {
                if (size.Height != value)
                {
                    this.size.Height = value;
                    RecreateBuffer();
                }
            }
        }

        /// <summary>
        /// Sizes the size of the window that is represented by the <see cref="LayeredWindow"/> object.
        /// </summary>
        /// <param name="size">The size of the window that is represented by the <see cref="LayeredWindow"/> object.</param>
        public void Size(Size size)
        {
            Size(size.Width, size.Height);
        }

        /// <summary>
        /// Activates (gives focus to) the window that is represented by the <see cref="LayeredWindow"/> object. 
        /// </summary>
        public virtual void Activate()
        {
            NativeMethods.SetActiveWindow(this.Handle);
        }

        /// <summary>
        /// Moves window to front. This method works only for visible windows.
        /// </summary>
        public virtual void BringToFront()
        {
            if (!this.Visible)
                return;

            NativeMethods.BringWindowToFront(this.Handle);
        }


        #region Events
        /// <summary>
        /// Occurs when [drag drop].
        /// </summary>
        public event DragEventHandler DragDrop;
        /// <summary>
        /// Occurs when [drag enter].
        /// </summary>
        public event DragEventHandler DragEnter;
        /// <summary>
        /// Occurs when [drag leave].
        /// </summary>
        public event EventHandler DragLeave;
        /// <summary>
        /// Occurs when [drag over].
        /// </summary>
        public event DragEventHandler DragOver;

        /// <summary>
        /// Occurs when the mouse pointer is moved over the window
        /// </summary>
        public event MouseEventHandler MouseMove;
        /// <summary>
        /// Occurs when the mouse pointer is over the window and a mouse button is pressed. 
        /// </summary>
        public event MouseEventHandler MouseDown;
        /// <summary>
        /// Occurs when the mouse pointer is over the window and a mouse button is released.
        /// </summary>
        public event MouseEventHandler MouseUp;
        /// <summary>
        /// Occurs when the mouse wheel moves while the window has focus. 
        /// </summary>
        public event MouseEventHandler MouseWheel;
        /// <summary>
        /// Occurs when the window is clicked. 
        /// </summary>
        public event MouseEventHandler MouseClick;
        /// <summary>
        /// Occurs when the window is double clicked by the mouse.
        /// </summary>
        public event MouseEventHandler MouseDoubleClick;
        /// <summary>
        /// Occurs when the window is moved.
        /// </summary>
        public event EventHandler Move;
        /// <summary>
        /// Occurs when the window is closing
        /// </summary>
        public event EventHandler Closing;
        /// <summary>
        /// Occurs when window begins to close. 
        /// </summary>
        public event EventHandler Quit;
        /// <summary>
        /// Occurs when a key is pressed while the window has focus.
        /// </summary>
        public event KeyEventHandler KeyDown;
        /// <summary>
        /// Occurs when a key is released while the window has focus.
        /// </summary>
        public event KeyEventHandler KeyUp;
        /// <summary>
        /// Occurs when a key is pressed while the window has focus.
        /// </summary>
        public event KeyPressEventHandler KeyPress;
        /// <summary>
        /// Occurs when the Size property value changes
        /// </summary>
        public event EventHandler SizeChanged;
        /// <summary>
        /// Occurs when the mouse pointer enters the window. 
        /// </summary>
        public event EventHandler MouseEnter;
        /// <summary>
        /// Occurs when the mouse pointer leaves the window. 
        /// </summary>
        public event EventHandler MouseLeave;
        /// <summary>
        /// Occurs when the window loses focus. 
        /// </summary>
        public event EventHandler LostFocus;

        /// <summary>
        /// Occurs when [paint].
        /// </summary>
        public event PaintEventHandler Painting;

        /// <summary>
        /// Occurs when [fore color changed].
        /// </summary>
        public event EventHandler ForeColorChanged;

        /// <summary>
        /// Occurs when the Font property value changes. 
        /// </summary>
        public event EventHandler FontChanged;

        public event EventHandler Shown;

        public event EventHandler Hides;

        public event EventHandler<FileDropEventArgs> FileDropped;

        public event EventHandler<TimerEventArgs> TimerTick;

        public event EventHandler<MoveDeltaEventArgs> MoveDelta;

        public event EventHandler SessionEnding;

        public event EventHandler BufferCreated;

        public event GiveFeedbackEventHandler GiveFeedback;

        public event QueryContinueDragEventHandler QueryContinueDrag;

        /// <summary>
        /// Occurs when fade is finished.
        /// </summary>
        public event EventHandler<FadeEventArgs> FadeFinished;
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
        /// Raises the <see cref="E:KeyDown"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        protected virtual void OnKeyDown(KeyEventArgs e)
        {
            KeyEventHandler handler = KeyDown;
            if (handler != null)
                handler(this, e);
        }

        protected virtual void OnLostFocus(EventArgs e)
        {
            EventHandler handler = LostFocus;
            if (handler != null)
                handler(this, e);
        }

        protected virtual void OnSessionEnding(EventArgs e)
        {
            EventHandler handler = SessionEnding;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:KeyUp"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.</param>
        protected virtual void OnKeyUp(KeyEventArgs e)
        {
            KeyEventHandler handler = KeyUp;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:KeyPress"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.KeyPressEventArgs"/> instance containing the event data.</param>
        protected virtual void OnKeyPress(KeyPressEventArgs e)
        {
            KeyPressEventHandler handler = KeyPress;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:Hides"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnHides(EventArgs e)
        {
            EventHandler handler = Hides;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:Move"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnMove(EventArgs e)
        {
            EventHandler handler = Move;
            if (handler != null)
                handler(this, e);
        }

        protected virtual void OnMoveDelta(MoveDeltaEventArgs e)
        {
            EventHandler<MoveDeltaEventArgs> handler = MoveDelta;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:Close"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnClose(EventArgs e)
        {
            processor.Clear();
            // To avoid multi-threading problems, get a reference to the handler in a local 
            // variable before checking it and calling it.
            EventHandler handler = Closing;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:Quit"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnQuit(EventArgs e)
        {
            EventHandler handler = Quit;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:MouseMove"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMouseMove(MouseEventArgs e)
        {
            MouseEventHandler handler = MouseMove;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:MouseDown"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMouseDown(MouseEventArgs e)
        {
            MouseEventHandler handler = MouseDown;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:MouseUp"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMouseUp(MouseEventArgs e)
        {
            MouseEventHandler handler = MouseUp;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:MouseWheel"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMouseWheel(MouseEventArgs e)
        {
            MouseEventHandler handler = MouseWheel;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:MouseClick"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMouseClick(MouseEventArgs e)
        {
            MouseEventHandler handler = MouseClick;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:MouseDoubleClick"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        protected virtual void OnMouseDoubleClick(MouseEventArgs e)
        {
            MouseEventHandler handler = MouseDoubleClick;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:FontChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnFontChanged(EventArgs e)
        {
            EventHandler handler = FontChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:ForeColorChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnForeColorChanged(EventArgs e)
        {
            EventHandler handler = ForeColorChanged;
            if (handler != null)
                handler(this, e);
        }

        protected virtual void OnShown(EventArgs e)
        {
            EventHandler handler = Shown;
            if (handler != null)
                handler(this, e);
        }


        /// <summary>
        /// Gets or sets a value indicating whether use Painting event to draw the window content
        /// </summary>
        /// <value><c>true</c> if Painting event is used; otherwise, <c>false</c>.</value>
        public bool CustomPaint { get; set; }

        /// <summary>
        /// Raises the <see cref="E:Paint"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPainting(PaintEventArgs e)
        {
            PaintEventHandler handler = Painting;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:MouseLeave"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnMouseLeave(EventArgs e)
        {
            EventHandler handler = MouseLeave;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:MouseEnter"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnMouseEnter(EventArgs e)
        {
            EventHandler handler = MouseEnter;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:SizeChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnSizeChanged(EventArgs e)
        {
            EventHandler handler = SizeChanged;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Raises the <see cref="E:FadeFinsied"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnFadeFinished(FadeEventArgs e)
        {
            EventHandler<FadeEventArgs> handler = FadeFinished;
            if (handler != null)
                handler(this, e);
        }

        protected virtual bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return false;
        }


        /// <summary>
        /// Internal implementation of Invalidate method. Calls Clear + Draw
        /// </summary>
        protected virtual void InvalidateInternal(object param)
        {
            Clear();
            Draw();
        }

        /// <summary>
        /// The full sequence is Clear + Draw + Update performed internally without using Windows messages
        /// </summary>
        protected virtual void RepaintInternal(object param)
        {
            InvalidateInternal(null);
            UpdateInternal(true);
        }

        /// <summary>
        /// Invalidates the entire surface of the window and 
        /// causes the window to be redrawn. This method incapsulates Clear + Draw
        /// </summary>
        public void Invalidate()
        {
            Invoke(InvalidateInternal);
        }

        /// <summary>
        /// Invalidate and Update the window; It calls first Invalidate then Update
        /// The full sequence is Clear + Draw + Update
        /// </summary>
        public void Repaint()
        {
            Invoke(RepaintInternal);
        }


        /// <summary>
        /// Draws the window surface withou clearing it. If you want to clear the surface before, use the Repaint method
        /// </summary>
        protected virtual void Draw()
        {
            if (CustomPaint)
            {
                using (PaintEventArgs e = new PaintEventArgs(canvas, new Rectangle(0, 0, size.Width, size.Height)))
                {
                    OnPainting(e);
                }
            }
        }


        public bool CanFocus
        {
            get
            {
                if (this.Handle.ToInt32() > 0)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Closes this window. When a window is closed, all resources created within 
        /// the object are closed and the window is disposed.
        /// </summary>
        public void Close()
        {
            if (IsHandleCreated)
            {
                this.PostMessageToSelf(0x0010, IntPtr.Zero, IntPtr.Zero);
            }
            else
            {
                this.Dispose();
            }
        }


        /// <summary>
        /// Gets or sets the foreground color of the window
        /// </summary>
        /// <value>The foreground color of the window.</value>
        public virtual Color ForeColor
        {
            get { return foreColor; }
            set
            {
                if (foreColor != value)
                {
                    foreColor = value;
                    OnForeColorChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Sets input focus to the window. 
        /// </summary>
        public void Focus()
        {
            NativeMethods.SetFocus(this.Handle);
        }

        /// <summary>
        /// Gets or sets the font of the text displayed by the window
        /// </summary>
        /// <value>The Font to apply to the text displayed by the window.</value>
        public virtual Font Font
        {
            get
            {
                if (font != null)
                    return font;
                else
                    return TextPainter.DefaultFont;
            }
            set
            {
                if (font != null && font.Equals(value))
                {
                    return;
                }

                font = value;
                Invalidate();
                OnFontChanged(EventArgs.Empty);
            }
        }


        public bool Dragged
        {
            get { return dragged; }
        }


        internal static bool TrackMouseEvent(TRACKMOUSEEVENT tme)
        {
            return NativeMethods.TrackMouseEvent(tme);
        }


        private void HookMouseEvent()
        {
            if (this.trackMouseEvent == null)
            {
                this.trackMouseEvent = new TRACKMOUSEEVENT();
                this.trackMouseEvent.dwFlags = NativeMethods.TME_LEAVE;
                this.trackMouseEvent.hwndTrack = this.Handle;
                this.trackMouseEvent.dwHoverTime = NativeMethods.HOVER_DEFAULT;
            }
            TrackMouseEvent(this.trackMouseEvent);
        }


        protected virtual void HandleTimerTick(int timerNumber)
        {
            MethodInvoker target;

            if (timerNumber > baseTimer)
            {
                if (timerHandlers.ContainsKey(timerNumber))
                {
                    target = timerHandlers[timerNumber];
                    if (target != null)
                    {
                        target();
                        return;
                    }
                }

                if (timeouts.ContainsKey(timerNumber))
                {
                    target = timeouts[timerNumber];
                    if (target != null)
                    {
                        StopTimer(timerNumber);
                        target();
                        timeouts.Remove(timerNumber);
                        return;
                    }
                }
            }

            using (TimerEventArgs eventArgs = new TimerEventArgs(timerNumber))
            {
                OnTimerTick(eventArgs);
            }
        }

        protected override void DestroyWindow()
        {
            OnHandleDestroyed(EventArgs.Empty);
            base.DestroyWindow();
            visible = false;
        }

        public void Perform(int msg, IntPtr wparam, IntPtr lparam)
        {
            PostMessageToSelf(msg, wparam, lparam);
        }

        #region Windows messages handlers

        private void WmDropFiles(ref Message m)
        {
            const int cnMaxFileNameLen = 255;
            int nCount;
            int len;
            StringBuilder acFileName = new StringBuilder(cnMaxFileNameLen);

            // find out how many files we're accepting
            nCount = NativeMethods.DragQueryFile(m.WParam,
                                     0xFFFFFFFF,
                                     acFileName,
                                     cnMaxFileNameLen);

            // query Windows one at a time for the file name
            for (int i = 0; i < nCount; i++)
            {
                len = NativeMethods.DragQueryFile(m.WParam, (uint)i,
                               acFileName, cnMaxFileNameLen);
                OnFileDrop(new FileDropEventArgs(acFileName.ToString(0, len)));

            }

            // let Windows know that you're done
            NativeMethods.DragFinish(m.WParam);

        }

        private void WMCreate(ref Message m)
        {
            /*LayeredWindow.*/DefWndProc(ref m);
            this.OnHandleCreated(EventArgs.Empty);
        }

        private void WmSetCursor(ref Message m)
        {
            if ((m.WParam == this.Handle) && (ParamConvertor.LoWord(m.LParam) == 1))
            {
                SetCursorInternal();
            }
        }

        private void WMTimer(ref Message msg)
        {
            switch ((int)msg.WParam)
            {
                case fadeTimer:
                    {
                        if (fadeUp)
                        {
                            if (Alpha >= initialAlpha)
                            {
                                StopFade();
                                return;
                            }
                            fadeAmount += fadeIncreaseBy;
                            Alpha = (byte)fadeAmount;
                            Update(true);
                        }
                        else
                        {
                            if (Alpha < 1)
                            {
                                Visible = false;
                                StopFade();
                                return;
                            }
                            fadeAmount -= fadeIncreaseBy;
                            Alpha = (byte)fadeAmount;
                            Update();
                        }
                        break;
                    }
                default:
                    {
                        HandleTimerTick((int)msg.WParam);

                        break;
                    }
            }
        }

        private void WmMouseEnter(ref Message m)
        {
            if (mouseInside)
                return;
            mouseInside = true;
            OnMouseEnter(EventArgs.Empty);
        }

        private void WmKillFocus(ref Message m)
        {
            this.OnLostFocus(EventArgs.Empty);
        }

        private void WmQueryEndSession(ref Message m)
        {
            this.OnSessionEnding(EventArgs.Empty);
        }

        private void WmMouseMove(ref Message m)
        {
            if (MouseWindow != this.Handle)
            {
                MouseWindow = this.Handle;
                HookMouseEvent();
                NativeMethods.SendMessage(new HandleRef(this, this.Handle), NativeMethods.WM_MOUSEENTER, IntPtr.Zero, IntPtr.Zero);
            }
            POINT Location = new POINT();

            int deltaX;
            int deltaY;
            Location.x = ParamConvertor.SignedLOWORD(m.LParam);
            Location.y = ParamConvertor.SignedHIWORD(m.LParam);
            POINT ScreenLocation = Location;

            if (this.clicked)
            {
                NativeMethods.ClientToScreen(this.Handle, ref ScreenLocation);
                deltaX = (ScreenLocation.x - ClickPoint.X);
                deltaY = (ScreenLocation.y - ClickPoint.Y);

                if ((System.Math.Abs(deltaX) > dragTreshold) || (System.Math.Abs(deltaY) > dragTreshold))
                {
                    ClickPoint.X = ScreenLocation.x;
                    ClickPoint.Y = ScreenLocation.y;
                    OnMoveDelta(new MoveDeltaEventArgs(deltaX, deltaY));
                    if (canDrag)
                    {
                        UpdatePosition(this.Position.X + deltaX, this.position.Y + deltaY);
                        NativeMethods.SetCapture(Handle);
                        dragged = true;
                    }
                }
            }
            else
            {
                dragged = false;
            }
            this.OnMouseMove(new MouseEventArgs(MouseInfo.MouseButtons, 0, Location.x, Location.y, 0));
        }

        private void WmMouseDown(ref Message m, MouseButtons button, int click)
        {
            POINT Location;
            Location.x = ParamConvertor.SignedLOWORD(m.LParam);
            Location.y = ParamConvertor.SignedHIWORD(m.LParam);
            this.OnMouseDown(new MouseEventArgs(button, click, Location.x, Location.y, 0));

            NativeMethods.ClientToScreen(this.Handle, ref Location);
            ClickPoint.X = Location.x;
            ClickPoint.Y = Location.y;
            NativeMethods.SetCapture(this.Handle);
            clicked = true;
        }

        private void WmMouseUp(ref Message m, MouseButtons button, int click)
        {
            POINT Location;
            Location.x = ParamConvertor.SignedLOWORD(m.LParam);
            Location.y = ParamConvertor.SignedHIWORD(m.LParam);
            if ((this.clicked) && (!Dragged))
            {
                OnMouseClick(new MouseEventArgs(button, click, Location.x, Location.y, 0));
            }

            this.OnMouseUp(new MouseEventArgs(button, click, Location.x, Location.y, 0));
            dragged = false;
            NativeMethods.ReleaseCapture();
            this.clicked = false;
        }

        private void WmMouseDoubleClick(ref Message m, MouseButtons button, int click)
        {
            POINT Location;
            Location.x = ParamConvertor.SignedLOWORD(m.LParam);
            Location.y = ParamConvertor.SignedHIWORD(m.LParam);
            this.OnMouseDoubleClick(new MouseEventArgs(button, click, Location.x, Location.y, 0));
        }

        /// <summary>
        /// WM_MOUSEWHEEL message handler
        /// </summary>
        /// <param name="m">The Windows message</param>
        private void WmMouseWheel(ref Message m)
        {
            POINT Location;
            int delta;
            bool isNegative;

            Location.x = ParamConvertor.SignedLOWORD(m.LParam);
            Location.y = ParamConvertor.SignedHIWORD(m.LParam);
            // This is due to inconsistent behavior of the WParam value on 64bit arch. IE wparam=0xffffffffff880000 or wparam=0x00000000ff100000
            delta = (int)((long)m.WParam << 32 >> 48);

            wheelAccumulator += delta;

            while (Math.Abs(wheelAccumulator) >= NativeMethods.WHEEL_DELTA)
            {
                isNegative = wheelAccumulator < 0;
                wheelAccumulator = Math.Abs(wheelAccumulator) - NativeMethods.WHEEL_DELTA;
                if (isNegative)
                {
                    if (wheelAccumulator != 0)
                        wheelAccumulator = -wheelAccumulator;
                }

                this.OnMouseWheel(new MouseEventArgs(MouseButtons.None, 0, Location.x, Location.y, delta));
            }


        }

        private void WmGetDlgCode(ref Message m)
        {
            m.Result = (IntPtr)DialogCodes.DLGC_WANTALLKEYS;
        }

        #endregion



        protected override void WndProc(ref Message m)
        {
            try
            {
                switch (m.Msg)
                {

                    case NativeMethods.CN_UPDATE:
                        {
                            UpdateInternal(m.LParam == (IntPtr)1);
                            return;
                        }
                    default:
                        break;
                }

                WindowMessage msg = (WindowMessage)m.Msg;

                switch (msg)
                {
                    case WindowMessage.WM_CREATE:
                        WMCreate(ref m);
                        return;

                    case WindowMessage.WM_TIMER:
                        WMTimer(ref m);
                        break;

                    case WindowMessage.WM_SETCURSOR:
                        WmSetCursor(ref m);
                        break;

                    case WindowMessage.WM_KILLFOCUS:
                        WmKillFocus(ref m);
                        break;

                    case WindowMessage.WM_WINDOWPOSCHANGED:
                        {
                            if (BottomMostWindow)
                            {
                                NativeMethods.SetWindowPos(Handle, NativeMethods.HWND_BOTTOM, 0, 0, 0, 0, 0x413);
                                m.Result = IntPtr.Zero;
                            }
                            break;
                        }


                    case WindowMessage.WM_DROPFILES:
                        {
                            WmDropFiles(ref m);
                            break;
                        }
                    case WindowMessage.WM_MOUSEMOVE:
                    case WindowMessage.WM_NCMOUSEMOVE:
                        {
                            WmMouseMove(ref m);
                            break;
                        }
                    case WindowMessage.WM_LBUTTONDOWN:
                    case WindowMessage.WM_NCLBUTTONDOWN:
                        {
                            //if ((NativeMethods.GetMessageExtraInfo().ToInt32() & 0xFF515700) == 0xFF515700)
                            //{
                            //    // Click was generated by wisptis.
                            //    break;
                            //}
                            //else
                            {
                                // Click was generated by the mouse.
                                WmMouseDown(ref m, MouseButtons.Left, 1);
                                break;
                            }

                        }
                    case WindowMessage.WM_RBUTTONDOWN:
                    case WindowMessage.WM_NCRBUTTONDOWN:
                        {
                            WmMouseDown(ref m, MouseButtons.Right, 1);
                            break;
                        }
                    case WindowMessage.WM_MBUTTONDOWN:
                    case WindowMessage.WM_NCMBUTTONDOWN:
                        {
                            WmMouseDown(ref m, MouseButtons.Middle, 1);
                            break;
                        }
                    case WindowMessage.WM_LBUTTONUP:
                    case WindowMessage.WM_NCLBUTTONUP:
                        {
                            //if ((NativeMethods.GetMessageExtraInfo().ToInt32() & 0xFF515700) == 0xFF515700)
                            //{
                            //    // Click was generated by wisptis.
                            //    break;
                            //}
                            //else
                            WmMouseUp(ref m, MouseButtons.Left, 1);
                            break;
                        }
                    case WindowMessage.WM_RBUTTONUP:
                    case WindowMessage.WM_NCRBUTTONUP:
                        {
                            WmMouseUp(ref m, MouseButtons.Right, 1);
                            break;
                        }
                    case WindowMessage.WM_MBUTTONUP:
                    case WindowMessage.WM_NCMBUTTONUP:
                        {
                            WmMouseUp(ref m, MouseButtons.Middle, 1);
                            break;
                        }
                    case WindowMessage.WM_LBUTTONDBLCLK:
                    case WindowMessage.WM_NCLBUTTONDBLCLK:
                        {
                            WmMouseDoubleClick(ref m, MouseButtons.Left, 2);
                            break;
                        }
                    case WindowMessage.WM_RBUTTONDBLCLK:
                    case WindowMessage.WM_NCRBUTTONDBLCLK:
                        {
                            WmMouseDoubleClick(ref m, MouseButtons.Right, 2);
                            break;
                        }
                    case WindowMessage.WM_MOUSEWHEEL:
                        {
                            WmMouseWheel(ref m);
                            break;
                        }
                    case WindowMessage.WM_MBUTTONDBLCLK:
                    case WindowMessage.WM_NCMBUTTONDBLCLK:
                        {
                            WmMouseDoubleClick(ref m, MouseButtons.Middle, 2);
                            break;
                        }
                    case WindowMessage.WM_MOUSELEAVE:
                        {
                            WmMouseLeave(ref m);
                            break;
                        }
                    case WindowMessage.WM_SIZE:
                        {
                            WmSize(ref m);
                            break;
                        }
                    case WindowMessage.WM_MOVE:
                        {
                            WmMove(ref m);
                            break;
                        }
                    case WindowMessage.WM_QUIT:
                        {
                            OnQuit(EventArgs.Empty);
                            break;
                        }

                    case WindowMessage.WM_QUERYENDSESSION:
                    case WindowMessage.WM_ENDSESSION:
                        {
                            WmQueryEndSession(ref m);
                            break;
                        }


                    case WindowMessage.WM_CLOSE:
                        {
                            OnClose(EventArgs.Empty);
                            DestroyWindow();
                            return;
                        }
                    case WindowMessage.WM_GETDLGCODE:
                        {
                            WmGetDlgCode(ref m);
                            return;
                        }
                    case WindowMessage.WM_CHAR:
                    case WindowMessage.WM_SYSCHAR:
                        {
                            KeyPressEventArgs e = new KeyPressEventArgs((char)((ushort)((long)m.WParam)));
                            OnKeyPress(e);
                            break;
                        }
                    case (WindowMessage)NativeMethods.CN_BEGININVOKE:
                    case (WindowMessage)NativeMethods.CN_INVOKE:
                        processor.ExecuteRequest();
                        return;
                    case WindowMessage.WM_KEYDOWN:
                    case WindowMessage.WM_SYSKEYDOWN:
                        {
                            Keys keyData = ((Keys)((int)((long)m.WParam))) | KeyboardInfo.ModifierKeys;
                            if (this.ProcessCmdKey(ref m, keyData))
                            {
                                return;
                            }
                            KeyEventArgs e = new KeyEventArgs(keyData);
                            OnKeyDown(e);
                            break;
                        }
                    case WindowMessage.WM_KEYUP:
                        {
                            KeyEventArgs e = new KeyEventArgs(((Keys)((int)((long)m.WParam))) | KeyboardInfo.ModifierKeys);
                            OnKeyUp(e);
                            break;
                        }

                }

                if (m.Msg == NativeMethods.WM_MOUSEENTER)
                {
                    WmMouseEnter(ref m);
                    return;
                }

                base.WndProc(ref m);
            }
            catch (Exception ex)
            {
                TraceDebug.Trace("LayeredWindow.WndProc:",  ex);
                dragged = false;
                this.clicked = false;
                NativeMethods.ReleaseCapture();
            }
        }


        private void WmMove(ref Message m)
        {
            int xPos = ParamConvertor.SignedLOWORD(m.LParam);
            int yPos = ParamConvertor.SignedHIWORD(m.LParam);
            position.X = xPos;
            position.Y = yPos;
            m.Result = IntPtr.Zero;
            OnMove(EventArgs.Empty);
        }

        private void WmSize(ref Message m)
        {
            OnSizeChanged(EventArgs.Empty);
        }

        private void WmMouseLeave(ref Message m)
        {
            mouseInside = false;
            MouseWindow = IntPtr.Zero;
            dragged = false;
            this.clicked = false;
            NativeMethods.ReleaseCapture();
            OnMouseLeave(EventArgs.Empty);
        }


        internal static bool IsCriticalException(Exception ex)
        {
            return (((((ex is NullReferenceException) || (ex is StackOverflowException)) || ((ex is OutOfMemoryException) || (ex is ThreadAbortException))) || ((ex is ExecutionEngineException) || (ex is IndexOutOfRangeException))) || (ex is AccessViolationException));
        }

        internal static bool IsSecurityOrCriticalException(Exception ex)
        {
            return ((ex is SecurityException) || IsCriticalException(ex));
        }

        private void OnHandleDestroyed(EventArgs eventArgs)
        {
            try
            {
                this.SetAcceptDrops(false);
            }
            catch (Exception exception)
            {
                if (IsSecurityOrCriticalException(exception))
                {
                    throw;
                }
            }

        }

        protected virtual void OnHandleCreated(EventArgs e)
        {
            if (this.IsHandleCreated)
            {
                this.SetAcceptDrops(this.AllowDrop);
            }
        }

        private void OnFileDrop(FileDropEventArgs e)
        {
            EventHandler<FileDropEventArgs> handler = FileDropped;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>
        /// Gets or sets the horizontal position of the window represented by the Window object, measured in points. 
        /// </summary>
        /// <value>The horizontal position of the window represented by the Window object, measured in points.</value>
        public int Left
        {
            get { return GetWindowLeft(); }
            set { SetWindowLeft(value); }
        }

        private void SetWindowLeft(int value)
        {
            if (this.position.X != value)
            {
                this.position.X = value;
                UpdatePosition();
            }
        }

        private int GetWindowLeft()
        {
            return this.position.X;
        }

        /// <summary>
        /// Gets or sets the vertical position of the window represented by the Window object, measured in points. 
        /// </summary>
        /// <value>The vertical position of the window represented by the Window object, measured in points.</value>
        public int Top
        {
            get { return GetWindowTop(); }
            set { SetWindowTop(value); }
        }

        private void SetWindowTop(int value)
        {
            if (this.position.Y != value)
            {
                this.position.Y = value;
                UpdatePosition();
            }
        }

        private int GetWindowTop()
        {
            return this.position.Y;
        }

        #region IDropTarget Members

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.DragDrop"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.DragEventArgs"/> that contains the event data.</param>
        public virtual void OnDragDrop(DragEventArgs e)
        {
            DragEventHandler handler = DragDrop;
            if (handler != null)
                handler(this, e);
            POINT wp = new POINT();
            NativeMethods.GetCursorPos(ref wp);

            if (dropHelper != null)
                dropHelper.Drop((ComIDataObject)e.Data, ref wp, (int)e.Effect);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.DragEnter"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.DragEventArgs"/> that contains the event data.</param>
        public virtual void OnDragEnter(DragEventArgs e)
        {
            DragEventHandler handler = DragEnter;
            if (handler != null)
                handler(this, e);

            POINT wp = new POINT();
            NativeMethods.GetCursorPos(ref wp);

            dropHelper = (IDropTargetHelper)new DragDropHelper();
            dropHelper.DragEnter(this.Handle, (ComIDataObject)e.Data, ref wp, (int)e.Effect);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.DragLeave"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        public virtual void OnDragLeave(EventArgs e)
        {
            EventHandler handler = DragLeave;
            if (handler != null)
                handler(this, e);
            if (dropHelper != null)
                dropHelper.DragLeave();
            dropHelper = null;

        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.DragOver"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.DragEventArgs"/> that contains the event data.</param>
        public virtual void OnDragOver(DragEventArgs e)
        {
            DragEventHandler handler = DragOver;
            if (handler != null)
                handler(this, e);

            POINT wp = new POINT();
            NativeMethods.GetCursorPos(ref wp);

            if (dropHelper != null)
                dropHelper.DragOver(ref wp, (int)e.Effect);
        }

        #endregion

        public virtual bool AllowDrop
        {
            get
            {
                return this.allowDrop;
            }
            set
            {
                if (this.allowDrop != value)
                {
                    if (value && !this.IsHandleCreated)
                    {
                        CodeAccessPermission ClipboardRead;
                        ClipboardRead = new UIPermission(UIPermissionClipboard.AllClipboard);
                        ClipboardRead.Demand();
                    }
                    this.allowDrop = value;
                    if (this.IsHandleCreated)
                    {
                        try
                        {
                            this.SetAcceptDrops(value);
                        }
                        catch
                        {
                            this.allowDrop = !value;
                            throw;
                        }
                    }
                }
            }
        }

        internal void SetAcceptDrops(bool value)
        {
            if ((value != this.dropTarget) && this.IsHandleCreated)
            {
                try
                {
                    if (Application.OleRequired() != ApartmentState.STA)
                    {
                        throw new ThreadStateException(Language.GetString("MustBeSTA", "Must be STA thread"));
                    }
                    if (value)
                    {
                        CodeAccessPermission ClipboardRead;
                        ClipboardRead = new UIPermission(UIPermissionClipboard.AllClipboard);
                        ClipboardRead.Demand();
                        OLEDropTarget = new DropTarget(this);
                        int error = NativeMethods.RegisterDragDrop(Handle, OLEDropTarget);
                        if ((error != 0) && (error != -2147221247))
                        {
                            OLEDropTarget.ClearOwner();
                            OLEDropTarget = null;
                            throw new Win32Exception(error);
                        }
                    }
                    else
                    {
                        int num2 = NativeMethods.RevokeDragDrop(Handle);
                        OLEDropTarget.ClearOwner();
                        OLEDropTarget = null;
                        if ((num2 != 0) && (num2 != -2147221248))
                        {
                            throw new Win32Exception(num2);
                        }
                    }
                    this.dropTarget = value;
                }
                catch (Exception exception)
                {
                    OLEDropTarget.ClearOwner();
                    OLEDropTarget = null;
                    throw new InvalidOperationException(Language.GetString("DragDropFailed", "Drag and drop registration failed"), exception);
                }
            }
        }

        #region ISupportOleDropSource Members

        public void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            GiveFeedbackEventHandler handler = GiveFeedback;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void OnQueryContinueDrag(QueryContinueDragEventArgs e)
        {
            QueryContinueDragEventHandler handler = QueryContinueDrag;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

    }
}
