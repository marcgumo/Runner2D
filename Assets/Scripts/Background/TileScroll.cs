using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScroll : MonoBehaviour
{
    public float scrollSpeed = 5.0f;
    public float mapSize = 1.0f;
    public PlayerController player;

    private Vector3 current;

    void Start()
    {
        current = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z);
    }

    void Update()
    {
        if (Camera.main.GetComponent<CameraController>().isMoving)
        {
            current.x -= player.transform.localScale.x * scrollSpeed * Time.deltaTime;

            if (current.x < -mapSize)
            {
                current.x += mapSize;
            }

            transform.localPosition = current;
        }
    }
}
