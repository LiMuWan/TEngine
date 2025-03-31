using System;
using System.IO;
using System.Net;
using System.Text;
using GameBase;
using TEngine;

namespace GameLogic
{
    public class NetworkMessageProcessor
    {
        private const int PacketHeaderSize = 4 + 4 + 1 + 2; //消息头长度 字节数
        private readonly NetworkMessageHandler _messageHandler;
        private readonly ConnnectClient _socket;
        private readonly MemoryStream _outStream;
        private readonly BinaryWriter _writer;
        private readonly MemoryStream _inStream;
        private readonly BinaryReader _reader;
        private readonly byte[] _msgIdBytes;
        private readonly StringBuilder _msgIdBuilder;
        private readonly byte[] _buffer;

        public NetworkMessageProcessor(NetworkMessageHandler messageHandler, ConnnectClient socket)
        {
            _messageHandler = messageHandler;
            _socket = socket;
            _outStream = new MemoryStream();
            _writer = new BinaryWriter(_outStream);
            _inStream = new MemoryStream();
            _reader = new BinaryReader(_inStream);
            _msgIdBytes = new byte[256]; // 假设最大消息ID长度为256字节
            _msgIdBuilder = new StringBuilder(260); // "Msg." + max message id length
            _buffer = new byte[1024 * 8]; // 8KB buffer, adjust size if necessary
        }

        public void SendPacket(string msgName, byte msgType, byte[] bodyBytes)
        {
            if (_socket == null)
            {
                return;
            }

            Log.Info($"Net SendPacket: {msgName}");

            int bodyLen = bodyBytes.Length;
            int packetLen = bodyLen + PacketHeaderSize;
            
            int sequence = 0; //目前没有实际作用
            byte compress = msgType;
            byte[] msgNameBytes = Encoding.UTF8.GetBytes(msgName);
            short msgNameLen = (short)msgNameBytes.Length;

            if (BitConverter.IsLittleEndian)
            {
                packetLen = IPAddress.HostToNetworkOrder(packetLen);
                sequence = IPAddress.HostToNetworkOrder(sequence);
                msgNameLen = IPAddress.HostToNetworkOrder(msgNameLen);
            }
            
            _outStream.SetLength(0);
            _writer.Write(packetLen); // 总长
            _writer.Write(sequence); // Sequence
            _writer.Write(compress); // Compress
            _writer.Write(msgNameLen); //名字位数，用2个字节
            _writer.Write(msgNameBytes); // 名字
            _writer.Write(bodyBytes);
  
            byte[] buffer = _outStream.ToArray();
            _socket.Send(buffer);
        }

        public void OnReceiveSocketMessage(byte[] receiveData)
        {
            MsgPacket packet;

            _inStream.SetLength(0);
            _inStream.Write(receiveData, 0, receiveData.Length);
            _inStream.Position = 0;

            int packetLen = _reader.ReadInt32();
            int sequence = _reader.ReadInt32();
            packet.Compress = _reader.ReadByte();

            if (BitConverter.IsLittleEndian)
            {
                packetLen = IPAddress.NetworkToHostOrder(packetLen);
                sequence = IPAddress.NetworkToHostOrder(sequence);
            }

            packet.PacketLen = packetLen;
            packet.Sequence = sequence;

            short msgIdLen = _reader.ReadInt16();
            if (BitConverter.IsLittleEndian)
            {
                msgIdLen = IPAddress.NetworkToHostOrder(msgIdLen);
            }

            if (msgIdLen > _msgIdBytes.Length)
            {
                Log.Error($"MsgId length {msgIdLen} exceeds maximum length {_msgIdBytes.Length}");
                return;
            }

            _reader.Read(_msgIdBytes, 0, msgIdLen);
            packet.MsgId = Encoding.UTF8.GetString(_msgIdBytes, 0, msgIdLen);
            Log.Info($"Net receive msgName: {packet.MsgId}");

            int contentSize = packetLen - PacketHeaderSize;
            packet.Content = _reader.ReadBytes(contentSize);

            _msgIdBuilder.Clear();
            _msgIdBuilder.Append("Msg.");
            _msgIdBuilder.Append(packet.MsgId);

            _messageHandler.OnReceiveSocketMessage(_msgIdBuilder.ToString(), packet.Content);
        }
    }
}