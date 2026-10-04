using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;




// Hamburger button that opens a pause overlay. Freezes the game while open.
// Created once in MainMenu and kept alive so it follows the player into every room.
public class PauseMenu : MonoBehaviour
{
    [Header("Panel shown while paused")]
    public GameObject pausePanel;




    [Header("Buttons")]
    public Button hamburgerButton;
    public Button resumeButton;
    public Button logOutButton;
    public Button quitButton;




    [Header("Currency display (optional)")]
    public GameObject currencyHud;




    [Header("Non-gameplay scenes: no hamburger on these")]
    public string loginSceneName = "MainMenu";
    public string gameMenuScene = "Main_Menu_normalno";




    public static PauseMenu Instance { get; private set; }




    bool isPaused;




    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }




        Instance = this;
        DontDestroyOnLoad(gameObject);




        // Hide in Awake, not Start, so neither can flash on the first frame.
        pausePanel.SetActive(false);
        hamburgerButton.gameObject.SetActive(false);
    }




    void Start()
    {
        hamburgerButton.onClick.AddListener(Toggle);
        resumeButton.onClick.AddListener(Close);
        logOutButton.onClick.AddListener(ExitToMenu);
        quitButton.onClick.AddListener(Quit);




        SceneManager.sceneLoaded += OnSceneLoaded;
        ApplyVisibility(SceneManager.GetActiveScene().name);
    }




    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Close();
        ApplyVisibility(scene.name);
    }




    // Nothing here belongs on the login screen itself.
    void ApplyVisibility(string sceneName)
    {
        bool inGame = sceneName != loginSceneName && sceneName != gameMenuScene;
        hamburgerButton.gameObject.SetActive(inGame);
        if (currencyHud != null) currencyHud.SetActive(inGame);
    }




    void Toggle()
    {
        if (isPaused) Close();
        else Open();
    }




    void Open()
    {
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }




    void Close()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }




    // Leaves the room for the game menu. Signing out properly lives there.
    void ExitToMenu()
    {
        // Restore time first: the next scene would otherwise load frozen.
        Time.timeScale = 1f;




        SceneManager.LoadScene(gameMenuScene);
    }




    void Quit()
    {
        Time.timeScale = 1f;




#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }




    // Leaving Play mode while paused would otherwise keep timeScale at 0.
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Time.timeScale = 1f;
    }
}