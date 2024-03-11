using UnityEngine;

public class GrapplePoint : MonoBehaviour
{
    private SphereCollider sphereCollider;

    void Start()
    {
        sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.radius = 0.1f;
        sphereCollider.excludeLayers = LayerMask.NameToLayer("Everything");
    }

    void Update()
    {
        
    }
}
