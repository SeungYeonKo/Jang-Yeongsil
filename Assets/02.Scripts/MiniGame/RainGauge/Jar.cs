using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jar : MonoBehaviourPun
{
    public GameObject Jar1;
    public GameObject Jar2;
    public GameObject Jar3;
    public GameObject Jar4;

    public GameObject brokenJarPrefab;
    public ParticleSystem waterSplashEffect;

    public int poolSize = 10;

    private List<GameObject> brokenJarPool;
    private List<ParticleSystem> splashEffectPool;

    private void Start()
    {
        brokenJarPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject brokenJar = Instantiate(brokenJarPrefab);
            PhotonView photonView = brokenJar.AddComponent<PhotonView>();
            if (PhotonNetwork.AllocateViewID(photonView))
            {
                brokenJar.SetActive(false);
                brokenJarPool.Add(brokenJar);
            }
            else
            {
                Debug.LogError("Failed to allocate a ViewID for brokenJar.");
                Destroy(brokenJar);
            }
        }

        splashEffectPool = new List<ParticleSystem>();
        for (int i = 0; i < poolSize; i++)
        {
            ParticleSystem splashEffect = Instantiate(waterSplashEffect);
            PhotonView photonView = splashEffect.gameObject.AddComponent<PhotonView>();
            if (PhotonNetwork.AllocateViewID(photonView))
            {
                splashEffect.gameObject.SetActive(false);
                splashEffectPool.Add(splashEffect);
            }
            else
            {
                Debug.LogError("Failed to allocate a ViewID for splashEffect.");
                Destroy(splashEffect.gameObject);
            }
        }
    }

    public void SetJarPosition(int jarNum, Vector3 position)
    {
        GameObject jar = GetJarObject(jarNum);
        if (jar != null)
        {
            jar.transform.position = position;
        }
    }

    private GameObject GetJarObject(int jarNum)
    {
        switch (jarNum)
        {
            case 1:
                return Jar1;
            case 2:
                return Jar2;
            case 3:
                return Jar3;
            case 4:
                return Jar4;
            default:
                Debug.LogError("Invalid jar number: " + jarNum);
                return null;
        }
    }

    public void BreakJar(int jarNum)
    {
        if (photonView == null)
        {
            Debug.LogError("PhotonView is null on BreakJar");
            return;
        }
        photonView.RPC("RPC_BreakJar", RpcTarget.All, jarNum);
    }

    [PunRPC]
    private void RPC_BreakJar(int jarNum)
    {
        GameObject jar = GetJarObject(jarNum);
        if (jar != null)
        {
            Vector3 jarPosition = jar.transform.position;

            GameObject brokenJar = GetPooledObject(brokenJarPool, brokenJarPrefab);
            brokenJar.transform.position = jarPosition;
            brokenJar.transform.rotation = jar.transform.rotation;
            brokenJar.SetActive(true);

            ParticleSystem splash = GetPooledObject(splashEffectPool, waterSplashEffect).GetComponent<ParticleSystem>();
            splash.transform.position = jarPosition;
            splash.gameObject.SetActive(true);
            splash.Play();

            StartCoroutine(FollowBrokenJar(brokenJar.transform, splash));
            StartCoroutine(StopSplashAfterDelay(splash, 1f));
            StartCoroutine(DeactivateObjectAfterDelay(brokenJar, 1f));

            jar.SetActive(false);

            StartCoroutine(ReplaceJarAfterDelay(jarNum, jarPosition, 1f));
        }
    }

    private IEnumerator ReplaceJarAfterDelay(int jarNum, Vector3 position, float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject jar = GetJarObject(jarNum);
        if (jar != null)
        {
            jar.transform.position = position;
            jar.SetActive(true);
        }
    }

    private IEnumerator StopSplashAfterDelay(ParticleSystem splash, float delay)
    {
        yield return new WaitForSeconds(delay);
        splash.Stop();
        splash.gameObject.SetActive(false);
    }

    private IEnumerator FollowBrokenJar(Transform brokenJarTransform, ParticleSystem splash)
    {
        while (splash.isPlaying)
        {
            splash.transform.position = brokenJarTransform.position + Vector3.down;
            yield return null; 
        }
    }
    private IEnumerator DeactivateObjectAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }

    private GameObject GetPooledObject(List<GameObject> pool, GameObject prefab)
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        GameObject newObj = Instantiate(prefab);
        PhotonView photonView = newObj.AddComponent<PhotonView>();
        if (PhotonNetwork.AllocateViewID(photonView))
        {
            newObj.SetActive(false);
            pool.Add(newObj);
            return newObj;
        }
        else
        {
            Debug.LogError("Failed to allocate a ViewID for newObj.");
            Destroy(newObj);
            return null;
        }
    }

    private GameObject GetPooledObject(List<ParticleSystem> pool, ParticleSystem prefab)
    {
        foreach (ParticleSystem obj in pool)
        {
            if (!obj.gameObject.activeInHierarchy)
            {
                return obj.gameObject;
            }
        }

        ParticleSystem newObj = Instantiate(prefab);
        PhotonView photonView = newObj.gameObject.AddComponent<PhotonView>();
        if (PhotonNetwork.AllocateViewID(photonView))
        {
            newObj.gameObject.SetActive(false);
            pool.Add(newObj);
            return newObj.gameObject;
        }
        else
        {
            Debug.LogError("Failed to allocate a ViewID for newObj.");
            Destroy(newObj.gameObject);
            return null;
        }
    }
}
