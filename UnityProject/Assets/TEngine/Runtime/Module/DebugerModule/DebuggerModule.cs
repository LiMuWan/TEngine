using UnityEngine;

namespace TEngine
{
    /// <summary>
    /// 调试器管理器。
    /// </summary>
    internal sealed partial class DebuggerModule : Module, IDebuggerModule, IUpdateModule
    {
        private bool _activeWindow;

        /// <summary>
        /// 初始化调试器管理器的新实例。
        /// </summary>
        public override void OnInit()
        {
            _activeWindow = false;
        }

        /// <summary>
        /// 获取游戏框架模块优先级。
        /// </summary>
        /// <remarks>优先级较高的模块会优先轮询，并且关闭操作会后进行。</remarks>
        public override int Priority => -1;

        /// <summary>
        /// 获取或设置调试器窗口是否激活。
        /// </summary>
        public bool ActiveWindow
        {
            get => _activeWindow;
            set => _activeWindow = value;
        }

        public IDebuggerWindowGroup DebuggerWindowRoot { get; }
        public void RegisterDebuggerWindow(string path, IDebuggerWindow debuggerWindow, params object[] args)
        {
            
        }

        public bool UnregisterDebuggerWindow(string path)
        {
            return false;
        }

        public IDebuggerWindow GetDebuggerWindow(string path)
        {
            throw new System.NotImplementedException();
        }

        public bool SelectDebuggerWindow(string path)
        {
            return false;
        }

        /// <summary>
        /// 调试器管理器轮询。
        /// </summary>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (!_activeWindow)
            {
                return;
            }
        }

        /// <summary>
        /// 关闭并清理调试器管理器。
        /// </summary>
        public override void Shutdown()
        {
            _activeWindow = false;
        }
    }
}