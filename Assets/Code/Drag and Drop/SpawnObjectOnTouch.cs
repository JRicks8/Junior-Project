using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjectOnTouch : MonoBehaviour
{
    [SerializeField] private string tagToCheck;
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private Vector3 spawnPosition;

    private void OnDrawGizmos()
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
