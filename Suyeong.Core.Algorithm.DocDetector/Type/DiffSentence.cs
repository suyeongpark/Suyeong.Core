using System.Collections.Generic;

namespace Suyeong.Core.Algorithm.DocDetector
{
    public struct DiffSentence
    {
        public DiffSentence(int index, string text)
        {
            this.Index = index;
            this.Text = text;
            this.Texts = text?.Split(new char[] { ' ' });
        }

        public int Index { get; private set; }
        public string Text { get; private set; }
        public string[] Texts { get; private set; }
    }

    public class DiffSentenceCollection : List<DiffSentence>
    {
        public DiffSentenceCollection()
        {

        }

        public DiffSentenceCollection(IEnumerable<DiffSentence> sentences) : base()
        {
            this.AddRange(sentences);
        }
    }
}
