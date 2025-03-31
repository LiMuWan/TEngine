using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameBase;
using Google.Protobuf;
using NativeWebSocket;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    public class NetworkPeer
    {
        private ConnnectClient _socket;
        private NetworkMessageHandler _messageHandler;
        private NetworkMessageProcessor _messageProcessor;

        private List<string> _sendMsgNameFilter = new List<string>()
        {
            "HeartC2S",
        };
        private List<string> _receiveMsgNameFilter = new List<string>()
        {
            //typeof(HeartS2C).FullName,
        };
        
        public ConnnectClient Socket => _socket;
        private Action<string> OnSendMessage { get; set; }
        private Action<string> OnReceiveMessage { get; set; }

        public NetworkPeer(string name, ProtobufMessageReflector reflector)
        {
            Name = name;
            _messageHandler = new NetworkMessageHandler(reflector);
        }

        public string Name { get; }
        public string Address => _socket.ConnnectConfig.URL;

        public bool Ready => _socket?.State == ConnectStates.Open;

        public async UniTask Start(string address, Action connectSuccess = null, Action<WebSocketCloseCode> connectFail = null,
            Action reconnectSuccess = null, Action reconnectFail = null, Action<int> reconnecting = null, 
            Action networkNotReachable = null,Action<string> onSendMsg = null,Action<string> onReveiveMsg = null)
        {
            var loadCerti = await CertificateLoader.LoadCertificateAsync();
            _socket = ConnnectClient.Conn(new ConnnectConfig()
            {
                URL = address,
                PingInterval = 10 * 1000, // ms
                DefaultWaitTime = 3, // 下次重连时间：初始时间
                NextWaitTimeStep = 5, // 下次重连时间：递增时间
                OnReconnectFail = reconnectFail,
                OnReconnectSuccess = reconnectSuccess,
                OnReconnecting = reconnecting,
                OnNetworkNotReachableHandler = networkNotReachable
            }, OnMessageCallBack, connectSuccess, connectFail, OnError,loadCerti);
            this.OnSendMessage = onSendMsg;
            this.OnReceiveMessage = onReveiveMsg;
            _messageHandler.AddListener(onReveiveMsg, (msgName) =>
            {
                return _receiveMsgNameFilter.Contains(msgName);
            });
            _messageProcessor = new NetworkMessageProcessor(_messageHandler, _socket);
        }

        private void OnError(string error)
        {
            // 连接失败回调
            Log.Warning($"{error}");
        }

        private void OnClosed(WebSocketCloseCode code)
        {
            // 连接关闭回调
        }

        private void OnMessageCallBack(byte[] receiveData)
        {
            _messageProcessor.OnReceiveSocketMessage(receiveData);
        }

        public void SendMsg<T>(T msg, byte type = 0) where T : IMessage
        {
            if (_socket == null)
            {
                return;
            }

            string msgName = msg.GetType().Name;
            byte[] bytes = _messageHandler.Serialize(msg);

            if (!_sendMsgNameFilter.Contains(msgName))
            {
                OnSendMessage?.Invoke(msgName);
            }
           
            _messageProcessor.SendPacket(msgName, type, bytes);
        }

        public void RegisterMessage<T>(Action<object> callback) where T : IMessage
        {
            _messageHandler?.RegisterMessage<T>(callback);
        }

        public void UnRegisterMessage<T>(Action<object> callback) where T : IMessage
        {
            _messageHandler?.UnRegisterMessage<T>(callback);
        }

        public void Stop()
        {
            _socket?.Close();
        }
        
        public void Dispose()
        {
            _socket?.Dispose();
            OnSendMessage = null;
            OnReceiveMessage = null;
            _messageHandler?.RemoveListener();
            _messageHandler = null;
        }
        
        public async UniTask<bool> Reconnect(int times, bool mute)
        {
            return await _socket.Reconnect(times, mute);
        }

        public async UniTask<bool> InsureConnect()
        {
            if (_socket == null)
            {
                return false;
            }

            return await _socket.Reconnect(1, false);
        }
    }
}