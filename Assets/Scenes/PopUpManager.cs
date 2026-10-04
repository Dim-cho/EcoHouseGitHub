using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using TMPro;


public class PopUpManager : MonoBehaviour
{
    public static PopUpManager Instance;


    [Header("Tilemap Reference")]
    public Tilemap targetTilemap; // The grid where your objects live


    [Header("UI Components")]
    public GameObject popUpPanel;
    public TextMeshProUGUI descriptionText;
    public Image itemImage;
    public Button upgradeButton;


    [Tooltip("Optional: shows the cost and whether the player can afford it.")]
    public TextMeshProUGUI costText;


    [Header("Reward for upgrading")]
    public int ecoPointsPerUpgrade = 5;


    // Coins live on the server through GameSync/CurrencyHUD, so there's no local
    // balance here. Reads 0 until the HUD has its first value.
    int PlayerCoins => CurrencyHUD.Instance != null ? CurrencyHUD.Instance.Coins : 0;


    // Hidden variables to track what we just clicked on
    private TileUpgradeData currentData;
    private Vector3Int currentCellPos;
    private bool isCurrentlyUpgraded;


    void Awake()
    {
        // Make this script easy to find for other scripts
        if (Instance == null) Instance = this;
        else Destroy(gameObject);


        // Hide the pop-up when the game starts
        popUpPanel.SetActive(false);
    }


    // This is called when we click a tile
    public void OpenTilePopUp(TileUpgradeData data, Vector3Int cellPos, bool isUpgraded)
    {
        currentData = data;
        currentCellPos = cellPos;
        isCurrentlyUpgraded = isUpgraded;


        popUpPanel.SetActive(true); // Show the menu
        UpdatePopUpUI(); // Update texts and sprites
    }


    // Called by the Close button
    public void ClosePopUp()
    {
        popUpPanel.SetActive(false);
    }


    // Called by the Upgrade button
    public void AttemptUpgrade()
    {
        // Check if we haven't upgraded yet AND have enough coins
        if (!isCurrentlyUpgraded && PlayerCoins >= currentData.upgradeCost)
        {
            // Spend through GameSync: it updates the HUD now and the server on
            // the next sync, so the purchase survives a restart.
            if (GameSync.Instance != null)
            {
                GameSync.Instance.SpendCoins(currentData.upgradeCost);
                GameSync.Instance.EarnEcoPoints(ecoPointsPerUpgrade);
            }


            isCurrentlyUpgraded = true; // Mark as upgraded


            // Change the physical tile on the Tilemap grid
            targetTilemap.SetTile(currentCellPos, currentData.upgradedTile);


            // Refresh the UI to show the new upgraded text/sprite
            UpdatePopUpUI();
        }
    }


    // Handles swapping the text, sprite, and button state
    private void UpdatePopUpUI()
    {
        if (isCurrentlyUpgraded)
        {
            descriptionText.text = currentData.upgradedDescription;
            itemImage.sprite = currentData.upgradedSprite;
            upgradeButton.interactable = false; // Gray out the button


            if (costText != null) costText.text = "Upgraded";
        }
        else
        {
            descriptionText.text = currentData.normalDescription;
            itemImage.sprite = currentData.normalSprite;


            // Button is clickable only if you have enough money
            bool canAfford = PlayerCoins >= currentData.upgradeCost;
            upgradeButton.interactable = canAfford;


            // Say why the button is greyed out rather than leaving them guessing.
            if (costText != null)
            {
                costText.text = canAfford
                    ? $"{currentData.upgradeCost} coins"
                    : $"{currentData.upgradeCost} coins — you have {PlayerCoins}";
            }
        }
    }
}