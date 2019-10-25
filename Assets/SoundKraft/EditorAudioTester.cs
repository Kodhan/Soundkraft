using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundKraft;
using UnityEditor;

[ExecuteInEditMode]
public class EditorAudioTester : MonoBehaviour {

    private AudioSource _source;
	private AudioObject _currentAudioObject;
    public void Initialize(AudioObject audioObject)
    {
        _source = gameObject.AddComponent<AudioSource>();
	    _currentAudioObject = audioObject;
	    _source.Play();
    }
#if UNITY_EDITOR
	void Start()
	{
		if(EditorApplication.isPlaying)
			DestroyImmediate(gameObject);
	}
#endif

	
	
	// Update is called once per frame
	void Update () {
        if (_source != null && _source.isPlaying) return;
	
        DestroyImmediate(gameObject);
	}

	public bool IsPlayingAudioObject(AudioObject audioObject)
	{
		return _currentAudioObject == audioObject && _source != null && _source.isPlaying;
	}
}
