using System;
using SoundKraft;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class OnCollisionSound : MonoBehaviour
{
    public AudioObject CollisionSound;
    public float MaxFallingSpeed;
    private Rigidbody _rigidbody;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision other)
    {
        AudioController controller = CollisionSound.Play(transform.position);
        controller.SetVolume(controller.Volume * Mathf.Clamp01(_rigidbody.velocity.magnitude / MaxFallingSpeed));
    }

    private void OnDisable()
    {
        _rigidbody.velocity = Vector3.zero;
    }
}
