namespace Suyeong.Core.Type
{
    public interface ILine<T>
    {
        int Index { get; }
        LineOrientation LineOrientation { get; }
        T StartX { get; }
        T StartY { get; }
        T EndX { get; }
        T EndY { get; }
        T MinX { get; }
        T MinY { get; }
        T MaxX { get; }
        T MaxY { get; }
        T CenterX { get; }
        T CenterY { get; }
        T DeltaX { get; }
        T DeltaY { get; }
        T Length { get; }
        double Tangent { get; }
    }
}
