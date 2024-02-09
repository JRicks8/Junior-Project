using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LaunchOnTouch : MonoBehaviour
{
    [Header("Requires a collider to work")]
    [SerializeField][Range(1f, 20f)] private float power = 2;
    [SerializeField] private bool shouldPreserveMomentum = false;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody rbRef = other.GetComponent<Rigidbody>();
            float fallMagnitude = 1f;
            if (shouldPreserveMomentum && rbRef.velocity.y < -5f) fallMagnitude =  -1 * rbRef.velocity.y;
            rbRef.AddForce(new(0f, fallMagnitude + power * 5f, 0f), ForceMode.Impulse);
        }
    }
}
