using System;
using System.Collections;
using System.Collections.Generic;
using _02.Scripts.RockGame;
using UnityEngine;

public class StoneObstacleController : MonoBehaviour
{
   public float Speed = 5f; // 장애물이 움직이는 속도
   public float Range = 2f; // 위아래로 움직일 범위
   public bool IsShowing = false;
  public GameObject _obstacle;
   private Vector3 _oriPosition;
   private Coroutine _obstacleCoroutine;
   private void Start()
   {
      _obstacle = this.gameObject;
      _oriPosition = transform.position;
      _obstacle.gameObject.SetActive(false);
   }

   public void StartObstacle()
   {
      if (_obstacleCoroutine == null)
      {
         _obstacleCoroutine = StartCoroutine(ObstacleRoutine());
      }
   }
   private IEnumerator ObstacleRoutine()
   {
      _obstacle.gameObject.SetActive(true);

      while (IsShowing)
      {
         MovingObstacle();
         yield return null; // 다음 프레임까지 대기
      }

      _obstacle.gameObject.SetActive(false);
      _obstacleCoroutine = null; // 코루틴 종료
   }

   public void MovingObstacle()
   {
      _obstacle.gameObject.SetActive(true);
      Vector3 cameraPosition = Camera.main.transform.position;
      float newY = Mathf.Sin(Time.time * Speed) * Range;
      transform.position = new Vector3(_oriPosition.x, cameraPosition.y + newY, _oriPosition.z);
   }
}
