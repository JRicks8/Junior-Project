using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class GrapplePoint : MonoBehaviour
{
    private SphereCollider sphereCollider;

    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.radius = 0.1f;
    }

    void Update()
    {
        
    }
}
