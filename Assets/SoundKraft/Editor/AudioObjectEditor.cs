using UnityEngine;
using UnityEditor;
using SoundKraft;
[CustomEditor(typeof(AudioObject))]
public class AudioObjectEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        AudioObject audioObject = (AudioObject)target;
        if (GUILayout.Button("TestAudio"))
        {
            audioObject.TestPlay();
        }
    }

}
