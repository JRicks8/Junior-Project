using UnityEngine.SceneManagement;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class OpenLevelOnTouch : MonoBehaviour
{
    [SerializeField] private string levelToOpenName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(levelToOpenName);
            
        }
    }
}
