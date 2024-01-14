namespace JuiceJam
{
    using UnityEngine;
    using UnityEngine.Audio;

    public class LowPassTween : RSLib.Framework.Singleton<LowPassTween>
    {
        private const string CUTOFF_FREQ_ID = "CutoffFreq";
        private const string RESONANCE_ID = "Resonance";
        
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
            _mixer.GetFloat(CUTOFF_FREQ_ID, out float initCutoffFrequency);
            _mixer.GetFloat(RESONANCE_ID, out float initResonance);

            _mixer.SetFloat(CUTOFF_FREQ_ID, targetCutoffFrequency);
            _mixer.SetFloat(RESONANCE_ID, targetResonance);

            RSLib.Audio.AudioManager.PlayNextPlaylistSound(_bassDropClip);

            for (float t = 0f; t <= 1f; t += Time.deltaTime / outDuration)
            {
                _mixer.SetFloat(CUTOFF_FREQ_ID, Mathf.Lerp(targetCutoffFrequency, initCutoffFrequency, t));
                _mixer.SetFloat(RESONANCE_ID, Mathf.Lerp(targetResonance, initResonance, t));
                yield return null;
            }

            _mixer.SetFloat(CUTOFF_FREQ_ID, initCutoffFrequency);
            _mixer.SetFloat(RESONANCE_ID, initResonance);

            IsTweening = false;
        }
    }
}