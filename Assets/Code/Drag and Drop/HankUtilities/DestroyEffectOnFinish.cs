using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffectOnFinish : MonoBehaviour
{

    private ParticleSystem ps;

    private void Awake()
    {
        enabled = false;
    }

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (!ps.isPlaying) Destroy(gameObject);
    }
}
