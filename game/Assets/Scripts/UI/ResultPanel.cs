using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StroopGame.UI
{
    /// <summary>
    /// 結算畫面：顯示最終分數、干擾代價、各項數據。
    /// 老師看到「干擾代價：250 ms」這種數據會覺得專業。
    /// </summary>
    public class ResultPanel : MonoBehaviour
    {
        [Header("UI References")]
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI accuracyText;
        public TextMeshProUGUI rtCongruentText;
        public TextMeshProUGUI rtIncongruentText;
        public TextMeshProUGUI interferenceCostText;
        public TextMeshProUGUI interpretationText;
        public TextMeshProUGUI exportPathText;

        [Header("Buttons")]
        public Button playAgainButton;
        public Button quitButton;

        public event System.Action OnPlayAgain;
        public event System.Action OnQuit;

        void Awake()
        {
            if (playAgainButton != null)
                playAgainButton.onClick.AddListener(() => OnPlayAgain?.Invoke());
            if (quitButton != null)
                quitButton.onClick.AddListener(() =>
                {
                    OnQuit?.Invoke();
                    #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                    #else
                    Application.Quit();
                    #endif
                });
        }

        public void Show(int finalScore, int totalTrials, DataLogger logger, string csvPath = null)
        {
            gameObject.SetActive(true);

            float congRT = logger.AverageRT(TrialCondition.Congruent);
            float incRT = logger.AverageRT(TrialCondition.Incongruent);
            float cost = logger.InterferenceCostMs();
            float accuracy = logger.OverallAccuracy() * 100f;

            if (titleText != null) titleText.text = "結果";
            if (scoreText != null) scoreText.text = $"最終得分：{finalScore}";
            if (accuracyText != null) accuracyText.text = $"準確率：{accuracy:F1}%";
            if (rtCongruentText != null) rtCongruentText.text = $"一致條件反應時間：{congRT:F0} 毫秒";
            if (rtIncongruentText != null) rtIncongruentText.text = $"不同條件反應時間：{incRT:F0} 毫秒";
            if (interferenceCostText != null) interferenceCostText.text = $"干擾成本：{cost:F0} 毫秒";
            if (interpretationText != null) interpretationText.text = InterpretCost(cost);
            if (exportPathText != null && !string.IsNullOrEmpty(csvPath))
                exportPathText.text = $"Data saved: {System.IO.Path.GetFileName(csvPath)}";
        }

        string InterpretCost(float costMs)
        {
            if (costMs < 100f) return "太棒了！你完全不受幻覺影響。";
            if (costMs < 200f) return "不錯，你擁有優秀的干擾控制。";
            if (costMs < 400f) return "你擁有很多的成長空間呢，再多加練習吧！";
            return "你太容易上當了，需要更多培訓！";
        }
    }
}
