using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundKraft;
[CreateAssetMenu(fileName = "AudioObject")]
public class AudioCaller : ScriptableObject {


    private static Dictionary<string, AudioObject> _audioObjects;
    private static AudioCaller _instance;

    void Awake()
    {
//        Debug.Log("The audio caller is waking up");

        if(_instance != null)
        {
            Debug.LogError("There is more than one AudioCaller fix this!");
            return;
        }
        _instance = this;
    }

    public static SoundKraft.AudioController PlayAudioByName(string name)
    {
        if (_instance == null) _instance = Resources.Load("TheAudioCaller") as AudioCaller;
#if UNITY_EDITOR
        if (_instance == null) {
            _instance = CreateAsset();
            return null;
        }




       if(_audioObjects == null || !_audioObjects.ContainsKey(name))
            _instance.RePopulate();
#endif


        if (!_audioObjects.ContainsKey(name))
        {
            Debug.LogError("There is no audio by the name " + name + " in the audio caller");
            return null;
        }
        return _audioObjects[name].Play();


    }

#if UNITY_EDITOR

    public static AudioCaller CreateAsset()
    {
        _instance = ScriptableObject.CreateInstance<AudioCaller>();

        string path = UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (System.IO.Path.GetExtension(path) != "")
        {
            path = path.Replace(System.IO.Path.GetFileName(UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.activeObject)), "");
        }

        string assetPathAndName = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path + "/Resources/" + "TheAudioCaller" + ".asset");

        UnityEditor.AssetDatabase.CreateAsset(_instance, assetPathAndName);

        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
        UnityEditor.EditorUtility.FocusProjectWindow();
        UnityEditor.Selection.activeObject = _instance;
        _audioObjects = new Dictionary<string, AudioObject>();
        _instance.RePopulate();
        return _instance;
    }

    public static T[] GetAllInstances<T>() where T : ScriptableObject
    {
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(T).Name);  //FindAssets uses tags check documentation for more info
        T[] a = new T[guids.Length];
        for (int i = 0; i < guids.Length; i++)         //probably could get optimized 
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
            a[i] = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
        }

        return a;

    }

    [ContextMenu("Populate")]
    private void RePopulate()
    {
        if (_instance == null) _instance = this;

        _audioObjects = new Dictionary<string, AudioObject>();

        foreach(AudioObject audio in GetAllInstances<AudioObject>())
        {
            _audioObjects.Add(audio.name, audio);
        }
      
    }

#endif
}

