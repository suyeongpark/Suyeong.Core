using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Suyeong.Core.Net.Lib;

namespace Suyeong.Core.Net.Tcp
{
    public class TcpListenerConcurrencyCryptSync : IDisposable
    {
        TcpListener listener;
        TcpClientHandlerConcurrencyCryptSyncGroupDictionary handlerDicGroup;
        Func<string, string, IPacket> userEnter, userExit;
        Func<IPacket, IPacket> response;
        byte[] key, iv;
        bool disposedValue;  // 중복호출 제거용

        /// <summary>
        /// userEnter, userExit은 사용자의 입장과 퇴장에 대한 callback으로써 StageID, UserID를 받고 IPacket을 반환한다.
        /// </summary>
        /// <param name="portNum"></param>
        /// <param name="userEnterCallback"></param>
        /// <param name="userExitCallback"></param>
        /// <param name="responseCallbak"></param>
        public TcpListenerConcurrencyCryptSync(int portNum, byte[] key, byte[] iv, Func<string, string, IPacket> userEnterCallback, Func<string, string, IPacket> userExitCallback, Func<IPacket, IPacket> responseCallbak)
        {
            this.key = key;
            this.iv = iv;
            this.userEnter = userEnterCallback;
            this.userExit = userExitCallback;
            this.response = responseCallbak;

            this.listener = new TcpListener(new IPEndPoint(address: IPAddress.Any, port: portNum));
            this.handlerDicGroup = new TcpClientHandlerConcurrencyCryptSyncGroupDictionary();
            this.disposedValue = false;
        }

        // TODO: 아래의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다. 
        ~TcpListenerConcurrencyCryptSync()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                // TODO: 관리되는 상태(관리되는 개체)를 삭제
                if (disposing)
                {
                    this.listener.Stop();
                }

                // TODO: 관리되지 않는 리소스(관리되지 않는 개체)를 해제

                // TODO: 큰 필드를 null로 설정.

