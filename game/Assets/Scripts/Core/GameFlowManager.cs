using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using StroopGame.Toolkit;
using StroopGame.UI;

namespace StroopGame
{
    /// <summary>
    /// 中央導演（Director）。
    /// 負責：生成刺激物 → 計時 → 玩家輸入 → 判定 → 回饋 → 數據紀錄 → 下一回合。
    /// 借自 Mind-Shuffle GameFlowManager 的「導演」角色模式。
    /// </summary>
    public class GameFlowManager : MonoBehaviour
    {
        [Header("=== Stimulus ===")]
        [Tooltip("刺激物 Prefab（必須掛 StimulusItem 與 Image）")]
        public StimulusItem stimulusPrefab;
        [Tooltip("生成位置（通常是 Canvas 中央的空 GameObject）")]
        public RectTransform spawnPoint;

        [Header("=== Modules ===")]
        public DifficultyManager difficulty;
        public DataLogger logger;
        public ScoreManager scoreManager;
        public TrialCounter trialCounter;
        public CountdownTimer countdown;
        public ReactionTimeRecorder rtRecorder;

        [Header("=== UI ===")]
        public Button redFireballButton;
        public Button blueWaterballButton;
        public TextMeshProUGUI stageText;
        public FeedbackText feedback;
        public StunOverlay stunOverlay;
        public ResultPanel resultPanel;

        [Header("=== Scoring ===")]
        public int pointsOnCorrect = 10;
        public int pointsOnWrong = 5;
        public int bonusOnIncongruentCorrect = 5;

        [Header("=== Timing ===")]
        public float stunDurationOnWrong = 3.0f;
        public float interTrialDelay = 0.4f;

        // === Runtime state ===
        StimulusItem currentStimulus;
        bool waitingForInput;

        void Start()
        {
            // 按鈕綁定
            redFireballButton.onClick.AddListener(() => OnAttack(AttackType.RedFireball));
            blueWaterballButton.onClick.AddListener(() => OnAttack(AttackType.BlueWaterball));

            // 各模組初始化
            difficulty.ResetProgress();
            logger.Clear();
            scoreManager.ResetScore();
            trialCounter.ResetCounter();

            // 監聽事件
            trialCounter.OnAllTrialsComplete += OnAllTrialsComplete;
            countdown.OnTimeout += OnCountdownTimeout;
            difficulty.OnStageAdvanced += OnStageAdvanced;
            if (resultPanel != null)
            {
                resultPanel.OnPlayAgain += RestartGame;
                resultPanel.gameObject.SetActive(false);
            }

            UpdateStageDisplay();
            StartCoroutine(RunTrial());
        }

        IEnumerator RunTrial()
        {
            // 短暫間隔
            yield return new WaitForSeconds(interTrialDelay);

            // 1. 決定怪物屬性
            bool incongruent = difficulty.ShouldGenerateIncongruent();
            StimulusShape shape = (Random.value < 0.5f) ? StimulusShape.Fire : StimulusShape.Water;
            StimulusColor color = incongruent
                ? (shape == StimulusShape.Fire ? StimulusColor.Blue : StimulusColor.Red)
                : (shape == StimulusShape.Fire ? StimulusColor.Red : StimulusColor.Blue);

            // 2. 生成（從 Prefab Instantiate 到 spawnPoint）
            currentStimulus = Instantiate(stimulusPrefab, spawnPoint);
            currentStimulus.transform.localPosition = Vector3.zero;
            currentStimulus.transform.localScale = Vector3.one;
            currentStimulus.Setup(shape, color);

            // 3. 開始計時 + 倒數
            waitingForInput = true;
            rtRecorder.StartTrial();
            countdown.StartCountdown(difficulty.CurrentTimeLimit());
        }

        // === 玩家輸入 ===
        void OnAttack(AttackType attack)
        {
            if (!waitingForInput) return;
            if (stunOverlay != null && stunOverlay.IsStunned) return;

            waitingForInput = false;
            countdown.Cancel();
            float rtMs = rtRecorder.StopTrial();
            bool correct = currentStimulus.IsCorrectAttack(attack);

            LogResult(attack, correct, rtMs, timedOut: false);

            if (correct)
            {
                int gained = pointsOnCorrect
                    + (currentStimulus.Condition == TrialCondition.Incongruent ? bonusOnIncongruentCorrect : 0);
                scoreManager.AddScore(gained);

                bool leveled = difficulty.RegisterCorrect();
                if (leveled) feedback.ShowLevelUp();
                else feedback.ShowCorrect();

                EndTrial();
            }
            else
            {
                scoreManager.DeductScore(pointsOnWrong);
                feedback.ShowWrong();
                if (stunOverlay != null) stunOverlay.TriggerStun(stunDurationOnWrong);
                StartCoroutine(WrongAnswerCooldown());
            }
        }

        IEnumerator WrongAnswerCooldown()
        {
            redFireballButton.interactable = false;
            blueWaterballButton.interactable = false;
            yield return new WaitForSeconds(stunDurationOnWrong);
            redFireballButton.interactable = true;
            blueWaterballButton.interactable = true;
            EndTrial();
        }

        // === 倒數超時 ===
        void OnCountdownTimeout()
        {
            if (!waitingForInput) return;
            waitingForInput = false;
            float rtMs = rtRecorder.StopTrial();
            LogResult(AttackType.RedFireball, correct: false, rtMs, timedOut: true);
            feedback.Show("Too Slow!", new Color(1f, 0.6f, 0.2f));
            scoreManager.DeductScore(pointsOnWrong);
            EndTrial();
        }

        // === 結束此回合，進下一回合 ===
        void EndTrial()
        {
            if (currentStimulus != null) Destroy(currentStimulus.gameObject);
            trialCounter.NextTrial();
            if (!trialCounter.IsAllComplete) StartCoroutine(RunTrial());
        }

        void OnAllTrialsComplete()
        {
            if (currentStimulus != null) Destroy(currentStimulus.gameObject);
            countdown.Cancel();

            string csvPath = logger.ExportCsvFile();
            if (resultPanel != null)
                resultPanel.Show(scoreManager.CurrentScore, trialCounter.TotalTrials, logger, csvPath);
        }

        void OnStageAdvanced(DifficultyStage newStage)
        {
            UpdateStageDisplay();
        }

        void UpdateStageDisplay()
        {
            if (stageText != null)
                stageText.text = difficulty.GetStageDisplayName();
        }

        void LogResult(AttackType response, bool correct, float rtMs, bool timedOut)
        {
            logger.LogTrial(new TrialResult
            {
                trialIndex = trialCounter.CurrentTrial + 1,
                stage = difficulty.CurrentStage,
                shape = currentStimulus.Shape,
                color = currentStimulus.Color,
                condition = currentStimulus.Condition,
                response = response,
                isCorrect = correct,
                reactionTimeMs = rtMs,
                timedOut = timedOut,
                timestamp = System.DateTime.UtcNow
            });
        }

        public void RestartGame()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
    }
}
