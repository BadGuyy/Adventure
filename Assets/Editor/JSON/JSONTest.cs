using UnityEngine;
using UnityEditor;

public class JSONTest
{
    [MenuItem("Tools/Save JSON")]
    public static void SaveJSON()
    {
        for (int i = 0; i < 3; i++)
        {
            UserData userData = new UserData();
            userData.name = "User" + i;
            userData.level = i;
            LocalConfig.SaveUserData(userData);
        }
    }

    [MenuItem("Tools/Load JSON")]
    public static void LoadJSON()
    {
        for (int i = 0; i < 3; i++)
        {
            UserData userData = LocalConfig.LoadUserData("USer" + i);
            if (userData!= null)
            {
                Debug.Log(userData.name + " : " + userData.level);
            }
        }
    }
}