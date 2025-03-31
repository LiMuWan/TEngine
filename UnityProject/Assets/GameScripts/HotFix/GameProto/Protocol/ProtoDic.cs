using Google.Protobuf;
using System;
using System.Collections.Generic;

namespace Proto
{
    public class ProtoDic
    {
        private static Dictionary<string, Type> _name2Type = new Dictionary<string, Type>()
        {
        };

        private static readonly Dictionary<RuntimeTypeHandle, MessageParser> Parsers = new Dictionary<RuntimeTypeHandle, MessageParser>()
        {
        };

        public static MessageParser GetMessageParser(RuntimeTypeHandle typeHandle)
        {
            Parsers.TryGetValue(typeHandle, out var messageParser);
            return messageParser;
        }

        public static Type GetProtoTypeByName(string name)
        {
            return _name2Type.GetValueOrDefault(name);
        }


        public static IMessage ParseBytesData(byte[] data, Type type)
        {
            MessageParser messageParse = GetMessageParser(type.TypeHandle);
            return messageParse.ParseFrom(data);
        }


        public static bool ContainName(string name)
        {
            if (_name2Type.ContainsKey(name))
            {
                return true;
            }

            return false;
        }
    }
}