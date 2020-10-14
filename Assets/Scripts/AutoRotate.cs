using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    [SerializeField] float rotationFactor;
    bool autoRotating;

    public bool AutoRotating
    {
        set
        {
            autoRotating = value;
        }
        get
        {
            return autoRotating;
        }
    }
    void Start()
    {
        autoRotating = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(autoRotating)
        {
            transform.Rotate(0f, rotationFactor * Time.deltaTime, 0f);
        }
        
    }
}
