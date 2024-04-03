using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HandApproachZone : MonoBehaviour
{
    public UnityEvent PlayerApproaching;

    private void Start()
    {
        PlayerApproaching ??= new UnityEvent();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController playerController) && playerController.HasPackage())
            PlayerApproaching?.Invoke();
    }
}
