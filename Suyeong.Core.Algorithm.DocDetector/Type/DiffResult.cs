using System.Collections.Generic;

namespace Suyeong.Core.Algorithm.DocDetector
{
    public struct DiffResult
    {
        public DiffResult(int index, DiffType diffType, DiffSentence main, DiffSentence sub, List<string> sameTexts, List<string> modifiedTexts)
        {
            this.Index = index;
            this.DiffType = diffType;
            this.Main = main;
            this.Sub = sub;
            this.SameTexts = sameTexts;
            this.ModifiedTexts = modifiedTexts;
        }

        public int Index { get; private set; }
        public DiffType DiffType { get; private set; }
        public DiffSentence Main { get; private set; }
        public DiffSentence Sub { get; private set; }
        public List<string> SameTexts { get; private set; }
        public List<string> ModifiedTexts { get; private set; }
    }

    public class DiffResultCollection : List<DiffResult>
    {
        public DiffResultCollection()
        {
        }

        public DiffResultCollection(IEnumerable<DiffResult> diffResults) : base()
        {
            this.AddRange(diffResults);
        }
    }

    public class DiffResultDictionary : Dictionary<int, DiffResult>
    {
        public DiffResultDictionary()
        {

        }

        public DiffResultDictionary(IEnumerable<KeyValuePair<int, DiffResult>> dic) : base()
        {
            if (dic != null)
            {
                foreach (KeyValuePair<int, DiffResult> kvp in dic)
                {
                    this.Add(kvp.Key, kvp.Value);
                }
            }
        }

        public DiffResultCollection GetValueCollection()
        {
            return new DiffResultCollection(this.Values);
        }
    }
}
