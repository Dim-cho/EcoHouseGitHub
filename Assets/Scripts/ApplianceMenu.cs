using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Shows the small menu when the mouse hovers an Appliance.
/// You do NOT have to add this to any scene. It creates itself (and its UI) automatically
/// when the game starts, and stays alive between scenes.
///
/// Layout (top to bottom): ON/OFF button, name, energy use, price, upgrade button.
/// </summary>
public class ApplianceMenu : MonoBehaviour
{
    private const float PanelWidth = 280f;
    private const float HideDelay = 0.15f;   // lets the mouse travel from the appliance to the menu

    private static ApplianceMenu instance;

    private Canvas canvas;
    private GameObject panel;
    private RectTransform panelRect;
    private Font font;

    private Button onOffButton;
    private Image onOffImage;
    private Text onOffLabel;

    private Text nameText;
    private Text energyText;
    private Text priceText;

    private Button upgradeButton;
    private Image upgradeImage;
    private Text upgradeLabel;

    private Appliance current;
    private float hideTimer;
    private string message;
    private float messageTimer;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (instance != null) return;
        new GameObject("ApplianceMenu").AddComponent<ApplianceMenu>();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        BuildUI();
    }

    private void Update()
    {
        // The appliance was destroyed (scene changed)
        if (current == null && panel.activeSelf) Hide();

        bool paused = Time.timeScale == 0f ||
                      (EnergyManager.Instance != null && EnergyManager.Instance.IsPaused);
        Camera cam = Camera.main;

        if (paused || cam == null)
        {
            Hide();
            return;
        }

        Vector2 mouse = GetMousePosition();

        // Mouse is on the menu: keep it open
        bool overMenu = panel.activeSelf &&
                        RectTransformUtility.RectangleContainsScreenPoint(panelRect, mouse, null);
        if (overMenu)
        {
            hideTimer = 0f;
            TickMessage();
            return;
        }

        Appliance under = FindApplianceAt(cam.ScreenToWorldPoint(mouse));

        if (under != null)
        {
            hideTimer = 0f;
            if (under != current) Show(under);
        }
        else if (current != null)
        {
            hideTimer += Time.unscaledDeltaTime;
            if (hideTimer >= HideDelay) Hide();
        }

        TickMessage();
    }

    // ---------------------------------------------------------------- show / hide

    private void Show(Appliance appliance)
    {
        if (current != null) current.SetHovered(false);

        current = appliance;
        current.SetHovered(true);

        message = null;
        messageTimer = 0f;

        panel.SetActive(true);
        EnsureEventSystem();
        Refresh();
        Position();
    }

    private void Hide()
    {
        if (current != null) current.SetHovered(false);
        current = null;

        if (panel != null && panel.activeSelf) panel.SetActive(false);
    }

    private void TickMessage()
    {
        if (messageTimer <= 0f) return;

        messageTimer -= Time.unscaledDeltaTime;
        if (messageTimer <= 0f) Refresh();
    }

    // ---------------------------------------------------------------- content

    private void Refresh()
    {
        if (current == null) return;

        onOffLabel.text = current.IsOn ? "ON" : "OFF";
        onOffImage.color = current.IsOn ? new Color(0.25f, 0.65f, 0.35f) : new Color(0.75f, 0.3f, 0.3f);

        nameText.text = current.ApplianceName;

        string usage = FormatNumber(current.EnergyCost) + " / " + FormatNumber(current.Interval) + " s";
        energyText.text = current.IsOn ? "Energy: " + usage : "Energy: 0 (switched off)";

        priceText.text = "Price: " + current.Price;

        upgradeButton.gameObject.SetActive(current.HasUpgrade);
        if (current.HasUpgrade)
        {
            if (current.IsUpgraded)
            {
                upgradeLabel.text = "Fully upgraded";
                upgradeButton.interactable = false;
                upgradeImage.color = new Color(0.4f, 0.4f, 0.4f);
            }
            else
            {
                upgradeLabel.text = messageTimer > 0f
                    ? message
                    : "Upgrade to " + current.UpgradeName + " (" + current.UpgradePrice + ")";
                upgradeButton.interactable = true;
                upgradeImage.color = new Color(0.25f, 0.45f, 0.8f);
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect);
    }

    // Puts the menu right next to the appliance (to the right, or to the left if no room).
    private void Position()
    {
        Camera cam = Camera.main;
        if (cam == null || current == null) return;

        LayoutRebuilder.ForceRebuildLayoutImmediate(panelRect);

        Bounds b = current.WorldBounds;
        Vector2 topRight = cam.WorldToScreenPoint(new Vector3(b.max.x, b.max.y, b.center.z));
        Vector2 topLeft = cam.WorldToScreenPoint(new Vector3(b.min.x, b.max.y, b.center.z));

        float scale = canvas.scaleFactor;
        float w = panelRect.rect.width * scale;
        float h = panelRect.rect.height * scale;

        float x = topRight.x;
        if (x + w > Screen.width) x = topLeft.x - w;
        x = Mathf.Clamp(x, 0f, Mathf.Max(0f, Screen.width - w));

        float y = Mathf.Clamp(topRight.y, Mathf.Min(h, Screen.height), Screen.height);

        panelRect.anchoredPosition = new Vector2(x, y) / scale;
    }

    // ---------------------------------------------------------------- buttons

    private void OnOnOffClicked()
    {
        if (current == null) return;

        current.Toggle();
        Refresh();
    }

    private void OnUpgradeClicked()
    {
        if (current == null || !current.CanUpgrade) return;

        if (!current.CanAffordUpgrade)
        {
            message = "Not enough coins!";
            messageTimer = 1.5f;
            Refresh();
            return;
        }

        current.TryUpgrade();
        Refresh();
    }

    // ---------------------------------------------------------------- building the UI

    private void BuildUI()
    {
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject canvasObject = new GameObject("ApplianceMenuCanvas");
        canvasObject.transform.SetParent(transform, false);

        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();

        panel = new GameObject("Panel", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(canvasObject.transform, false);

        panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.zero;
        panelRect.pivot = new Vector2(0f, 1f);
        panelRect.sizeDelta = new Vector2(PanelWidth, 0f);

        panel.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.13f, 0.95f);

        VerticalLayoutGroup layout = panel.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(12, 12, 12, 12);
        layout.spacing = 8f;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        ContentSizeFitter fitter = panel.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // Top to bottom
        onOffButton = CreateButton("OnOffButton", 40f, out onOffImage, out onOffLabel);
        nameText = CreateText("Name", 24, FontStyle.Bold, 30f);
        energyText = CreateText("Energy", 18, FontStyle.Normal, 24f);
        priceText = CreateText("Price", 18, FontStyle.Normal, 24f);
        upgradeButton = CreateButton("UpgradeButton", 48f, out upgradeImage, out upgradeLabel);

        onOffButton.onClick.AddListener(OnOnOffClicked);
        upgradeButton.onClick.AddListener(OnUpgradeClicked);

        panel.SetActive(false);
    }

    private Text CreateText(string objectName, int size, FontStyle style, float height)
    {
        GameObject go = new GameObject(objectName, typeof(RectTransform), typeof(Text), typeof(LayoutElement));
        go.transform.SetParent(panel.transform, false);

        go.GetComponent<LayoutElement>().preferredHeight = height;

        Text t = go.GetComponent<Text>();
        t.font = font;
        t.fontSize = size;
        t.fontStyle = style;
        t.color = Color.white;
        t.alignment = TextAnchor.MiddleCenter;
        t.raycastTarget = false;
        return t;
    }

    private Button CreateButton(string objectName, float height, out Image image, out Text label)
    {
        GameObject go = new GameObject(objectName,
            typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
        go.transform.SetParent(panel.transform, false);

        go.GetComponent<LayoutElement>().preferredHeight = height;

        image = go.GetComponent<Image>();
        image.color = Color.gray;

        Button button = go.GetComponent<Button>();
        button.targetGraphic = image;

        GameObject labelObject = new GameObject("Label", typeof(RectTransform), typeof(Text));
        labelObject.transform.SetParent(go.transform, false);

        RectTransform lr = labelObject.GetComponent<RectTransform>();
        lr.anchorMin = Vector2.zero;
        lr.anchorMax = Vector2.one;
        lr.offsetMin = new Vector2(6f, 2f);
        lr.offsetMax = new Vector2(-6f, -2f);

        label = labelObject.GetComponent<Text>();
        label.font = font;
        label.fontSize = 18;
        label.fontStyle = FontStyle.Bold;
        label.color = Color.white;
        label.alignment = TextAnchor.MiddleCenter;
        label.raycastTarget = false;

        return button;
    }

    // ---------------------------------------------------------------- helpers

    private static string FormatNumber(float value)
    {
        return value.ToString("0.##");
    }

    private static Appliance FindApplianceAt(Vector3 world)
    {
        Collider2D[] hits = Physics2D.OverlapPointAll(world);

        foreach (Collider2D hit in hits)
        {
            Appliance a = hit.GetComponent<Appliance>();
            if (a != null) return a;
        }
        return null;
    }

    // Buttons need an EventSystem. Your scenes already have one; this is only a safety net.
    private void EnsureEventSystem()
    {
        if (EventSystem.current != null) return;

        GameObject es = new GameObject("EventSystem", typeof(EventSystem));
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        es.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
        es.AddComponent<StandaloneInputModule>();
#endif
    }

    private static Vector2 GetMousePosition()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        return UnityEngine.InputSystem.Mouse.current != null
            ? UnityEngine.InputSystem.Mouse.current.position.ReadValue()
            : Vector2.zero;
#else
        return Input.mousePosition;
#endif
    }
}
