using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [Header("Change this object's layer type to 'turrain'.\nDo not add a trigger box to this object.\nAn added trigger box will move player with weird behavior.")]
    [SerializeField] List<Vector3> locationsToCycleThrough = new();
    [SerializeField][Range(0.5f, 60)] private float timeToMove;
    [SerializeField][Range(.5f, 60)] private float waitDuration;
    [SerializeField][Range(0, 360)] private float rotateAmountOnWait;
    private float elapsedTime = 0;
    private float waitTime = 0;

    private List<Color> locationColor = new();
    private BoxCollider playerRidingBox;
    private BoxCollider objectCollision;

    private int currentLocationIndex = 0;
    private int targetLocationIndex = 1;
    private Rigidbody playerRigidBody;

    //private Vector3 lastFramePosition = new();
    private Quaternion lastRotationOnWait;


    private void Awake()
    {
        objectCollision = GetComponent<BoxCollider>();
        playerRidingBox = gameObject.AddComponent<BoxCollider>();
        playerRidingBox.isTrigger = true;
        playerRidingBox.size = new(objectCollision.size.x, 0.2f, objectCollision.size.z);
        playerRidingBox.center = new Vector3(0f, objectCollision.size.y + playerRidingBox.size.y * 2, 0f);  
        lastRotationOnWait = Quaternion.identity;
    }

    void Start()
    {
        if (targetLocationIndex > locationsToCycleThrough.Count - 1) targetLocationIndex = 0;
        if (locationsToCycleThrough.Count <= 1) { Debug.Log("MovingObject Script only has one location and will not move."); return; }
    }

    private void OnDrawGizmosSelected()
    {
        if (locationsToCycleThrough.Count > 0) locationsToCycleThrough[0] = transform.position;

        for (int i = 0; i < locationsToCycleThrough.Count; i++)
        {
            while (locationColor.Count < locationsToCycleThrough.Count) locationColor.Add(new Color(
                                                                                            Random.Range(0,1f),
                                                                                            0f,
                                                                                            Random.Range(0, 1f)
                                                                                        ));
            while (locationColor.Count > locationsToCycleThrough.Count) locationColor.RemoveAt(locationColor.Count - 1);
            if (locationsToCycleThrough.Count == 0) locationColor.RemoveAt(0);
            Gizmos.color = locationColor[i];
            Gizmos.DrawSphere(locationsToCycleThrough[i], .2f);
        }
    }

    private void FixedUpdate()
    {
        MoveObject();

        //if (playerRigidBody)
        //{

        //    Vector3 platformChangeInPosition = new(transform.position.x - lastFramePosition.x,
        //                                            0f,
        //                                            transform.position.z - lastFramePosition.z);
        //    playerRigidBody.AddForce(platformChangeInPosition * 700 , ForceMode.Force);            
        //}
        
    }

    private void MoveObject()
    {
        //lastFramePosition = transform.position;
        if (waitTime < waitDuration) 
        {
            transform.rotation = Quaternion.Lerp(lastRotationOnWait, Quaternion.Euler(0f, lastRotationOnWait.eulerAngles.y + rotateAmountOnWait, 0f), waitTime / waitDuration);

            waitTime += Time.fixedDeltaTime;
            return;
        } 
        lastRotationOnWait = transform.rotation;
       
        transform.position = Vector3.Lerp(locationsToCycleThrough[currentLocationIndex], 
                                          locationsToCycleThrough[targetLocationIndex], 
                                          elapsedTime / timeToMove);

        elapsedTime += Time.fixedDeltaTime;
        if (elapsedTime >= timeToMove)
        {
            elapsedTime = 0;
            currentLocationIndex++;
            targetLocationIndex++;
            if (targetLocationIndex > locationsToCycleThrough.Count - 1) targetLocationIndex = 0;
            if (currentLocationIndex > locationsToCycleThrough.Count - 1) currentLocationIndex = 0;
            waitTime = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerRigidBody = other.gameObject.GetComponent<Rigidbody>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerRigidBody = null;
        }
    }
}
