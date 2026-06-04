using TMPro;
using UnityEngine;

namespace StroopGame.Toolkit
{
    /// <summary>
    /// 每回合的倒數計時器（借自 Mind-Shuffle 的 CountdownTimer）。
    /// StartCountdown(seconds) → 倒數結束觸發 OnTimeout。
    /// 可用 Cancel() 中斷（玩家正確按了按鈕時）。
    /// </summary>
    public class CountdownTimer : MonoBehaviour
    {
        [Header("UI")]
        public TextMeshProUGUI timerText;

        [Header("Visuals")]
        public Color warningColor = new Color(1f, 0.3f, 0.2f);
        [Tooltip("剩餘多少秒時轉為警告色")]
        public float warningThreshold = 0.5f;

        public event System.Action OnTimeout;

        private float remaining;
        private bool isRunning;

        void Update()
        {
            if (!isRunning) return;

            remaining -= Time.deltaTime;

            if (timerText != null)
            {
                timerText.text = remaining.ToString("F1") + "s";
                timerText.color = remaining < warningThreshold ? warningColor : UnityEngine.Color.white;
            }

            if (remaining <= 0f)
            {
                isRunning = false;
                if (timerText != null) timerText.text = "0.0s";
                OnTimeout?.Invoke();
            }
        }

        public void StartCountdown(float seconds)
        {
            remaining = seconds;
            isRunning = true;
        }

        public void Cancel()
        {
            isRunning = false;
            if (timerText != null) timerText.text = "";
        }
    }
}
