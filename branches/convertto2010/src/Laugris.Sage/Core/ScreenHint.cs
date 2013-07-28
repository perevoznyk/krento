using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Laugris.Sage
{
    public class ScreenHint : Window
    {
        private Color hintOutlineColor;
        private Color hintBorderColor;
        private Color hintBodyColor;

        private Pen penOutLine;
        private SolidBrush brushBorder;
        private SolidBrush brushBody;
        private WindowsFont windowsFont;

        private VisualButton buttonAbout;
        private VisualButton buttonChangeType;
        private VisualButton buttonRemoveStone;
        private VisualButton buttonConfigureStone;

        private Bitmap Background;

        private int buttonSpace = 4;
        private int buttonSize = 26;

        public ScreenHint()
            : base()
        {
            Name = "ScreenHint";
            Alpha = 230;
            ColorKey = ColorUtils.WhiteKey;

            CanDrag = false;
            TopMostWindow = true;
            CustomPaint = true;

            this.Painting += new PaintEventHandler(ScreenHint_Painting);
            
            //Font = new Font("Tahoma", 18.0f, FontStyle.Bold, GraphicsUnit.Pixel);
            windowsFont = new WindowsFont("Tahoma", 11, FontStyle.Bold, WindowsFontQuality.ClearType);

            ForeColor = Color.White;
            HintOutlineColor = Color.Black;
            HintBodyColor = Color.Black;
            HintBorderColor = Color.Gainsboro;

            windowsFont.Color = ForeColor;

            buttonAbout = new VisualButton();
            ButtonAbout.Tag = 4;
            ButtonAbout.VisualFeedback = false;

            buttonChangeType = new VisualButton();
            buttonChangeType.Tag = 3;
            buttonChangeType.VisualFeedback = false;

            buttonConfigureStone = new VisualButton();
            buttonConfigureStone.Tag = 2;
            buttonConfigureStone.VisualFeedback = false;

            buttonRemoveStone = new VisualButton();
            buttonRemoveStone.Tag = 1;
            buttonRemoveStone.VisualFeedback = false;

            ReloadButtonResources();

            buttonAbout.Parent = this;
            buttonChangeType.Parent = this;
            buttonConfigureStone.Parent = this;
            buttonRemoveStone.Parent = this;
        }


        public VisualButton ButtonAbout
        {
            get { return buttonAbout; }
        }

        public VisualButton ButtonChangeType
        {
            get { return buttonChangeType; }
        }

        public VisualButton ButtonConfigureStone
        {
            get { return buttonConfigureStone; }
        }

        public int ButtonSize
        {
            get { return buttonSize; }
            set { buttonSize = value; }
        }

        public int ButtonSpace
        {
            get { return buttonSpace; }
            set { buttonSpace = value; }
        }

        public VisualButton ButtonRemoveStone
        {
            get { return buttonRemoveStone; }
        }

        public void ReloadButtonResources()
        {
            buttonAbout.NormalFace = BitmapPainter.ResizeBitmap(NativeThemeManager.LoadBitmap("ButtonStoneAbout.png"), buttonSize, buttonSize, true);
            buttonChangeType.NormalFace = BitmapPainter.ResizeBitmap(NativeThemeManager.LoadBitmap("ButtonStoneSelect.png"), buttonSize, buttonSize, true);
            buttonConfigureStone.NormalFace = BitmapPainter.ResizeBitmap(NativeThemeManager.LoadBitmap("ButtonStoneEdit.png"), buttonSize, buttonSize, true);
            buttonRemoveStone.NormalFace = BitmapPainter.ResizeBitmap(NativeThemeManager.LoadBitmap("ButtonStoneDelete.png"), buttonSize, buttonSize, true);
            buttonAbout.SetSize(buttonSize, buttonSize);
            buttonChangeType.SetSize(buttonSize, buttonSize);
            buttonConfigureStone.SetSize(buttonSize, buttonSize);
            buttonRemoveStone.SetSize(buttonSize, buttonSize);
        }

        public void ReloadButtonAbout(string fileName)
        {
            buttonAbout.NormalFace = FastBitmap.FromFile(fileName);
            if (buttonAbout.NormalFace == null)
                buttonAbout.NormalFace = BitmapPainter.ResizeBitmap(NativeThemeManager.LoadBitmap("ButtonStoneAbout.png"), buttonSize, buttonSize, true);
            buttonAbout.SetSize(buttonSize, buttonSize);
        }

        public void ReloadButtonSelect(string fileName)
        {
            buttonChangeType.NormalFace = FastBitmap.FromFile(fileName);
            if (buttonChangeType.NormalFace == null)
                buttonChangeType.NormalFace = BitmapPainter.ResizeBitmap(NativeThemeManager.LoadBitmap("ButtonStoneSelect.png"), buttonSize, buttonSize, true);
            buttonChangeType.SetSize(buttonSize, buttonSize);
        }

        public void ReloadButtonEdit(string fileName)
        {
            buttonConfigureStone.NormalFace = FastBitmap.FromFile(fileName);
            if (buttonConfigureStone.NormalFace == null)
                buttonConfigureStone.NormalFace = BitmapPainter.ResizeBitmap(NativeThemeManager.LoadBitmap("ButtonStoneEdit.png"), buttonSize, buttonSize, true);
            buttonConfigureStone.SetSize(buttonSize, buttonSize);
        }

        public void ReloadButtonDelete(string fileName)
        {
            buttonRemoveStone.NormalFace = FastBitmap.FromFile(fileName);
            if (buttonRemoveStone.NormalFace == null)
                buttonRemoveStone.NormalFace = BitmapPainter.ResizeBitmap(NativeThemeManager.LoadBitmap("ButtonStoneDelete.png"), buttonSize, buttonSize, true);
            buttonRemoveStone.SetSize(buttonSize, buttonSize);
        }

        public void SelectNewFont(string fontName, int fontSize, FontStyle fontStyle)
        {
            if (windowsFont != null)
                windowsFont.Dispose();
            fontSize = NativeMethods.EmToPixels(fontSize);
            windowsFont = new WindowsFont(fontName, fontSize, fontStyle, WindowsFontQuality.ClearType);
            windowsFont.Color = ForeColor;
        }

        protected override void Dispose(bool disposing)
        {
            windowsFont.Dispose();
            if (penOutLine != null)
                penOutLine.Dispose();
            if (brushBody != null)
                brushBody.Dispose();
            if (brushBorder != null)
                brushBorder.Dispose();
            if (Background != null)
            {
                Background.Dispose();
                Background = null;
            }
            base.Dispose(disposing);
        }

        public Color HintOutlineColor
        {
            get { return hintOutlineColor; }
            set
            {
                hintOutlineColor = value;
                if (penOutLine != null)
                    penOutLine.Dispose();
                penOutLine = new Pen(hintOutlineColor);
            }
        }

        public Color HintBorderColor
        {
            get { return hintBorderColor; }
            set
            {
                hintBorderColor = value;
                if (brushBorder != null)
                    brushBorder.Dispose();
                brushBorder = new SolidBrush(hintBorderColor);
            }
        }

        public Color HintBodyColor
        {
            get { return hintBodyColor; }
            set
            {
                hintBodyColor = value;
                if (brushBody != null)
                    brushBody.Dispose();
                brushBody = new SolidBrush(hintBodyColor);
            }
        }

        void ScreenHint_Painting(object sender, PaintEventArgs e)
        {
            if (Background != null)
            {
                Canvas.DrawImageUnscaled(Background, 0, 0);
            }
        }

        
        public void PaintCaption(string caption, Bitmap glyph)
        {
            this.Text = caption;
            if (string.IsNullOrEmpty(caption))
            {
                Clear();
                Update(false);
                Hide();
                return;
            }
            else
            {
                Size size = windowsFont.GetTextSize(caption, 200, TextFormatFlags.Left | TextFormatFlags.WordBreak, true);
                int captionHeight = (int)size.Height + 1;
                int captionWidth = (int)size.Width + 1;

                int buttonsWidth = buttonSize * 4 + buttonSpace * 5;
                

                int totalWidth = 46 + captionWidth;
                if (totalWidth < buttonsWidth)
                    totalWidth = buttonsWidth;

                int totalHeight = Math.Max(captionHeight, 34) + 10 + buttonSize;

                buttonConfigureStone.Left = buttonSpace;
                buttonChangeType.Left = buttonSpace * 2 + buttonSize;
                buttonRemoveStone.Left = buttonSpace * 3 + buttonSize * 2;
                buttonAbout.Left = buttonSpace * 4 + buttonSize * 3;

                int buttonTop = totalHeight - buttonSpace - buttonSize;

                buttonConfigureStone.Top = buttonTop;
                buttonChangeType.Top = buttonTop;
                buttonRemoveStone.Top = buttonTop;
                buttonAbout.Top = buttonTop;

                if (Background != null)
                {
                    Background.Dispose();
                    Background = null;
                }
                Background = new Bitmap(totalWidth, totalHeight, PixelFormat.Format32bppPArgb);
                using (Graphics g = Graphics.FromImage(Background))
                {
                    g.FillRectangle(brushBorder, new Rectangle(1, 1, totalWidth - 2, totalHeight - 2));
                    g.FillRectangle(brushBody, new Rectangle(3, 3, totalWidth - 6, totalHeight - 6));
                    g.DrawRectangle(penOutLine, new Rectangle(0, 0, totalWidth - 1, totalHeight - 1));
                    windowsFont.DrawTextRect(g, caption, ForeColor, hintBodyColor, new Rectangle(43, 5, captionWidth, captionHeight), TextFormatFlags.WordBreak);
                    if (glyph != null)
                        BitmapPainter.DrawImageScaled(glyph, g, 7, 6, 32, 32);
                }

                Size(totalWidth, totalHeight);
                Repaint();
            }
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case NativeMethods.WM_NCHITTEST:
                    m.Result = (IntPtr)NativeMethods.HTTRANSPARENT;
                    break;

            }
            base.WndProc(ref m);
        }

        public void FollowMovement(int deltaX, int deltaY)
        {
            this.UpdatePosition(this.Left + deltaX, this.Top + deltaY);
        }

        public void Activate(int x, int y)
        {
            UpdatePosition(x, y);
        }
    }
}
