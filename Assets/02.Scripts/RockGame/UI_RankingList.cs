using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _02.Scripts.RockGame
{
    public class UI_RankingList : MonoBehaviour
    {
        public RankManager rankManager; // RankManager를 참조합니다.
        public Transform rankingContent; // 랭킹 리스트를 표시할 부모 객체
        public GameObject rankingEntryPrefab; // 하나의 랭킹 항목을 표시할 프리팹

        void Start()
        {
            DisplayRanking();
        }

        public void DisplayRanking()
        {
            // 상위 10개 랭킹 가져오기
            List<Rank> topRanks = rankManager.GetTopRanks(10);

            // 기존 UI 항목 제거
            foreach (Transform child in rankingContent)
            {
                Destroy(child.gameObject);
            }

            // 새로운 랭킹 항목 생성
            foreach (var rank in topRanks)
            {
                GameObject entry = Instantiate(rankingEntryPrefab, rankingContent);
                UI_RankFrame rankFrame = entry.GetComponent<UI_RankFrame>();
                
                // Rank 데이터를 UI_RankFrame에 전달하여 초기화
                rankFrame.Init(rank);
            }
        }
    }
}