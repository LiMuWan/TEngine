using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using NativeWebSocket;
using UnityEngine;
using UnityEngine.Networking;
using WebSocket = NativeWebSocket.WebSocket;

namespace GameBase
{
    public class ConnnectClient
    {
        private ConnectStates LastState { get; set; }
        public ConnectStates State { get; private set; }

        private ConnnectConfig _connnectConfig;
        public ConnnectConfig ConnnectConfig => _connnectConfig;
        
        // async cancel token control
        private CancellationTokenSource tokenSource;
        private CancellationToken cancelToken;

        private WebSocket websocket;
        private X509Certificate2 certificate;
        private Action<byte[]> OnMessageCallback;
        private Action OnConnectSuccessHandler;
        private Action<WebSocketCloseCode> OnClosedHandler;
        private Action<string> OnErrorHandler;
        private Dictionary<string, UniTaskCompletionSource<byte[]>> awaitEvents;
       
        /// <summary>
        /// 建立新连接
        /// </summary>
        public static ConnnectClient Conn(ConnnectConfig connnectConfig, Action<byte[]> onMessageCallback, Action onConned,
            Action<WebSocketCloseCode> onClosed, Action<string> onError,X509Certificate2 certificate)
        {
            ConnnectClient connnectClient = new ConnnectClient();
            connnectClient._connnectConfig = connnectConfig;
            connnectClient.tokenSource = new CancellationTokenSource();
            connnectClient.cancelToken = connnectClient.tokenSource.Token;
            connnectClient.State = ConnectStates.Connecting;
            connnectClient.Open(connnectConfig, onMessageCallback, onConned, onClosed, onError,certificate);
            return connnectClient;
        }

        private void Open(ConnnectConfig connnectConfig, Action<byte[]> onMessageCallback, Action onConned,
            Action<WebSocketCloseCode> onClosed, Action<string> onError,X509Certificate2 certificate)
        {
            string url = ConnExtension.AddParams(connnectConfig.URL, connnectConfig.Param);
            this.OnMessageCallback = onMessageCallback;
            this.OnConnectSuccessHandler = onConned;
            this.OnClosedHandler = onClosed;
            this.OnErrorHandler = onError;
            this.certificate = certificate;
            Open(url,certificate).Forget();
        }

        private async UniTask Open(string url,X509Certificate2 certificate)
        {
            string cookieName = "Basic-Info";
            string cookieValue = _connnectConfig.BasicInfo;

            awaitEvents = new Dictionary<string, UniTaskCompletionSource<byte[]>>();
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Cookie", $"{cookieName}={cookieValue}");
            headers.Add("appId", _connnectConfig.AppId);
            websocket = new WebSocket(url, headers);
            websocket.SetPingInterval(TimeSpan.FromMilliseconds(_connnectConfig.PingInterval));
            TaskCompletionSource<bool> connectCompletionSource = new TaskCompletionSource<bool>();

            websocket.OnOpen += OnOpen;
            websocket.OnOpen += () => { connectCompletionSource.TrySetResult(true); };

            websocket.OnClose += OnClosed;
            websocket.OnClose += code => { connectCompletionSource.TrySetResult(false); };

            websocket.OnError += error => { OnErrorHandler?.Invoke(error); };

            websocket.OnMessage += OnMessage;

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                // OnClosed(WebSocketCloseCode.Abnormal);
                Debug.Log("NotReachable");
                State = ConnectStates.NotReachable;
                _connnectConfig?.OnNetworkNotReachableHandler?.Invoke();
            }
            else
            {
                _ = websocket.Connect(this.certificate);
                await connectCompletionSource.Task;
            }
        }
        

        public void DispatchMessageQueue()
        {
            websocket?.DispatchMessageQueue();
        }

        public void Send(byte[] buffer)
        {
            if (State != ConnectStates.Open)
            {
                Debug.LogWarning($"You are trying to send message when connection is not open, state: {State}");
                return;
            }

            websocket?.Send(buffer);
        }

        public async UniTask<byte[]> AwaitSend(string evt, byte[] buffer, float timeout = 5)
        {
            if (State != ConnectStates.Open)
            {
                return null;
            }

            UniTaskCompletionSource<byte[]> source = new UniTaskCompletionSource<byte[]>();
            awaitEvents[evt] = source;

            websocket?.Send(buffer);
            (bool isTimeout, byte[] bytes) = await source.Task.TimeoutWithoutException(TimeSpan.FromSeconds(timeout));
            if (isTimeout)
            {
                awaitEvents.Remove(evt);
            }

            return bytes;
        }

