namespace Suyeong.Core.Type
{
    public interface ICircle<T> : IDiagram<T>
    {
        T Radius { get; }
    }
}
