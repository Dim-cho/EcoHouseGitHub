using System.Collections;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [Header("Coin Setup")]
    [SerializeField] private GameObject coinPrefab;

    [Header("Spawn Timing")]
    [SerializeField] private float minSpawnDelay = 0.5f;
    [SerializeField] private float maxSpawnDelay = 1.2f;

    [Header("Screen Margins")]
    [SerializeField] private float padding = 0.1f;

    private Camera mainCamera;
    private Coroutine spawnCoroutine;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    public void StartSpawning()
    {
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }

        // Clean up any remaining coins left on screen
        Coin[] activeCoins = FindObjectsByType<Coin>(FindObjectsSortMode.None);
        foreach (Coin c in activeCoins)
        {
            Destroy(c.gameObject);
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(waitTime);

            Vector3 randomViewportPos = new Vector3(
                Random.Range(padding, 1f - padding),
                Random.Range(padding, 1f - padding),
                10f
            );

            Vector3 worldPos = mainCamera.ViewportToWorldPoint(randomViewportPos);
            worldPos.z = 0f;

            Instantiate(coinPrefab, worldPos, Quaternion.identity);
        }
    }
}