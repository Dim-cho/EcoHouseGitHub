using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Day and night cycle. You do NOT add this to any scene: it creates itself when the game
/// starts and keeps running between rooms. It only shows (and counts time) in scenes that
/// have an EnergyManager, so the main menu is not affected.
///
/// - Darkens the screen at evening/night (a colored layer under your UI).
/// - Shows "Day N - HH:00" in the top-right corner.
/// - Bed.cs calls TrySleep(): allowed only after the day is half over.
/// </summary>
public class DayNightCycle : MonoBehaviour
{
    // ======================== SETTINGS - change these numbers ========================
    private const float DayLengthSeconds = 90f;      // one full day + night, in real seconds
    private const float MinTimeToSleep = 0.5f;       // 0.5 = the day must be at least half over
    private const bool RestoreEnergyOnNewDay = true;  // energy goes back to full when a new day begins
    private const float FadeSeconds = 1f;            // fade to black, and fade back
    private const float BlackScreenSeconds = 0.5f;   // how long the screen stays black
    private const bool ShowClock = true;
    // =================================================================================

    public static DayNightCycle Instance { get; private set; }

    /// <summary>0 = morning (06:00), 0.5 = evening (18:00), 1 = next morning.</summary>
    public float TimeOfDay { get; private set; }
    public int DayNumber { get; private set; } = 1;
    public bool IsSleeping { get; private set; }
    public bool CanSleep => TimeOfDay >= MinTimeToSleep;

