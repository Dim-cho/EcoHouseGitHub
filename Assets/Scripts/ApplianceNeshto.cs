using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// One electrical appliance that is drawn with tiles on a Tilemap.
/// Create an empty GameObject as a CHILD of the Tilemap, add this script (it adds a
/// Box Collider 2D automatically) and size the box so it covers the appliance's tiles.
///
/// - Uses energy only while ON.
/// - Hovering darkens its tiles and opens the info menu (see ApplianceMenu.cs).
/// - Can be upgraded to a new appliance (tile swap + new energy cost).
/// - Remembers ON/OFF and the upgrade when you leave the room and come back.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class Appliance : MonoBehaviour
{
    [Serializable]
    public class TileSwap
    {
        public TileBase oldTile;
        public TileBase newTile;
    }

    // ---- remembered between scenes (rooms) ----
    private struct SavedState
    {
        public bool isOn;
        public bool upgraded;
    }

    private static readonly Dictionary<string, SavedState> savedStates = new Dictionary<string, SavedState>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatesOnPlay()
    {
        savedStates.Clear();
    }

    /// <summary>Call when starting a NEW game so every appliance goes back to its starting state.</summary>
    public static void ResetSavedStates()
    {
        savedStates.Clear();
    }

    [Header("Info shown in the hover menu")]
    [SerializeField] private string applianceName = "Fridge";
    [SerializeField] private int price = 100;

    [Header("Energy usage")]
    [SerializeField] private float energyCost = 5f;   // energy lost per interval
    [SerializeField] private float interval = 10f;    // seconds between each drain
    [SerializeField] private bool isOn = true;

    [Header("Look")]
    [Tooltip("Leave empty if this object is a child of the Tilemap.")]
    [SerializeField] private Tilemap tilemap;
    [Range(0.3f, 1f)][SerializeField] private float hoverBrightness = 0.8f;
    [Range(0.2f, 1f)][SerializeField] private float offBrightness = 0.6f;

    [Header("Upgrade (optional)")]
    [SerializeField] private bool hasUpgrade = true;
    [SerializeField] private string upgradeName = "New Fridge";
    [SerializeField] private int upgradePrice = 200;
    [SerializeField] private float upgradeEnergyCost = 3f;
    [Tooltip("For every tile of the appliance: the old tile and the tile that replaces it.")]
    [SerializeField] private TileSwap[] upgradeTileSwaps;

    private Collider2D col;
    private float timer;
    private bool hovered;
    private bool upgraded;
    private string stateKey;

    // Read-only info for the menu
    public string ApplianceName => applianceName;
    public int Price => price;
    public float EnergyCost => energyCost;
    public float Interval => interval;
    public bool IsOn => isOn;
    public bool HasUpgrade => hasUpgrade;
    public bool IsUpgraded => upgraded;
    public bool CanUpgrade => hasUpgrade && !upgraded;
    public string UpgradeName => upgradeName;
    public int UpgradePrice => upgradePrice;
    public bool CanAffordUpgrade => CurrencyBridge.GetCoins() >= upgradePrice;
    public Bounds WorldBounds => col.bounds;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;   // so the player can walk through it

        if (tilemap == null) tilemap = GetComponentInParent<Tilemap>();

        stateKey = BuildStateKey();
    }

    private void Start()
    {
        if (tilemap == null)
        {
            Debug.LogWarning("Appliance '" + name + "': no Tilemap found, so it can't darken or upgrade tiles. " +
                             "Make this object a CHILD of the Tilemap (e.g. furn 3) or drag the Tilemap into the Tilemap field.", this);
        }
        else
        {
            int tilesUnderBox = 0;
            foreach (Vector3Int cell in GetCells())
                if (tilemap.GetTile(cell) != null) tilesUnderBox++;

            if (tilesUnderBox == 0)
                Debug.LogWarning("Appliance '" + name + "': there are no tiles of '" + tilemap.name + "' under the box. " +
                                 "Move/resize the box over the appliance, or pick the right Tilemap.", this);
        }

        // Coming back to this room: restore what the player did before
        SavedState saved;
        if (savedStates.TryGetValue(stateKey, out saved))
        {
            isOn = saved.isOn;
            if (saved.upgraded && hasUpgrade) ApplyUpgrade();
        }

        ApplyTint();
    }

    private void Update()
    {
        if (!isOn || interval <= 0f) return;
        if (EnergyManager.Instance == null || EnergyManager.Instance.IsPaused) return;

        timer += Time.deltaTime;

        while (timer >= interval)
        {
            timer -= interval;
            EnergyManager.Instance.Drain(energyCost);
        }
    }

    /// <summary>Switch on/off. Only ON appliances use energy.</summary>
    public void Toggle()
    {
        isOn = !isOn;
        timer = 0f;
        SaveState();
        ApplyTint();
    }

    public void SetHovered(bool value)
    {
        hovered = value;
        ApplyTint();
    }

    /// <summary>Pays for the upgrade and swaps to the new appliance. Returns false if it can't.</summary>
    public bool TryUpgrade()
    {
        if (!CanUpgrade) return false;
        if (!CurrencyBridge.TrySpend(upgradePrice)) return false;

        ApplyUpgrade();
        SaveState();
        ApplyTint();
        return true;
    }

    // Swaps the tiles and sets the new name / energy use / price (no payment here).
    private void ApplyUpgrade()
    {
        int swapped = 0;

        if (tilemap != null && upgradeTileSwaps != null)
        {
            foreach (Vector3Int cell in GetCells())
            {
                TileBase tileHere = tilemap.GetTile(cell);

                foreach (TileSwap swap in upgradeTileSwaps)
                {
                    if (swap.oldTile != null && tileHere == swap.oldTile)
                    {
                        tilemap.SetTile(cell, swap.newTile);
                        swapped++;
                        break;
                    }
                }
            }
        }

        if (swapped == 0)
        {
            string underBox = "";
            if (tilemap != null)
            {
                foreach (Vector3Int cell in GetCells())
                {
                    TileBase t = tilemap.GetTile(cell);
                    underBox += (t != null ? t.name : "empty") + " at " + cell + ";  ";
                }
            }

            string oldTiles = "";
            if (upgradeTileSwaps != null)
            {
                foreach (TileSwap swap in upgradeTileSwaps)
                    oldTiles += (swap.oldTile != null ? swap.oldTile.name : "EMPTY") + ";  ";
            }

            // Key info is in the FIRST line so it is visible in the Console list.
            Debug.LogWarning("Appliance '" + name + "': no tile replaced. " +
                             "TILE UNDER BOX = " + (underBox == "" ? "none" : underBox) +
                             "  |  OLD TILE IN LIST = " + (oldTiles == "" ? "none" : oldTiles) +
                             "\n(Tilemap used: " + (tilemap != null ? tilemap.name : "NONE") + "). " +
                             "At least one Old Tile must be exactly one of the tiles under the box.", this);
        }

        applianceName = upgradeName;
        energyCost = upgradeEnergyCost;
        price = upgradePrice;
        upgraded = true;
    }

    // ---------------------------------------------------------------- remembering

    private void SaveState()
    {
        SavedState state;
        state.isOn = isOn;
        state.upgraded = upgraded;
        savedStates[stateKey] = state;
    }

    // A unique name for this appliance: scene + its place in the Hierarchy.
    private string BuildStateKey()
    {
        string key = gameObject.scene.name;

        Transform t = transform;
        while (t != null)
        {
            key += "/" + t.name + "#" + t.GetSiblingIndex();
            t = t.parent;
        }
        return key;
    }

    // ---------------------------------------------------------------- tiles

    // Darkens the tiles: a bit when hovered, more when switched off.
    private void ApplyTint()
    {
        if (tilemap == null) return;

        float v = (isOn ? 1f : offBrightness) * (hovered ? hoverBrightness : 1f);
        Color color = new Color(v, v, v, 1f);

        foreach (Vector3Int cell in GetCells())
        {
            if (tilemap.GetTile(cell) == null) continue;

            tilemap.SetTileFlags(cell, TileFlags.None);   // allow the color to change
            tilemap.SetColor(cell, color);
        }
    }

    // All cells whose CENTER is inside the collider box (at least the cell under the middle).
    private List<Vector3Int> GetCells()
    {
        List<Vector3Int> cells = new List<Vector3Int>();
        if (tilemap == null || col == null) return cells;

        Bounds b = col.bounds;
        Vector3Int min = tilemap.WorldToCell(b.min);
        Vector3Int max = tilemap.WorldToCell(b.max);

        for (int x = min.x; x <= max.x; x++)
        {
            for (int y = min.y; y <= max.y; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                Vector3 center = tilemap.GetCellCenterWorld(cell);
                center.z = b.center.z;

                if (b.Contains(center)) cells.Add(cell);
            }
        }

        // Box smaller than a cell? Then use the cell under the middle of the box.
        if (cells.Count == 0)
        {
            Vector3Int middle = tilemap.WorldToCell(b.center);
            cells.Add(new Vector3Int(middle.x, middle.y, 0));
        }
        return cells;
    }
}
