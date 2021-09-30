using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowRotate : MonoBehaviour
{
    public enum axis {X, Y, Z};
    public float speed = 10.0f;
    public axis axisOfRotation = axis.Z;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (axisOfRotation)
        {
            case axis.X:
                transform.Rotate(speed * Time.deltaTime, 0, 0);
                break;
            case axis.Y:
                transform.Rotate(0, speed * Time.deltaTime, 0);
                break;
            default:
                transform.Rotate(0, 0, speed * Time.deltaTime);
                break;
        }        
    }
}
