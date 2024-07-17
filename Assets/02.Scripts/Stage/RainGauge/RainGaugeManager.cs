using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainGaugeManager : MonoBehaviour
{
    public static RainGaugeManager Instance { get; private set; }

    private void Start()
    {
        Instance = this;
    }
}
