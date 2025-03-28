using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace TEngine
{
    /// <summary>
    /// 调试器模块。
    /// </summary>
    [DisallowMultipleComponent]
    public sealed partial class Debugger : MonoBehaviour
    {
        private static Debugger _instance;

        public static Debugger Instance => _instance;
        
        /// <summary>
        /// 默认调试器窗口缩放比例。
        /// </summary>
        internal static readonly float DefaultWindowScale = 1.5f;

        private static TextEditor s_TextEditor = null;
        private IDebuggerModule _debuggerModule = null;

        [SerializeField]
        private DebuggerActiveWindowType activeWindow = DebuggerActiveWindowType.AlwaysOpen;

        public DebuggerActiveWindowType ActiveWindowType => activeWindow;

        [SerializeField]
        private bool _showFullWindow = false;
        
        private FpsCounter _fpsCounter = null;

        /// <summary>
        /// 获取或设置调试器窗口是否激活。
        /// </summary>
        public bool ActiveWindow
        {
            get => _debuggerModule.ActiveWindow;
            set
            {
                _debuggerModule.ActiveWindow = value;
                enabled = value;
            }
        }

        /// <summary>
        /// 获取或设置是否显示完整调试器界面。
        /// </summary>
        public bool ShowFullWindow
        {
            get => _showFullWindow;
            set
            {
                if (_eventSystem != null)
                {
                    _eventSystem.SetActive(!value);
                }

                _showFullWindow = value;
            }
        }
        

        private GameObject _eventSystem;

        /// <summary>
        /// 游戏框架模块初始化。
        /// </summary>
        void Awake()
        {
            _instance = this;
            s_TextEditor = new TextEditor();
            _instance.gameObject.name = $"[{nameof(Debugger)}]";
            _eventSystem = GameObject.Find("UIRoot/EventSystem");
        }

        private void OnDestroy()
        {
            PlayerPrefs.Save();
        }

        private void Initialize()
        {
            _debuggerModule = ModuleSystem.GetModule<IDebuggerModule>();
            if (_debuggerModule == null)
            {
                Log.Fatal("Debugger manager is invalid.");
                return;
            }
        }

        private void Start()
        {
            Initialize();
            switch (activeWindow)
            {
                case DebuggerActiveWindowType.AlwaysOpen:
                    ActiveWindow = true;
                    break;

                case DebuggerActiveWindowType.OnlyOpenWhenDevelopment:
                    ActiveWindow = Debug.isDebugBuild;
                    break;

                case DebuggerActiveWindowType.OnlyOpenInEditor:
                    ActiveWindow = Application.isEditor;
                    break;

                default:
                    ActiveWindow = false;
                    break;
            }

            if (ActiveWindow)
            {
                SRDebug.Init();
            }
        }
        

        /// <summary>
        /// 注册调试器窗口。
        /// </summary>
        /// <param name="path">调试器窗口路径。</param>
        /// <param name="debuggerWindow">要注册的调试器窗口。</param>
        /// <param name="args">初始化调试器窗口参数。</param>
        public void RegisterDebuggerWindow(string path, IDebuggerWindow debuggerWindow, params object[] args)
        {
            _debuggerModule.RegisterDebuggerWindow(path, debuggerWindow, args);
        }

        /// <summary>
        /// 解除注册调试器窗口。
        /// </summary>
        /// <param name="path">调试器窗口路径。</param>
        /// <returns>是否解除注册调试器窗口成功。</returns>
        public bool UnregisterDebuggerWindow(string path)
        {
            return _debuggerModule.UnregisterDebuggerWindow(path);
        }

        /// <summary>
        /// 获取调试器窗口。
        /// </summary>
        /// <param name="path">调试器窗口路径。</param>
        /// <returns>要获取的调试器窗口。</returns>
        public IDebuggerWindow GetDebuggerWindow(string path)
        {
            return _debuggerModule.GetDebuggerWindow(path);
        }
        

        private static void CopyToClipboard(string content)
        {
            s_TextEditor.text = content;
            s_TextEditor.OnFocus();
            s_TextEditor.Copy();
            s_TextEditor.text = string.Empty;
        }
    }
}