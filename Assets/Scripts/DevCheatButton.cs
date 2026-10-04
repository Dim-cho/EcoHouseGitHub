using UnityEngine;
using UnityEngine.UI;


// Testing aid: hands out currency so upgrades can be tried without grinding.
// Deletes itself outside the editor, so a stray build can't ship free coins.
public class DevCheatButton : MonoBehaviour
{
    [Header("Buttons")]
    public Button addCoinsButton;
    public Button addEcoPointsButton;


    [Header("Amounts")]
    public int coinsPerClick = 100;
    public int ecoPointsPerClick = 25;


    void Awake()
    {
#if !UNITY_EDITOR
        gameObject.SetActive(false);
        return;
#endif
    }


    void Start()
    {
        if (addCoinsButton != null) addCoinsButton.onClick.AddListener(AddCoins);
        if (addEcoPointsButton != null) addEcoPointsButton.onClick.AddListener(AddEcoPoints);
    }


    void AddCoins()
    {
        if (GameSync.Instance == null) return;
        GameSync.Instance.EarnCoins(coinsPerClick);
    }


    void AddEcoPoints()
    {
        if (GameSync.Instance == null) return;
        GameSync.Instance.EarnEcoPoints(ecoPointsPerClick);
    }
}