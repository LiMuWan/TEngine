using UnityEditor;
using UnityEngine;

namespace TEngine.Editor
{
    public static class LubanTools
    {
        [MenuItem("TEngine/Luban/转表", priority = -100)]
        private static void ZhuanXiaoYi()
        {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            string path = Application.dataPath + "/../../LubanTools/Configs/build_config_to_client.sh";
#elif UNITY_EDITOR_WIN
            string path = Application.dataPath + "/../../LubanTools/Configs/build_config_to_client.bat";
#endif
            Debug.Log($"执行转表：{path}");
            ShellHelper.RunByPath(path);
        }
        
        [MenuItem("TEngine/Luban/生成协议", priority = -101)]
        private static void BuildProto()
        {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            string path = Application.dataPath + "/../../LubanTools/Proto/build_proto_to_client.sh";
#elif UNITY_EDITOR_WIN
            string path = Application.dataPath + "/../../LubanTools/Proto/build_proto_to_client.bat";
#endif
            Debug.Log($"生成proto：{path}");
            ShellHelper.RunByPath(path);
        }
    }
}