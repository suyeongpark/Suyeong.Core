using System;
using System.Collections.Generic;
using System.Linq;

namespace Suyeong.Core.Algorithm.DocDetector
{
    public struct TextBlockSet
    {
        public TextBlockSet(int index, TextBlockCollection textBlocks)
        {
            this.Index = index;
            this.TextBlocks = textBlocks;

            if (this.TextBlocks.Count > 0)
            {
                this.FirstText = this.TextBlocks[0];
                this.LastText = this.TextBlocks[this.TextBlocks.Count - 1];
                this.Text = string.Join(" ", this.TextBlocks.Select(text => text.Text));
            }
            else
            {
                this.FirstText = new TextBlock();
                this.LastText = new TextBlock();
                this.Text = string.Empty;
            }

            this.LeftX = this.FirstText.LeftX;
            this.RightX = this.LastText.RightX;
            this.TopY = this.FirstText.TopY;
            this.BottomY = this.LastText.BottomY;
            this.CenterX = (int)Math.Round((this.LeftX + this.RightX) * 0.5d, MidpointRounding.AwayFromZero);
            this.CenterY = (int)Math.Round((this.TopY + this.BottomY) * 0.5d, MidpointRounding.AwayFromZero);
            this.Width = this.RightX - this.LeftX;
            this.Height = this.BottomY - this.TopY;
            this.Rotate = this.FirstText.Rotate;
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
        public TextBlock FirstText { get; private set; }
        public TextBlock LastText { get; private set; }
        public TextBlockCollection TextBlocks { get; private set; }
    }


    public class TextBlockSetCollection : List<TextBlockSet>
    {
        public TextBlockSetCollection()
        {

        }

        public TextBlockSetCollection(IEnumerable<TextBlockSet> blocks) : base()
        {
            this.AddRange(blocks);
        }
    }
}
