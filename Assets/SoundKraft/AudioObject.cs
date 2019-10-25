using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace SoundKraft
{

    [CreateAssetMenu(menuName = "SoundKraft/AudioObject", fileName = "AudioObject")]
    public class AudioObject : ScriptableObject
    {
        public AudioMixerGroup MixerGroup;
        public List<AudioClip> Clips;

        [MinMax(0, 1, ShowEditRange = true)] public MathHelper.FloatMinMax Volume = new MathHelper.FloatMinMax(1, 1);
        [MinMax(0, 3, ShowEditRange = true)] public MathHelper.FloatMinMax Pitch = new MathHelper.FloatMinMax(1, 1);
        [MinMax(1, 1000, ShowEditRange = true)] public Vector2 Distance = new Vector2(1, 1000);
        [Range(0, 1)] public float SpatialBlend = 1;
        public AudioRolloffMode RolloffMode = AudioRolloffMode.Logarithmic;
        public bool IsLooping;
        [Header("Fade in")]
        public float FadeInTime = 0;
        public AnimationCurve FadeInCurve;
        [Header("Fade out")]
        public float FadeOutTime = 0;
        public AnimationCurve FadeOutCurve;
        public delegate void AuidoEvent(AudioSource source);
        private static AudioListener _audioListener;

        private List<int> _indexList;
        
        public AudioClip GetClip()
        {
            if(Clips.Count < 3)
            {
                return Clips[Random.Range(0, Clips.Count)];
            }

            if (_indexList == null || _indexList.Count != Clips.Count)
            {
                _indexList = null;
                _indexList = new List<int>();
                for (int i = 0; i < Clips.Count; i++)
                {
                    _indexList.Add(i);
                }
            }


            int quarantinedIndex = Mathf.CeilToInt(Clips.Count / 3f);

            int tempIndex = _indexList[Random.Range(quarantinedIndex + 1, _indexList.Count)];

            _indexList.Remove(tempIndex);
            _indexList.Insert(0, tempIndex);

            return Clips[tempIndex];
        }

        public void TestPlay()
        {
            IEnumerable<EditorAudioTester> testerList = FindObjectsOfType<EditorAudioTester>().Where(audio => audio.IsPlayingAudioObject(this));
            if (IsLooping && testerList.ToArray().Length > 0)
            {
                foreach (var editorAudioTester in testerList)
                {
                    DestroyImmediate(editorAudioTester.gameObject);
                }
            }
            else
                new GameObject("EditorAudio(temp)").AddComponent<EditorAudioTester>().Initialize(this);
            
    
   
        }
    }
}