namespace GameLogic
{
    public interface IEncoder
    {
        string Encode(string data);
        string Decode(byte[] data);
    }
}