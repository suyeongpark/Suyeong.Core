using System;

namespace Suyeong.Core.Net.Lib
{
    [Serializable]
    public abstract class Packet : IPacket
    {
        public Packet(string protocol)
        {
            this.Protocol = protocol;
        }

        public string Protocol { get; private set; }
    }

    [Serializable]
    public class PacketValue : Packet
    {
        public PacketValue(string protocol, object value) : base(protocol: protocol)
        {
            this.Value = value;
        }

        public object Value { get; private set; }
    }

    [Serializable]
    public class PacketJson : Packet
    {
        public PacketJson(string protocol, string json) : base(protocol: protocol)
        {
            this.Json = json;
        }

        public string Json { get; private set; }
    }

    [Serializable]
    public class PacketSerialized : Packet
    {
        public PacketSerialized(string protocol, byte[] serializedData) : base(protocol: protocol)
        {
            this.SerializedData = serializedData;
        }

        public byte[] SerializedData { get; private set; }
    }

    [Serializable]
    public class PacketFile : Packet
    {
        public PacketFile(string protocol, string fileName, byte[] fileData) : base(protocol: protocol)
        {
            this.FileName = fileName;
            this.FileData = fileData;
        }

        public string FileName { get; private set; }
        public byte[] FileData { get; private set; }
    }

}