        public void PushAwait(string evt, byte[] bytes)
        {
            if (awaitEvents.TryGetValue(evt, out UniTaskCompletionSource<byte[]> source))
            {
                source.TrySetResult(bytes);
                awaitEvents.Remove(evt);
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            if (State != ConnectStates.Closing && State != ConnectStates.Closed)
            {
                State = ConnectStates.Closing;
                Disable();

                // if (tokenSource != null)
                // {
                //     tokenSource.Cancel();
                //     tokenSource.Dispose();
                //     tokenSource = null;
                // }

                websocket?.Close();
            }
        }

        public void Dispose()
        {
            Disable();
            websocket?.Close();
            if (tokenSource != null)
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
                tokenSource = null;
            }
            _connnectConfig?.ClearEvent();
        }
        
        public void Disable()
        {
            // 需要先移除监听
            websocket.OnOpen -= OnOpen;
            websocket.OnMessage -= OnMessage;
            websocket.OnClose -= OnClosed;
        }

        private void OnOpen()
        {
            if (State == ConnectStates.MuteReconnecting || State == ConnectStates.Reconnecting)
            {
                State = ConnectStates.Open;
                _connnectConfig?.OnReconnectSuccess?.Invoke();
            }
            else
            {
                State = ConnectStates.Open;
                OnConnectSuccessHandler?.Invoke();
            }
        }

        private void OnMessage(byte[] bytes)
        {
            OnMessageCallback?.Invoke(bytes);
        }

        private void OnClosed(WebSocketCloseCode code)
        {
            bool reconnectingMute = State == ConnectStates.MuteReconnecting;
            State = ConnectStates.Closed;
            if (reconnectingMute)
            {
                return;
            }

            // 根据关闭状态判断是否回调错误
            WebSocketCloseStatus? closeStatus = websocket.CloseStatus;
            if (!closeStatus.HasValue || closeStatus.Value != WebSocketCloseStatus.NormalClosure)
            {
                OnErrorHandler?.Invoke(code.ToString());
            }

            OnClosedHandler?.Invoke(code);
        }

        public async UniTask<bool> Reconnect(int maxRetryTimes, bool mute)
        {
            if (State == ConnectStates.NotReachable)
            {
                State = ConnectStates.None;
            }
            if (State == ConnectStates.Open || Reconnecting())
            {
                return true;
            }

            Debug.Log($"Reconnect......{1}");
            
            try
            {
                // maxRetryTimes == -1 时只重连一次
                await OnReconnect(mute);

                float waitTime = _connnectConfig.DefaultWaitTime;
                for (int i = 1; State != ConnectStates.Open && i <= maxRetryTimes; i++)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(waitTime), cancellationToken: cancelToken);
                    waitTime += (i - 1) * _connnectConfig.NextWaitTimeStep;
                    if (State == ConnectStates.NotReachable)
                    {
                        break;
                    }
                    Debug.Log($"Reconnect......{i}");
                    _connnectConfig?.OnReconnecting?.Invoke(i);
                    await OnReconnect(false);
                    if (i == maxRetryTimes)
                    {
                        bool reconnectSucess = State == ConnectStates.Open;
                        if (!reconnectSucess)
                        {
                            _connnectConfig?.OnReconnectFail?.Invoke();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Reconnect failed with exception: {ex.Message}");
                _connnectConfig?.OnReconnectFail?.Invoke();
            }
            finally
            {
                // 确保状态设置为适当的状态
                if (State != ConnectStates.Open)
                {
                    State = ConnectStates.Closed;
                }
            }
            
            return State == ConnectStates.Open;
        }

        private async UniTask OnReconnect(bool mute)
        {
            State = mute ? ConnectStates.MuteReconnecting : ConnectStates.Reconnecting;

            Dictionary<string, string> paramsDict = default;
            if (_connnectConfig.Param != null)
            {
                paramsDict = new Dictionary<string, string>(_connnectConfig.Param);
                paramsDict[ConnnectConfig.ReconnectTag] = ConnnectConfig.ReconnectTagValue;
            }

            string url = ConnExtension.AddParams(_connnectConfig.URL, paramsDict);
            // _connnectConfig?.OnReconnecting?.Invoke(1);
            await Open(url,certificate);
        }

        private bool Reconnecting() => State == ConnectStates.Reconnecting || State == ConnectStates.MuteReconnecting;
    }
}