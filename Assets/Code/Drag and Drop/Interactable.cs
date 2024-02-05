using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

[RequireComponent(typeof(SphereCollider))]
public class Interactable : MonoBehaviour
{
    [Serializable]
    public enum OnInteractType { Dialogue, SpawnObject, MoveObject, DestroyObject }
    public enum InteractMethod { Action, TriggerEnter }

    [SerializeField] private InteractMethod method;
    [SerializeField] private OnInteractType interactionType;
    // Dialogue
    [SerializeField] private string dialogueText;
    [SerializeField] private TextMeshProUGUI textBox;
    [SerializeField] private Vector3 textLocation;
    [SerializeField] private Vector3 textRotation;
    // Spawn Object
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private Vector3 locationToSpawn;
    // Move Object
    [SerializeField] private EasingMode moveType;
    [SerializeField] private Transform objectToMove;
    [SerializeField] private Vector3 origin;
    [SerializeField] private Vector3 destination;
    [SerializeField] private float moveTime;
    // Destroy Object
    [SerializeField] private GameObject objectToDestroy;

    private SphereCollider sphereCollider;

    private List<Action> easeFunctions = new List<Action>()
    {
        //x => Easing.Linear(x),
    };

    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.radius = 0.1f;
    }

    public void Interact(InteractMethod incomingMethod)
    {
        if (incomingMethod != method) return;

        if (interactionType == OnInteractType.Dialogue)
        {
            textBox.text = dialogueText;
            textBox.transform.localPosition = textLocation;
            textBox.transform.localRotation = Quaternion.Euler(textRotation);
        }
        else if (interactionType == OnInteractType.SpawnObject)
        {
            Instantiate(objectToSpawn, locationToSpawn, Quaternion.identity);
        }
        else if (interactionType == OnInteractType.MoveObject)
        {

        }
        else if (interactionType == OnInteractType.DestroyObject)
        {

        }
    }

    public OnInteractType GetInteractionType()
    {
        return interactionType;
    }
}