                this.disposedValue = true;
            }
        }

        public void Start()
        {
            this.listener.Start();

            Thread thread;
            TcpClient client;
            NetworkStream stream;
            TcpClientHandlerConcurrencyCryptSync handler;
            PacketValue connectPacket;
            int nbytes, receiveDataLength;
            string stageID, userID;
            byte[] receiveHeader, receiveData, decryptData;

            while (true)
            {
                try
                {
                    client = this.listener.AcceptTcpClient();
                    stream = client.GetStream();

                    // 사용자가 접속하면 우선 사용자로부터 입장할 채널 정보와 사용자 정보를 받는다. 
                    // 1. 요청 헤더를 받는다.
                    receiveHeader = new byte[Consts.SIZE_HEADER];
                    nbytes = stream.Read(buffer: receiveHeader, offset: 0, size: receiveHeader.Length);

                    // 2. 요청 데이터를 받는다.
                    receiveDataLength = BitConverter.ToInt32(value: receiveHeader, startIndex: 0);
                    receiveData = TcpUtil.ReceiveData(networkStream: stream, dataLength: receiveDataLength);

                    stream.Flush();

                    // 3. 받은 요청은 암호화되어 있으므로 푼다.
                    decryptData = NetUtil.Decrypt(data: receiveData, key: this.key, iv: this.iv);
                    connectPacket = NetUtil.DeserializeObject(data: decryptData) as PacketValue;

                    // protocol에 입장하려는 stage의 id를 넣고, value에 user id를 넣는다.
                    stageID = connectPacket.Protocol;
                    userID = connectPacket.Value.ToString();

                    // 사용자 정보를 이용해서 handler를 추가한다.

                    // 사용자 정보를 이용해서 handler를 만든다.
                    handler = new TcpClientHandlerConcurrencyCryptSync(client: client, stageID: stageID, userID: userID, key: this.key, iv: this.iv);
                    handler.Disconnect += Disconnect;
                    handler.Receive += Receive;

                    AddStage(handler: handler, stageID: stageID, userID: userID);

                    // hander를 시작한다.
                    thread = new Thread(handler.Start);
                    thread.IsBackground = true;
                    thread.Start();

                    // 사용자가 입장한 정보를 broadcast 한다.
                    IPacket sendPacket = this.userEnter(stageID, userID);
                    BroadcastToStage(stageID: stageID, sendPacket: sendPacket);
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        public void Disconnect(string stageID, string userID)
        {
            TcpClientHandlerConcurrencyCryptSyncDictionary handlerDic;
            TcpClientHandlerConcurrencyCryptSync handler;

            if (this.handlerDicGroup.TryGetValue(stageID, out handlerDic))
            {
                if (handlerDic.TryGetValue(userID, out handler))
                {
                    handler.Dispose();
                }

                handlerDic.Remove(userID);
            }

            IPacket sendPacket = this.userExit(stageID, userID);
            BroadcastToStage(stageID: stageID, sendPacket: sendPacket);
        }

        public void MoveStage(string oldStageID, string newStageID, string userID)
        {
            // 1. 기존 stage에서 제거
            TcpClientHandlerConcurrencyCryptSync handler = RemoveStage(stageID: oldStageID, userID: userID);
            handler.SetStageID(stageID: newStageID);

            // 2. 기존 stage에 퇴장 알림
            IPacket exitPacket = this.userExit(oldStageID, userID);
            BroadcastToStage(stageID: oldStageID, sendPacket: exitPacket);

            // 3. 새로운 stage로 입장
            AddStage(handler: handler, stageID: newStageID, userID: userID);

            // 4. 새로운 stage에 입장 알림
            IPacket enterPacket = this.userEnter(newStageID, userID);
            BroadcastToStage(stageID: newStageID, sendPacket: enterPacket);
        }

        public void BroadcastToServer(IPacket sendPacket)
        {
            foreach (KeyValuePair<string, TcpClientHandlerConcurrencyCryptSyncDictionary> kvp in this.handlerDicGroup)
            {
                foreach (KeyValuePair<string, TcpClientHandlerConcurrencyCryptSync> kvp2 in kvp.Value)
                {
                    kvp2.Value.Send(packet: sendPacket);
                }
            }
        }

        void AddStage(TcpClientHandlerConcurrencyCryptSync handler, string stageID, string userID)
        {
            TcpClientHandlerConcurrencyCryptSyncDictionary handlerDic;

            if (this.handlerDicGroup.TryGetValue(stageID, out handlerDic))
            {
                handlerDic.Add(userID, handler);

                this.handlerDicGroup[stageID] = handlerDic;
            }
            else
            {
                handlerDic = new TcpClientHandlerConcurrencyCryptSyncDictionary();
                handlerDic.Add(userID, handler);

                this.handlerDicGroup.Add(stageID, handlerDic);
            }
        }

        TcpClientHandlerConcurrencyCryptSync RemoveStage(string stageID, string userID)
        {
            TcpClientHandlerConcurrencyCryptSyncDictionary handlerDic;
            TcpClientHandlerConcurrencyCryptSync handler;

            if (this.handlerDicGroup.TryGetValue(stageID, out handlerDic))
            {
                if (handlerDic.TryGetValue(userID, out handler))
                {
                    handlerDic.Remove(userID);
                    this.handlerDicGroup[stageID] = handlerDic;

                    return handler;
                }
            }

            return null;
        }


        void Receive(string stageID, IPacket receivePacket)
        {
            // 클라이언트가 발생시킨 요청을 처리하고 그 결과를 채널 내 사용자들에게 모두 broadcasting 한다. 당사자 포함.
            IPacket sendPacket = this.response(receivePacket);

            // disconnect나 stage 이동시에는 null이 나올 수 있음
            if (sendPacket != null)
            {
                BroadcastToStage(stageID: stageID, sendPacket: sendPacket);
            }
        }

        void BroadcastToStage(string stageID, IPacket sendPacket)
        {
            TcpClientHandlerConcurrencyCryptSyncDictionary handlerDic;

            if (this.handlerDicGroup.TryGetValue(stageID, out handlerDic))
            {
                foreach (KeyValuePair<string, TcpClientHandlerConcurrencyCryptSync> kvp in handlerDic)
                {
                    kvp.Value.Send(packet: sendPacket);
                }
            }
        }
    }

    public class TcpListenerConcurrencyCryptSyncCollection : List<TcpListenerConcurrencyCryptSync>
    {
        public TcpListenerConcurrencyCryptSyncCollection()
        {

        }
    }
}
