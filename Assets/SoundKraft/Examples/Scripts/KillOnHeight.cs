using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillOnHeight : MonoBehaviour
{
    public float KillHeight;

    public void Update()
    {
         if(transform.position.y <= KillHeight)
             ObjectPool.Destroy(gameObject);
    }
}
