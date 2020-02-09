using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSpawner : MonoBehaviour
{
    public GameObject Cube;
    
    public void SpawnBox()
    {
        GameObject box = ObjectPool.Instantiate(Cube, transform.position, Random.rotation);
        box.GetComponent<Collider>().material.bounciness = Random.Range(0.7f, 0.9f);
    }
}
