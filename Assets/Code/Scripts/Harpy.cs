using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harpy : MonoBehaviour
{

    private PlayerController playerController;
    [SerializeField][Range(1,100)] private float pullPower;

    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerController)
        {
            Vector3 directionToPlayer = (transform.position - playerController.transform.position).normalized;
            Rigidbody playerRB = playerController.GetComponent<Rigidbody>();
            playerRB.AddForce(-directionToPlayer * pullPower * 10, ForceMode.Force);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

        }
    }
}
