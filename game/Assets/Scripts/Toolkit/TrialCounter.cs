using TMPro;
using UnityEngine;

namespace StroopGame.Toolkit
{
    /// <summary>
    /// 自動化回合管理（借自 Mind-Shuffle 的 TrialCounter 概念）。
    /// 達到上限自動觸發 OnAllTrialsComplete。
    /// </summary>
    public class TrialCounter : MonoBehaviour
    {
        [Header("Settings")]
        public int totalTrials = 30;

        [Header("UI")]
        public TextMeshProUGUI trialText;
        public string format = "Round {0} / {1}";

        public int CurrentTrial { get; private set; } = 0;
        public int TotalTrials => totalTrials;
        public bool IsAllComplete => CurrentTrial >= totalTrials;

        public event System.Action OnAllTrialsComplete;

        void Start() => UpdateUI();

        public void NextTrial()
        {
            CurrentTrial++;
            UpdateUI();
            if (CurrentTrial >= totalTrials)
            {
                OnAllTrialsComplete?.Invoke();
            }
        }

        public void ResetCounter()
        {
            CurrentTrial = 0;
            UpdateUI();
        }

        void UpdateUI()
        {
            if (trialText != null)
                trialText.text = string.Format(format, CurrentTrial, totalTrials);
        }
    }
}
