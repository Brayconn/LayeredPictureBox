using System;
using System.Drawing;

namespace LayeredPictureBox
{
    public static class Extensions
    {
        public static Rectangle PositiveOffset(this Rectangle rect, Point p)
        {
            return new Rectangle(rect.X + p.X, rect.Y + p.Y, rect.Width, rect.Height);
        }
        public static Rectangle NegativeOffset(this Rectangle rect, Point p)
        {
            return new Rectangle(rect.X - p.X, rect.Y - p.Y, rect.Width, rect.Height);
        }
        public static Rectangle Multiply(this Rectangle rect, int num)
        {
            return new Rectangle(rect.X * num, rect.Y * num, rect.Width * num, rect.Height * num);
        }
        public static Rectangle ExpandDivide(this Rectangle rect, int num)
        {
                                //coordinates round down...
            return new Rectangle(rect.X/num, rect.Y/num,
                                //size rounds up to the nearest multiple of Num, + and extra num... for some reason?!
                                 (int)Math.Ceiling(decimal.Divide(rect.Width, num)) + num,
                                 (int)Math.Ceiling(decimal.Divide(rect.Height, num)) + num);
        }
        /*This was a cool function I didn't need
        public static Rectangle RectangleAct(Rectangle r1, Rectangle r2, Func<int, int, int> act)
        {
            return new Rectangle(
                act(r1.X, r2.X),
                act(r1.Y, r2.Y),
                act(r1.Width, r2.Width),
                act(r1.Height, r2.Height)
                );
        }
        */
    }
}
