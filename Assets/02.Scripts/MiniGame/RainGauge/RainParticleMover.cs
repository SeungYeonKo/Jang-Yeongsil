using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainParticleMover : MonoBehaviour
{
    public Vector3 direction = Vector3.forward; // 비가 이동할 방향
    public float speed = 1.0f; // 비의 이동 속도
    private ParticleSystem rainParticleSystem;

    void Start()
    {
        rainParticleSystem = GetComponent<ParticleSystem>();
        if (rainParticleSystem == null)
        {
            Debug.LogError("ParticleSystem component is missing on this GameObject.");
        }
    }

    void Update()
    {
        if (rainParticleSystem != null)
        {
            // 파티클의 위치를 이동시킴
            var main = rainParticleSystem.main;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            transform.position += direction.normalized * speed * Time.deltaTime;
        }
    }
}
