
using Google.Protobuf;
using Msg;
using System;
using System.Collections.Generic;

namespace Proto
{
   public class ProtoDic
   {
      private static Dictionary<string,Type> _name2Type = new Dictionary<string,Type>()
      {
           {"Msg.HeartC2S",typeof(Msg.HeartC2S)},
           {"Msg.HeartS2C",typeof(Msg.HeartS2C)},
           {"Msg.KickS2C",typeof(Msg.KickS2C)},
           {"Msg.ResultS2C",typeof(Msg.ResultS2C)},
           {"Msg.GetServerTimeC2S",typeof(Msg.GetServerTimeC2S)},
           {"Msg.GetServerTimeS2C",typeof(Msg.GetServerTimeS2C)},
       };

       private static readonly Dictionary<RuntimeTypeHandle, MessageParser> Parsers = new Dictionary<RuntimeTypeHandle, MessageParser>()
       {
            {typeof(HeartC2S).TypeHandle, Msg.HeartC2S.Parser },
            {typeof(HeartS2C).TypeHandle, Msg.HeartS2C.Parser },
            {typeof(KickS2C).TypeHandle, Msg.KickS2C.Parser },
            {typeof(ResultS2C).TypeHandle, Msg.ResultS2C.Parser },
            {typeof(GetServerTimeC2S).TypeHandle, Msg.GetServerTimeC2S.Parser },
            {typeof(GetServerTimeS2C).TypeHandle, Msg.GetServerTimeS2C.Parser },
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
            if(_name2Type.ContainsKey(name))
            {
                return true;
            }
            return false;
        }

    }
}