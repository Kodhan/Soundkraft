using System;
using System.Collections;
using System.Collections.Generic;
using SoundKraft;
using UnityEngine;

public class SoundKraftManager : MonoBehaviour
{
    private static SoundKraftManager _instance;
    private static List<(AudioController, AudioSource)> _sources;
    
    private static void Validate()
    {
        if(_sources == null)
            _sources = new List<(AudioController, AudioSource)>();

        if (_instance == null)
        {
            GameObject go = new GameObject("SoundKraftManager");
            DontDestroyOnLoad(go);
            _instance = go.AddComponent<SoundKraftManager>();
        }
    }

    private void Update()
    {
        for (int i = _sources.Count - 1; i >= 0; i--)
        {
            (AudioController controller, AudioSource source) = _sources[i];

            if (source == null)
            {
                _sources.RemoveAt(i);
                continue;
            }
            
            if (controller.Paused == source.isPlaying && !(source.time >= source.clip.length))
            {
                if (controller.Paused)
                    source.Pause();
                else
                    source.Play();

            }

            source.volume = controller.DesiredVolume;
            source.pitch = controller.DesiredPitch;

            if (!controller.Destroy && (source.isPlaying || controller.Looping || controller.Paused))
                continue;
            
            ObjectPool.Destroy<AudioSource>(source.gameObject);
            _sources.RemoveAt(i);
        }
    }

    public static AudioController Initialize(AudioObject audioObject, Vector3 pos = default, Transform parent = null)
    {
        Validate();
       
        AudioController controller = new AudioController();
        AudioSource source = ObjectPool.Instantiate<AudioSource>();

        _sources.Add((controller, source));

        source.gameObject.transform.position = pos;
        source.gameObject.transform.parent = parent;

        source.clip = audioObject.GetClip();
        source.rolloffMode = audioObject.RolloffMode;
        source.volume = audioObject.Volume;
        source.pitch = audioObject.Pitch;
        source.minDistance = audioObject.Distance.x;
        source.maxDistance = audioObject.Distance.y;
        source.spatialBlend = audioObject.SpatialBlend;
        source.loop = audioObject.IsLooping;
        source.outputAudioMixerGroup = audioObject.MixerGroup;

        controller.Pause();
        controller.SetLooping(source.loop);
        controller.SetPitch(source.pitch);
        controller.SetVolume(source.volume);
        
        return controller;
    }
    
    
    public static AudioController InitializeAndPlay(AudioObject audioObject, Vector3 pos = default, Transform parent = null)
    {
        AudioController controller = Initialize(audioObject, pos, parent);
        controller.Play();
        return controller;
    }
    
    

}
