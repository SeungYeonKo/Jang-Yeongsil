using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FireballController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Particle_Coroutine());
    }

    IEnumerator Particle_Coroutine()
    {
        yield return new WaitForSeconds(10f);
        SceneManager.LoadScene("MainScene");
    }
}
