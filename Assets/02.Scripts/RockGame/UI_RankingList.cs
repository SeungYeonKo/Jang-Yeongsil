using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _02.Scripts.RockGame
{
    public class UI_RankingList : MonoBehaviour
    {
        public RankManager rankManager; // RankManager를 참조합니다.
        public List<UI_RankFrame> UIRankFrames;
        void Start()
        {
            Refresh();
        }

        public void Refresh()
        {
            List<Rank> topRanks = rankManager.GetTopRanks(4);

            // 기존 UI 항목 비활성화
            foreach (var uiRank in UIRankFrames)
            {
                uiRank.gameObject.SetActive(false);
            }

            for (int i = 0; i < topRanks.Count && i < UIRankFrames.Count; i++)
            {
                UIRankFrames[i].gameObject.SetActive(true);
                topRanks[i].RankPosition = i + 1;
                UIRankFrames[i].Init(topRanks[i]);
            }
        }
    }
}