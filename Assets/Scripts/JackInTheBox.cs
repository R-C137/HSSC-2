using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JackInTheBox : TrapBehaviour
{
    public Transform targetObject;
    public float proximityDistance = 2.0f;
    public Animator jackAnim;
    public spinForever crank;

    private void Awake()
    {
        targetObject = SantaBehaviour.singleton.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetObject != null)
        {
            // Calculate the distance between this object and the target object
            float distance = Vector3.Distance(transform.position, targetObject.position);

            // Check if the distance is less than or equal to the proximity distance
            if (distance <= proximityDistance)
            {
                // Perform the action when the objects are close enough
                PopGoesTheWeasel();
            }
        }
        else
        {
            Debug.LogWarning("Target object not assigned. Please assign a target object in the inspector.");
        }

        // Check if the target object is assigned
        if (targetObject != null)
        {
            // Make the current object look at the target object
            transform.LookAt(targetObject);
        }
        else
        {
            Debug.LogWarning("Target object not assigned. Please assign a target object in the inspector.");
        }
    }

    void PopGoesTheWeasel()
    {
        crank.enabled = false;
        if (jackAnim != null) jackAnim.enabled = true;
    }
}