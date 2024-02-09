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
    [SerializeField] private bool moveFromCurrentPosition;
    [SerializeField] private Vector3 moveOrigin;
    [SerializeField] private Vector3 moveDestination;
    [SerializeField] private float moveTime;
    private IEnumerator moveHandler;
    private bool objectMoving = false;
    // Destroy Object
    [SerializeField] private GameObject objectToDestroy;

    private SphereCollider sphereCollider;

    private List<Func<float, float>> easeFunctions = new List<Func<float, float>>()
    {
        Easing.InOutSine, //Ease,
        Easing.InQuad,//EaseIn,
        Easing.OutQuad,//EaseOut,
        Easing.InOutQuad,//EaseInOut,
        Easing.Linear,//Linear,
        Easing.InSine,//EaseInSine,
        Easing.OutSine,//EaseOutSine,
        Easing.InOutSine,//EaseInOutSine,
        Easing.InCubic,//EaseInCubic,
        Easing.OutCubic,//EaseOutCubic,
        Easing.InOutCubic,//EaseInOutCubic,
        Easing.InCirc,//EaseInCirc,
        Easing.OutCirc,//EaseOutCirc,
        Easing.InOutCirc,//EaseInOutCirc,
        Easing.InElastic,//EaseInElastic,
        Easing.OutElastic,//EaseOutElastic,
        Easing.InOutElastic,//EaseInOutElastic,
        Easing.InBack,//EaseInBack,
        Easing.OutBack,//EaseOutBack,
        Easing.InOutBack,//EaseInOutBack,
        Easing.InBounce,//EaseInBounce,
        Easing.OutBounce,//EaseOutBounce,
        Easing.InOutBounce,//EaseInOutBounce
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
            if (!objectMoving)
            {
                moveHandler = MoveHandler(easeFunctions[(int)moveType]);
                StartCoroutine(moveHandler);
            }
        }
        else if (interactionType == OnInteractType.DestroyObject)
        {
            Destroy(objectToDestroy);
        }
    }

    private IEnumerator MoveHandler(Func<float, float> func)
    {
        float timer = 0.0f;
        float maxTime = moveTime;
        Vector3 origin = moveOrigin;
        if (moveFromCurrentPosition)
            origin = objectToMove.transform.position;

        while (timer < maxTime)
        {
            yield return new WaitForFixedUpdate();
            float t = timer / moveTime;
            t = func(t);
            Vector3 newPosition = Vector3.LerpUnclamped(origin, moveDestination, t);
            objectToMove.transform.position = newPosition;
            timer += Time.fixedDeltaTime;
        }

        objectToMove.transform.position = moveDestination;
    }

    public OnInteractType GetInteractionType()
    {
        return interactionType;
    }

    public bool GetMoveFromCurrentPosition()
    {
        return moveFromCurrentPosition;
    }
}
