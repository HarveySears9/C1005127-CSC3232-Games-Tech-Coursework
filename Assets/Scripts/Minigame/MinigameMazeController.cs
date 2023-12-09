using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameMazeController : MonoBehaviour
{   
    public float rotateSpeed = 180f;

    // called at fixed interval not dependent on framerate
    private void FixedUpdate()
    {
        
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(-Vector3.forward * rotateSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
        }
        
    }
}
