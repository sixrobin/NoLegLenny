namespace RSLib.Audio
{
    public abstract class ClipProvider : UnityEngine.ScriptableObject, IClipProvider
    {
        [UnityEngine.SerializeField] private UnityEngine.Audio.AudioMixerGroup _mixerGroup = null;

        public UnityEngine.Audio.AudioMixerGroup MixerGroup => _mixerGroup;

        public virtual void Init() { }
        public abstract AudioClipPlayDatas GetNextClipDatas();
    }
}