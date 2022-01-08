namespace JuiceJam
{
    using UnityEngine;

    public class Rain : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _rainParticleSystem = null;

        public void Play()
        {
            if (!_rainParticleSystem.isPlaying)
                _rainParticleSystem.Play();
        }

        public void Stop()
        {
            _rainParticleSystem.Stop();
        }
    }
}