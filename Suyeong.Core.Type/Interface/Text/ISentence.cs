using System.Collections.Generic;

namespace Suyeong.Core.Type
{
    public interface ISentence<T> : IRect<T>
    {
        string Text { get; }
        IEnumerable<IWord<T>> Words { get; }
    }
}
