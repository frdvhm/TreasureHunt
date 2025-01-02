using UnityEngine;
using System.Collections; // Pastikan namespace ini sudah ditambahkan

public class Player : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite[] runSprites;
    public Sprite climbSprite;
    private int spriteIndex;

    private new Rigidbody2D rigidbody;
    private new Collider2D collider;
    private Collider2D[] results;
    private Vector2 direction;
    public float moveSpeed = 1f;
    public float jumpStrength = 1f;
    private bool grounded;
    private bool climbing;

    public bool moveLeft;
    public bool moveRight;
    public bool jump;

    public AudioClip hitSound;
    public AudioClip jumpSound;
    public AudioClip pickupDiamondSound;
    private AudioSource audioSource;

    private bool isFrozen = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        results = new Collider2D[4];
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnEnable()
    {
        InvokeRepeating(nameof(AnimateSprite), 1f / 12f, 1f / 12f);
    }

    private void OnDisable()
    {
        CancelInvoke();
        rigidbody.gravityScale = 1f;
    }

    private void CheckCollision()
    {
        if (isFrozen) return;

        grounded = false;
        climbing = false;

        Vector2 size = collider.bounds.size;
        size.y += 0.1f;
        size.x /= 2f;

        int amount = Physics2D.OverlapBoxNonAlloc(transform.position, size, 0f, results);

        for (int i = 0; i < amount; i++)
        {
            GameObject hit = results[i].gameObject;

            if (hit.layer == LayerMask.NameToLayer("Ground"))
            {
                grounded = hit.transform.position.y < (transform.position.y - 0.5f);
                Physics2D.IgnoreCollision(collider, results[i], !grounded);
            }
            else if (hit.layer == LayerMask.NameToLayer("Ladder"))
            {
                climbing = true;
                rigidbody.gravityScale = 0f;
            }
        }
    }

    private void Update()
    {
        if (isFrozen) return; // Jangan izinkan input jika beku

        CheckCollision();

        if (climbing)
        {
            direction.y = Input.GetAxis("Vertical") * moveSpeed;

            if (Input.GetButton("Jump") || jump)
            {
                direction.y = moveSpeed;
            }
        }
        else if (grounded && (Input.GetButtonDown("Jump") || jump))
        {
            direction = Vector2.up * jumpStrength;
            jump = false;

            if (jumpSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(jumpSound);
            }
        }
        else
        {
            direction += Physics2D.gravity * Time.deltaTime;
        }

        float horizontalInput = 0f;

        if (Input.GetKey(KeyCode.LeftArrow) || moveLeft)
        {
            horizontalInput = -1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow) || moveRight)
        {
            horizontalInput = 1f;
        }

        direction.x = horizontalInput * moveSpeed;

        if (grounded)
        {
            direction.y = Mathf.Max(direction.y, -1f);
        }

        if (direction.x > 0f)
        {
            transform.eulerAngles = Vector3.zero;
        }
        else if (direction.x < 0f)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    private void FixedUpdate()
    {
        if (isFrozen)
        {
            rigidbody.velocity = Vector2.zero; // Hentikan pergerakan fisik
            return;
        }

        if (!climbing)
        {
            direction.y += Physics2D.gravity.y * Time.fixedDeltaTime;
        }

        rigidbody.MovePosition(rigidbody.position + direction * Time.fixedDeltaTime);
    }

    private void AnimateSprite()
    {
        if (isFrozen) return; // Jangan animasikan jika beku

        if (climbing)
        {
            spriteRenderer.sprite = climbSprite;
        }
        else if (direction.x != 0f)
        {
            spriteIndex++;

            if (spriteIndex >= runSprites.Length)
            {
                spriteIndex = 0;
            }

            spriteRenderer.sprite = runSprites[spriteIndex];
        }
    }

    public void MoveLeft(bool isPressed)
    {
        moveLeft = isPressed;
    }

    public void MoveRight(bool isPressed)
    {
        moveRight = isPressed;
    }

    public void Jump(bool isPressed)
    {
        jump = isPressed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Objective"))
        {
            if (pickupDiamondSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(pickupDiamondSound);
            }

            StartCoroutine(FreezePlayer(1.5f)); // Membekukan pemain selama 1.5 detik
            GameManager.Instance.LevelComplete();
        }
        else if (collision.gameObject.CompareTag("Obstacle"))
        {
            PlayHitSound();
            StartCoroutine(FreezePlayer(1.5f)); // Membekukan pemain selama 1.5 detik
            GameManager.Instance.LevelFailed();
        }
    }


    private IEnumerator FreezePlayer(float duration)
    {
        isFrozen = true;
        direction = Vector2.zero; // Hentikan input pergerakan
        yield return new WaitForSeconds(duration);
        isFrozen = false;
    }

    void PlayHitSound()
    {
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }
}
