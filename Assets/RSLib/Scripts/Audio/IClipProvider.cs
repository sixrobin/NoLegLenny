namespace RSLib.Audio
{
    public interface IClipProvider
    {
        UnityEngine.Audio.AudioMixerGroup MixerGroup { get; }

        AudioClipPlayDatas GetNextClipDatas();
    }
}