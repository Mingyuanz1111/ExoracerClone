using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public Transform from;
    public Transform to;

    void Start()
    {
        
    }

    void Update()
    {
        Debug.Log(to.position);
        transform.position = (from.position + to.position) / 2f;
        Vector2 diff = to.position - from.position;
        transform.eulerAngles = new Vector3(0f, 0f, Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg);
        transform.localScale = new Vector2(diff.magnitude, transform.localScale.y);
    }
}
