namespace JuiceJam
{
    using RSLib.Maths;
    using UnityEngine;

    public class DitherFade : RSLib.Framework.Singleton<DitherFade>
    {
        [SerializeField] private Sprite[] _spritesSequence = null;
        [SerializeField] private SpriteRenderer _ditherSprite = null;

        public static void FadeIn(float duration, Curve curve, float delay = 0f, System.Action callback = null)
        {
            Instance.StartCoroutine(Instance.FadeCoroutine(duration, curve, true, delay, callback));
        }

        public static void FadeOut(float duration, Curve curve, float delay = 0f, System.Action callback = null)
        {
            Instance.StartCoroutine(Instance.FadeCoroutine(duration, curve, false, delay, callback));
        }

        private System.Collections.IEnumerator FadeCoroutine(float duration, Curve curve, bool fadeIn, float delay, System.Action callback)
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(delay);

            _ditherSprite.enabled = true;

            for (float t = 0f; t < 1f; t += Time.deltaTime / duration)
            {
                float lerp = fadeIn ? Mathf.Lerp(1f, 0f, t.Ease(curve)) : Mathf.Lerp(0f, 1f, t.Ease(curve));
                int spriteIndex = Mathf.RoundToInt(lerp * (_spritesSequence.Length - 1));

                _ditherSprite.sprite = _spritesSequence[spriteIndex];

                yield return null;
            }

            if (fadeIn)
                _ditherSprite.sprite = _spritesSequence[0];
            else
                _ditherSprite.enabled = false;

            callback?.Invoke();
        }

        protected override void Awake()
        {
            base.Awake();

            _ditherSprite.enabled = false;
        }
    }
}