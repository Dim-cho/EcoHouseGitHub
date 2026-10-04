using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


// The menu you land on after signing in: Continue / New game / Sign out / Quit.
public class GameMenuUI : MonoBehaviour
{
    [Header("Buttons")]
    public Button continueButton;
    public Button newGameButton;
    public Button signOutButton;
    public Button quitButton;


    [Header("New game confirmation (optional but recommended)")]
    public GameObject confirmPanel;
    public Button confirmYesButton;
    public Button confirmNoButton;


    [Header("Text (optional)")]
    public TMP_Text welcomeText;
    public TMP_Text statusText;


    [Header("Scenes")]
    public string firstRoomScene = "LeftRoom";
    public string loginSceneName = "MainMenu";


    const string LastRoomKey = "ecohouse_last_room";


    void Start()
    {
        continueButton.onClick.AddListener(Continue);
        newGameButton.onClick.AddListener(AskNewGame);
        signOutButton.onClick.AddListener(SignOut);
        quitButton.onClick.AddListener(Quit);


        if (confirmPanel != null)
        {
            confirmPanel.SetActive(false);
            if (confirmYesButton != null) confirmYesButton.onClick.AddListener(ConfirmNewGame);
            if (confirmNoButton != null) confirmNoButton.onClick.AddListener(CancelNewGame);
        }


        if (welcomeText != null && EcoHouseApi.Instance != null)
        {
            welcomeText.text = EcoHouseApi.Instance.IsLoggedIn
                ? $"Signed in as {EcoHouseApi.Instance.Username}"
                : "";
        }


        SetStatus("");
    }


    // Always enabled: for a new player there's nothing saved, so this is just
    // "play" and lands them in the first room. Unlike New game it never resets.
    void Continue()
    {
        var room = PlayerPrefs.GetString(LastRoomKey, firstRoomScene);
        PlayerPrefs.SetString(LastRoomKey, room);
        PlayerPrefs.Save();
        SceneManager.LoadScene(room);
    }


    void AskNewGame()
    {
        // No confirm panel wired? Go straight through rather than dead-ending.
        if (confirmPanel == null)
        {
            ConfirmNewGame();
            return;
        }


        confirmPanel.SetActive(true);
    }


    void CancelNewGame()
    {
        confirmPanel.SetActive(false);
    }


    void ConfirmNewGame()
    {
        if (confirmPanel != null) confirmPanel.SetActive(false);


        SetInteractable(false);
        SetStatus("Starting over...");
        StartCoroutine(ResetThenPlay());
    }


    System.Collections.IEnumerator ResetThenPlay()
    {
        var api = EcoHouseApi.Instance;


        if (api == null || !api.IsLoggedIn)
        {
            // Not signed in: nothing on the server to clear.
            StartRun();
            yield break;
        }


        yield return api.ResetProgress((ok, error) =>
        {
            if (!ok)
            {
                SetInteractable(true);
                SetStatus(error);
                return;
            }


            // Queued deltas would otherwise re-apply the progress we just cleared.
            if (GameSync.Instance != null) GameSync.Instance.DiscardPending();
            if (CurrencyHUD.Instance != null) CurrencyHUD.Instance.SetBalances(0, 0);
            StartRun();
        });
    }


    void StartRun()
    {
        PlayerPrefs.SetString(LastRoomKey, firstRoomScene);
        PlayerPrefs.Save();
        SceneManager.LoadScene(firstRoomScene);
    }


    void SignOut()
    {
        if (EcoHouseApi.Instance != null) EcoHouseApi.Instance.LogOut();
        PlayerPrefs.DeleteKey(LastRoomKey);
        PlayerPrefs.Save();
        SceneManager.LoadScene(loginSceneName);
    }


    void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


    void SetInteractable(bool value)
    {
        continueButton.interactable = value;
        newGameButton.interactable = value;
        signOutButton.interactable = value;
        quitButton.interactable = value;
    }


    void SetStatus(string message)
    {
        if (statusText != null) statusText.text = message ?? "";
    }


    // Rooms call this so Continue returns where the player left off.
    public static void RememberRoom(string sceneName)
    {
        PlayerPrefs.SetString(LastRoomKey, sceneName);
        PlayerPrefs.Save();
    }
}