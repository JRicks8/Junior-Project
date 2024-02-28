using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DeathBoxWithRespawnPoint : MonoBehaviour
{
    [SerializeField] private Vector3 respawnLocation;
    private GameObject playerTemp;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;  
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(respawnLocation, 0.4f);
        Gizmos.DrawLine(transform.position, respawnLocation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // tell player to die
            playerTemp = other.gameObject;
            Invoke("MovePlayer", 3f);
        }
    }

    void MovePlayer()
    {
        playerTemp.transform.position = respawnLocation;
    }
}
