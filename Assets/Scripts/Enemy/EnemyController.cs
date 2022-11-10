using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy settings")]
    public int damage = 1;
    bool dead;

    [Header("Movement settings")]
    public float movementSpeed = 5f;

    [Header("Ground detection settings")]
    public Transform groundPoint;
    public LayerMask groundLayer;
    float distance = 0.25f;
    RaycastHit2D groundInfo;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        dead = false;
    }

    // Update is called once per frame
    void Update()
    {
        groundInfo = Physics2D.Raycast(groundPoint.position, Vector2.down, distance, groundLayer);

        Debug.DrawRay(groundPoint.position, Vector2.down, Color.red, distance);

        if (!groundInfo.collider)
        {
            Flip();
        }
    }

    private void FixedUpdate()
    {
        if (!dead)
        {
            rb.transform.Translate(Vector2.left * movementSpeed * Time.fixedDeltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<HealthManager>() && !dead)
        {
            collision.gameObject.GetComponent<HealthManager>().TakeDamage(damage, "Player");
        }
    }

    public void Dead()
    {
        dead = true;
        GetComponent<Animator>().Play("EnemyDead");
        rb.transform.Translate(Vector3.zero);
    }

    public void Flip()
    {
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, 0);
    }
}
