using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _02.Scripts.RockGame
{
    public class UI_RankFrame : MonoBehaviour
    {
        public TextMeshProUGUI RankText;
        public GameObject MaleIcon;
        public GameObject FemaleIcon;
        public TextMeshProUGUI PlayerNameText;
        public TextMeshProUGUI PlayerScoreText;
        public TextMeshProUGUI DateNumberText;

        private Rank _rank;

        public void Init(Rank rank)
        {
            _rank = rank;
            RankText.text = _rank.RankPosition.ToString();

            // 플레이어 이름과 점수를 표시
            PlayerNameText.text = _rank.Name;
            PlayerScoreText.text = _rank.Score.ToString();

            // 날짜를 표시 (포맷은 필요에 따라 수정)
            DateNumberText.text = _rank.DateTime.ToString("yy-MM-dd HH:mm");

            // 성별 아이콘 표시
            if (_rank.SelectCharacter == CharacterGender.Male)
            {
                MaleIcon.SetActive(true);
                FemaleIcon.SetActive(false);
            }
            else
            {
                MaleIcon.SetActive(false);
                FemaleIcon.SetActive(true);
            }
        }
    }
}