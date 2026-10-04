using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

/// <summary>
/// The bed. Create an empty GameObject as a CHILD of the Tilemap that holds the bed,
/// add this script (a Box Collider 2D is added automatically) and size the box over the bed.
///
/// - Hovering the bed darkens it slightly.
/// - Clicking it tries to sleep (see DayNightCycle.TrySleep).
///   Sleeping is only possible once the day is at least half over.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class Bed : MonoBehaviour
{
    [Tooltip("Leave empty if this object is a child of the Tilemap.")]
    [SerializeField] private Tilemap tilemap;
    [Range(0.3f, 1f)] [SerializeField] private float hoverBrightness = 0.8f;

    private Collider2D col;
    private bool hovered;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;

        if (tilemap == null) tilemap = GetComponentInParent<Tilemap>();
    }

    private void Start()
    {
        if (tilemap == null)
        {
            Debug.LogWarning("Bed '" + name + "': no Tilemap found, so it can't be darkened on hover. " +
                             "Make this object a CHILD of the Tilemap or drag the Tilemap into the Tilemap field.", this);
        }
    }

    private void OnDisable()
    {
        if (hovered)
        {
            hovered = false;
            ApplyTint();
        }
    }

    private void Update()
    {
        bool blocked = Time.timeScale == 0f ||
                       (EnergyManager.Instance != null && EnergyManager.Instance.IsPaused) ||
                       (DayNightCycle.Instance != null && DayNightCycle.Instance.IsSleeping);

        bool over = !blocked && IsMouseOverBed();

        if (over != hovered)
        {
            hovered = over;
            ApplyTint();
        }

        if (over && LeftClickDown())
        {
            if (DayNightCycle.Instance != null) DayNightCycle.Instance.TrySleep();
        }
    }

    private bool IsMouseOverBed()
    {
        Camera cam = Camera.main;
        if (cam == null) return false;

        // Ignore the bed when the mouse is over a button or menu
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return false;

        Vector3 world = cam.ScreenToWorldPoint(GetMousePosition());

        foreach (Collider2D hit in Physics2D.OverlapPointAll(world))
        {
            if (hit == col) return true;
        }
        return false;
    }

    private void ApplyTint()
    {
        if (tilemap == null) return;

        float v = hovered ? hoverBrightness : 1f;
        Color color = new Color(v, v, v, 1f);

        foreach (Vector3Int cell in GetCells())
        {
            if (tilemap.GetTile(cell) == null) continue;

            tilemap.SetTileFlags(cell, TileFlags.None);
            tilemap.SetColor(cell, color);
        }
    }

    // All cells whose CENTER is inside the box (at least the cell under the middle).
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

        if (cells.Count == 0)
        {
            Vector3Int middle = tilemap.WorldToCell(b.center);
            cells.Add(new Vector3Int(middle.x, middle.y, 0));
        }
        return cells;
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

    private static bool LeftClickDown()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        return UnityEngine.InputSystem.Mouse.current != null &&
               UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame;
#else
        return Input.GetMouseButtonDown(0);
#endif
    }
}
