using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Holds the house's energy and updates the energy bar.
/// Put this on an empty GameObject (e.g. "GameManager") in your scene.
/// </summary>
public class EnergyManager : MonoBehaviour
{
    public static EnergyManager Instance { get; private set; }

    // Remembers the energy between scenes (-1 = not set yet).
    private static float savedEnergy = -1f;

    // Timer for the constant light consumption (static so it keeps counting between rooms).
    private static float lightTimer;

    // Makes sure the saved value is cleared every time you press Play in the editor.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticsOnPlay()
    {
        savedEnergy = -1f;
        lightTimer = 0f;
    }

    /// <summary>Call this when starting a NEW game so the energy goes back to full.</summary>
    public static void ResetSavedEnergy()
    {
        savedEnergy = -1f;
        lightTimer = 0f;
    }

    [Header("Energy")]
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float currentEnergy = 100f;

    [Header("Constant light consumption")]
    [Tooltip("Energy the house lights use all the time...")]
    [SerializeField] private float lightConsumption = 2f;
    [Tooltip("...every this many seconds.")]
    [SerializeField] private float lightInterval = 5f;

    [Header("UI (optional)")]
    [SerializeField] private Image energyFill;   // UI Image with Image Type = Filled
    [SerializeField] private Text energyText;    // optional text like "75 / 100"

    public float CurrentEnergy => currentEnergy;
    public float MaxEnergy => maxEnergy;
    public bool IsPaused { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);   // removes only this extra script, not the whole GameObject
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (Instance != this) return;
        if (IsPaused || lightInterval <= 0f || lightConsumption <= 0f) return;

        lightTimer += Time.deltaTime;

        while (lightTimer >= lightInterval)
        {
            lightTimer -= lightInterval;
            Drain(lightConsumption);
        }
    }

    private void Start()
    {
        // First time: start full. Later scenes: continue from the saved value.
        if (savedEnergy < 0f) savedEnergy = maxEnergy;
        currentEnergy = Mathf.Clamp(savedEnergy, 0f, maxEnergy);

        // If the bar was not assigned in the Inspector, try to find an object named "EnergyBar".
        if (energyFill == null)
        {
            GameObject bar = GameObject.Find("EnergyBar");
            if (bar != null) energyFill = bar.GetComponent<Image>();
        }
        if (energyFill == null)
            Debug.LogWarning("EnergyManager on '" + name + "': Energy Fill is not assigned, so the bar can't move.", this);

        SetupRectangularBar();

        UpdateUI();
    }

    /// <summary>
    /// Replaces the rounded default sprite with a plain white square,
    /// so the green bar is a clean rectangle.
    /// </summary>
    private void SetupRectangularBar()
    {
        if (energyFill == null) return;

        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();

        energyFill.sprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
        energyFill.type = Image.Type.Filled;
        energyFill.fillMethod = Image.FillMethod.Horizontal;
        energyFill.fillOrigin = (int)Image.OriginHorizontal.Left;
    }

    /// <summary>Call with true when the pause scene opens, false when it closes.</summary>
    public void SetPaused(bool paused)
    {
        IsPaused = paused;
    }

    /// <summary>Removes energy (called by appliances). Does nothing while paused.</summary>
    public void Drain(float amount)
    {
        if (IsPaused) return;

        currentEnergy = Mathf.Clamp(currentEnergy - amount, 0f, maxEnergy);
        savedEnergy = currentEnergy;
        UpdateUI();
    }

    /// <summary>Adds energy (e.g. solar panels, power-ups).</summary>
    public void Add(float amount)
    {
        currentEnergy = Mathf.Clamp(currentEnergy + amount, 0f, maxEnergy);
        savedEnergy = currentEnergy;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (energyFill != null) energyFill.fillAmount = currentEnergy / maxEnergy;
        if (energyText != null)
            energyText.text = Mathf.CeilToInt(currentEnergy) + " / " + Mathf.CeilToInt(maxEnergy);
    }
}
