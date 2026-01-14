using UnityEngine;

namespace CoinsOfHope.Gameplay
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float jumpForce = 10f;
        [SerializeField] private float slideDuration = 0.5f;
        [SerializeField] private float gravity = -20f;

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask groundLayer;

        private Rigidbody2D rb;
        private Collider2D playerCollider;
        private bool isGrounded;
        private bool isSliding;
        private float slideTimer;
        private Vector2 originalColliderSize;
        private Vector2 originalColliderOffset;

        public bool IsAlive { get; private set; } = true;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
            }
            rb.gravityScale = 0;

            playerCollider = GetComponent<Collider2D>();
            if (playerCollider == null)
            {
                var box = gameObject.AddComponent<BoxCollider2D>();
                box.size = new Vector2(0.5f, 1f);
                playerCollider = box;
            }

            if (playerCollider is BoxCollider2D boxCollider)
            {
                originalColliderSize = boxCollider.size;
                originalColliderOffset = boxCollider.offset;
            }

            if (groundLayer.value == 0)
            {
                groundLayer = ~0;
            }

            if (groundCheck == null)
            {
                GameObject checkObj = new GameObject("GroundCheck");
                checkObj.transform.SetParent(transform);
                checkObj.transform.localPosition = new Vector3(0, -0.5f, 0);
                groundCheck = checkObj.transform;
            }
        }

        void Update()
        {
            if (!IsAlive) return;

            CheckGrounded();
            HandleInput();
            UpdateSlide();
        }

        void FixedUpdate()
        {
            if (!IsAlive) return;

            ApplyGravity();
        }

        void CheckGrounded()
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }

        void HandleInput()
        {
            if (Input.GetButtonDown("Jump") || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                if (!isSliding)
                {
                    TryJump();
                }
            }

            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || IsSwipeDown())
            {
                TrySlide();
            }
        }

        bool IsSwipeDown()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved)
                {
                    float deltaY = touch.deltaPosition.y;
                    if (deltaY < -50f)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        void TryJump()
        {
            if (isGrounded && !isSliding)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
        }

        void TrySlide()
        {
            if (isGrounded && !isSliding)
            {
                isSliding = true;
                slideTimer = slideDuration;

                if (playerCollider is BoxCollider2D boxCollider)
                {
                    boxCollider.size = new Vector2(originalColliderSize.x, originalColliderSize.y * 0.5f);
                    boxCollider.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - originalColliderSize.y * 0.25f);
                }
            }
        }

        void UpdateSlide()
        {
            if (isSliding)
            {
                slideTimer -= Time.deltaTime;
                if (slideTimer <= 0)
                {
                    EndSlide();
                }
            }
        }

        void EndSlide()
        {
            isSliding = false;

            if (playerCollider is BoxCollider2D boxCollider)
            {
                boxCollider.size = originalColliderSize;
                boxCollider.offset = originalColliderOffset;
            }
        }

        void ApplyGravity()
        {
            if (!isGrounded)
            {
                rb.velocity += new Vector2(0, gravity * Time.fixedDeltaTime);
            }
            else if (rb.velocity.y < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
        }

        public void Die()
        {
            if (!IsAlive) return;
            IsAlive = false;
            rb.velocity = Vector2.zero;
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Obstacle"))
            {
                Die();
                if (CoinRunnerManager.Instance != null)
                {
                    CoinRunnerManager.Instance.OnPlayerHitObstacle();
                }
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Coin"))
            {
                if (CoinRunnerManager.Instance != null)
                {
                    CoinRunnerManager.Instance.OnCoinCollected();
                }
                Destroy(other.gameObject);
            }
        }

        void OnDrawGizmosSelected()
        {
            if (groundCheck != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
            }
        }
    }
}
