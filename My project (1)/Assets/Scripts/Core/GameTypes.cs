using System;

namespace StroopGame
{
    /// <summary>
    /// 怪物形狀 — 目標屬性（玩家要辨識的）
    /// </summary>
    public enum StimulusShape
    {
        Fire,   // 火焰 → 按紅火球
        Water   // 水滴 → 按藍水球
    }

    /// <summary>
    /// 怪物顏色 — 干擾屬性（玩家要忽略的）
    /// </summary>
    public enum StimulusColor
    {
        Red,
        Blue
    }

    /// <summary>
    /// 玩家攻擊類型
    /// </summary>
    public enum AttackType
    {
        RedFireball,
        BlueWaterball
    }

    /// <summary>
    /// 試驗情境
    /// </summary>
    public enum TrialCondition
    {
        Congruent,   // 顏色 / 形狀相符（紅火、藍水）
        Incongruent  // 衝突（藍火、紅水）
    }

    /// <summary>
    /// 難度階段
    /// </summary>
    public enum DifficultyStage
    {
        Stage1_Training = 1,
        Stage2_Mixed = 2,
        Stage3_TimePressure = 3
    }

    /// <summary>
    /// 單次試驗結果（給 DataLogger 用）
    /// </summary>
    [Serializable]
    public class TrialResult
    {
        public int trialIndex;
        public DifficultyStage stage;
        public StimulusShape shape;
        public StimulusColor color;
        public TrialCondition condition;
        public AttackType response;
        public bool isCorrect;
        public float reactionTimeMs;
        public bool timedOut;
        public DateTime timestamp;
    }
}
