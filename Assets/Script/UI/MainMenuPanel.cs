using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuPanel : MonoBehaviour
{
    private string _savePath;
    private string _saveFilePath;

    void Awake()
    {
        _savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Adventure");
        _saveFilePath = Path.Combine(_savePath, "save");
        transform.GetChild(0).GetComponent<Button>().onClick.AddListener(StartNewGame);
        transform.GetChild(1).GetComponent<Button>().onClick.AddListener(LoadGame);
        transform.GetChild(2).GetComponent<Button>().onClick.AddListener(QuitGame);
    }

    private void StartNewGame()
    {
        if (File.Exists(_saveFilePath))
        {
            File.Delete(_saveFilePath);
        }
        SceneManager.LoadScene("Demo");
    }

    private void LoadGame()
    {
        SceneManager.LoadScene("Demo");
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}