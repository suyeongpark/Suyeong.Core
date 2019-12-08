using System;
using System.Collections.Generic;

namespace Suyeong.Core.GoogleVision.OCR
{
    public struct OcrText
    {
        public OcrText(int index, int rotate, int leftX, int rightX, int topY, int bottomY, string text)
        {
            this.Index = index;
            this.Rotate = rotate;
            this.LeftX = leftX;
            this.RightX = rightX;
            this.TopY = topY;
            this.BottomY = bottomY;
            this.Width = this.RightX - this.LeftX;
            this.Height = this.BottomY - this.TopY;
            this.CenterX = (int)Math.Round((this.LeftX + this.RightX) * 0.5d, MidpointRounding.AwayFromZero);
            this.CenterY = (int)Math.Round((this.TopY + this.BottomY) * 0.5d, MidpointRounding.AwayFromZero);
            this.Text = text;
        }

        public int Index { get; private set; }
        public int Rotate { get; private set; }
        public int LeftX { get; private set; }
        public int RightX { get; private set; }
        public int TopY { get; private set; }
        public int BottomY { get; private set; }
        public int CenterX { get; private set; }
        public int CenterY { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public string Text { get; private set; }

        public static OcrText operator +(OcrText text1, OcrText text2)
        {
            // 둘의 회전이 동일하면 같은 것을 쓰고, 다르면 글자가 더 긴 것을 쓴다.
            int rotate = text1.Rotate == text2.Rotate ? text1.Rotate : (text1.Text.Length < text2.Text.Length ? text2.Rotate : text1.Rotate);

            int index = text1.Index <= text2.Index ? text1.Index : text2.Index;
            int leftX = text1.LeftX <= text2.LeftX ? text1.LeftX : text2.LeftX;
            int rightX = text1.RightX >= text2.RightX ? text1.RightX : text2.RightX;
            int topY = text1.TopY <= text2.TopY ? text1.TopY : text2.TopY;
            int bottomY = text1.BottomY >= text2.BottomY ? text1.BottomY : text2.BottomY;
            string text = text1.Text + text2.Text;

            return new OcrText(index: index, rotate: rotate, leftX: leftX, rightX: rightX, topY: topY, bottomY: bottomY, text: text);
        }
    }

    public class OcrTexts : List<OcrText>
    {
        public OcrTexts()
        {

        }

        public OcrTexts(IEnumerable<OcrText> ocrTexts) : base()
        {
            this.AddRange(ocrTexts);
        }
    }
}
