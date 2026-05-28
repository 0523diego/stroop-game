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

            if (titleText != null) titleText.text = "🏆 冒險結算";
            if (scoreText != null) scoreText.text = $"最終得分：{finalScore}";
            if (accuracyText != null) accuracyText.text = $"正確率：{accuracy:F1}%";
            if (rtCongruentText != null) rtCongruentText.text = $"順境反應：{congRT:F0} ms";
            if (rtIncongruentText != null) rtIncongruentText.text = $"幻術反應：{incRT:F0} ms";
            if (interferenceCostText != null) interferenceCostText.text = $"🧠 干擾代價：{cost:F0} ms";
            if (interpretationText != null) interpretationText.text = InterpretCost(cost);
            if (exportPathText != null && !string.IsNullOrEmpty(csvPath))
                exportPathText.text = $"資料已存：{System.IO.Path.GetFileName(csvPath)}";
        }

        string InterpretCost(float costMs)
        {
            if (costMs < 100f) return "💎 強大！你幾乎不被幻術影響";
            if (costMs < 200f) return "✨ 不錯！抗干擾能力良好";
            if (costMs < 400f) return "🌱 還在成長，多訓練幾次";
            return "💀 容易被騙，需要更多練習";
        }
    }
}
