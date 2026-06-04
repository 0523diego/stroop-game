using TMPro;
using UnityEngine;

namespace StroopGame.UI
{
    /// <summary>
    /// 中央回饋文字（「正確！」「被騙了！」「升級！」）。
    /// 自動淡入 → 停留 → 淡出。
    /// </summary>
    public class FeedbackText : MonoBehaviour
    {
        [Header("References")]
        public TextMeshProUGUI text;

        [Header("Timing")]
        public float showDuration = 0.6f;
        public float fadeDuration = 0.2f;

        [Header("Colors")]
        public Color correctColor = new Color(0.3f, 0.95f, 0.45f);
        public Color wrongColor = new Color(0.95f, 0.3f, 0.3f);
        public Color levelUpColor = new Color(1f, 0.85f, 0.2f);

        Coroutine current;

        public void ShowCorrect(string message = "Correct!")
            => Show(message, correctColor);

        public void ShowWrong(string message = "Fooled by color!")
            => Show(message, wrongColor);

        public void ShowLevelUp(string message = "Level Up!")
            => Show(message, levelUpColor);

        public void Show(string message, Color color)
        {
            if (text == null) return;
            if (current != null) StopCoroutine(current);
            text.text = message;
            text.color = new Color(color.r, color.g, color.b, 0f);
            current = StartCoroutine(Sequence(color));
        }

        System.Collections.IEnumerator Sequence(Color target)
        {
            // Fade in
            yield return FadeAlpha(0f, 1f, target);
            // Hold
            yield return new WaitForSeconds(showDuration);
            // Fade out
            yield return FadeAlpha(1f, 0f, target);
            text.text = "";
        }

        System.Collections.IEnumerator FadeAlpha(float from, float to, Color baseColor)
        {
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                float a = Mathf.Lerp(from, to, t / fadeDuration);
                text.color = new Color(baseColor.r, baseColor.g, baseColor.b, a);
                yield return null;
            }
            text.color = new Color(baseColor.r, baseColor.g, baseColor.b, to);
        }
    }
}
