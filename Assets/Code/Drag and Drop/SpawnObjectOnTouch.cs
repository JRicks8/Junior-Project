using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjectOnTouch : MonoBehaviour
{
    [Header("Set this to 'Player' if you want player touch to spawn the object.")]
    [SerializeField] private string tagToCheck;
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private Vector3 spawnPosition;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, spawnPosition);
        Gizmos.DrawSphere(spawnPosition, 0.2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagToCheck))
        {
            Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        }
    }
}
