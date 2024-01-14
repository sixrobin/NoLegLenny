namespace JuiceJam
{
    using UnityEngine;
    using UnityEngine.Audio;

    public class LowPassTween : RSLib.Framework.Singleton<LowPassTween>
    {
        [SerializeField] private AudioMixer _mixer = null;
        [SerializeField] private RSLib.Audio.ClipProvider _bassDropClip = null;

        public static bool IsTweening { get; private set; }

        public void TweenLowPass(float targetCutoffFrequency, float targetResonance, float outDuration)
        {
            if (IsTweening)
                return;

            IsTweening = true;
            StartCoroutine(TweenLowPassCoroutine(targetCutoffFrequency, targetResonance, outDuration));
        }

        private System.Collections.IEnumerator TweenLowPassCoroutine(float targetCutoffFrequency, float targetResonance, float outDuration)
        {
            _mixer.GetFloat("CutoffFreq", out float initCutoffFrequency);
            _mixer.GetFloat("Resonance", out float initResonance);

            _mixer.SetFloat("CutoffFreq", targetCutoffFrequency);
            _mixer.SetFloat("Resonance", targetResonance);

            RSLib.Audio.AudioManager.PlayNextPlaylistSound(_bassDropClip);

            for (float t = 0f; t <= 1f; t += Time.deltaTime / outDuration)
            {
                _mixer.SetFloat("CutoffFreq", Mathf.Lerp(targetCutoffFrequency, initCutoffFrequency, t));
                _mixer.SetFloat("Resonance", Mathf.Lerp(targetResonance, initResonance, t));
                yield return null;
            }

            _mixer.SetFloat("CutoffFreq", initCutoffFrequency);
            _mixer.SetFloat("Resonance", initResonance);

            IsTweening = false;
        }
    }
}