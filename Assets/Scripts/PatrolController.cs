using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolController : MonoBehaviour
{
    [Header("Movement settings")]
    public float movementSpeed = 5f;
    public List<Transform> patroList;
    int currentPatrolPoint;

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

            //if (patroList[currentPatrolPoint].position.x > transform.position.x)
            //{
            //    Flip(0);
            //}
            //else
            //{
            //    Flip(180);
            //}
        }
    }

    private void FixedUpdate()
    {
        rb.transform.position = Vector2.MoveTowards(transform.position, patroList[currentPatrolPoint].position, movementSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.transform.SetParent(GameObject.Find("Level").transform);
        }
    }

    //private void Flip(int v)
    //{
    //    transform.eulerAngles = new Vector3(0, v, 0);
    //}
}
