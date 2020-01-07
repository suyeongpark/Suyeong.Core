using System;
using System.Collections.Generic;

namespace Suyeong.Core.Algorithm.DocDetector
{
    public struct TextBlock
    {
        public TextBlock(int index, int x, int y, int width, int height, int rotate, string text)
        {
            this.Index = index;
            this.LeftX = x;
            this.RightX = x + width;
            this.TopY = y;
            this.BottomY = y + height;
            this.CenterX = (int)Math.Round((this.LeftX + this.RightX) * 0.5d, MidpointRounding.AwayFromZero);
            this.CenterY = (int)Math.Round((this.TopY + this.BottomY) * 0.5d, MidpointRounding.AwayFromZero);
            this.Width = width;
            this.Height = height;
            this.Rotate = rotate;
            this.Text = text;
        }

        public int Index { get; private set; }
        public int LeftX { get; private set; }
        public int RightX { get; private set; }
        public int TopY { get; private set; }
        public int BottomY { get; private set; }
        public int CenterX { get; private set; }
        public int CenterY { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Rotate { get; private set; }
        public string Text { get; private set; }
    }


    public class TextBlockCollection : List<TextBlock>
    {
        public TextBlockCollection()
        {

        }

        public TextBlockCollection(IEnumerable<TextBlock> blocks) : base()
        {
            this.AddRange(blocks);
        }
    }
}
