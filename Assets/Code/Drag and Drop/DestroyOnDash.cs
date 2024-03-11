using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DestroyOnDash : MonoBehaviour
{
    private PlayerController playerController;

    private void Update()
    {
        if (playerController)
        {
            if (playerController.IsDashing()) Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController = other.GetComponent<PlayerController>();
            if (playerController.IsDashing()) Destroy(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerController.IsDashing()) Destroy(gameObject);
            playerController = null;
        }
    }
}
