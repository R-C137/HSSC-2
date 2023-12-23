using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spinForever : MonoBehaviour
{
    public float rotationSpeed = 10f;

    // Update is called once per frame
    void Update()
    {
        // Rotate the object around the X-axis
        transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);
    }
}
