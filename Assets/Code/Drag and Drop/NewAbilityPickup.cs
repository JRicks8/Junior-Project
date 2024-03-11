using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class NewAbilityPickup : MonoBehaviour
{
    [SerializeField] private PlayerController.Abilities abilityToGet;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().SetHasAbility(abilityToGet, true);
        }
    }

}
