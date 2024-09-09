using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiceSpawner : MonoBehaviourPun
{
    public GameObject rockPrefab;
    public int poolSize = 10;
    //public float spawnRate = 5f;
    public float spawnHeight = 10f;
    public float spawnAreaWidth = 5f;
    public float spawnAreaDepth = 5f;

    private List<GameObject> rockPool;
    private float nextSpawnTime;

    private void Start()
    {
        InitializeRockPool();

        if (PhotonNetwork.IsMasterClient)
        {
            FillRockPool();
            photonView.RPC("SyncRockPool", RpcTarget.OthersBuffered, poolSize);
        }
    }

  /*  private void Update()
    {
        if (PhotonNetwork.IsMasterClient && RainGaugeManager.Instance.CurrentGameState == GameState.Go && Time.time >= nextSpawnTime)
        {
            Vector3 spawnPosition = GenerateSpawnPosition();
            photonView.RPC("SpawnRock", RpcTarget.All, spawnPosition);
            //nextSpawnTime = Time.time + 1f / spawnRate;
        }
    }*/

    private void InitializeRockPool()
    {
        rockPool = new List<GameObject>(poolSize);
    }

    private void FillRockPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject rock = PhotonNetwork.Instantiate(rockPrefab.name, Vector3.zero, Quaternion.identity);
            rock.SetActive(false);
            rockPool.Add(rock);
        }
    }

    private Vector3 GenerateSpawnPosition()
    {
        float spawnX = Random.Range(-spawnAreaWidth / 2f, spawnAreaWidth / 2f);
        float spawnZ = Random.Range(-spawnAreaDepth / 2f, spawnAreaDepth / 2f);
        return new Vector3(spawnX, spawnHeight, spawnZ) + transform.position;
    }

    [PunRPC]
    private void SyncRockPool(int poolSize)
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject rock = PhotonNetwork.Instantiate(rockPrefab.name, Vector3.zero, Quaternion.identity);
            rock.SetActive(false);
            rockPool.Add(rock);
        }
    }


    [PunRPC]
    public void SpawnRock(Vector3 spawnPosition)
    {
        GameObject rock = GetPooledRock();
        if (rock != null)
        {
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

