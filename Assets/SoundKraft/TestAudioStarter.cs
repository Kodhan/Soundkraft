using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SoundKraft
{
public class TestAudioStarter : MonoBehaviour
{

    public AudioObject TestAudio;
    public bool FromThisPosition;
    private AudioController _testController;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Delete)) PlaySound();
    }
    
    public void PlaySound()
    {
        SoundKraftManager.InitializeAndPlay(TestAudio);
    }
}
}