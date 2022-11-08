using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public enum playerType { Endless, Platformer}
    public enum worldType { world01, world02, world03 }

    [Header("Player Type")]
    public playerType currentPlayerType;

    [Header("World Type")]
    public worldType currentWorldType;

    [Header("Movement Settings")]
    public float movementSpeed = 5f;
    public float movementSmoothing = 0.1f;
    Vector3 velocity = Vector3.zero;
    float inputX = 0;

    [Header("Jumping Settings")]
    public float jumpForce = 2f;
    public Transform checkGround;
    public float checkGroundRadio;
    public LayerMask groundLayer;
    public LayerMask rampLayer;

    //double jump
    public int jumpsOnAir = 1;
    int currentJumpsOnAir = 0;

    [Header("Dash Settings")]
    public float dashForce = 8f;
    public float dashDuration = 0.4f;
    public TrailRenderer trailRenderer;
    Vector2 dashDirection = Vector2.zero;
    bool isDashing = false;
    bool canDash = true;
    float currentGravity = 0;

    [Header("Melee Attack Settings")]
    public Transform meleeAttackPoint;
    public float meleeAttackRadio;
    public LayerMask enemyLayer;
    public int meleeDamage;
    public GameObject meleeAttackEffect;
    public GameObject AttackEffectPrefab;

    [Header("Melee Attack Settings")]
    public GameObject projectile;
    public Transform firePoint;
    public float fireImpulse = 6f;
    public float fireRate = 1f;
    float currentFireRate;

    [Header("Audio Settings")]
    public List<AudioClip> audioList;

    Rigidbody2D rb;
    Animator anim;

    AudioSource source;

    Vector2 initialPosition;

    [Header("Player Prefs Settings")]
    public int currentWorld = 0;

    [Header("World Settings")]
    public Transform finishPoint;
    public int scoreGoal;
    bool worldfinished = false;

    UIController _UIController;
    bool gameOver = false;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        source = GetComponent<AudioSource>();

        source.PlayOneShot(audioList[2]);

        currentGravity = rb.gravityScale;

        initialPosition = transform.position;

        _UIController = GameObject.FindGameObjectWithTag("UI").GetComponent<UIController>();
    }

    
    void Update()
    {
        if (gameOver || worldfinished)
        {
            WorldFinished();
            return;
        }
        
        Flip(inputX);

        GeneralInput();

        Jump();

        Dash();

        SetAnimation();

        WorldFinished();
    }

    private void FixedUpdate()
    {
        Move(inputX);
    }

    #region GeneralInput
    void GeneralInput()
    {
        if (currentPlayerType == playerType.Endless)
        {
            inputX = 1;
        }
        else
        {
            inputX = Input.GetAxisRaw("Horizontal");
        }

        if (inputX == 0 && OnGround(rampLayer))
        {
            rb.velocity = Vector2.zero;
        }

        if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2"))
        {
            if (!(anim.GetBool("jabIzquierdo") || anim.GetBool("jabDerecho")))
            {
                MeleeAttackEffect(true);
            }
        }

        currentFireRate += Time.deltaTime;

        if (Input.GetButton("Fire1") && Input.GetButton("Fire2"))
        {
            MeleeAttackEffect(false);
        }
    }
    #endregion

    #region Animation
    private void SetAnimation()
    {
        anim.SetBool("run", inputX != 0f);

        anim.SetBool("onGround", OnGround(groundLayer) || OnGround(rampLayer));
        anim.SetBool("isFalling", rb.velocity.y < 0);
        anim.SetBool("firstOnGround", !anim.GetBool("onGround"));

        anim.SetBool("dash", isDashing);

        if (!(Input.GetButton("Fire1") && Input.GetButton("Fire2")))
        {
            anim.SetBool("jabIzquierdo", Input.GetButtonDown("Fire1"));
            anim.SetBool("jabDerecho", Input.GetButtonDown("Fire2"));
        }
    }
    #endregion

    #region Attack
    private void MeleeAttackEffect(bool melee)
    {
        if (melee)
        {
            meleeAttackEffect.SetActive(false);
            meleeAttackEffect.SetActive(true);

            source.PlayOneShot(audioList[1]);

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(meleeAttackPoint.position, meleeAttackRadio, enemyLayer);

            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<HealthManager>().TakeDamage(meleeDamage, "Enemy");
            }
        }
        else
        {
            if (currentFireRate > fireRate)
            {
                currentFireRate = 0;
                GameObject _projectile = Instantiate(projectile, firePoint.position, Quaternion.identity);
                _projectile.GetComponent<Projectilecontroller>().SetProjectile(transform.localScale.x == 1 ? Vector2.right : Vector2.left, fireImpulse, "Enemy");
                _projectile.GetComponent<SpriteRenderer>().enabled = false;

                GameObject effect = Instantiate(AttackEffectPrefab, _projectile.transform.position, Quaternion.identity, _projectile.transform);
                effect.GetComponent<ParticleSystem>().loop = true;
                effect.GetComponent<ParticleSystem>().startColor = Color.black;
                effect.GetComponent<ParticleSystem>().startSize = 2;
            }
        }
    }
    #endregion

    #region Jump
    void Jump()
    {
        if (OnGround(groundLayer) || OnGround(rampLayer))
        {
            if (Input.GetButtonDown("Jump"))
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);

                source.PlayOneShot(audioList[0]);
            }

            currentJumpsOnAir = 0;
            
            if (!canDash)
            {
                StartCoroutine(EnableDashing());
            }
        }
        else
        {
            if (Input.GetButtonDown("Jump") && currentJumpsOnAir < jumpsOnAir)
            {
                currentJumpsOnAir++;

                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);

                source.PlayOneShot(audioList[0]);
            }
        }

        if (Input.GetButtonUp("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }
    #endregion

    #region Dash
    void Dash()
    {
        if (Input.GetButtonDown("Dash") && canDash)
        {
            isDashing = true;
            canDash = false;
            trailRenderer.emitting = true;

            dashDirection = new Vector2(inputX, 0);
            rb.gravityScale = 0;

            if (dashDirection == Vector2.zero)
            {
                dashDirection = new Vector2(transform.localScale.x, 0);
            }

            StartCoroutine(StopDashing());
        }

        if (isDashing)
        {
            rb.velocity = dashDirection.normalized * dashForce;
        }
    }

    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(dashDuration);

        rb.gravityScale = currentGravity;
        isDashing = false;
        trailRenderer.emitting = false;
    }
    private IEnumerator EnableDashing()
    {
        yield return new WaitForSeconds(0.6f);

        canDash = true;
    }
    #endregion

    #region Movement
    private void Move(float value)
    {
        Vector3 targetVelocity = new Vector2(value * movementSpeed, rb.velocity.y);
        if (OnGround(rampLayer))
        {
            targetVelocity.x *= 5;
        }
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);

        if (rb.velocity.magnitude < 1)
        {
            rb.velocity = new Vector3(0, rb.velocity.y);
        }
    }
    #endregion

    #region OnGround
    private bool OnGround(LayerMask layer)
    {
        return Physics2D.OverlapCircle(checkGround.position, checkGroundRadio, layer);
    }
    #endregion

    #region DeadZone
    public void ResetGame()
    {
        gameOver = true;
        _UIController.SetGameOver();
        
        rb.velocity = Vector2.zero;
        trailRenderer.emitting = false;
        isDashing = false;
        
        transform.localScale = Vector3.one;

        transform.position = initialPosition;
    }
    #endregion

    #region World Finished
    public void WorldFinished()
    {
        if (worldfinished)
        {
            PlayerPrefs.SetInt("World" + currentWorld, 1);
            _UIController.SetWorldFinished();
        }

        switch (currentWorldType)
        {
            case worldType.world01:
                worldfinished = Vector2.Distance(checkGround.position, finishPoint.position) <= checkGroundRadio;
                break;
            case worldType.world02:
                worldfinished = _UIController.GetScore() >= scoreGoal;
                break;
            case worldType.world03:
                break;
            default:
                break;
        }
    }
    #endregion

    #region Flip
    private void Flip(float value)
    {
        if (value < 0)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }
        else if (value > 0)
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
    }
    #endregion

    #region DrawingGizmos
    private void OnDrawGizmos()
    {
        if (OnGround(groundLayer) && OnGround(rampLayer))
        {
            Gizmos.color = Color.black;
        }
        else if (OnGround(rampLayer))
        {
            Gizmos.color = Color.magenta;
        }
        else if (OnGround(groundLayer))
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }
        
        Gizmos.DrawWireSphere(checkGround.position, checkGroundRadio);
        
        Gizmos.DrawWireSphere(meleeAttackPoint.position, meleeAttackRadio);
    }
    #endregion
}
