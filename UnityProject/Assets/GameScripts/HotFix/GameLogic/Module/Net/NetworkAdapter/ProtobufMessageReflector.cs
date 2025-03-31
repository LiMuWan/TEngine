using System;
using System.IO;
using Google.Protobuf;
using Proto;
using UnityEngine;

namespace GameLogic
{
    public class ProtobufMessageReflector
    {
        public string GetMessageId<T>() where T : IMessage
        {
            var msg = typeof(T);
            return msg.ToString();
        }

        public string GetMessageId(IMessage msg)
        {
            return msg.ToString();
        }
        
        public byte[] Serialize<T>(T msg) where T : IMessage
        {
            return msg.ToByteArray();
        }

        public object Deserialize(string msgId, byte[] data)
        {
            Type protoType = ProtoDic.GetProtoTypeByName(msgId);
            if (protoType == null)
            {
                throw new Exception($"Protobuf Deserialize error: {msgId}");
            }
            try
            {
                MessageParser messageParser = ProtoDic.GetMessageParser(protoType.TypeHandle);
                object obj = messageParser.ParseFrom(data);
                return obj;
            }
            catch
            {
                Debug.LogError("Deserialize error: " + msgId);
            }
            return null;
        }
        
        public object Deserialize<T>(uint msgID, byte[] data) where T : class, IMessage, new()
        {
            T obj = new T();
            IMessage message = obj.Descriptor.Parser.ParseFrom(data);
            return message as T;
        }
    }
}