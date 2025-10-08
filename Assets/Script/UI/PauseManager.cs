using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    private GameObject _pasuePanel;
    private bool _isPaused;

    void Awake()
    {
        _pasuePanel = Addressables.LoadAssetAsync<GameObject>("Assets/Prefab/UI/Panel/PausePanel.prefab").WaitForCompletion();
        Transform canvas = GameObject.Find("/UI").transform.Find("Canvas");
        _pasuePanel = Instantiate(_pasuePanel, canvas);
        Transform pauseArea = _pasuePanel.transform.GetChild(0);
        pauseArea.GetChild(0).GetComponent<Button>().onClick.AddListener(Resume);
        pauseArea.GetChild(1).GetComponent<Button>().onClick.AddListener(BackToMainMenu);
        pauseArea.GetChild(2).GetComponent<Button>().onClick.AddListener(QuitGame);
        _pasuePanel.SetActive(false);
        _isPaused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    private void Pause()
    {
        Time.timeScale = 0;
        _pasuePanel.SetActive(true);
        _isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Resume()
    {
        Time.timeScale = 1;
        _pasuePanel.SetActive(false);
        _isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    private async void BackToMainMenu()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        await SceneManager.LoadSceneAsync("MainMenu");
        await SceneManager.UnloadSceneAsync(currentScene);
    }

    private void OnApplicationQuit()
    {
        Transform pauseArea = _pasuePanel.transform.GetChild(0);
        pauseArea.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
        pauseArea.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
        pauseArea.GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
    }
}