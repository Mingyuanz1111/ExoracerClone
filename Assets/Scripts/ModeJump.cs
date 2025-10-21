using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeJump : MonoBehaviour
{
    private float lastUpdateTime = 0f;

    private Player player;
    private Rigidbody2D rb;

    public int dir = 1;
    private bool isLongJump = false;
    public Vector2 jumpVelocity = new Vector2(3f, 20f);
    public Vector2 longJumpVelocity = new Vector2(6f, 20f);


    private bool enterJump = false;
    private bool startJump = false;
    public float maxJumpTime = 1f;
    private float timeJumped;

    public float forwardSpeed = 3f;
    public float forwardAcc = 2f;
    public float slimeFallSpeed = 0.2f;

    private float nextSummonTrailTime = 0f;
    public GameObject trailObject;

    void UpdateValues()
    {
        dir = (player.facingRight)?1:(-1);
        enterJump = false;
        startJump = false;
        timeJumped = 0f;
        lastUpdateTime = Time.time;  Debug.Log($"Updated at {lastUpdateTime}");
    }

    void Start()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        UpdateValues();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            enterJump = true;
            SpacebarEvent();
        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (enterJump)
            {
                SpacebarEvent();
            }
            if (startJump)
            {
                if (timeJumped <= maxJumpTime)
                {
                    timeJumped += Time.deltaTime;
                    if (isLongJump)
                    {
                        rb.velocity = new Vector2(rb.velocity.x, longJumpVelocity.y * (1 - timeJumped / maxJumpTime));
                    }
                    else
                    {
                        rb.velocity = new Vector2(jumpVelocity.x * dir, jumpVelocity.y * (1 - timeJumped / maxJumpTime));
                    }
                    Debug.Log(rb.velocity.x);
                }
                else
                {
                    EndJump();
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (startJump) EndJump();
        }

        if (!startJump && rb.velocity.y <= slimeFallSpeed && player.slimeCount > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, -slimeFallSpeed);
        }

        if (rb.velocity.x * dir < forwardSpeed)
        {
            rb.velocity = rb.velocity + new Vector2(forwardAcc * dir, 0f) * Time.deltaTime;
        }

        if (lastUpdateTime < player.updateCalledTime) UpdateValues();
        
        if (Time.time >= nextSummonTrailTime)
        {
            Instantiate(trailObject, transform.position, Quaternion.identity);
            nextSummonTrailTime += 0.05f;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Data data = other.gameObject.GetComponent<Data>();
        if (data == null || data.type == "none") return;
        if (data.type == "spring")
        {
            float angle = (other.gameObject.transform.eulerAngles.z + 90) * Mathf.Deg2Rad;
            rb.velocity = data.val * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }
        else if (data.type == "arrow")
        {
            FaceTo(data.dir == "R");
        }
    }

    void SpacebarEvent()
    {
        if (player.groundCount > 0) StartJump();
        else if (player.slimeCount > 0)
        {
            EndJump();
            Flip();
            StartJump();
        }
    }

    void StartJump()
    {
        enterJump = false;
        startJump = true;
        //
        if (isLongJump)
        {
            rb.velocity = new Vector2((longJumpVelocity.x + (rb.velocity.x * dir - longJumpVelocity.x) / 2) * dir, longJumpVelocity.y * (1 - timeJumped / maxJumpTime));
        }
        isLongJump = (rb.velocity.x * dir >= longJumpVelocity.x);
    }

    void EndJump()
    {
        enterJump = false;
        startJump = false;
        timeJumped = 0f;
        rb.velocity = new Vector2 (rb.velocity.x, 0f);
    }

    void FaceTo(bool toRight)
    {
        if (toRight != player.facingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        dir *= -1;
        player.Flip();
    }
}
