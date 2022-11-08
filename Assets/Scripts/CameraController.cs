using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerController target;
    public float smoothTime = 0.3f;
    public Vector3 offset;
    public bool isMoving = false;
    Vector3 velocity;
    public float prevPosition;

    void Start()
    {
        prevPosition = (float)System.Math.Round(transform.position.x, 2);
    }

    
    void Update()
    {
        if (target.transform.localScale.x == 1)
        {
            offset.x = 1.5f;
        }
        else
        {
            offset.x = -1.5f;
        }
        transform.position = Vector3.SmoothDamp(transform.position, target.transform.position + offset, ref velocity, smoothTime);
        
        isMoving = prevPosition != (float)System.Math.Round(transform.position.x, 2);
        prevPosition = (float)System.Math.Round(transform.position.x, 2);
        
    }
}
