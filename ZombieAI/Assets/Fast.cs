using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fast : MonoBehaviour
{
    public float rotationTime = 5f;

    void Update()
    {
        transform.Rotate(new Vector3(0, (360 / rotationTime) * Time.deltaTime, 0));
    }
}
