using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.U2D;

public class SceneSplitter : EditorWindow
{
    [MenuItem("Tools/Split Large Scene")]
    static void SplitScene()
    {
        // 1. 获取当前场景的所有根物体
        GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

        // 2. 定义分块规则（如每500单位一个区块）
        float chunkSize = 5f;
        Dictionary<Vector2Int, List<GameObject>> chunks = new Dictionary<Vector2Int, List<GameObject>>();

        // 3. 将物体分配到对应区块
        foreach (GameObject obj in rootObjects)
        {
            Vector2Int chunkCoord = new Vector2Int(
                Mathf.FloorToInt(obj.transform.position.x / chunkSize),
                Mathf.FloorToInt(obj.transform.position.z / chunkSize)
            );
            if (!chunks.ContainsKey(chunkCoord))
            {
                chunks.Add(chunkCoord, new List<GameObject>());
            }
            chunks[chunkCoord].Add(obj);
        }

        // 4. 为每个区块创建子场景
        string basePath = "Assets/Scenes/SubScenes/";
        foreach (var chunk in chunks)
        {
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            foreach (GameObject obj in chunk.Value)
            {
                GameObject newObj = (GameObject)Instantiate(obj, newScene);
                SceneManager.MoveGameObjectToScene(newObj, newScene);
                newObj.name = obj.name;
            }
            EditorSceneManager.SaveScene(newScene, $"{basePath}Chunk_{chunk.Key.x}_{chunk.Key.y}.unity");
            EditorSceneManager.CloseScene(newScene, true);
        }
    }
}
