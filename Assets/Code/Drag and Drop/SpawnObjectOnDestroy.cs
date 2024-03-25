using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjectOnDestroy : MonoBehaviour
{
    [Header("Does not actually spawn 'new' objects. Requires references to disabled prefabs from hierarchy to re-enable.")]
    [SerializeField] private List<GameObject> objectsToSpawn;

    private void OnDestroy()
    {
        foreach (GameObject o in objectsToSpawn)
        {
            if (!o) continue;
            o.SetActive(true);
            Collider oCollider = o.GetComponent<Collider>();
           if (oCollider) oCollider.enabled = false;
            o.GetComponent<SpawnAnim>().StartSpawnAnimFrom(transform.position);
        }
    }
}
