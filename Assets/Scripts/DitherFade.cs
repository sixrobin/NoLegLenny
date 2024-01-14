namespace JuiceJam
{
    using RSLib.Maths;
    using UnityEngine;

    public class DitherFade : RSLib.Framework.Singleton<DitherFade>
    {
        [SerializeField] private Sprite[] _spritesSequence = null;
        [SerializeField] private SpriteRenderer _ditherSprite = null;
        [SerializeField] private bool _fadeOutOnStart = false;

        public delegate void FadeEventHandler(bool fadeIn);
        public static event FadeEventHandler FadeBegan;
        public static event FadeEventHandler FadeOver;

        public static bool IsFading { get; private set; }

        public static float FadedPercentage { get; private set; }
        public static float FadePercentageEased { get; private set; }

        public static void FadeIn(float duration, Curve curve, float delay = 0f, System.Action callback = null)
        {
            IsFading = true;
            Instance.StartCoroutine(Instance.FadeCoroutine(duration, curve, true, delay, callback));
        }

        public static void FadeOut(float duration, Curve curve, float delay = 0f, System.Action callback = null)
        {
            IsFading = true;
            Instance.StartCoroutine(Instance.FadeCoroutine(duration, curve, false, delay, callback));
        }

        private System.Collections.IEnumerator FadeCoroutine(float duration, Curve curve, bool fadeIn, float delay, System.Action callback)
        {
            FadeBegan?.Invoke(fadeIn);

            if (!fadeIn)
            {
                _ditherSprite.sprite = _spritesSequence[0];
                _ditherSprite.enabled = true;
            }
            else
            {
                _ditherSprite.enabled = false;
            }

            FadedPercentage = fadeIn ? 0f : 1f;

            yield return RSLib.Yield.SharedYields.WaitForSeconds(delay);

            _ditherSprite.enabled = true;

            for (float t = 0f; t < 1f; t += Time.unscaledDeltaTime / duration)
            {
                FadedPercentage = fadeIn ? t : 1f - t;

                float lerp = fadeIn ? Mathf.Lerp(1f, 0f, t.Ease(curve)) : Mathf.Lerp(0f, 1f, t.Ease(curve));
                int spriteIndex = Mathf.RoundToInt(lerp * (_spritesSequence.Length - 1));

                _ditherSprite.sprite = _spritesSequence[spriteIndex];

                yield return null;
            }

            if (fadeIn)
                _ditherSprite.sprite = _spritesSequence[0];
            else
                _ditherSprite.enabled = false;

            IsFading = false;
            FadedPercentage = fadeIn ? 1f : 0f;

            FadeOver?.Invoke(fadeIn);
            callback?.Invoke();
        }

        protected override void Awake()
        {
            base.Awake();

            _ditherSprite.enabled = false;
        }

        private void Start()
        {
            if (_fadeOutOnStart)
                FadeOut(0.5f, Curve.Linear, 0.5f);
        }
    }
}