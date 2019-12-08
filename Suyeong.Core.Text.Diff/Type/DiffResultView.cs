using System.Collections.Generic;

namespace Suyeong.Core.Text.Diff
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

    public class DiffResultViews : List<DiffResultView>
    {
        public DiffResultViews()
        {
        }

        public DiffResultViews(IEnumerable<DiffResultView> diffResultViews) : base()
        {
            this.AddRange(diffResultViews);
        }
    }
}
