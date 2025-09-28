using UnityEditor;
using UnityEngine;

public class DevTool
{
    [MenuItem("Tools/Sync Scene Camera to Object")]
    static void SyncSceneCameraToObject()
    {
        GameObject target = Selection.activeGameObject;
        if (target == null)
        {
            Debug.Log("未选中任何对象");
            return;
        }
        else
        {
            SceneView sceneView = SceneView.lastActiveSceneView;
            if (sceneView != null)
            {
                sceneView.AlignViewToObject(target.transform);
            }
        }
    }

    [MenuItem("Tools/Force Reload Domain _F5")]
    static void ForceReload()
    {
        EditorUtility.RequestScriptReload(); // 强制请求脚本重载
    }
}
