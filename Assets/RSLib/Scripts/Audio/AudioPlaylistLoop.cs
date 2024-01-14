﻿namespace RSLib.Audio
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Looping Playlist", menuName = "RSLib/Audio/Playlist/Loop")]
    public class AudioPlaylistLoop : ClipProvider
    {
        [SerializeField] private AudioClipPlayDatas[] _clipsPlayDatas = null;

        private Framework.Collections.Loop<AudioClipPlayDatas> _clipsLoop;

        public override AudioClipPlayDatas GetNextClipDatas()
        {
            if (_clipsLoop == null)
                Init();

            return _clipsLoop.Next();
        }

        [ContextMenu("Init")]
        public override void Init()
        {
            _clipsLoop = new Framework.Collections.Loop<AudioClipPlayDatas>(_clipsPlayDatas);
        }
    }
}