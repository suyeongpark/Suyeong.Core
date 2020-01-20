using System.Collections.Generic;

namespace Suyeong.Core.Type
{
    public interface IWord<T> : IRect<T>
    {
        TextOrientation TextOrientation { get; }
        string Text { get; }
        ICharacter<T> StartCharacter { get; }
        ICharacter<T> EndCharacter { get; }
        IEnumerable<ICharacter<T>> Characters { get; }
    }
}
