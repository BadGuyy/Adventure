using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
public class UserData
{
    public string name;
    public int level;
}

public class LocalConfig
{
    static string _configPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    public static Dictionary<string, UserData> userDataDict = new();
    public static char[] encryptionKey = { 'H', 'e', 'l', 'l', 'o' };
    public static void SaveUserData(UserData userData)
    {
        userDataDict[userData.name] = userData;
        string configFilePath = Path.Combine(_configPath, "User", userData.name + ".json");
        if (!File.Exists(configFilePath))
        {
            Directory.CreateDirectory(Path.Combine(_configPath, "User"));
            File.Create(configFilePath).Close();
        }
        string jsonData = JsonConvert.SerializeObject(userData);
        string encryptedData = EncryptData(jsonData);
        File.WriteAllText(configFilePath, encryptedData);
    }

    public static UserData LoadUserData(string userName)
    {
        if (userDataDict.ContainsKey(userName))
        {
            return userDataDict[userName];
        }
        string configFilePath = Path.Combine(_configPath, "User", userName + ".json");
        if (File.Exists(configFilePath))
        {
            string jsonData = File.ReadAllText(configFilePath);
            string decryptedData = DecryptData(jsonData);
            UserData userData = JsonConvert.DeserializeObject<UserData>(decryptedData);
            return userData;
        }
        else
        {
            return null;
        }
    }

    public static string EncryptData(string data)
    {
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

    public static string DecryptData(string data)
    {
        return EncryptData(data);
    }
}
