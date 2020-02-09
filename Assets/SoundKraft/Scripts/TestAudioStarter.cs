using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SoundKraft
{
public class TestAudioStarter : MonoBehaviour
{

    public AudioObject TestAudio;
    private AudioController _testController;
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Delete)) PlaySound();
    }
    
    public void PlaySound()
    {
        if (!TestAudio.IsLooping)
        {
            TestAudio.PlayOneShot();   
            return;
        }

        if(_testController == null || !_testController.IsPlaying)
            _testController = TestAudio.Play();
        else
            _testController.Stop();
    }
}
}