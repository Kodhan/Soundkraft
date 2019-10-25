using UnityEngine;
namespace SoundKraft
{ 
    public class AudioController 
    {
        public bool Paused { get; private set; }
        public bool Destroy { get; private set; }
        public float DesiredVolume { get; private set; }
        public float DesiredPitch { get; private set; }
        public bool Looping { get; private set; }
        
        public void Play()
       {
           Paused = false;
       }
        public void Pause(bool isPaused = true)
       {
           Paused = isPaused;
       }
        public void Stop()
       {
           Destroy = true;
       }
        public void SetVolume(float volume)
       {
           DesiredVolume = volume;
       }
       public void SetPitch(float pitch)
       {
           DesiredPitch = pitch;
       }
       public void SetLooping(bool loop)
       {
           Looping = loop;
       }
   }
}
