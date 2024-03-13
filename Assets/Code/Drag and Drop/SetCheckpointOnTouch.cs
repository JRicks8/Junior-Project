using UnityEngine;

public class SetCheckpointOnTouch : MonoBehaviour
{
    public Vector3 checkpointLocation = Vector3.zero;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameState.instance.SetCheckpoint(checkpointLocation);
        }
    }
}
