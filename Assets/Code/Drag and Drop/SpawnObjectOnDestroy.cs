using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnObjectOnDestroy : MonoBehaviour
{
    [Header("Does not actually spawn 'new' objects. Requires references to disabled prefabs from hierarchy to re-enable.")]
    [SerializeField] private List<GameObject> coinVisuals;

    private void OnDestroy()
    {
        foreach (GameObject o in coinVisuals)
        {
            o.SetActive(true);
            //Collider oCollider = o.GetComponent<Collider>();
            //if (oCollider) oCollider.enabled = false;
            o.transform.parent.parent = null;
            SpawnAnim animScript = o.transform.parent.GetComponent<SpawnAnim>();
            animScript.StartSpawnAnimFrom(transform.position);
        }
    }
}
