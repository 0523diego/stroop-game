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

            if (titleText != null) titleText.text = "Result";
            if (scoreText != null) scoreText.text = $"Final Score: {finalScore}";
            if (accuracyText != null) accuracyText.text = $"Accuracy: {accuracy:F1}%";
            if (rtCongruentText != null) rtCongruentText.text = $"Congruent RT: {congRT:F0} ms";
            if (rtIncongruentText != null) rtIncongruentText.text = $"Incongruent RT: {incRT:F0} ms";
            if (interferenceCostText != null) interferenceCostText.text = $"Interference Cost: {cost:F0} ms";
            if (interpretationText != null) interpretationText.text = InterpretCost(cost);
            if (exportPathText != null && !string.IsNullOrEmpty(csvPath))
                exportPathText.text = $"Data saved: {System.IO.Path.GetFileName(csvPath)}";
        }

        string InterpretCost(float costMs)
        {
            if (costMs < 100f) return "Excellent! Immune to illusions.";
            if (costMs < 200f) return "Good interference control.";
            if (costMs < 400f) return "Growing. Practice more.";
            return "Easily fooled. More training needed.";
        }
    }
}
