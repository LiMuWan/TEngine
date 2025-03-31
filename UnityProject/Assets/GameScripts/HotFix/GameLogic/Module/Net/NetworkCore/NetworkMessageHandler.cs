using System;
using System.Collections.Generic;
using Google.Protobuf;
using TEngine;

namespace GameLogic
{
    public class NetworkMessageHandler
    {
        private readonly ProtobufMessageReflector _reflector;
        private readonly Dictionary<string, Action<object>> _msg2Callbacks = new Dictionary<string, Action<object>>();
        private Action<string> _onReceiveMessage;
        private Func<string,bool> _onFilterReceiveMessage;
        public NetworkMessageHandler(ProtobufMessageReflector reflector)
        {
            _reflector = reflector;
        }

        public void AddListener(Action<string> onReceiveMessage, Func<string, bool> onFilterReceiveMessage)
        {
            this._onReceiveMessage = onReceiveMessage;
            this._onFilterReceiveMessage = onFilterReceiveMessage;
        }
        
        public void RemoveListener()
        {
            this._onReceiveMessage = null;
            this._onFilterReceiveMessage = null;
        }

        public byte[] Serialize<T>(T message) where T : IMessage
        {
            return _reflector.Serialize(message);
        }

        public object Deserialize(string msgId, byte[] bytes)
        {
            try
            {
                return _reflector.Deserialize(msgId, bytes);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to deserialize message: {msgId}", e);
            }
        }

        public void RegisterMessage<T>(Action<object> callback) where T : IMessage
        {
            string msgId = _reflector.GetMessageId<T>();
            if (_msg2Callbacks.TryGetValue(msgId, out var existingCallbacks))
            {
                existingCallbacks += callback;
                _msg2Callbacks[msgId] = existingCallbacks;
            }
            else
            {
                _msg2Callbacks[msgId] = callback;
            }
        }

        public void UnRegisterMessage<T>(Action<object> callback) where T : IMessage
        {
            string msgId = _reflector.GetMessageId<T>();
            if (_msg2Callbacks.TryGetValue(msgId, out var existingCallbacks))
            {
                existingCallbacks -= callback;
                if (existingCallbacks == null)
                {
                    _msg2Callbacks.Remove(msgId);
                }
                else
                {
                    _msg2Callbacks[msgId] = existingCallbacks;
                }
            }
        }

        public void OnReceiveSocketMessage(string msgId, byte[] bytes)
        {
            // 尝试获取并调用回调
            if (_msg2Callbacks.TryGetValue(msgId, out var callbacks))
            {
                // try
                // {
                    // 反序列化消息
                    var msg = Deserialize(msgId, bytes);
                    // 安全调用回调函数
                    callbacks?.Invoke(msg);
                // }
                // catch (Exception ex)
                // {
                //     Debug.LogError($"Error deserializing or invoking callback for msgId {msgId}: {ex.Message}\nStack Trace: {ex.StackTrace}");
                // }
            }

            try
            {
                if (_onFilterReceiveMessage != null && !_onFilterReceiveMessage.Invoke(msgId) && msgId.Contains("S2C"))
                {
                    // 安全调用接收消息的回调
                    _onReceiveMessage?.Invoke(msgId);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error invoking _onReceiveMessage for msgId {msgId}: {ex.Message}\nStack Trace: {ex.StackTrace}");
            }
        }

    }
}