using UnityEditor;
using UnityEngine;
using UnityEngine.UI; // 必须根据实际使用的 Graphics 组件引入相应的命名空间
using System.IO;

namespace TEngine.Editor
{
    public class RaycastTargetEditor : EditorWindow
    {
        private string prefabDirectory; // 存储用户输入的目录

        // 创建编辑器窗口
        [MenuItem("Tools/Raycast Target Editor")]
        public static void ShowWindow()
        {
            GetWindow<RaycastTargetEditor>("Raycast Target Editor");
        }

        private void OnGUI()
        {
            GUILayout.Label("选择预制体目录", EditorStyles.boldLabel);
            
            // 输入框和拖拽区域
            prefabDirectory = EditorGUILayout.TextField("目录路径", prefabDirectory);

            if (GUILayout.Button("选择目录"))
            {
                string path = EditorUtility.OpenFolderPanel("选择预制体目录", "Assets", "");
                if (!string.IsNullOrEmpty(path))
                {
                    // 确保路径以 "Assets" 开头
                    if (path.StartsWith(Application.dataPath))
                    {
                        prefabDirectory = "Assets" + path.Substring(Application.dataPath.Length);
                    }
                }
            }

            if (GUILayout.Button("禁用目录下所有预制体的 Raycast Target"))
            {
                DisableRaycastTargetsInDirectory();
            }

            if (GUILayout.Button("禁用选中预制体的 Raycast Target"))
            {
                DisableRaycastTargetsInSelectedPrefab();
            }
        }

        // 禁用指定目录下所有预制体的 Graphic 组件的 raycastTarget
        private void DisableRaycastTargetsInDirectory()
        {
            if (string.IsNullOrEmpty(prefabDirectory) || !Directory.Exists(prefabDirectory))
            {
                Debug.LogWarning("无效的目录路径！");
                return;
            }

            string[] prefabPaths = Directory.GetFiles(prefabDirectory, "*.prefab", SearchOption.AllDirectories);

            if (prefabPaths.Length == 0)
            {
                Debug.LogWarning("该目录下没有找到任何预制体！");
                return;
            }

            foreach (string prefabPath in prefabPaths)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (prefab != null)
                {
                    Debug.Log($"正在处理预制体: {prefab.name}");
                    DisableRaycastTargetsInPrefab(prefab);
                }
            }

            Debug.Log("所有预制体的 Graphic 组件的 raycastTarget 已成功设置为 false！");
        }

        // 禁用选中预制体的 Graphic 组件的 raycastTarget
        private void DisableRaycastTargetsInSelectedPrefab()
        {
            if (Selection.activeGameObject == null)
            {
                Debug.LogError("未选中任何游戏对象！");
                return;
            }

            GameObject selectedObj = Selection.activeGameObject;

            // 获取当前预制体模式下的根对象
            UnityEditor.SceneManagement.PrefabStage stage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
            GameObject rootPrefab = stage != null ? stage.prefabContentsRoot : null;

            if (rootPrefab == null)
            {
                Debug.LogError("当前不在预制体模式中，或者未找到根预制体！");
                return;
            }

            Debug.Log($"预制体根节点: {rootPrefab.name}");

            var graphics = selectedObj.GetComponentsInChildren<Graphic>(true);
            if (graphics.Length == 0)
            {
                Debug.LogWarning("选中的对象及其子对象中没有 Graphic 组件！");
                return;
            }

            foreach (var graphic in graphics)
            {
                Undo.RecordObject(graphic, "Disable Raycast Target in Prefab");
                graphic.raycastTarget = false;
            }

            EditorUtility.SetDirty(rootPrefab);
            PrefabUtility.SavePrefabAsset(rootPrefab);

            Debug.Log("成功将选中预制体的所有 Graphic 组件的 raycastTarget 设置为 false！");
        }

        // 设置指定预制体及其子物体中的所有 Graphic 组件的 raycastTarget 为 false
        private static void DisableRaycastTargetsInPrefab(GameObject prefab)
        {
            var graphics = prefab.GetComponentsInChildren<Graphic>(true);
            foreach (var graphic in graphics)
            {
                Undo.RecordObject(graphic, "Disable Raycast Target in Prefab");
                graphic.raycastTarget = false;
            }

            EditorUtility.SetDirty(prefab);
            PrefabUtility.SavePrefabAsset(prefab);
        }
    }
}
