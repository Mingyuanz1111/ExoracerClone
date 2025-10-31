using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.name == "Player")
        {
            GameObject.Find("Level Manager").GetComponent<LevelManager>().CompleteLevel();
        }
    }
}
