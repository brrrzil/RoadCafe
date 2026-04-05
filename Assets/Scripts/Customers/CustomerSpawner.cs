using UnityEngine;
using System.Collections;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private float minSpawnInterval = 8f;
    [SerializeField] private float maxSpawnInterval = 15f;
    [SerializeField] private float maxPollutionMultiplier = 2.5f;

    public static CustomerSpawner Instance { get; private set; }

    private Transform spawnPoint;

    public static int ActiveCustomersCount { get; private set; }
    public static int TotalCustomersSpawned { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint")?.transform;        

        ActiveCustomersCount = 0;
        TotalCustomersSpawned = 0;

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            float pollution = Ecology.CurrentPollution;
            float multiplier = 1f + (pollution / 100f) * (maxPollutionMultiplier - 1f);
            float interval = Mathf.Lerp(minSpawnInterval, maxSpawnInterval, multiplier);

            yield return new WaitForSeconds(interval);

            if (customerPrefab != null && spawnPoint != null)
            {
                GameObject customer = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
                ActiveCustomersCount++;
                TotalCustomersSpawned++;                
            }
        }
    }
}