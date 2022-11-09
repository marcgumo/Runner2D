using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject.Find("UI").GetComponent<UIController>().SetScore(25);

            collision.gameObject.GetComponent<HealthManager>().GiveHealth(1);

            collision.gameObject.GetComponent<AudioSource>().PlayOneShot(
                collision.gameObject.GetComponent<PlayerController>().audioList[3]);

            Destroy(gameObject);
        }
    }
}
