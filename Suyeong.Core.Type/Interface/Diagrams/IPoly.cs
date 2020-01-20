using System.Collections.Generic;

namespace Suyeong.Core.Type
{
    public interface IPoly<T> : IDiagram<T>
    {
        IEnumerable<IPoint<T>> Points { get; }
    }
}
