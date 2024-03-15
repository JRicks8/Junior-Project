using UnityEngine;

public class SetCheckpointOnTouch : MonoBehaviour
{
    [Header("There must be a Collider component attached to this gameobject.")]
    [Header("The collider must also be a trigger.")]
    public Vector3 checkpointLocation = Vector3.zero;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        if (other.CompareTag("Player"))
        {
            GameState.instance.SetCheckpoint(checkpointLocation);
        }
    }
}
