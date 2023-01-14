using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 컴포넌트
    Rigidbody2D playerRigidbody;
    CapsuleCollider2D capsuleCollider;
    Animator animator;

    // 게임 전체
    bool isDead = false;
    /// <summary>
    /// 달리는 스테이지라면 true, 떨어지는 스테이지라면 false
    /// </summary>
    [SerializeField] bool isRunningStage = true;
    public bool IsRunningStage
    {
        get { return isRunningStage; }
        set
        {
            isRunningStage = value;
            Debug.Log(value);
            animator.SetBool(nameof(isRunningStage), value);
        }
    }
    Vector3 originScale;

    // 지상 필드
    [SerializeField] float jumpForce;
    int jumpCount = 0;
    bool isGrounded = false;
    private bool IsGrounded
    {
        get
        {
            return isGrounded;
        }
        set
        {
            isGrounded = value;
            animator.SetBool(nameof(isGrounded), value);
        }
    }
    bool isSliding = false;
    private bool IsSliding
    {
        get
        {
            return isSliding;
        }
        set
        {
            isSliding = value;
            animator.SetBool(nameof(isSliding), value);
        }
    }

    // 공중 필드
    [SerializeField] float playerSpeed;
    [SerializeField] float dashSpeed;
    Vector2 moveVector;
    float delayBetweenPress = 0.4f;
    bool isDetectingDash = true;
    bool isDashing = false;

    void Start()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        originScale = transform.localScale;
    }

    void Update()
    {
        if (isDead) return;

        if (IsRunningStage)
        {
            Jump();
            Sliding();
            playerRigidbody.gravityScale = 1.0f;
        }
        else
        {
            moveVector.x = Input.GetAxisRaw("Horizontal");
            if (!isDashing) transform.Translate(playerSpeed * Time.deltaTime * moveVector);
            if (Input.GetButtonDown("Horizontal") && isDetectingDash) StartCoroutine(DashDetection(moveVector.x));
            playerRigidbody.gravityScale = 0.0f;
        }

        //if (playerRigidbody.velocity.x > 0) playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x - Time.deltaTime, playerRigidbody.velocity.y);
        //else if (playerRigidbody.velocity.x < 0) playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x + Time.deltaTime, playerRigidbody.velocity.y);
        //Debug.Log(playerRigidbody.velocity);
    }

    private void LateUpdate()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpCount = 0;
            IsGrounded = true;
        }
        else if (collision.gameObject.CompareTag("Exit"))
        {
            IsRunningStage = !IsRunningStage;
        }
    }
    
    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < 2)
        {
            IsGrounded = false;
            jumpCount++;
            playerRigidbody.velocity = Vector2.zero;
            playerRigidbody.AddForce(new Vector2(0, jumpForce));
        }
    }

    private void Sliding()
    {
        if (Input.GetButtonDown("Sliding") && jumpCount == 0)
        {
            IsSliding = true;
            capsuleCollider.size = new Vector2(20.48f, 12);
        }
        else if (Input.GetButtonUp("Sliding") && jumpCount == 0)
        {
            IsSliding = false;
            capsuleCollider.size = new Vector2(20.48f, 20.48f);
        }
    }

    private IEnumerator DashDetection(float direction)
    {
        isDetectingDash = false;

        float t = 0;
        while (t < delayBetweenPress)
        {
            if (t > 0 && Input.GetButtonDown("Horizontal") && Input.GetAxisRaw("Horizontal") == direction)
            {
                StartCoroutine(Dash(direction));

                isDetectingDash = true;
                yield break;
            }
            t += Time.deltaTime;
            yield return null;
        }

        isDetectingDash = true;
    }

    private IEnumerator Dash(float direction)
    {
        isDashing = true;
        float t = 0;
        while (t < 0.1f)
        {
            transform.Translate(new Vector2(direction, 0) * dashSpeed * Time.deltaTime);
            t += Time.deltaTime;
            yield return null;
        }
        isDashing = false;
    }
}
