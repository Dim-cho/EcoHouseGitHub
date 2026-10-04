using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Put this on ONE empty GameObject in the "Computer" scene. It builds the whole screen by itself:
/// a pixel-art monitor, a desktop with two game icons, and a power button to leave.
/// Clicking an icon loads that game's scene. Esc or the power button goes back to the room.
/// </summary>
public class ComputerScreen : MonoBehaviour
{
    /// <summary>Set by Computer.cs: the room to go back to when the computer is closed.</summary>
    public static string ReturnScene = "";

    // Sizes of the pixel art (monitor_frame.png and desktop_bg.png)
    private const float FrameW = 320f;
    private const float FrameH = 180f;
    private const float ScreenX = 32f;    // where the desktop sits inside the monitor frame
    private const float ScreenY = 14f;

    [Header("Sprites (drag them in)")]
    [SerializeField] private Sprite monitorFrame;
    [SerializeField] private Sprite desktopBackground;
    [SerializeField] private Sprite game1Icon;
    [SerializeField] private Sprite game2Icon;
    [SerializeField] private Sprite powerIcon;

    [Header("Game 1")]
    [SerializeField] private string game1Label = "Game 1";
    [Tooltip("The exact name of the scene file of game 1.")]
    [SerializeField] private string game1Scene = "Game 1";

    [Header("Game 2")]
    [SerializeField] private string game2Label = "Game 2";
    [Tooltip("The exact name of the scene file of game 2.")]
    [SerializeField] private string game2Scene = "Game 2";

    [Header("Leaving the computer")]
    [Tooltip("Only used when this scene is opened without clicking the computer in a room (for testing).")]
    [SerializeField] private string defaultReturnScene = "LeftRoom";

    [Header("Starting and leaving screens")]
    [Tooltip("Black screen shown when the computer opens.")]
    [SerializeField] private string startingMessage = "Starting...";
    [Tooltip("Black screen shown when leaving the computer (Esc or the power button).")]
    [SerializeField] private string quittingMessage = "Quitting...";
    [Tooltip("Drag your TextMeshPro Font Asset here (use the same one as in the games). Leave empty for the default font.")]
    [SerializeField] private TMP_FontAsset screenFont;
    [SerializeField] private float screenFontSize = 72f;
    [SerializeField] private Color screenTextColor = Color.white;
    [Tooltip("Seconds to stay on the black screen before it fades away.")]
    [SerializeField] private float startHoldTime = 0.6f;
    [Tooltip("Seconds to fade from black into the computer.")]
    [SerializeField] private float startFadeDuration = 0.6f;
    [Tooltip("Seconds to fade to black when leaving.")]
    [SerializeField] private float quitFadeDuration = 0.6f;
    [Tooltip("Seconds to stay on the black screen before the scene changes.")]
    [SerializeField] private float quitHoldTime = 0.6f;

    [Header("Colors")]
    [SerializeField] private Color roomColor = new Color(0.13f, 0.11f, 0.17f);
    [SerializeField] private Color labelColor = Color.white;

    private bool isBusy;   // true during the "Starting..." / "Quitting..." screens

    private void Awake()
    {
        EnsureEventSystem();
        BuildUI();

        isBusy = true;
        StartCoroutine(IntroRoutine());
    }

    private void Update()
    {
        if (isBusy) return;
        if (EscapePressed()) CloseComputer();
    }

    // ---------------------------------------------------------------- actions

    // Opens another scene right away (used by the game icons)
    private void OpenScene(string sceneName)
    {
        if (isBusy) return;
        if (!CanLoad(sceneName)) return;

        SceneManager.LoadScene(sceneName);
    }

    // Leaves the computer: fades to black with "Quitting...", then loads the room
    private void CloseComputer()
    {
        if (isBusy) return;

        string target = string.IsNullOrEmpty(ReturnScene) ? defaultReturnScene : ReturnScene;
        if (!CanLoad(target)) return;

        isBusy = true;
        StartCoroutine(QuitRoutine(target));
    }