    private Gradient tintGradient;
    private GameObject tintCanvasObject;
    private GameObject hudCanvasObject;
    private Image tintImage;
    private Image fadeImage;
    private Text clockText;
    private Text messageText;
    private Font font;
    private float messageTimer;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null) return;
        new GameObject("DayNightCycle").AddComponent<DayNightCycle>();
    }

    /// <summary>Call when starting a NEW game: back to day 1, morning.</summary>
    public static void ResetDay()
    {
        if (Instance == null) return;
        Instance.TimeOfDay = 0f;
        Instance.DayNumber = 1;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        tintGradient = BuildGradient();
        BuildUI();
    }

    private void Update()
    {
        // Only run in game scenes (they have an EnergyManager)
        bool inGame = EnergyManager.Instance != null;
        SetVisible(inGame);
        if (!inGame) return;

        bool paused = Time.timeScale == 0f || EnergyManager.Instance.IsPaused;

        if (!paused && !IsSleeping)
        {
            TimeOfDay += Time.deltaTime / DayLengthSeconds;
            if (TimeOfDay >= 1f)
            {
                TimeOfDay -= 1f;
                DayNumber++;
                RestoreEnergyForNewDay();
                ShowMessage("Day " + DayNumber + (RestoreEnergyOnNewDay ? " - energy restored" : ""));
            }
        }

        UpdateVisuals();

        if (messageTimer > 0f)
        {
            messageTimer -= Time.unscaledDeltaTime;
            if (messageTimer <= 0f) messageText.text = "";
        }
    }

    // ---------------------------------------------------------------- sleeping

    /// <summary>Called by the bed. Returns true if the player went to sleep.</summary>
    public bool TrySleep()
    {
        if (IsSleeping) return false;

        if (!CanSleep)
        {
            ShowMessage("You can't sleep yet. Wait until " + FormatHour(MinTimeToSleep) + ".");
            return false;
        }

        StartCoroutine(SleepRoutine());
        return true;
    }

    private IEnumerator SleepRoutine()
    {
        IsSleeping = true;
        if (EnergyManager.Instance != null) EnergyManager.Instance.SetPaused(true);
        fadeImage.raycastTarget = true;   // blocks clicks on the UI while sleeping

        yield return Fade(0f, 1f);

        // Screen is black: it becomes the next morning
        TimeOfDay = 0f;
        DayNumber++;
        RestoreEnergyForNewDay();
        UpdateVisuals();

        yield return new WaitForSecondsRealtime(BlackScreenSeconds);
        yield return Fade(1f, 0f);

        fadeImage.raycastTarget = false;
        if (EnergyManager.Instance != null) EnergyManager.Instance.SetPaused(false);
        IsSleeping = false;

        ShowMessage("Good morning! Day " + DayNumber + (RestoreEnergyOnNewDay ? " - energy restored" : ""));
    }

    private void RestoreEnergyForNewDay()
    {
        if (!RestoreEnergyOnNewDay || EnergyManager.Instance == null) return;
        EnergyManager.Instance.Add(EnergyManager.Instance.MaxEnergy);   // back to full
    }

    private IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        while (t < FadeSeconds)
        {
            t += Time.unscaledDeltaTime;
            SetFadeAlpha(Mathf.Lerp(from, to, t / FadeSeconds));
            yield return null;
        }
        SetFadeAlpha(to);
    }

    private void SetFadeAlpha(float a)
    {
        fadeImage.color = new Color(0f, 0f, 0f, a);
    }

    // ---------------------------------------------------------------- visuals

    private void UpdateVisuals()
    {
        tintImage.color = tintGradient.Evaluate(TimeOfDay);

        if (ShowClock) clockText.text = "Day " + DayNumber + "   " + FormatHour(TimeOfDay);
    }

    private void ShowMessage(string text)
    {
        messageText.text = text;
        messageTimer = 2.5f;
    }

    private void SetVisible(bool visible)
    {
        if (tintCanvasObject.activeSelf != visible) tintCanvasObject.SetActive(visible);
        if (hudCanvasObject.activeSelf != visible) hudCanvasObject.SetActive(visible);
    }

    // 0 -> "06:00", 0.5 -> "18:00"
    private static string FormatHour(float timeOfDay)
    {
        int hour = Mathf.FloorToInt((6f + timeOfDay * 24f) % 24f);
        return hour.ToString("00") + ":00";
    }

    // Screen tint over the day: clear morning and afternoon -> sunset -> dark blue night -> clear morning
    private static Gradient BuildGradient()
    {
        Gradient g = new Gradient();
        g.SetKeys(
            new[]
            {
                new GradientColorKey(Color.white,                    0.00f),   // 06:00 morning (no tint)
                new GradientColorKey(Color.white,                    0.50f),   // 18:00 evening starts
                new GradientColorKey(new Color(1.00f, 0.55f, 0.30f), 0.60f),   // sunset
                new GradientColorKey(new Color(0.10f, 0.10f, 0.35f), 0.70f),   // night
                new GradientColorKey(new Color(0.10f, 0.10f, 0.35f), 0.90f),
                new GradientColorKey(Color.white,                    1.00f)    // morning again
            },
            new[]
            {
                new GradientAlphaKey(0.00f, 0.00f),
                new GradientAlphaKey(0.00f, 0.50f),
                new GradientAlphaKey(0.30f, 0.60f),
                new GradientAlphaKey(0.55f, 0.70f),
                new GradientAlphaKey(0.55f, 0.90f),
                new GradientAlphaKey(0.00f, 1.00f)
            });
        return g;
    }

    // ---------------------------------------------------------------- building the UI

    private void BuildUI()
    {
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // 1) Tint layer: BELOW your own UI (sorting order -100), never blocks clicks
        tintCanvasObject = CreateCanvas("TintCanvas", -100);
        tintImage = CreateStretchedImage("Tint", tintCanvasObject.transform);
        tintImage.color = new Color(1f, 1f, 1f, 0f);
        tintImage.raycastTarget = false;

        // 2) HUD layer: clock, messages and the black fade (above your UI)
        hudCanvasObject = CreateCanvas("DayNightHud", 50);

        clockText = CreateText("Clock", hudCanvasObject.transform, 30, TextAnchor.UpperRight);
        RectTransform cr = clockText.rectTransform;
        cr.anchorMin = new Vector2(1f, 1f);
        cr.anchorMax = new Vector2(1f, 1f);
        cr.pivot = new Vector2(1f, 1f);
        cr.sizeDelta = new Vector2(420f, 50f);
        cr.anchoredPosition = new Vector2(-30f, -20f);
        clockText.gameObject.SetActive(ShowClock);

        messageText = CreateText("Message", hudCanvasObject.transform, 34, TextAnchor.UpperCenter);
        RectTransform mr = messageText.rectTransform;
        mr.anchorMin = new Vector2(0.5f, 1f);
        mr.anchorMax = new Vector2(0.5f, 1f);
        mr.pivot = new Vector2(0.5f, 1f);
        mr.sizeDelta = new Vector2(1200f, 60f);
        mr.anchoredPosition = new Vector2(0f, -120f);

        fadeImage = CreateStretchedImage("Fade", hudCanvasObject.transform);
        fadeImage.color = new Color(0f, 0f, 0f, 0f);
        fadeImage.raycastTarget = false;
    }

    private GameObject CreateCanvas(string objectName, int sortingOrder)
    {
        GameObject go = new GameObject(objectName);
        go.transform.SetParent(transform, false);

        Canvas canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortingOrder;

        CanvasScaler scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        go.AddComponent<GraphicRaycaster>();
        return go;
    }

    private static Image CreateStretchedImage(string objectName, Transform parent)
    {
        GameObject go = new GameObject(objectName, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        return go.GetComponent<Image>();
    }

    private Text CreateText(string objectName, Transform parent, int size, TextAnchor alignment)
    {
        GameObject go = new GameObject(objectName, typeof(RectTransform), typeof(Text), typeof(Outline));
        go.transform.SetParent(parent, false);

        Text t = go.GetComponent<Text>();
        t.font = font;
        t.fontSize = size;
        t.fontStyle = FontStyle.Bold;
        t.color = Color.white;
        t.alignment = alignment;
        t.raycastTarget = false;
        return t;
    }
}
