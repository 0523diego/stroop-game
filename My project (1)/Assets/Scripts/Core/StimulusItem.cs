using UnityEngine;
using UnityEngine.UI;

namespace StroopGame
{
    /// <summary>
    /// 場上的單一刺激物（怪物）。
    /// 由 GameFlowManager 在 Instantiate 後呼叫 Setup() 配置屬性。
    ///
    /// 設計借自 Mind-Shuffle 的 Prefab + ItemProperty 模式：
    /// 一個白色模板 Prefab，透過程式動態染色，產生多種衝突條件。
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class StimulusItem : MonoBehaviour
    {
        [Header("Shape Sprites (透明背景的白色形狀)")]
        public Sprite fireSprite;
        public Sprite waterSprite;

        [Header("Color Mapping (動態染色)")]
        public Color redTint = new Color(0.96f, 0.30f, 0.18f);
        public Color blueTint = new Color(0.20f, 0.55f, 0.95f);

        [Header("出現 / 消失 動畫")]
        public float fadeInDuration = 0.15f;

        private Image image;

        public StimulusShape Shape { get; private set; }
        public StimulusColor Color { get; private set; }
        public TrialCondition Condition { get; private set; }

        void Awake()
        {
            image = GetComponent<Image>();
        }

        /// <summary>
        /// 由 GameFlowManager 在生成此刺激物後呼叫。
        /// </summary>
        public void Setup(StimulusShape shape, StimulusColor color)
        {
            if (image == null) image = GetComponent<Image>();

            Shape = shape;
            Color = color;
            Condition = IsCongruent(shape, color) ? TrialCondition.Congruent : TrialCondition.Incongruent;

            // 套用視覺
            image.sprite = (shape == StimulusShape.Fire) ? fireSprite : waterSprite;
            image.color = (color == StimulusColor.Red) ? redTint : blueTint;

            // 簡單淡入
            StartCoroutine(FadeIn());
        }

        /// <summary>
        /// 判斷玩家攻擊類型是否正確（只看形狀，不看顏色）。
        /// </summary>
        public bool IsCorrectAttack(AttackType attack)
        {
            return (Shape == StimulusShape.Fire && attack == AttackType.RedFireball)
                || (Shape == StimulusShape.Water && attack == AttackType.BlueWaterball);
        }

        static bool IsCongruent(StimulusShape s, StimulusColor c)
        {
            return (s == StimulusShape.Fire && c == StimulusColor.Red)
                || (s == StimulusShape.Water && c == StimulusColor.Blue);
        }

        System.Collections.IEnumerator FadeIn()
        {
            float t = 0f;
            Color targetColor = image.color;
            Color startColor = new Color(targetColor.r, targetColor.g, targetColor.b, 0f);
            image.color = startColor;
            while (t < fadeInDuration)
            {
                t += Time.deltaTime;
                image.color = UnityEngine.Color.Lerp(startColor, targetColor, t / fadeInDuration);
                yield return null;
            }
            image.color = targetColor;
        }
    }
}
