using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jar : MonoBehaviour
{
    public GameObject Jar1;
    public GameObject Jar2;
    public GameObject Jar3;
    public GameObject Jar4;

    public void SetJarPosition(int jarNum, Vector3 position)
    {
        GameObject jar = GetJarObject(jarNum);
        if (jar != null)
        {
            position.y += 3; // 플레이어 이름 때문에 일단 높이 띄움
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

}