    private bool CanLoad(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("ComputerScreen: no scene name is set for this icon.", this);
            return false;
        }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError("ComputerScreen: scene '" + sceneName + "' can't be loaded. Check the spelling, and make sure it is in " +
                           "File > Build Profiles (Build Settings) > Scene List.", this);
            return false;
        }

        return true;
    }

    // ---------------------------------------------------------------- starting / leaving screens

    // Starts on a black screen saying "Starting...", then fades into the computer
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
        isBusy = false;
    }

    // Fades to black with "Quitting...", then loads the scene
    private IEnumerator QuitRoutine(string sceneName)
    {
        CanvasGroup overlay = CreateOverlay(quittingMessage, 0f);

        float t = 0f;
        float duration = Mathf.Max(0.01f, quitFadeDuration);
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            overlay.alpha = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t / duration));
            yield return null;
        }
        overlay.alpha = 1f;

        if (quitHoldTime > 0f) yield return new WaitForSecondsRealtime(quitHoldTime);

        SceneManager.LoadScene(sceneName);
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
        group.blocksRaycasts = true;   // no clicks on the desktop while the black screen is up

        GameObject black = new GameObject("Black", typeof(RectTransform), typeof(Image));
        black.transform.SetParent(canvasObject.transform, false);
        Stretch(black.GetComponent<RectTransform>());
        black.GetComponent<Image>().color = Color.black;

        GameObject textObject = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(canvasObject.transform, false);
        Stretch(textObject.GetComponent<RectTransform>());

        TextMeshProUGUI label = textObject.GetComponent<TextMeshProUGUI>();
        if (screenFont != null) label.font = screenFont;
        label.text = message;
        label.fontSize = screenFontSize;
        label.color = screenTextColor;
        label.alignment = TextAlignmentOptions.Center;
        label.raycastTarget = false;

        return group;
    }

    // ---------------------------------------------------------------- building the screen

    private void BuildUI()
    {
        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject canvasObject = new GameObject("ComputerCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();

        // Dark room behind the monitor
        Image background = CreateImage("Room", canvasObject.transform);
        Stretch(background.rectTransform);
        background.color = roomColor;

        // The monitor keeps its 16:9 shape on every screen size
        GameObject box = new GameObject("Monitor", typeof(RectTransform), typeof(AspectRatioFitter));
        box.transform.SetParent(canvasObject.transform, false);
        Stretch(box.GetComponent<RectTransform>());

        AspectRatioFitter fitter = box.GetComponent<AspectRatioFitter>();
        fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
        fitter.aspectRatio = FrameW / FrameH;

        // Monitor frame at the BACK, so the desktop and icons are always drawn on top of it
        Image frame = CreateImage("Frame", box.transform);
        Stretch(frame.rectTransform);
        frame.sprite = monitorFrame;
        frame.type = Image.Type.Simple;
        frame.enabled = monitorFrame != null;

        // Desktop wallpaper (inside the monitor)
        Image desktop = CreateImage("Desktop", box.transform);
        SetNativeRect(desktop.rectTransform, ScreenX, ScreenY, 256f, 128f);
        if (desktopBackground != null) desktop.sprite = desktopBackground;
        else desktop.color = new Color(0.45f, 0.7f, 0.9f);

        // The two game icons
        CreateIconButton(box.transform, font, game1Icon, game1Label, game1Scene, ScreenX + 16f, ScreenY + 12f);
        CreateIconButton(box.transform, font, game2Icon, game2Label, game2Scene, ScreenX + 16f, ScreenY + 64f);

        // Power button in the taskbar: goes back to the room
        Button power = CreateImageButton("Power", box.transform, powerIcon, ScreenX + 238f, ScreenY + 112f, 16f);
        power.onClick.AddListener(CloseComputer);

        // Warn in the Console if a sprite has the wrong size (usually the wrong sprite was dragged in)
        CheckSpriteSize(monitorFrame, "Monitor Frame", FrameW, FrameH);
        CheckSpriteSize(desktopBackground, "Desktop Background", 256f, 128f);
    }

    private void CheckSpriteSize(Sprite sprite, string fieldName, float expectedW, float expectedH)
    {
        if (sprite == null)
        {
            Debug.LogWarning("ComputerScreen: '" + fieldName + "' is empty.", this);
            return;
        }

        if (Mathf.Abs(sprite.rect.width - expectedW) > 0.1f || Mathf.Abs(sprite.rect.height - expectedH) > 0.1f)
        {
            Debug.LogWarning("ComputerScreen: '" + fieldName + "' is " + sprite.rect.width + "x" + sprite.rect.height +
                             " (sprite '" + sprite.name + "') but should be " + expectedW + "x" + expectedH +
                             ". Check you assigned the right sprite.", this);
        }
    }

    private void CreateIconButton(Transform parent, Font font, Sprite icon, string label, string sceneName, float x, float y)
    {
        Button button = CreateImageButton(label, parent, icon, x, y, 32f);
        string target = sceneName;
        button.onClick.AddListener(() => OpenScene(target));

        GameObject textObject = new GameObject(label + " Label", typeof(RectTransform), typeof(Text), typeof(Outline));
        textObject.transform.SetParent(parent, false);
        SetNativeRect(textObject.GetComponent<RectTransform>(), x - 8f, y + 34f, 48f, 11f);

        Text text = textObject.GetComponent<Text>();
        text.font = font;
        text.text = label;
        text.color = labelColor;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.UpperCenter;
        text.resizeTextForBestFit = true;
        text.resizeTextMinSize = 6;
        text.resizeTextMaxSize = 60;
        text.raycastTarget = false;
    }

    private Button CreateImageButton(string objectName, Transform parent, Sprite sprite, float x, float y, float size)
    {
        Image image = CreateImage(objectName, parent);
        SetNativeRect(image.rectTransform, x, y, size, size);
        image.raycastTarget = true;

        if (sprite != null) image.sprite = sprite;
        else image.color = new Color(0.55f, 0.55f, 0.6f);

        Button button = image.gameObject.AddComponent<Button>();
        button.targetGraphic = image;

        // Slightly darker on hover, like the appliances in the rooms
        ColorBlock colors = ColorBlock.defaultColorBlock;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.8f, 0.8f, 0.8f);
        colors.pressedColor = new Color(0.6f, 0.6f, 0.6f);
        colors.selectedColor = Color.white;
        colors.fadeDuration = 0.05f;
        button.colors = colors;

        return button;
    }

    private static Image CreateImage(string objectName, Transform parent)
    {
        GameObject go = new GameObject(objectName, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(parent, false);

        Image image = go.GetComponent<Image>();
        image.raycastTarget = false;
        return image;
    }

    private static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    // Places something using the pixel coordinates of the 320x180 monitor picture
    private static void SetNativeRect(RectTransform rt, float x, float y, float w, float h)
    {
        rt.anchorMin = new Vector2(x / FrameW, 1f - (y + h) / FrameH);
        rt.anchorMax = new Vector2((x + w) / FrameW, 1f - y / FrameH);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    // ---------------------------------------------------------------- helpers

    // Buttons need an EventSystem. A new scene has none, so make one if needed.
    private static void EnsureEventSystem()
    {
        if (EventSystem.current != null) return;

        GameObject es = new GameObject("EventSystem", typeof(EventSystem));
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
        es.AddComponent<StandaloneInputModule>();
#endif
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
}