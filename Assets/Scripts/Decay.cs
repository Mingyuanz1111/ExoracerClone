using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decay : MonoBehaviour
{
    public float decayTime = 2f;

    void Start()
    {
        Invoke("Kill", decayTime);
    }

    void Kill()
    {
        Destroy(gameObject);
    }

}
