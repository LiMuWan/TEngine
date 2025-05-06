using UnityEngine;
using UnityEditor;

public class CopyPathUtility : MonoBehaviour
{
    [MenuItem("GameObject/Copy Path", false, 0)]
    private static void CopyPath(MenuCommand menuCommand)
    {
        // 获取选中的游戏对象
        GameObject selectedObject = Selection.activeGameObject;
        if (selectedObject == null)
        {
            Debug.LogWarning("No game object selected.");
            return;
        }

        // 获取选中对象的路径
        string path = GetHierarchyPath(selectedObject.transform);

        // 将路径复制到剪贴板
        EditorGUIUtility.systemCopyBuffer = path;

        Debug.Log($"Path copied to clipboard: {path}");
    }

    private static string GetHierarchyPath(Transform transform)
    {
        // 获取根节点
        Transform root = transform.root;
        
        // 构建路径
        string path = transform.name;
        while (transform.parent != null && transform.parent != root)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }
        
        return path;
    }

    // 验证菜单项是否可用
    [MenuItem("GameObject/Copy Path", true)]
    private static bool ValidateCopyPath(MenuCommand menuCommand)
    {
        // 仅在有选中对象时启用菜单项
        return Selection.activeGameObject != null;
    }
}