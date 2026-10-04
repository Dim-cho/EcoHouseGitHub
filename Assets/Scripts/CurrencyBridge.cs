using UnityEngine;


/// <summary>
/// The upgrade button uses this to check and spend coins.
///
/// Backed by the real currency system: CurrencyHUD holds the balance the server
/// last confirmed, and GameSync queues the change and pushes it. Falls back to a
/// local wallet only when those aren't in the scene, so appliances can still be
/// tested from a room scene opened on its own.
/// </summary>
public static class CurrencyBridge
{
    private const int FallbackCoins = 500;
    private static int offlineCoins = FallbackCoins;


    static bool Live => CurrencyHUD.Instance != null && GameSync.Instance != null;


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetOnPlay()
    {
        offlineCoins = FallbackCoins;
    }


    public static int GetCoins()
    {
        return Live ? CurrencyHUD.Instance.Coins : offlineCoins;
    }


    public static bool TrySpend(int amount)
    {
        if (GetCoins() < amount) return false;


        if (Live) GameSync.Instance.SpendCoins(amount);
        else offlineCoins -= amount;


        return true;
    }


    public static void AddCoins(int amount)
    {
        if (Live) GameSync.Instance.EarnCoins(amount);
        else offlineCoins += amount;
    }
}