using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DevTool
{
    [MenuItem("Tools/Sync Scene Camera to Object _F4")]
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

    [MenuItem("Tools/ReadSaveFile")]
    static void ReadSaveFile()
    {
        string savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Adventure");
        string saveFilePath = Path.Combine(savePath, "save");
        if (File.Exists(saveFilePath))
        {
            string content = File.ReadAllText(saveFilePath);
            content = EncryptData(content);
            Debug.Log(content);
        }
    }
    
    private static string EncryptData(string data)
    {
        char[]encryptionKey = { 'H', 'e', 'l', 'l', 'o' };
        char[] dataChars = data.ToCharArray();
        for (int i = 0; i < data.Length; i++)
        {
            char dataChar = data[i];
            char keyChar = encryptionKey[i % encryptionKey.Length];
            char encryptedChar = (char)(dataChar ^ keyChar);
            dataChars[i] = encryptedChar;
        }
        return new string(dataChars);
    }
}
