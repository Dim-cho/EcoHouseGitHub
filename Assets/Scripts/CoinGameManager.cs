using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class CoinGameManager : MonoBehaviour
{
    [Header("Spawners")]
    [Tooltip("Leave this empty to automatically find ALL spawners in the scene!")]
    [SerializeField] private CoinSpawner[] spawners;


    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI centerText;
    [SerializeField] private TextMeshProUGUI timerText;


    [Header("Match Settings")]
    [SerializeField] private float gameDuration = 30f;


    [Header("Reward")]
    [Tooltip("Where the score comes from. Leave empty to find it in the scene.")]
    [SerializeField] private CoinScoreUI scoreUI;
    [Tooltip("Coins per coin collected, paid once when the round ends.")]
    [SerializeField] private int coinsPerPoint = 2;


    [Header("Leaving the game")]
    [Tooltip("The exact name of the scene to go back to when Esc is pressed.")]
    [SerializeField] private string returnSceneName = "Computer";
    [Tooltip("The text shown on the black screen.")]
    [SerializeField] private string quittingMessage = "Quitting...";
    [Tooltip("Drag your TextMeshPro Font Asset here. Leave empty for the default font.")]
    [SerializeField] private TMP_FontAsset quittingFont;
    [SerializeField] private float quittingFontSize = 72f;
    [SerializeField] private Color quittingTextColor = Color.white;
    [Tooltip("Seconds to fade to black.")]
    [SerializeField] private float fadeDuration = 0.6f;
    [Tooltip("Seconds to stay on the black screen before the scene changes.")]
    [SerializeField] private float holdTime = 0.6f;


    [Header("Starting the game")]
    [Tooltip("Shown on the black screen when the game opens. Font, size and color are the same as the quitting text above.")]
    [SerializeField] private string startingMessage = "Starting...";
    [Tooltip("Seconds to stay on the black screen before it fades away.")]
    [SerializeField] private float startHoldTime = 0.6f;
    [Tooltip("Seconds to fade from black into the game.")]
    [SerializeField] private float startFadeDuration = 0.6f;

    private bool isQuitting;
    private bool isIntro;


    private void Awake()
    {
        // If you didn't manually assign spawners, find all of them in the scene automatically!
        if (spawners == null || spawners.Length == 0)
        {
            spawners = FindObjectsByType<CoinSpawner>(FindObjectsSortMode.None);
        }


        if (scoreUI == null) scoreUI = FindFirstObjectByType<CoinScoreUI>();
    }


    private void Start()
    {
        isIntro = true;
        StartCoroutine(IntroThenGame());
    }


    // The 3-2-1 countdown only begins once the "Starting..." screen has faded away
    private IEnumerator IntroThenGame()
    {
        yield return StartCoroutine(IntroRoutine());
        isIntro = false;
        yield return StartCoroutine(GameLoopRoutine());
    }


    private void Update()
    {
        if (EscapePressed()) ReturnToComputer();
    }


    // ---------------------------------------------------------------- leaving

    public void ReturnToComputer()
    {
        if (isQuitting || isIntro) return;

        if (string.IsNullOrEmpty(returnSceneName) || !Application.CanStreamedLevelBeLoaded(returnSceneName))
        {
            Debug.LogError("CoinGameManager: scene '" + returnSceneName + "' can't be loaded. Check the spelling, and make sure it is in " +
                           "File > Build Profiles (Build Settings) > Scene List.", this);
            return;
        }

        isQuitting = true;
        StopAllCoroutines();   // stops the round, so nothing is paid out mid-fade
        StartCoroutine(QuitRoutine());
    }


    // Fades the screen to black with the "Quitting..." text, then goes back to the computer
    private IEnumerator QuitRoutine()
    {
        Time.timeScale = 1f;   // in case the game was paused

        CanvasGroup overlay = CreateOverlay(quittingMessage, 0f);

        float t = 0f;
        float duration = Mathf.Max(0.01f, fadeDuration);
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            overlay.alpha = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / duration));
            yield return null;
        }
        overlay.alpha = 1f;

        if (holdTime > 0f) yield return new WaitForSecondsRealtime(holdTime);

        SceneManager.LoadScene(returnSceneName);
    }


    // Starts on a black screen saying "Starting...", then fades into the game
    private IEnumerator IntroRoutine()
    {
        CanvasGroup overlay = CreateOverlay(startingMessage, 1f);

        if (startHoldTime > 0f) yield return new WaitForSecondsRealtime(startHoldTime);

        float t = 0f;
        float duration = Mathf.Max(0.01f, startFadeDuration);
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            overlay.alpha = Mathf.SmoothStep(1f, 0f, Mathf.Clamp01(t / duration));
            yield return null;
        }

        Destroy(overlay.gameObject);
    }


    // Builds a full-screen black panel with centered text, on top of everything.
    // startAlpha 0 = invisible (for fading in), 1 = fully black (for fading out).
    private CanvasGroup CreateOverlay(string message, float startAlpha)
    {
        GameObject canvasObject = new GameObject("ScreenOverlay", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(CanvasGroup));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        CanvasGroup group = canvasObject.GetComponent<CanvasGroup>();
        group.alpha = startAlpha;
        group.blocksRaycasts = true;

        GameObject black = new GameObject("Black", typeof(RectTransform), typeof(Image));
        black.transform.SetParent(canvasObject.transform, false);
        StretchToParent(black.GetComponent<RectTransform>());
        black.GetComponent<Image>().color = Color.black;

        GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(canvasObject.transform, false);
        StretchToParent(textObject.GetComponent<RectTransform>());

        TextMeshProUGUI label = textObject.GetComponent<TextMeshProUGUI>();
        if (quittingFont != null) label.font = quittingFont;
        label.text = message;
        label.fontSize = quittingFontSize;
        label.color = quittingTextColor;
        label.alignment = TextAlignmentOptions.Center;
        label.raycastTarget = false;

        return group;
    }


    private static void StretchToParent(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }


    private static bool EscapePressed()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        return UnityEngine.InputSystem.Keyboard.current != null &&
               UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame;
