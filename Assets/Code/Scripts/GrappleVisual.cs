using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleVisual : MonoBehaviour
{
    [SerializeField] private Transform meshTransform;

    public Vector3 Point1;
    public Vector3 Point2;

    private void Update()
    {
        Vector3 difference = Point2 - Point1;
        transform.position = Vector3.Lerp(Point1, Point2, 0.5f);
        meshTransform.localScale = new Vector3(0.1f, difference.magnitude / 2, 0.1f);

        Vector3 directionToPoint = difference.normalized;
        transform.rotation = Quaternion.LookRotation(directionToPoint, Vector3.Cross(directionToPoint, transform.right));
    }
}
