namespace Suyeong.Core.Net.Lib
{
    public interface IPacket
    {
        PacketType Type { get; }
        string Protocol { get; }
    }
}
