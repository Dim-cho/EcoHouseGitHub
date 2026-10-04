using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

// This lets us define the rules for each tile in the Unity Inspector
[System.Serializable]
public class TileUpgradeData
{
    public string itemName;
    public int upgradeCost = 50;

    [Header("Normal State")]
    public TileBase normalTile; // The tile on the map
    public Sprite normalSprite; // The picture in the UI
    [TextArea] public string normalDescription; // The UI text

    [Header("Upgraded State")]
    public TileBase upgradedTile; // The new tile on the map
    public Sprite upgradedSprite; // The new picture in the UI
    [TextArea] public string upgradedDescription; // The new UI text
}

public class TileInteractManager : MonoBehaviour
{
    public Tilemap tilemap;

    [Header("Database of Upgradable Tiles")]
    public List<TileUpgradeData> upgradableTiles;

    void Update()
    {
        // If we left-click AND we are NOT clicking on a UI button...
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            // 1. Get the mouse position in the game world
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;

            // 2. Convert world position to grid cell coordinates (like A4, B6)
            Vector3Int cellPosition = tilemap.WorldToCell(mouseWorldPos);

            // 3. Find out what tile asset is painted at that exact cell
            TileBase clickedTile = tilemap.GetTile(cellPosition);

            if (clickedTile != null)
            {
                CheckTile(clickedTile, cellPosition);
            }
        }
    }

    void CheckTile(TileBase clickedTile, Vector3Int cellPosition)
    {
        // Check our database to see if the clicked tile is upgradable
        foreach (TileUpgradeData data in upgradableTiles)
        {
            if (clickedTile == data.normalTile)
            {
                // We clicked a normal tile. Open menu in normal mode.
                PopUpManager.Instance.OpenTilePopUp(data, cellPosition, false);
                return;
            }
            else if (clickedTile == data.upgradedTile)
            {
                // We clicked a tile that is already upgraded! Open in upgraded mode.
                PopUpManager.Instance.OpenTilePopUp(data, cellPosition, true);
                return;
            }
        }
    }
}