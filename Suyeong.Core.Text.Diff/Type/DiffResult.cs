using System.Collections.Generic;

namespace Suyeong.Core.Text.Diff
{
    public struct DiffResult
    {
        public DiffResult(int index, DiffType diffType, Sentence main, Sentence sub, List<string> sameTexts, List<string> modifiedTexts)
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
        public Sentence Main { get; private set; }
        public Sentence Sub { get; private set; }
        public List<string> SameTexts { get; private set; }
        public List<string> ModifiedTexts { get; private set; }
    }

    public class DiffResults : List<DiffResult>
    {
        public DiffResults()
        {
        }

        public DiffResults(IEnumerable<DiffResult> diffResults) : base()
        {
            this.AddRange(diffResults);
        }
    }

    public class DiffResultDic : Dictionary<int, DiffResult>
    {
        public DiffResultDic()
        {

        }

        public DiffResultDic(IEnumerable<KeyValuePair<int, DiffResult>> dic) : base()
        {
            foreach (KeyValuePair<int, DiffResult> kvp in dic)
            {
                this.Add(kvp.Key, kvp.Value);
            }
        }

        public DiffResults GetValues()
        {
            return new DiffResults(this.Values);
        }
    }
}
