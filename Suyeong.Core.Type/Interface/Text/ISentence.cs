using System.Collections.Generic;

namespace Suyeong.Core.Type
{
    public interface ISentence<T> : IRect<T>
    {
        string Text { get; }
        IWord<T> StartWord { get; }
        IWord<T> EndWord { get; }
        IEnumerable<IWord<T>> Words { get; }
    }
}
