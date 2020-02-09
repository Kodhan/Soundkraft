using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace SoundKraft
{

    [CreateAssetMenu(fileName = "AudioObject", menuName = "AudioObject")]
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
        public bool Sound2D = true;
        [Header("Fade in")]
        public float FadeInTime = 0;
        public AnimationCurve FadeInCurve;
        [Header("Fade out")]
        public float FadeOutTime = 0;
        public AnimationCurve FadeOutCurve;
        public delegate void AuidoEvent(AudioSource source);

        [Header("")] public float MinSpawnInterval = 0f;
        private static AudioListener _audioListener;
        private List<int> _indexList;
        private float _lastSpawnTime;
        private AudioController _lastAvailableController;
        private Camera _camera;

        private GameObject AudioSceeneObject
        {
            get
            {

#if UNITY_EDITOR
                if (Resources.Load("AudioSceneObject")) return Resources.Load("AudioSceneObject") as GameObject;
                else
                {
                    GameObject temp = new GameObject("AudioSceneObject");
                    temp.AddComponent<AudioSource>();
                    temp.AddComponent<AudioController>();
                    if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Resources")) UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");
                    UnityEngine.Object prefab = UnityEditor.PrefabUtility.CreateEmptyPrefab("Assets/Resources/AudioSceneObject.prefab");
                    UnityEditor.PrefabUtility.ReplacePrefab(temp, prefab, UnityEditor.ReplacePrefabOptions.ConnectToPrefab);
                }

#endif
                return Resources.Load("AudioSceneObject") as GameObject;
            }
        }

        public AudioController Play(Vector3 pos = default(Vector3), Transform parent = default(Transform), float delay = 0)
        {
            if (Time.time - _lastSpawnTime < MinSpawnInterval && _lastAvailableController != null)
                return _lastAvailableController;
                
            _lastAvailableController = ObjectPool.Instantiate(AudioSceeneObject, pos, Quaternion.identity, parent).GetComponent<AudioController>();

            _lastAvailableController.SetUp(this);
            _lastAvailableController.transform.position = pos;
            
            
            if (parent != default(Transform)) _lastAvailableController.transform.parent = parent;
            _lastSpawnTime = Time.time;
            return _lastAvailableController;
        }

        public AudioController Play()
        {
            Transform currentCameraTransform = null;

            if (Camera.current)
                currentCameraTransform = Camera.current.transform;
            else if (Camera.main)
                currentCameraTransform = Camera.main.transform;
            else if (_audioListener)
            {
                currentCameraTransform = _audioListener.transform;
            }
            else
            {
                _audioListener = FindObjectOfType<AudioListener>();
                currentCameraTransform = _audioListener.transform;
            }

            if (currentCameraTransform != null)
                return Play(currentCameraTransform.position, currentCameraTransform);

            Debug.LogError("There is no audio listener in the scene");
            return null;

        }

        public void PlayOneShot()
        {
            Play();
        }

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