using System.Drawing;
namespace Laugris.Sage
{
    public struct SkinOffset
    {
        public int Left { get; set; }
        public int Right { get; set; }
        public int Top { get; set; }
        public int Bottom { get; set; }

        public SkinOffset(int left, int top, int right, int bottom) : this()
        {
            this.Left = left;
            this.Right = right;
            this.Top = top;
            this.Bottom = bottom;
        }

        public SkinOffset(Rectangle r) : this()
        {
            this.Left = r.Left;
            this.Top = r.Top;
            this.Right = r.Right;
            this.Bottom = r.Bottom;
        }

    }
}