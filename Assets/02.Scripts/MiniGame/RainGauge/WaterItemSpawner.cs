using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterItemSpawner : MonoBehaviour
{
    public GameObject waterItemPrefab;
    public int poolSize = 10;
    public float spawnRate = 5f;
    public float spawnHeight = 10f;
    public float spawnAreaWidth = 5f;
    public float spawnAreaDepth = 5f;

    private List<GameObject> waterItemPool;
    private float nextSpawnTime;

    private void Start()
    {
        waterItemPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject waterItem = Instantiate(waterItemPrefab);
            waterItem.SetActive(false);
            waterItemPool.Add(waterItem);
        }
    }

    private void Update()
    {
        if (RainGaugeManager.Instance.CurrentGameState == GameState.Go && Time.time >= nextSpawnTime)
        {
            SpawnWaterItem();
            nextSpawnTime = Time.time + 1f / spawnRate;
        }
    }

    private void SpawnWaterItem()
    {
        GameObject waterItem = GetPooledWaterItem();
        if (waterItem != null)
        {
            float spawnX = Random.Range(-spawnAreaWidth / 2f, spawnAreaWidth / 2f);
            float spawnZ = Random.Range(-spawnAreaDepth / 2f, spawnAreaDepth / 2f);
            Vector3 spawnPosition = new Vector3(spawnX, spawnHeight, spawnZ) + transform.position;
            waterItem.transform.position = spawnPosition;
            waterItem.SetActive(true);
        }
    }

    private GameObject GetPooledWaterItem()
    {
        for (int i = 0; i < waterItemPool.Count; i++)
        {
            if (!waterItemPool[i].activeInHierarchy)
            {
                return waterItemPool[i];
            }
        }
        return null;
    }
}
