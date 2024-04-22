using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SpawnAbility : MonoBehaviour
{
    [SerializeField] private GameObject ability;

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PlayerController>().HasPackage())
        {
            //ability
        }
    }
}
