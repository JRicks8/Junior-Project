using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class NewAbilityPickup : MonoBehaviour
{
    [SerializeField] private PlayerController.Abilities abilityToGet;

    private void Awake()
    {
        GameObject playerController = GameObject.FindGameObjectWithTag("Player");
        if (playerController != null)
        {
            PlayerController pScript = playerController.GetComponent<PlayerController>();
            if (pScript.GetHasAbility(abilityToGet))
                gameObject.SetActive(false);
        }

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
