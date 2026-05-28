using TMPro;
using UnityEngine;

namespace StroopGame.Toolkit
{
    /// <summary>
    /// 通用記分板（借自 Mind-Shuffle ScoreManager 架構）。
    /// 不負責判斷遊戲規則，只負責加減分與 UI 更新。
    /// </summary>
    public class ScoreManager : MonoBehaviour
    {
        [Header("UI")]
        public TextMeshProUGUI scoreText;
        public string prefix = "分數：";

        [Header("Animation")]
        public float popScale = 1.3f;
        public float popDuration = 0.2f;

        public int CurrentScore { get; private set; }

        void Start() => UpdateUI();

        /// <summary>
        /// 加分。
        /// </summary>
        public void AddScore(int points)
        {
            CurrentScore += points;
            UpdateUI();
            if (scoreText != null) StartCoroutine(Pop());
        }

        /// <summary>
        /// 扣分（自動防呆，不會低於 0）。
        /// </summary>
        public void DeductScore(int points)
        {
            CurrentScore = Mathf.Max(0, CurrentScore - points);
            UpdateUI();
            if (scoreText != null) StartCoroutine(Pop());
        }

        public void ResetScore()
        {
            CurrentScore = 0;
            UpdateUI();
        }

        void UpdateUI()
        {
            if (scoreText != null) scoreText.text = prefix + CurrentScore;
        }

        System.Collections.IEnumerator Pop()
        {
            Vector3 baseScale = Vector3.one;
            float t = 0f;
            while (t < popDuration)
            {
                t += Time.deltaTime;
                float p = t / popDuration;
                float s = Mathf.Lerp(popScale, 1f, p);
                scoreText.transform.localScale = baseScale * s;
                yield return null;
            }
            scoreText.transform.localScale = baseScale;
        }
    }
}
