using UnityEngine;

namespace StroopGame.Toolkit
{
    /// <summary>
    /// 毫秒級反應時間計時器。
    /// StartTrial() 開始計時 → StopTrial() 結束並回傳 RT (ms)。
    /// </summary>
    public class ReactionTimeRecorder : MonoBehaviour
    {
        private float startTime;
        private bool isRecording;

        public bool IsRecording => isRecording;

        public void StartTrial()
        {
            startTime = Time.realtimeSinceStartup;
            isRecording = true;
        }

        /// <summary>
        /// 結束計時並回傳反應時間（毫秒）。
        /// </summary>
        public float StopTrial()
        {
            if (!isRecording) return -1f;
            isRecording = false;
            return (Time.realtimeSinceStartup - startTime) * 1000f;
        }

        public float PeekCurrentRTms()
        {
            if (!isRecording) return -1f;
            return (Time.realtimeSinceStartup - startTime) * 1000f;
        }
    }
}
