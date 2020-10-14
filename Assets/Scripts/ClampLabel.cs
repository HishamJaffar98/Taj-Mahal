using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClampLabel : MonoBehaviour
{
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        transform.LookAt(Camera.main.transform,Vector3.up);
    }
}
