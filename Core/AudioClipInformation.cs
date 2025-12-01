using UnityEngine;

namespace Sonar
{
    public class AudioClipInformation
    {
        private AudioClip audioClip;
        public AudioClip AudioClip => audioClip;

        private float volume;
        public float Volume => volume;

        public AudioClipInformation(AudioClip audioClip, float volume)
        {
            this.audioClip = audioClip;
            this.volume = volume;
        }
    }    
}
