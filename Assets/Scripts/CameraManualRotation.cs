using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraManualRotation : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    float lastTouchSpeed;
    float increment = 0f;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount>0)
        {
            foreach(Touch touch in Input.touches)
            {
                int id = touch.fingerId;
                if(EventSystem.current.IsPointerOverGameObject(id))
                {
                    return;
                }
            }
        }
        if (Input.touchCount == 1 && Input.touches[0].phase == TouchPhase.Moved)
        {
            increment = 0f;
            Touch firstTouch1 = Input.GetTouch(0);
            transform.Rotate(0, firstTouch1.deltaPosition.x * speed, 0, relativeTo: Space.World);
            lastTouchSpeed = firstTouch1.deltaPosition.x;
        }
        else if(Input.touchCount==0)
        {
            increment += 0.01f;
            float newSpeed = Mathf.Lerp(speed, 0, increment);
            transform.Rotate(0, lastTouchSpeed * newSpeed, 0, relativeTo: Space.World);
        }

    }

    private void StopRotationOverUI()
    {

    }
}
