using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Suyeong.Core.Net.Lib;

namespace Suyeong.Core.Net.Tcp
{
    public class TcpListenerConcurrencyAsync : IDisposable
    {
        TcpListener listener;
        TcpClientHandlerConcurrencyAsyncDicGroup handlerDicGroup;
        Func<string, string, Task<IPacket>> userEnterAsync, userExitAsync;
        Func<IPacket, Task<IPacket>> response;

        /// <summary>
        /// userEnter, userExit은 사용자의 입장과 퇴장에 대한 callback으로써 StageID, UserID를 받고 IPacket을 반환한다.
        /// </summary>
        /// <param name="portNum"></param>
        /// <param name="userEnterCallbackAsync"></param>
        /// <param name="userExitCallbackAsync"></param>
        /// <param name="responseCallbakAsync"></param>
        public TcpListenerConcurrencyAsync(int portNum, Func<string, string, Task<IPacket>> userEnterCallbackAsync, Func<string, string, Task<IPacket>> userExitCallbackAsync, Func<IPacket, Task<IPacket>> responseCallbakAsync)
        {
            this.userEnterAsync = userEnterCallbackAsync;
            this.userExitAsync = userExitCallbackAsync;
            this.response = responseCallbakAsync;

            this.listener = new TcpListener(new IPEndPoint(address: IPAddress.Any, port: portNum));
            this.handlerDicGroup = new TcpClientHandlerConcurrencyAsyncDicGroup();
        }

        public void Dispose()
        {
            this.listener.Stop();
        }

        async public Task StartAsync()
        {
            this.listener.Start();

            TcpClient client;
            NetworkStream stream;
            TcpClientHandlerConcurrencyAsync handler;
            PacketValue connectPacket;
            int nbytes, receiveDataLength;
            string stageID, userID;
            byte[] receiveHeader, receiveData, decompressData;

            while (true)
            {
                try
                {
                    client = await this.listener.AcceptTcpClientAsync().ConfigureAwait(false);
                    stream = client.GetStream();

                    // 사용자가 접속하면 우선 사용자로부터 입장할 채널 정보와 사용자 정보를 받는다. 
                    // 1. 요청 헤더를 받는다.
                    receiveHeader = new byte[Consts.SIZE_HEADER];
                    nbytes = await stream.ReadAsync(buffer: receiveHeader, offset: 0, count: receiveHeader.Length);

                    // 2. 요청 데이터를 받는다.
                    receiveDataLength = BitConverter.ToInt32(value: receiveHeader, startIndex: 0);
                    receiveData = await TcpUtil.ReceiveDataAsync(networkStream: stream, dataLength: receiveDataLength);

                    await stream.FlushAsync();

                    // 3. 받은 요청은 압축되어 있으므로 푼다.
                    decompressData = await NetUtil.DecompressAsync(data: receiveData);
                    connectPacket = NetUtil.DeserializeObject(data: decompressData) as PacketValue;

                    // protocol에 입장하려는 stage의 id를 넣고, value에 user id를 넣는다.
                    stageID = connectPacket.Protocol;
                    userID = connectPacket.Value.ToString();

                    // 사용자 정보를 이용해서 handler를 추가한다.

                    // 사용자 정보를 이용해서 handler를 만든다.
                    handler = new TcpClientHandlerConcurrencyAsync(client: client, stageID: stageID, userID: userID);
                    handler.Disconnect += DisconnectAsync;
                    handler.Receive += ReceiveAsync;

                    AddStage(handler: handler, stageID: stageID, userID: userID);

                    // hander를 시작한다.
                    await handler.StartAsync();

                    // 사용자가 입장한 정보를 broadcast 한다.
                    IPacket sendPacket = await this.userEnterAsync(stageID, userID);
                    await BroadcastToStageAsync(stageID: stageID, sendPacket: sendPacket);
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        async public Task DisconnectAsync(string stageID, string userID)
        {
            TcpClientHandlerConcurrencyAsyncDic handlerDic;
            TcpClientHandlerConcurrencyAsync handler;

            if (this.handlerDicGroup.TryGetValue(stageID, out handlerDic))
            {
                if (handlerDic.TryGetValue(userID, out handler))
                {
                    handler.Dispose();
                }

                handlerDic.Remove(userID);
            }

            IPacket sendPacket = await this.userExitAsync(stageID, userID);
            await BroadcastToStageAsync(stageID: stageID, sendPacket: sendPacket);
        }

        async public Task MoveStageAsync(string oldStageID, string newStageID, string userID)
        {
            // 1. 기존 stage에서 제거
            TcpClientHandlerConcurrencyAsync handler = RemoveStage(stageID: oldStageID, userID: userID);
            handler.SetStageID(stageID: newStageID);

            // 2. 기존 stage에 퇴장 알림
            IPacket exitPacket = await this.userExitAsync(oldStageID, userID);
            await BroadcastToStageAsync(stageID: oldStageID, sendPacket: exitPacket);

            // 3. 새로운 stage로 입장
            AddStage(handler: handler, stageID: newStageID, userID: userID);

            // 4. 새로운 stage에 입장 알림
            IPacket enterPacket = await this.userEnterAsync(newStageID, userID);
            await BroadcastToStageAsync(stageID: newStageID, sendPacket: enterPacket);
        }

        async public Task BroadcastToServerAsync(IPacket sendPacket)
        {
            foreach (KeyValuePair<string, TcpClientHandlerConcurrencyAsyncDic> kvp in this.handlerDicGroup)
            {
                foreach (KeyValuePair<string, TcpClientHandlerConcurrencyAsync> kvp2 in kvp.Value)
                {
                    await kvp2.Value.SendAsync(packet: sendPacket);
                }
            }
        }

        void AddStage(TcpClientHandlerConcurrencyAsync handler, string stageID, string userID)
        {
            TcpClientHandlerConcurrencyAsyncDic handlerDic;

            if (this.handlerDicGroup.TryGetValue(stageID, out handlerDic))
            {
                handlerDic.Add(userID, handler);

                this.handlerDicGroup[stageID] = handlerDic;
            }
            else
            {
                handlerDic = new TcpClientHandlerConcurrencyAsyncDic();
                handlerDic.Add(userID, handler);

                this.handlerDicGroup.Add(stageID, handlerDic);
            }
        }

        TcpClientHandlerConcurrencyAsync RemoveStage(string stageID, string userID)
        {
            TcpClientHandlerConcurrencyAsyncDic handlerDic;
            TcpClientHandlerConcurrencyAsync handler;

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


        async Task ReceiveAsync(string stageID, IPacket receivePacket)
        {
            // 클라이언트가 발생시킨 요청을 처리하고 그 결과를 채널 내 사용자들에게 모두 broadcasting 한다. 당사자 포함.
            IPacket sendPacket = await this.response(receivePacket);

            // disconnect나 stage 이동시에는 null이 나올 수 있음
            if (sendPacket != null)
            {
                await BroadcastToStageAsync(stageID: stageID, sendPacket: sendPacket);
            }
        }

        async Task BroadcastToStageAsync(string stageID, IPacket sendPacket)
        {
            TcpClientHandlerConcurrencyAsyncDic handlerDic;

            if (this.handlerDicGroup.TryGetValue(stageID, out handlerDic))
            {
                foreach (KeyValuePair<string, TcpClientHandlerConcurrencyAsync> kvp in handlerDic)
                {
                    await kvp.Value.SendAsync(packet: sendPacket);
                }
            }
        }
    }

    public class TcpListenerConcurrencyAsyncs : List<TcpListenerConcurrencyAsync>
    {
        public TcpListenerConcurrencyAsyncs()
        {

        }
    }
}
