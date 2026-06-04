using UnityEngine;

namespace StroopGame
{
    /// <summary>
    /// 三階段難度。
    /// Stage 1: 100% Congruent（建立直覺）
    /// Stage 2: 75% Congruent + 25% Incongruent（測試抗干擾）
    /// Stage 3: 50% Incongruent + 縮短反應時限
    /// </summary>
    public class DifficultyManager : MonoBehaviour
    {
        [Header("Stage Progression")]
        [Tooltip("Stage 1（訓練之地）需要答對幾次進階")]
        public int stage1TrialsToAdvance = 3;
        [Tooltip("Stage 2（幻術森林）需要答對幾次進階")]
        public int stage2TrialsToAdvance = 8;

        [Header("Incongruent Rate by Stage")]
        [Range(0f, 1f)] public float stage2IncongruentRate = 0.25f;
        [Range(0f, 1f)] public float stage3IncongruentRate = 0.5f;

        [Header("Time Limits (seconds)")]
        public float stage1TimeLimit = 2.5f;
        public float stage2TimeLimit = 2.0f;
        public float stage3TimeLimit = 1.0f;

        public DifficultyStage CurrentStage { get; private set; } = DifficultyStage.Stage1_Training;
        public int CorrectInCurrentStage { get; private set; } = 0;

        public event System.Action<DifficultyStage> OnStageAdvanced;

        public void ResetProgress()
        {
            CurrentStage = DifficultyStage.Stage1_Training;
            CorrectInCurrentStage = 0;
        }

        /// <summary>
        /// 通報一次正確；若達標自動進階。回傳是否升級。
        /// </summary>
        public bool RegisterCorrect()
        {
            CorrectInCurrentStage++;
            int target = CurrentStage switch
            {
                DifficultyStage.Stage1_Training => stage1TrialsToAdvance,
                DifficultyStage.Stage2_Mixed => stage2TrialsToAdvance,
                _ => int.MaxValue   // Stage 3 永遠不會「升級」
            };
            if (CorrectInCurrentStage >= target
                && CurrentStage != DifficultyStage.Stage3_TimePressure)
            {
                CurrentStage = (DifficultyStage)((int)CurrentStage + 1);
                CorrectInCurrentStage = 0;
                OnStageAdvanced?.Invoke(CurrentStage);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 下一個怪物是否該是 Incongruent。
        /// </summary>
        public bool ShouldGenerateIncongruent()
        {
            float roll = Random.value;
            return CurrentStage switch
            {
                DifficultyStage.Stage1_Training => false,
                DifficultyStage.Stage2_Mixed => roll < stage2IncongruentRate,
                DifficultyStage.Stage3_TimePressure => roll < stage3IncongruentRate,
                _ => false
            };
        }

        public float CurrentTimeLimit()
        {
            return CurrentStage switch
            {
                DifficultyStage.Stage1_Training => stage1TimeLimit,
                DifficultyStage.Stage2_Mixed => stage2TimeLimit,
                DifficultyStage.Stage3_TimePressure => stage3TimeLimit,
                _ => stage1TimeLimit
            };
        }

        public string GetStageDisplayName()
        {
            return CurrentStage switch
            {
                DifficultyStage.Stage1_Training => "關卡一：訓練營地",
                DifficultyStage.Stage2_Mixed => "關卡二：幻象森林",
                DifficultyStage.Stage3_TimePressure => "關卡三：魔王城堡",
                _ => "Unknown"
            };
        }
    }
}
