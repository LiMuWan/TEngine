using System;
using Cysharp.Threading.Tasks;
using GameBase;
using QFramework;
using TEngine;
using UnityEngine;

namespace GameLogic
{
    public class SocketModule :Singleton<SocketModule>, IUpdate
    {
        private static NetworkPeer _gamePeer;

        protected override void OnInit()
        {
            if(_gamePeer != null) return;
            var reflector = new ProtobufMessageReflector();
            
            _gamePeer = new NetworkPeer("game", reflector);
            TEngine.Utility.Unity.AddOnApplicationPauseListener(OnApplicationPause);
        }

        private void OnApplicationPause(bool isPause)
        {
            if (isPause)
            {
                //     TypeEventSystem.Global.Send(new EventApplicationPause(){OnResume = false});
            }
            else
            {
                //     _gamePeer.SendMsg(new HeartC2S());
                //     Log.Info("OnResume");
                //     TypeEventSystem.Global.Send(new EventApplicationPause(){OnResume = true});
            }
        }

        protected override void OnRelease()
        {
            TEngine.Utility.Unity.RemoveOnApplicationPauseListener(OnApplicationPause);
            _gamePeer.Dispose();
            _gamePeer = null;
        }

        /// <summary>
        /// 解析错误
        /// </summary>
        /// <param name="obj"></param>
        void OnUnmarshalError(object obj)
        {
            Debug.LogError(obj as string);
        }

        public static async void Reconnect()
        {
            _gamePeer.Stop();
            await _gamePeer.Reconnect(3, true);
        }

        public override void Release()
        {
            base.Release();
            Logout();
        }

        public static void Logout()
        {
            if (null != _gamePeer)
            {
                _gamePeer.Stop();
            }
        }
        
        public static NetworkPeer GamePeer => _gamePeer;
        public void OnUpdate()
        {
            _gamePeer?.Socket?.DispatchMessageQueue();
        }
        
        /// <summary>
        /// 检测网络状态
        /// </summary>
        public static void OnCheckNetReachability()
        {
            NetworkReachability reachability = Application.internetReachability;
            switch (reachability)
            {
                case NetworkReachability.NotReachable:
                    Debug.Log("当前网络不可用");
                    break;
                case NetworkReachability.ReachableViaCarrierDataNetwork:
                    Debug.Log("当前网络为移动网络");
                    break;
                case NetworkReachability.ReachableViaLocalAreaNetwork:
                    Debug.Log("当前网络为wifi");
                    break;
                default:
                    break;
            }
        }
    }
}