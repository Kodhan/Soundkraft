using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SoundKraft
{
    public class AudioController : MonoBehaviour
   {
        [SerializeField] private bool _playOnCameraZ = true;
        private bool _paused, _stoping;
        private AudioSource _audioSourceHelper;
        private AudioSource _audioSource
        {
            get
            {
                if (!_audioSourceHelper)
                    _audioSourceHelper = GetComponent<AudioSource>();
                if (!_audioSourceHelper)
                    _audioSourceHelper = gameObject.AddComponent<AudioSource>();

                return _audioSourceHelper;
            }
        }
        private float _fadeingOutTimer, _maxVolume;
        private AudioObject _audioInfo;
        public bool IsPlaying
        {
            get { return _audioSource && _audioSource.isPlaying; }
        }

        public float Volume => _audioSource.volume;

        public void SetUp(AudioObject audioInfo)
        {
            if (!_audioSource)

                if (audioInfo.Clips.Count == 0)
                {
                    Debug.LogWarning("There is no audio clip in " + audioInfo.name);
                    return;
                }

            SetAudioObject(_audioSource, audioInfo);
            _maxVolume = _audioSource.volume;
            _audioSource.Play();
            _fadeingOutTimer = audioInfo.FadeOutTime;
            if (audioInfo.FadeInTime > 0) _audioSource.volume = 0;
            _audioInfo = audioInfo;
            _stoping = false;
            _paused = false;
            _playOnCameraZ = audioInfo.Sound2D;
        }

        public static void SetAudioObject(AudioSource source, AudioObject audioInfo)
        {
            source.clip = audioInfo.GetClip();
            source.rolloffMode = audioInfo.RolloffMode;
            source.volume = audioInfo.Volume;
            source.pitch = audioInfo.Pitch;
            source.minDistance = audioInfo.Distance.x;
            source.maxDistance = audioInfo.Distance.y;
            source.spatialBlend = audioInfo.SpatialBlend;
            source.loop = audioInfo.IsLooping;
            source.outputAudioMixerGroup = audioInfo.MixerGroup;
        }

        internal bool IsLooping()
        {
            return _audioSource.loop;
        }

        void Update()
        {
            if(_playOnCameraZ && Camera.main)
            transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);

            if (_audioSource.time < _audioInfo.FadeInTime)
                _audioSource.volume = _audioInfo.FadeInCurve.Evaluate(_audioSource.time / _audioInfo.FadeInTime) * _maxVolume;
            else if (_audioSource.time > _audioSource.clip.length - _audioInfo.FadeOutTime)
                _audioSource.volume = _audioInfo.FadeOutCurve.Evaluate((_audioSource.clip.length - _audioSource.time) / _audioInfo.FadeOutTime) * _maxVolume;
            else if (_stoping)
            {
                if (_fadeingOutTimer <= 0 && _paused)
                {
                    _audioSource.Pause();
                    _stoping = false;
                }

                else if (_fadeingOutTimer <= 0)
                {
                    _audioSource.Stop();
                    ObjectPool.Destroy(gameObject);
                    _stoping = false;
                }

                _fadeingOutTimer -= Time.deltaTime;
                _audioSource.volume = _audioInfo.FadeOutCurve.Evaluate(_fadeingOutTimer / _audioInfo.FadeOutTime) * _maxVolume;
            }

            if (IsPlaying && !_paused) return;
            Stop();
        }

        public void Pause()
        {
            _stoping = true;
            _paused = true;
        }

        public void Play()
        {
            _paused = false;
            _stoping = false;
            _audioSource.Play();
        }

        public void Stop()
        {
            _stoping = true;
        }

        public void SetVolume(MathHelper.FloatMinMax volumeFactor)
        {
            float volume = _audioInfo.Volume.Evaluate(volumeFactor.GetRandom());
            _audioSource.volume = volume;
        }
        public void SetVolume(float volume)
        {
             _audioSource.volume = volume;
        }
    }
}
