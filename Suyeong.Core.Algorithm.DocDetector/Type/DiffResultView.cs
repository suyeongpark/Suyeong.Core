using System.Collections.Generic;

namespace Suyeong.Core.Algorithm.DocDetector
{
    public struct DiffResultView
    {
        public DiffResultView(int index, DiffResult mainResult, DiffResult subResult)
        {
            this.Index = index;
            this.IndexMain = mainResult;
            this.IndexSub = subResult;
        }

        public int Index { get; private set; }
        public DiffResult IndexMain { get; private set; }
        public DiffResult IndexSub { get; private set; }
    }

    public class DiffResultViewCollection : List<DiffResultView>
    {
        public DiffResultViewCollection()
        {
        }

        public DiffResultViewCollection(IEnumerable<DiffResultView> diffResultViews) : base()
        {
            this.AddRange(diffResultViews);
        }
    }
}
