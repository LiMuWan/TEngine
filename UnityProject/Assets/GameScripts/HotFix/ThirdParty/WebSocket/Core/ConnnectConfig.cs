using System;
using System.Collections.Generic;

namespace GameBase
{
    public class ConnnectConfig
    {
        // 是否开启调试模式
#if UNITY_EDITOR
        public static bool DebugEnable = true;
#else
        public static bool DebugEnable = false;
#endif

        public const string ReconnectTag = "reconnect"; // 重连标记
        public const string ReconnectTagValue = "1"; // 重连标记

        public string URL { get; set; }
        public string AppId { get; set; }
        public string BasicInfo { get; set; }
        public int PingInterval { get; set; } = 10 * 1000; // ms
        public float DefaultWaitTime { get; set; } = 3; // 下次重连时间：初始时间
        public float NextWaitTimeStep { get; set; } = 5; // 下次重连时间：递增时间
        public Dictionary<string, string> Param { get; set; }
        
        public Action OnReconnectFail;
        public Action<int> OnReconnecting;
        public Action OnNetworkNotReachableHandler;
        public Action OnReconnectSuccess;

        public void ClearEvent()
        {
            OnReconnectFail = null;
            OnReconnecting = null;
            OnNetworkNotReachableHandler = null;
            OnReconnectSuccess = null;
        }
    }
}