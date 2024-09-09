using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RainParticleMover : MonoBehaviour
{
    public float speed = 3.0f; 
    public float waitTimeAtPosition = 2.0f;
    public List<Slider> progressBars;
    public List<RiceSpawner> riceSpawners;

    private ParticleSystem rainParticleSystem;
    private List<Vector3> targetPositions = new List<Vector3>(); 
    private List<string> positionNames = new List<string>(); 
    private int currentTargetIndex = 0; 
    private bool isMoving = true;

    void Start()
    {
        rainParticleSystem = GetComponent<ParticleSystem>();
        if (rainParticleSystem == null)
        {
            Debug.LogError("ParticleSystem component is missing on this GameObject.");
        }

        targetPositions.Add(new Vector3(466.53f, transform.position.y, 706.083f));
        targetPositions.Add(new Vector3(499.8f, transform.position.y, 692.2f));
        targetPositions.Add(new Vector3(471.5f, transform.position.y, 681.2f));
        targetPositions.Add(new Vector3(431.9f, transform.position.y, 702.5f));
        targetPositions.Add(new Vector3(502.83f, transform.position.y, 713.2f));
        targetPositions.Add(new Vector3(435.944f, transform.position.y, 678.51f));
        targetPositions.Add(new Vector3(413.26f, transform.position.y, 664.8f));
        targetPositions.Add(new Vector3(487.4f, transform.position.y, 668.3f));
        targetPositions.Add(new Vector3(450.095f, transform.position.y, 664.008f));
        targetPositions.Add(new Vector3(435.14f, transform.position.y, 650.1f));
        targetPositions.Add(new Vector3(504.7f, transform.position.y, 656.84f));
        targetPositions.Add(new Vector3(486.95f, transform.position.y, 644.97f));

        positionNames.Add("Cheugugi");
        positionNames.Add("Cheugugi (1)");
        positionNames.Add("Cheugugi (2)");
        positionNames.Add("Cheugugi (3)");
        positionNames.Add("Cheugugi (4)");
        positionNames.Add("Cheugugi (5)");
        positionNames.Add("Cheugugi (6)");
        positionNames.Add("Cheugugi (7)");
        positionNames.Add("Cheugugi (8)");
        positionNames.Add("Cheugugi (9)");
        positionNames.Add("Cheugugi (10)");
        positionNames.Add("Cheugugi (11)");

        if (rainParticleSystem != null)
        {
            var main = rainParticleSystem.main;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
        }

        Debug.Log($"Starting at position: {positionNames[currentTargetIndex]} ({targetPositions[currentTargetIndex]})");
        StartCoroutine(MoveToNextPosition());
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient && RainGaugeManager.Instance.CurrentGameState == GameState.Go)
        {
            if (isMoving && rainParticleSystem != null)
            {
                // 현재 타겟 위치로 이동
                Vector3 targetPosition = targetPositions[currentTargetIndex];
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

                // 타겟 위치에 도착하면 이동 멈추고 대기 시작
                if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                {
                    isMoving = false;
                    Debug.Log($"Reached position: {positionNames[currentTargetIndex]} ({targetPosition})");
                    StartCoroutine(WaitAtPosition());
                }
            }
        }
    }

    private IEnumerator WaitAtPosition()
    {
        float elapsedTime = 0f;
        Slider currentProgressBar = progressBars[currentTargetIndex];
        RiceSpawner currentRiceSpawner = riceSpawners[currentTargetIndex];
        currentProgressBar.value = 0;

        while (elapsedTime < waitTimeAtPosition)
        {
            currentProgressBar.value = Mathf.Lerp(0, 1, elapsedTime / waitTimeAtPosition); 
            elapsedTime += Time.deltaTime;
            yield return null;
        }


        currentProgressBar.value = 1;

        if (currentRiceSpawner != null)
        {
            currentRiceSpawner.SpawnRock(currentRiceSpawner.transform.position);
        }

        // 다음 위치를 랜덤하게 설정
        currentTargetIndex = Random.Range(0, targetPositions.Count);

        Debug.Log($"New target position set: {positionNames[currentTargetIndex]} ({targetPositions[currentTargetIndex]})");
        isMoving = true;  // 다음 위치로 이동 시작
    }

    private IEnumerator MoveToNextPosition()
    {
        // 첫 번째 목표 위치로 이동을 시작
        while (true)
        {
            if (isMoving)
            {
                Vector3 targetPosition = targetPositions[currentTargetIndex];
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

                // 타겟 위치에 도착하면 이동 멈추고 대기 시작
                if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                {
                    isMoving = false;
                    Debug.Log($"Reached position: {positionNames[currentTargetIndex]} ({targetPosition})");
                    StartCoroutine(WaitAtPosition());
                }
            }

            yield return null;
        }
    }
}

