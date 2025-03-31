namespace GameBase
{
    public enum ConnectStates
    {
        None = 0,
        Connecting = 1,
        Open = 2,
        Closing = 3,
        Closed = 4,
        Reconnecting = 5,
        MuteReconnecting = 6,
        NotReachable = 7,
    };
}