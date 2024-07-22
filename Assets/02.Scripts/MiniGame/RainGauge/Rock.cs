using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{
    public float fallSpeed = 5f;
    public float deactivateHeight = -10f;

    private void Update()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        if (transform.position.y <= deactivateHeight)
        {
            gameObject.SetActive(false);
        }
    }
}
