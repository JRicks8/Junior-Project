using UnityEngine;

public class DestroyOnDive : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision");
        if (collision.gameObject.CompareTag("Player") && collision.gameObject.TryGetComponent(out PlayerController playerController))
        {
            if (playerController.IsDiving())
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger");
        if (other.gameObject.CompareTag("Player") && other.gameObject.TryGetComponent(out PlayerController playerController))
        {
            if (playerController.IsDiving())
            {
                Destroy(gameObject);
            }
        }
    }
}
