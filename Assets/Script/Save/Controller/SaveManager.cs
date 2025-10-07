using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    /// <summary>
    /// 未序列化的存档数据
    /// </summary>
    private SaveData _saveData;
    /// <summary>
    /// 存档数据加密密钥
    /// </summary>
    private char[] _encryptionKey = { 'H', 'e', 'l', 'l', 'o' };
    private string _savePath;
    private string _saveFilePath;

    void Awake()
    {
        Instance = this;
        // 初始化存档路径
        _savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Adventure");
        _saveFilePath = Path.Combine(_savePath, "save");
        // 检查存档文件是否存在，不存在则创建然后加载
        if (!File.Exists(_saveFilePath))
        {
            _saveData = CreateSaveData();
        }
        else
        {
            // 读取存档数据
            LoadSaveData();
        }
    }

    private void LoadSaveData()
    {
        try
        {
            string jsonData = File.ReadAllText(_saveFilePath);
            jsonData = DecryptData(jsonData);
            _saveData = JsonConvert.DeserializeObject<SaveData>(jsonData);
        }
        catch (Exception e)
        {
            Debug.LogError($"存档加载失败：{e.Message}");
        }
    }

    private SaveData CreateSaveData()
    {
        _saveData = new();
        _saveData._playerPositionX = 6.27f;
        _saveData._playerPositionY = 0f;
        _saveData._playerPositionZ = 3.72f;
        _saveData._playerRotationX = 0f;
        _saveData._playerRotationY = 0f;
        _saveData._playerRotationZ = 0f;
        _saveData._playerRotationW = 1f;
        SaveData();
        return _saveData;
    }

    public void SaveData()
    {
        // 检查存档目录是否存在，不存在则创建
        if (!Directory.Exists(_savePath))
        {
            Directory.CreateDirectory(_savePath);
        }

        // 序列化存档数据并加密和写入文件
        string jsonData = JsonConvert.SerializeObject(_saveData);
        jsonData = EncryptData(jsonData);
        File.WriteAllText(_saveFilePath, jsonData);
    }

    public void SaveNPCDialogueData(string npcName, int dialoguePhaseIndex)
    {
        _saveData._npcDialoguePhase[npcName] = dialoguePhaseIndex;
        SaveData();
    }

    public int LoadNPCDialoguePhase(string npcName)
    {
        if (!_saveData._npcDialoguePhase.ContainsKey(npcName))
        {
            return 0;
        }
        else
        {
            return _saveData._npcDialoguePhase[npcName];
        }
    }

    public void SavePlayerTransform(Vector3 position, Quaternion rotation)
    {
        _saveData._playerPositionX = position.x;
        _saveData._playerPositionY = position.y;
        _saveData._playerPositionZ = position.z;
        _saveData._playerRotationX = rotation.x;
        _saveData._playerRotationY = rotation.y;
        _saveData._playerRotationZ = rotation.z;
        _saveData._playerRotationW = rotation.w;
        SaveData();
    }

    public Transform LoadPlayerTransform(Transform playerTransform)
    {
        playerTransform.position = new Vector3(_saveData._playerPositionX, _saveData._playerPositionY, _saveData._playerPositionZ);
        playerTransform.rotation = new Quaternion(_saveData._playerRotationX, _saveData._playerRotationY, _saveData._playerRotationZ, _saveData._playerRotationW);
        return playerTransform;
    }

    public Dictionary<string, int> LoadNPCDialogueData()
    {
        Dictionary<string, int> npcDialogueData = new();

        return npcDialogueData;
    }

    private string EncryptData(string data)
    {
        char[] dataChars = data.ToCharArray();
        for (int i = 0; i < data.Length; i++)
        {
            char dataChar = data[i];
            char keyChar = _encryptionKey[i % _encryptionKey.Length];
            char encryptedChar = (char)(dataChar ^ keyChar);
            dataChars[i] = encryptedChar;
        }
        return new string(dataChars);
    }

    private string DecryptData(string data)
    {
        return EncryptData(data);
    }

#if UNITY_EDITOR
    void OnDestroy()
    {
        Instance = null;
    }
#endif
}