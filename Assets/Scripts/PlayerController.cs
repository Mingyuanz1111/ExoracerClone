using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int cnt = 0;
    public float jumpForce = 5f;
    public Vector2 jumpDirection = new Vector2(1f ,1f);
    
    private Rigidbody2D rb;

    public bool enterJump = true;
    public bool startJump = true;
    public float maxJumpTime = 1f;
    public float timeJumped;

    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    public LayerMask slimeLayer;
    public Vector2 slimeFallVelocity = new Vector2(0f, -0.2f);

    public LayerMask springLayer;
    public Vector2 springVelocity = new Vector2(1f, 5f);

    public LayerMask arrowLayer;

    public bool facingRight = true;
    private int dir = 1;
    private Collider2D groundCollider;
    private Collider2D slimeCollider;
    private Collider2D springCollider;
    private Collider2D arrowCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpDirection.Normalize();
    }

    void Update()
    {
        Vector3 positiveScale = new Vector3(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z));
        groundCollider = Physics2D.OverlapBox(groundCheck.position, new Vector3(0.98f,0.1f,1f), transform.eulerAngles.z, groundLayer);
        slimeCollider = Physics2D.OverlapBox(transform.position, positiveScale, transform.eulerAngles.z, slimeLayer);
        springCollider = Physics2D.OverlapBox(transform.position, positiveScale, transform.eulerAngles.z, springLayer);
        arrowCollider = Physics2D.OverlapBox(transform.position, positiveScale, transform.eulerAngles.z, arrowLayer);

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
                    rb.velocity = jumpDirection * jumpForce;
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

        if (!startJump && slimeCollider && rb.velocity.y <= 0)
        {
            rb.velocity = slimeFallVelocity;
        }

        if (springCollider)
        {
            rb.velocity = springVelocity;
        }

        if (arrowCollider)
        {
            FaceTo(arrowCollider.gameObject.GetComponent<Data>().type == "R");
        }

        if (rb.velocity.x * dir < 2f)
        {
            rb.velocity = rb.velocity + new Vector2(0.1f * dir, 0f);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Die();
        }
    }

    void SpacebarEvent()
    {
        Debug.Log($"{groundCollider} , {slimeCollider}");
        if (groundCollider) StartJump();
        else if (slimeCollider)
        {
            EndJump();
            Flip();
            StartJump();
        }
    }

    void StartJump()
    {
        cnt++;
        enterJump = false;
        startJump = true;
    }

    void EndJump()
    {
        enterJump = false;
        startJump = false;
        timeJumped = 0f;
    }

    void FaceTo(bool toRight)
    {
        if (toRight != facingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        dir *= -1;
        jumpDirection.x = -jumpDirection.x;
        springVelocity.x = -springVelocity.x;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1f);
    }

    public void Die()
    {
        FaceTo(true);
        transform.position = Vector3.zero;
        rb.velocity = Vector3.zero;
    }

    void OnDrawGizmosSelected()
    {
        Vector3 positiveScale = new Vector3(Mathf.Abs(transform.localScale.x), Mathf.Abs(transform.localScale.y), Mathf.Abs(transform.localScale.z));
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, new Vector3(0.95f, 0.1f, 1f));
    }
}
