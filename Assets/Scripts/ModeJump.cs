using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeJump : MonoBehaviour
{
    private float lastUpdateTime = 0f;

    private Player player;
    public GameObject ropeObject;
    private Rope rope;
    private Rigidbody2D rb;

    public int dir = 1;
    private bool isLongJump = false;
    private float initialLongJumpVelocity = 0f;
    private float initialUpVelocity = 0f;
    public Vector2 jumpVelocity = new Vector2(13f, 16f);
    public Vector2 longJumpVelocity = new Vector2(18f, 16f);

    private bool enterJump = false;
    private bool startJump = false;
    public float maxJumpTime = 1f;
    public int dirJump = 1;
    private float timeJumped;

    public float forwardSpeed = 3f;
    public float forwardAcc = 2f;
    public float slimeFallSpeed = 0.2f;

    private bool startHover = false;
    public float hoverSpeed = 20f;
    private float hoverInitialVelovityX;
    public float hoverAngleMin = -45f;
    public float hoverAngleMax = 60f;
    public float hoverDuration = 0.3f;
    private float timeHovered = 0f;

    private bool startPuller = false;
    public float pullerConst = 20f;
    public Vector2 centerPos;

    private bool startGlider = false;
    public float gliderSpeed = 20f;

    private bool startSwing = false;
    private float swingLength;
    public Vector2 swingBoost;

    public float dasherDist = 5f;
    public float dasherSpeed = 32f;
    private float dasherDuration;
    public float dasherAngle;
    private float dasherTimeLeft = 0f;

    public float maxJumperTime = 0.25f;
    private float timeJumpered = 100000f;

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

    void DisableStates()
    {
        dasherTimeLeft = 0f;
        timeJumpered = 100000f;
    }

    void Start()
    {
        player = GetComponent<Player>();
        rope = ropeObject.GetComponent<Rope>();
        rope.from = transform;
        rb = GetComponent<Rigidbody2D>();
        UpdateValues();

        dasherDuration = dasherDist / dasherSpeed;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            enterJump = true;
            initialLongJumpVelocity = rb.velocity.x;
            SpacebarEvent();
        }

        if (Input.GetKey(KeyCode.Space))
        {
            SpacebarHold();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (startJump) EndJump();
            if (startSwing) rb.velocity = new Vector2(Mathf.Max(rb.velocity.x, swingBoost.x), Mathf.Max(rb.velocity.y, swingBoost.y));

            startHover = false;
            startPuller = false;
            startGlider = false;
            startSwing = false;
            rb.gravityScale = 8f;
            ropeObject.SetActive(false);
        }

        if (dasherTimeLeft > 0)
        {
            dasherTimeLeft -= Time.deltaTime;
            float slowTime = dasherDuration / 4f;
            float speed = dasherSpeed * Mathf.Min(slowTime, dasherTimeLeft) / slowTime;
            rb.velocity = speed * new Vector2(Mathf.Cos(dasherAngle * Mathf.Deg2Rad), Mathf.Sin(dasherAngle * Mathf.Deg2Rad));
        }

        if (timeJumpered < maxJumperTime)
        {
            timeJumpered += Time.deltaTime;
            rb.velocity = new Vector2(Mathf.Max(jumpVelocity.x, rb.velocity.x * dirJump) * dirJump, jumpVelocity.y * (1 - timeJumpered / maxJumperTime));
        }

        if (!startJump && rb.velocity.y <= slimeFallSpeed && player.slimeCount > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, -slimeFallSpeed);
        }

        if (player.groundCount == 0 && !startSwing && !startPuller && rb.velocity.x * dir < forwardSpeed)
        {
            rb.velocity = rb.velocity + new Vector2(forwardAcc * dir, 0f) * Time.deltaTime;
        }

        if (!startPuller && !startSwing) ropeObject.SetActive(false);

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
        if (data.type == "arrow")
        {
            FaceTo(data.dir == "R");
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        Data data = other.gameObject.GetComponent<Data>();
        if (data == null || data.type == "none") return;
        if (data.type == "spring")
        {
            float angle = (other.gameObject.transform.eulerAngles.z + 90) * Mathf.Deg2Rad;
            rb.velocity = data.val * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
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
            DisableStates();
        }
        else if (!startGlider && player.gliderCount > 0)
        {
            startGlider = true;
            rb.gravityScale = 0f;
            DisableStates();
        }
        else if(!startPuller && player.pullerCount > 0)
        {
            startPuller = true;
            centerPos = player.center.position;
            ropeObject.SetActive(true);
            rope.to = player.center;
            rb.gravityScale = 0f;
            if (((centerPos - (Vector2)transform.position).x > 0) == (dir == -1)) Flip();
            DisableStates();
        }
        else if (!startSwing && player.swingCount > 0)
        {
            startSwing = true;
            swingLength = (player.center.position - transform.position).magnitude;
            centerPos = player.center.position;
            ropeObject.SetActive(true);
            rope.to = player.center;
            DisableStates();
        }
        else if (player.dasherCount > 0)
        {
            dasherTimeLeft = dasherDuration;
            dasherAngle = player.dasherAngle;
        }
        else if (player.jumperCount > 0)
        {
            timeJumpered = 0;
            dirJump = dir;
        }
        else if (!startHover && player.hoverCount > 0)
        {
            startHover = true;
            float hoverAngleInitial = Mathf.Clamp(Mathf.Atan2(rb.velocity.y, rb.velocity.x * dir) * Mathf.Rad2Deg, hoverAngleMin, hoverAngleMax);
            hoverInitialVelovityX = rb.velocity.x * dir;
            timeHovered = (hoverAngleInitial - hoverAngleMin) / (hoverAngleMax - hoverAngleMin) * hoverDuration;
            dasherTimeLeft = 0f;
        }
    }

    void SpacebarHold()
    {
        if (startGlider && player.gliderCount > 0)
        {
            rb.velocity = gliderSpeed * new Vector2(Mathf.Cos(player.gliderAngle * Mathf.Deg2Rad), Mathf.Sin(player.gliderAngle * Mathf.Deg2Rad));
            return;
        }

        if (startPuller)
        {
            Vector2 diff = centerPos - (Vector2)transform.position;
            Vector2 force = diff.normalized * (diff.magnitude * pullerConst * Time.deltaTime);
            force = new Vector2(force.x, force.y /** 1.5f*/);
            rb.velocity = rb.velocity + force;
            return;
        }
        else startPuller = false; rb.gravityScale = 8f;

        if (startSwing)
        {
            Vector2 diff = (centerPos - (Vector2)transform.position);
            Vector2 tangent = new Vector2(diff.y, -diff.x).normalized;
            rb.velocity = Projection(rb.velocity, tangent);
            transform.position = (Vector2)transform.position + diff.normalized * (diff.magnitude - swingLength);
        }

        if (startHover && player.hoverCount > 0)
        {
            timeHovered += Time.deltaTime;
            float angle = hoverAngleMin + (hoverAngleMax - hoverAngleMin) * Mathf.Min(timeHovered, hoverDuration) / hoverDuration; //Debug.Log($"ANG: {angle}, T: {timeHovered}/{hoverDuration}");
            float vx = Mathf.Max(hoverSpeed * Mathf.Cos(angle * Mathf.Deg2Rad), hoverInitialVelovityX) * dir;
            float vy = Mathf.Max(hoverSpeed * Mathf.Sin(angle * Mathf.Deg2Rad), rb.velocity.y);
            rb.velocity = new Vector2(vx, vy);
            return;
        }
        else startHover = false;

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
                    rb.velocity = new Vector2(rb.velocity.x, initialUpVelocity + longJumpVelocity.y * (1 - timeJumped / maxJumpTime));
                }
                else
                {
                    rb.velocity = new Vector2(jumpVelocity.x * dirJump, initialUpVelocity + jumpVelocity.y * (1 - timeJumped / maxJumpTime));
                }
            }
            else
            {
                EndJump();
            }
            return;
        }
    }

    void StartJump()
    {
        enterJump = false;
        startJump = true;
        dirJump = dir;

        if (isLongJump)
        {
            rb.velocity = new Vector2((longJumpVelocity.x + (initialLongJumpVelocity * dir - longJumpVelocity.x) / 2) * dir, rb.velocity.y);
        }
        isLongJump = (initialLongJumpVelocity * dir >= longJumpVelocity.x);
        if (isLongJump) rb.velocity = new Vector2(initialLongJumpVelocity, rb.velocity.y);

        initialUpVelocity = Mathf.Max(rb.velocity.y, 0f);
    }

    void EndJump()
    {
        enterJump = false;
        startJump = false;
        timeJumped = 0f;
        rb.velocity = new Vector2 (rb.velocity.x, initialUpVelocity);
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

    Vector2 Projection(Vector2 of, Vector2 to)
    {
        return Vector2.Dot(of, to) / to.sqrMagnitude * to.normalized;
    }
}
