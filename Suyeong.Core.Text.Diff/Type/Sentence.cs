using System.Collections.Generic;

namespace Suyeong.Core.Text.Diff
{
    public struct Sentence
    {
        public Sentence(int index, string text)
        {
            this.Index = index;
            this.Text = text;
            this.Texts = text?.Split(new char[] { ' ' });
        }

        public int Index { get; private set; }
        public string Text { get; private set; }
        public string[] Texts { get; private set; }
    }

    public class SentenceCollection : List<Sentence>
    {
        public SentenceCollection()
        {

        }

        public SentenceCollection(IEnumerable<Sentence> sentences) : base()
        {
            this.AddRange(sentences);
        }
    }
}
