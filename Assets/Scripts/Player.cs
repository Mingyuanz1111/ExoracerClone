using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float updateCalledTime = 0f;
    public Vector2 spawnpoint;

    private Rigidbody2D rb;

    public string mode = "Jump";
    public bool facingRight = true;

    public int groundCount = 0;
    public int slimeCount = 0;
    public int boosterCount = 0;
    public int hoverCount = 0;
    public int pullerCount = 0;
    public int gliderCount = 0;
    public int swingCount = 0;
    public int dasherCount = 0;
    public int jumperCount = 0;

    public Transform center;
    public float gliderAngle;
    public float dasherAngle;

    public int boosterDir;
    public float boosterSpeed = 15f;
    public float boosterAcc = 30f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spawnpoint = transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Die();
        }

        if (boosterCount > 0 && Mathf.Abs(rb.velocity.x) < boosterSpeed)
        {
            float vx = rb.velocity.x;
            Vector2 v = new Vector2(((boosterDir == 1 && vx < 0) || (boosterDir == -1 && vx > 0))?0f:vx, rb.velocity.y);
            rb.velocity = v + new Vector2(boosterAcc * boosterDir, 0f) * Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Data data = other.gameObject.GetComponent<Data>();
        if (data == null || data.type == "none") return;
        if (data.type == "ground" || data.type == "booster") groundCount++;
        if (data.type == "slime") slimeCount++;
        if (data.type == "booster")
        {
            boosterCount++;
            boosterDir = (data.dir == "R") ? 1 : (-1);
        }
        if (data.type == "hover") hoverCount++;
        if (data.type == "puller") { pullerCount++; center = data.trans; }
        if (data.type == "glider") { gliderCount++; gliderAngle = other.gameObject.transform.eulerAngles.z; }
        if (data.type == "swing") { swingCount++; center = data.trans; }
        if (data.type == "dasher") { dasherCount++; dasherAngle = other.gameObject.transform.eulerAngles.z; }
        if (data.type == "jumper") jumperCount++;
        if (data.type == "spike") Die();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Data data = other.gameObject.GetComponent<Data>();
        if (data == null || data.type == "none") return;
        if (data.type == "ground" || data.type == "booster") groundCount--;
        if (data.type == "slime") slimeCount--;
        if (data.type == "booster") boosterCount--;
        if (data.type == "hover") hoverCount--;
        if (data.type == "puller") pullerCount--;
        if (data.type == "glider") gliderCount--;
        if (data.type == "swing") swingCount--;
        if (data.type == "dasher") dasherCount--;
        if (data.type == "jumper") jumperCount--;
    }

    public void FaceTo(bool toRight)
    {
        if (toRight != facingRight)
        {
            Flip();
        }
    }

    public void Flip()
    {
        facingRight = !facingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1f);
    }

    public void Die()
    {
        updateCalledTime = Time.time;
        FaceTo(true);
        transform.position = spawnpoint;
        rb.velocity = Vector2.zero;
    }
}
