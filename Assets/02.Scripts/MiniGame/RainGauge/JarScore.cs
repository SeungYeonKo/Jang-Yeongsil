using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JarScore : MonoBehaviour
{
    public static JarScore Instance { get; private set; }

    public GameObject Jar1;
    public GameObject Jar2;
    public GameObject Jar3;
    public GameObject Jar4;

    private void Awake()
    {
        Instance = this;
    }
}