#else
        return Input.GetKeyDown(KeyCode.Escape);
#endif
    }


    // ---------------------------------------------------------------- game loop

    private IEnumerator GameLoopRoutine()
    {
        // --- 1. COUNTDOWN PHASE ---
        timerText.gameObject.SetActive(false);
        centerText.gameObject.SetActive(true);


        centerText.text = "3";
        yield return new WaitForSeconds(1f);


        centerText.text = "2";
        yield return new WaitForSeconds(1f);


        centerText.text = "1";
        yield return new WaitForSeconds(1f);


        centerText.text = "GO!";
        yield return new WaitForSeconds(0.5f);


        centerText.gameObject.SetActive(false);


        // --- 2. START ALL SPAWNERS ---
        timerText.gameObject.SetActive(true);


        foreach (CoinSpawner spawner in spawners)
        {
            if (spawner != null)
                spawner.StartSpawning();
        }


        // --- 3. GAMEPLAY TIMER ---
        float timeRemaining = gameDuration;


        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            timerText.text = "Time: " + Mathf.CeilToInt(Mathf.Max(0, timeRemaining)).ToString();
            yield return null;
        }


        // --- 4. STOP ALL SPAWNERS ---
        foreach (CoinSpawner spawner in spawners)
        {
            if (spawner != null)
                spawner.StopSpawning();
        }


        timerText.text = "Time: 0";
        centerText.gameObject.SetActive(true);


        // Paid once at the end, not per coin: picking up a coin shouldn't be a
        // separate server-bound transaction.
        int score = scoreUI != null ? scoreUI.GetScore() : 0;
        int paid = MinigameReward.Pay(score * coinsPerPoint);


        centerText.text = $"TIME'S UP!\n{score} collected · {MinigameReward.Describe(paid)}";
    }
}