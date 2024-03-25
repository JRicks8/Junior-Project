using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Harpy : MonoBehaviour
{

    private PlayerController playerController;
    [SerializeField][Range(1, 100)] private float pullPower;
    [SerializeField][Range(0.1f, 3)] private float pullDuration;
    [SerializeField][Range(0.1f, 4)] private float waitDuration;
    private float waitTime, pullTime = 0;
    private bool hitPlayer = false;

    enum HarpyState { 
        IDLE,
        ACTIVE,
        WAITING,
    }
    private HarpyState state;

    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        waitTime += Time.fixedDeltaTime;
        if (playerController)
        {
            pullTime += Time.fixedDeltaTime;
            if (pullTime > pullDuration)
            {
                hitPlayer = false;
                state = HarpyState.WAITING;
                if (waitTime > waitDuration)
                {
                    pullTime = 0;
                    waitTime = 0;
                }
                return;
            }
            state = HarpyState.ACTIVE;

            Vector3 directionToPlayer = (transform.position - playerController.transform.position).normalized;
            Rigidbody playerRB = playerController.GetComponent<Rigidbody>();
            playerRB.AddForce(  directionToPlayer * pullPower * 10, ForceMode.Force);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerController = other.GetComponent<PlayerController>();
            pullTime = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            playerController = null;
            state = HarpyState.IDLE;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }
}
