using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectOnTouch : MonoBehaviour
{
    // was planning on adding rotation

    [SerializeField] private string tagToCheck;
    [SerializeField] private GameObject objectToMove;
    [SerializeField] private Vector3 targetLocation;
    //[SerializeField] private Vector3 targetRotation;
    private bool isMoving = false;

    private Vector3 homePosition;
    //private Quaternion homeRotation;
    private float elapsedTime = 0;
    [SerializeField][Range(0.1f, 60)] private float timeToMove;

    private void Start()
    {
        homePosition = objectToMove.transform.position;
        //homeRotation = objectToMove.transform.rotation;
    }

    private void FixedUpdate()
    {
        if (!isMoving) return;
        if (elapsedTime >= timeToMove) return;
        objectToMove.transform.position = Vector3.Lerp(homePosition, targetLocation, elapsedTime / timeToMove);
        elapsedTime += Time.fixedDeltaTime;
    }

    private void OnDrawGizmosSelected()
    {
        if (!objectToMove) return;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetLocation, .2f);
        Gizmos.DrawLine(objectToMove.transform.position, targetLocation);
        //Gizmos.DrawLine(targetLocation, targetLocation + targetRotation.normalized);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagToCheck))
        {
            isMoving = true;
        }
    }
}
