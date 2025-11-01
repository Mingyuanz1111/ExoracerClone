using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decay : MonoBehaviour
{
    public int type;
    public List<Color> trailColor;
    public List<float> trailSize;

    public float decayDuration = 2f;
    private float timePassed = 0f;
    SpriteRenderer sr;
    private float size;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        Invoke("Kill", decayDuration);

        sr.color = trailColor[type];
        size = trailSize[type];
    }

    void FixedUpdate()
    {
        timePassed += Time.deltaTime;
        float ratio = 1 - timePassed / decayDuration;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, (225 * ratio) / 255f);
        transform.localScale = new Vector3 (size * ratio, size * ratio, 1f);
    }

    void Kill()
    {
        Destroy(gameObject);
    }

}
