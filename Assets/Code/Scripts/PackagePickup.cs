using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PackagePickup : MonoBehaviour
{
    [SerializeField] private List<GameObject> packages;
    private void Start()
    {

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player.GetComponent<PlayerController>().HasPackage())
        {
            Debug.Log("Package will not spawn: already have a package");
            return;
        }
        foreach (GameObject obj in packages)
        {
            if (obj.TryGetComponent(out Package pkg) && !pkg.GetWasCollected())
            {
                obj.SetActive(true);
                return;
            }
            obj.SetActive(true);

        }
    }
}
