using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SetCheckpointOnTouch : MonoBehaviour
{
    [Header("There must be a Collider component attached to this gameobject.")]
    [Header("The collider must also be a trigger.")]
    public Vector3 checkpointLocation = Vector3.zero;

    private void OnDrawGizmosSelected()
    {
        if (TryGetComponent(out BoxCollider collider))
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(checkpointLocation, 1.0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameState.instance.SetCheckpoint(checkpointLocation);
        }
    }
}
