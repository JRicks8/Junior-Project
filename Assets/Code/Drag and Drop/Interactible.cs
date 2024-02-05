using System;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Interactible : MonoBehaviour
{
    [Serializable]
    public enum OnInteractType { Dialogue, SpawnObject, MoveObject, DestroyObject }
    public enum InteractMethod { Action, TriggerEnter }
    public enum MoveType { Teleport, Linear, EaseIn, EaseOut, EaseInOut }

    [SerializeField] private InteractMethod method;
    [SerializeField] private OnInteractType interactionType;
    // Dialogue
    [SerializeField] private string dialogueText;
    // Spawn Object
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private Vector3 locationToSpawn;
    // Move Object
    [SerializeField] private MoveType moveType;
    [SerializeField] private GameObject objectToMove;
    [SerializeField] private Vector3 origin;
    [SerializeField] private Vector3 destination;
    [SerializeField] private float moveTime;
    // Destroy Object
    [SerializeField] private GameObject objectToDestroy;

    private SphereCollider sphereCollider;

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
            Debug.Log(dialogueText);
        }
        else if (interactionType == OnInteractType.SpawnObject)
        {

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

    public MoveType GetMoveType() { return moveType; }
}
