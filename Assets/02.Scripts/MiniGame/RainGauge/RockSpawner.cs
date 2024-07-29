using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    public GameObject rockPrefab;
    public int poolSize = 10;
    public float spawnRate = 5f;
    public float spawnHeight = 10f;
    public float spawnAreaWidth = 5f;
    public float spawnAreaDepth = 5f;

    private List<GameObject> rockPool;
    private float nextSpawnTime;

    private void Start()
    {
        rockPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject rock = Instantiate(rockPrefab);
            rock.SetActive(false);
            rockPool.Add(rock);
        }
    }

    private void Update()
    {
        if (RainGaugeManager.Instance.CurrentGameState == GameState.Go && Time.time >= nextSpawnTime)
        {
            SpawnRock();
            nextSpawnTime = Time.time + 1f / spawnRate;
        }
    }

    private void SpawnRock()
    {
        GameObject rock = GetPooledRock();
        if (rock != null)
        {
            float spawnX = Random.Range(-spawnAreaWidth / 2f, spawnAreaWidth / 2f);
            float spawnZ = Random.Range(-spawnAreaDepth / 2f, spawnAreaDepth / 2f);
            Vector3 spawnPosition = new Vector3(spawnX, spawnHeight, spawnZ) + transform.position;
            rock.transform.position = spawnPosition;
            rock.SetActive(true);
        }
    }

    private GameObject GetPooledRock()
    {
        for (int i = 0; i < rockPool.Count; i++)
        {
            if (!rockPool[i].activeInHierarchy)
            {
                return rockPool[i];
            }
        }
        return null;
    }
}

