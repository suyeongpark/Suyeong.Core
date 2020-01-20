namespace Suyeong.Core.Type
{
    public interface IDiagram<T>
    {
        int Index { get; }
        T MinX { get; }
        T MinY { get; }
        T MaxX { get; }
        T MaxY { get; }
        T CenterX { get; }
        T CenterY { get; }
    }
}
