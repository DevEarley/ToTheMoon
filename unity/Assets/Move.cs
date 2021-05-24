using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{

    public Vector3 Velocity;
    public float UpperBound = 9999.0f;
    public float LowerBound = -9999.0f;
    public float RightBound = 9999.0f;
    public float LeftBound = -9999.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = transform.localPosition + (Velocity * Time.deltaTime);
        if (transform.localPosition.x > RightBound)
        {
            transform.localPosition = new Vector3(LeftBound, transform.localPosition.y, transform.localPosition.z);
        }
        if (transform.localPosition.x < LeftBound)
        {
            transform.localPosition = new Vector3(RightBound, transform.localPosition.y, transform.localPosition.z);
        }
        if (transform.localPosition.y > UpperBound)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, UpperBound, transform.localPosition.z);
        }
        if (transform.localPosition.y < LowerBound)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, LowerBound, transform.localPosition.z);
        }
    }
}
