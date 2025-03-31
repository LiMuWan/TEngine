using System.Collections.Generic;

namespace GameBase
{
    public struct MsgPacket
    {
        public int PacketLen; //总包长度
        public int Sequence; // 序号
        public byte Compress; //是否压缩 一个字节
        public string MsgId; //消息名字
        public byte[] Content; //包内容
    }
    
    /// <summary>
    /// 可靠发送 TODO
    /// </summary>
    public class ReliableSender
    {
        private readonly Queue<long> msgIdQueue = new Queue<long>();
        private readonly Queue<byte[]> msgQueue = new Queue<byte[]>();

        public ConnnectClient Client;

        private int msgId;
        public int IncrId => ++msgId;

        public void Send(long id, byte[] buffer)
        {
            if (id > 0)
            {
                msgIdQueue.Enqueue(id);
                msgQueue.Enqueue(buffer);
            }

            Client.Send(buffer);
        }

        public void OnAck(long id)
        {
            if (id == msgIdQueue.Peek())
            {
                msgIdQueue.Dequeue();
                msgQueue.Dequeue();
            }
        }

        public void Resend()
        {
            if (msgQueue.Count > 0)
            {
                foreach (byte[] bytes in msgQueue)
                {
                    Client.Send(bytes);
                }
            }
        }
    }
}