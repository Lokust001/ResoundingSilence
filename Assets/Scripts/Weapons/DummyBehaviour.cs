/******************************************************************************
 * Author: Brad Dixon
 * Contributors: 
 * Last Modified: 9/21/2026
 * Brief: Testing script to test weapons on. 
 * TODO:
 * ***************************************************************************/
using UnityEngine;
using System.Collections.Generic;

public class DummyBehaviour : MonoBehaviour
{
    [Tooltip("Only used with moving dummies.")]
    [SerializeField] List<Vector3> pathPoints = new List<Vector3>();
    int pointIndex;
    [SerializeField] float moveSpeed;
    Rigidbody rb;

    [Tooltip("If true, the dummy will move between points.")]
    [SerializeField] bool movingDummy;

    /// <summary>
    /// Sets the rigidbody and starting velocity.
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if(movingDummy)
        {
            pointIndex = 0;
            SetVelocity();
        }
    }

    /// <summary>
    /// Sets the velocity of the dummy. Moves to the next point in it's list
    /// </summary>
    private void SetVelocity()
    {
        Vector3 moveDir = pathPoints[pointIndex] - transform.position;
        moveDir.y = 0;

        rb.linearVelocity = moveDir.normalized * moveSpeed;
    }

    /// <summary>
    /// Checks if the dummy reaches it's designated point and moves it to the next one
    /// </summary>
    private void FixedUpdate()
    {
        if (movingDummy)
        {
            if (transform.position == pathPoints[pointIndex])
            {
                //Wraps through the list.
                pointIndex = pointIndex + 1 < pathPoints.Count ? ++pointIndex : 0;
                SetVelocity();
            }
        }
    }
}
