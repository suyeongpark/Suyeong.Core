﻿using System;
using System.Net;
using System.Net.Sockets;
using Suyeong.Core.Net.Lib;

namespace Suyeong.Core.Net.Tcp
{
    public class TcpListenerSync
    {
        TcpListener listener;

        public TcpListenerSync(int portNum)
        {
            this.listener = new TcpListener(new IPEndPoint(address: IPAddress.Any, port: portNum));
        }

        public void ListenerStart(Func<IPacket, IPacket> callback)
        {
            this.listener.Start();

            IPacket receivePacket, sendPacket;
            PacketType type;
            int receiveDataLength, sendDataLength, nbytes;
            byte[] receiveHeader, sendHeader, receiveData, sendData, decompressData, compressData;

            while (true)
            {
                try
                {
                    using (TcpClient client = this.listener.AcceptTcpClient())
                    using (NetworkStream stream = client.GetStream())
                    {
                        // 1. 요청 헤더를 받는다.
                        receiveHeader = new byte[Consts.SIZE_HEADER];
                        nbytes = stream.Read(buffer: receiveHeader, offset: 0, size: receiveHeader.Length);

                        if (nbytes > 0)
                        {
                            // 2. 요청 데이터를 받는다.
                            type = (PacketType)BitConverter.ToInt32(value: receiveHeader, startIndex: 0);
                            receiveDataLength = BitConverter.ToInt32(value: receiveHeader, startIndex: Consts.SIZE_INDEX);  // BitConverter.ToInt32 자체가 4바이트를 읽겠다는 의미라서 Start Index만 있으면 된다.
                            receiveData = TcpUtil.ReceiveData(networkStream: stream, dataLength: receiveDataLength);

                            // 3. 받은 요청은 압축되어 있으므로 푼다.
                            decompressData = NetUtil.Decompress(data: receiveData);
                            receivePacket = NetUtil.DeserializeObject(data: decompressData) as IPacket;

                            // 4. 요청을 처리한다.
                            sendPacket = callback(receivePacket);

                            // 5. 처리 결과를 압축한다.
                            sendData = NetUtil.SerializeObject(data: sendPacket);
                            compressData = NetUtil.Compress(data: sendData);

                            // 6. 처리한 결과의 헤더를 보낸다.
                            sendDataLength = compressData.Length;
                            sendHeader = BitConverter.GetBytes(value: sendDataLength);
                            stream.Write(buffer: sendHeader, offset: 0, size: sendHeader.Length);

                            // 7. 처리한 결과의 데이터를 보낸다.
                            TcpUtil.SendData(networkStream: stream, data: compressData, dataLength: sendDataLength);

                            // 8. flush
                            stream.Flush();
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public void ListenerStop()
        {
            this.listener.Stop();
        }
    }
}
