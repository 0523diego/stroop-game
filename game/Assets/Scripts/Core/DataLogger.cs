using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace StroopGame
{
    /// <summary>
    /// 試驗資料收集 + CSV 匯出。
    /// 同時支援兩種方法：
    /// 1. 直接寫檔（Application.persistentDataPath）
    /// 2. Mind-Shuffle 風格 Debug.Log "@DATA" 印到 Console 給研究員複製
    /// </summary>
    public class DataLogger : MonoBehaviour
    {
        [Header("Console Export")]
        [Tooltip("是否也透過 Debug.Log 印出 @DATA 標記行，方便從 Console 過濾")]
        public bool alsoPrintToConsole = true;

        private readonly List<TrialResult> trials = new List<TrialResult>();

        public int TotalTrials => trials.Count;
        public IReadOnlyList<TrialResult> Trials => trials;

        public void Clear() => trials.Clear();

        public void LogTrial(TrialResult result)
        {
            trials.Add(result);

            if (alsoPrintToConsole)
            {
                // Mind-Shuffle 風格：印 @DATA 前綴方便 Console 過濾
                Debug.Log(
                    "@DATA," + result.trialIndex
                    + "," + result.stage
                    + "," + result.shape
                    + "," + result.color
                    + "," + result.condition
                    + "," + result.response
                    + "," + result.isCorrect
                    + "," + result.reactionTimeMs.ToString("F1")
                    + "," + result.timedOut
                );
            }
        }

        // ===== 統計指標 =====

        /// <summary>
        /// 干擾代價 (Interference Cost) = RT_incongruent − RT_congruent
        /// Stroop 任務的核心指標。數值越小，抗干擾越好。
        /// </summary>
        public float InterferenceCostMs()
        {
            float c = AverageRT(TrialCondition.Congruent);
            float i = AverageRT(TrialCondition.Incongruent);
            if (c <= 0f || i <= 0f) return 0f;
            return i - c;
        }

        public float AverageRT(TrialCondition condition)
        {
            float sum = 0f;
            int n = 0;
            foreach (var t in trials)
            {
                if (t.condition == condition && t.isCorrect && !t.timedOut)
                {
                    sum += t.reactionTimeMs;
                    n++;
                }
            }
            return n > 0 ? sum / n : 0f;
        }

        public float ErrorRate(TrialCondition condition)
        {
            int total = 0, errors = 0;
            foreach (var t in trials)
            {
                if (t.condition != condition) continue;
                total++;
                if (!t.isCorrect) errors++;
            }
            return total > 0 ? (float)errors / total : 0f;
        }

        public float OverallAccuracy()
        {
            if (trials.Count == 0) return 0f;
            int correct = 0;
            foreach (var t in trials) if (t.isCorrect) correct++;
            return (float)correct / trials.Count;
        }

        public int CountByStage(DifficultyStage stage)
        {
            int n = 0;
            foreach (var t in trials) if (t.stage == stage) n++;
            return n;
        }

        /// <summary>
        /// 匯出完整 CSV 到 persistentDataPath。
        /// </summary>
        public string ExportCsvFile()
        {
            var sb = new StringBuilder();
            sb.AppendLine("trial,stage,shape,color,condition,response,correct,rt_ms,timed_out,timestamp");
            foreach (var t in trials)
            {
                sb.Append(t.trialIndex).Append(',')
                  .Append(t.stage).Append(',')
                  .Append(t.shape).Append(',')
                  .Append(t.color).Append(',')
                  .Append(t.condition).Append(',')
                  .Append(t.response).Append(',')
                  .Append(t.isCorrect).Append(',')
                  .Append(t.reactionTimeMs.ToString("F1")).Append(',')
                  .Append(t.timedOut).Append(',')
                  .Append(t.timestamp.ToString("o"))
                  .AppendLine();
            }

            string filename = $"stroop_session_{System.DateTime.Now:yyyyMMdd_HHmmss}.csv";
            string path = Path.Combine(Application.persistentDataPath, filename);
            File.WriteAllText(path, sb.ToString());
            Debug.Log($"[DataLogger] CSV exported to: {path}");
            return path;
        }
    }
}
