using System;

namespace Suyeong.Core.Net.Lib
{
    [Serializable]
    public abstract class Packet : IPacket
    {
        PacketType type;
        string protocol;

        public Packet(PacketType type, string protocol)
        {
            this.type = type;
            this.protocol = protocol;
        }

        public PacketType Type { get { return this.type; } }
        public string Protocol { get { return this.protocol; } }
    }

    [Serializable]
    public class PacketMessage : Packet
    {
        object data;

        public PacketMessage(PacketType type, string protocol, object data) : base(type: type, protocol: protocol)
        {
            this.data = data;
        }

        public object Data { get { return this.data; } }
    }

    [Serializable]
    public class PacketFile : Packet
    {
        string fileName;
        byte[] fileData;

        public PacketFile(PacketType type, string protocol, string fileName, byte[] fileData) : base(type: type, protocol: protocol)
        {
            this.fileName = fileName;
            this.fileData = fileData;
        }

        public string FileName { get { return this.fileName; } }
        public byte[] FileData { get { return this.fileData; } }
    }

}
