using UnityEngine;
using UnityEngine.UI;

namespace StroopGame.UI
{
    /// <summary>
    /// 暈眩特效。錯誤時觸發：
    /// - 紫色覆蓋層淡入
    /// - 螢幕輕微抖動（可選）
    /// - 鎖定攻擊按鈕 N 秒
    ///
    /// 理論依據：Metcalfe & Mischel (1999) 冷/熱認知系統
    /// 強制中斷玩家「狂按」的衝動行為，逼迫切換回冷認知。
    /// </summary>
    public class StunOverlay : MonoBehaviour
    {
        [Header("References")]
        public Image stunImage;
        public RectTransform shakeTarget;

        [Header("Visuals")]
        public Color stunColor = new Color(0.4f, 0.1f, 0.6f, 0.7f);
        public float fadeInDuration = 0.15f;
        public float fadeOutDuration = 0.5f;

        [Header("Screen Shake")]
        public bool enableShake = true;
        public float shakeMagnitude = 12f;
        public float shakeDuration = 0.25f;

        public bool IsStunned { get; private set; }

        void Awake()
        {
            if (stunImage != null) stunImage.color = new Color(stunColor.r, stunColor.g, stunColor.b, 0f);
        }

        public void TriggerStun(float duration)
        {
            if (gameObject.activeInHierarchy)
                StartCoroutine(StunCoroutine(duration));
        }

        System.Collections.IEnumerator StunCoroutine(float duration)
        {
            IsStunned = true;

            // 抖動
            if (enableShake && shakeTarget != null)
                StartCoroutine(Shake());

            // 淡入
            yield return Fade(0f, stunColor.a, fadeInDuration);

            // 保持
            float hold = Mathf.Max(0f, duration - fadeInDuration - fadeOutDuration);
            yield return new WaitForSeconds(hold);

            // 淡出
            yield return Fade(stunColor.a, 0f, fadeOutDuration);

            IsStunned = false;
        }

        System.Collections.IEnumerator Fade(float from, float to, float dur)
        {
            if (stunImage == null) yield break;
            float t = 0f;
            while (t < dur)
            {
                t += Time.deltaTime;
                float a = Mathf.Lerp(from, to, t / dur);
                stunImage.color = new Color(stunColor.r, stunColor.g, stunColor.b, a);
                yield return null;
            }
            stunImage.color = new Color(stunColor.r, stunColor.g, stunColor.b, to);
        }

        System.Collections.IEnumerator Shake()
        {
            Vector3 origin = shakeTarget.localPosition;
            float elapsed = 0f;
            while (elapsed < shakeDuration)
            {
                float x = (Random.value - 0.5f) * 2f * shakeMagnitude;
                float y = (Random.value - 0.5f) * 2f * shakeMagnitude;
                shakeTarget.localPosition = origin + new Vector3(x, y, 0f);
                elapsed += Time.deltaTime;
                yield return null;
            }
            shakeTarget.localPosition = origin;
        }
    }
}
