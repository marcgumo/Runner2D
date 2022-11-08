using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    [Header("Ship settings")]
    public int damage = 1;

    [Header("Movement settings")]
    public float movementSpeed = 5f;
    public List<Transform> patroList;
    int currentPatrolPoint;

    [Header("Projectile settings")]
    public GameObject projectile;
    public Transform firePoint;
    public float range = 10f;
    public float impulse = 3;
    public float fireRate = 1f;
    float currentFireRate = 0f;
    public Transform playerTransform;
    public bool aimPlayer = false;


    Rigidbody2D rb;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, patroList[currentPatrolPoint].position) < 0.1f)
        {
            currentPatrolPoint++;

            if (currentPatrolPoint >= patroList.Count)
            {
                currentPatrolPoint = 0;
            }

            if (patroList[currentPatrolPoint].position.x > transform.position.x)
            {
                Flip(0);
            }
            else
            {
                Flip(180);
            }
        }

        currentFireRate += Time.deltaTime;

        if (currentFireRate > fireRate)
        {
            currentFireRate = 0;

            if (aimPlayer && Vector2.Distance(playerTransform.position, transform.position) < range)
            {
                GameObject _projectile = Instantiate(projectile, firePoint.position, Quaternion.identity);
                Vector2 direction = playerTransform.position - transform.position;
                _projectile.GetComponent<Projectilecontroller>().SetProjectile(direction.normalized, impulse / 2, playerTransform.tag);
                
            }
            else
            {
                GameObject _projectile = Instantiate(projectile, firePoint.position, Quaternion.identity);
                _projectile.GetComponent<Projectilecontroller>().SetProjectile(Vector2.down, impulse, playerTransform.tag);
            }
        }
    }

    private void FixedUpdate()
    {
        rb.transform.position = Vector2.MoveTowards(transform.position, patroList[currentPatrolPoint].position, movementSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<HealthManager>())
        {
            collision.gameObject.GetComponent<HealthManager>().TakeDamage(damage, "Player");
        }
    }

    private void Flip(int v)
    {
        transform.eulerAngles = new Vector3(0, v, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
