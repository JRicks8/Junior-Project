using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayEffectOnDestroy : MonoBehaviour
{
    [Header("Required: Attach a new component of type 'Particle System'\nthen drag from the created component into here.")]
    [SerializeField] public GameObject m_EffectSourcePrefab;

    private void OnDestroy()
    {
        m_EffectSourcePrefab.GetComponent<ParticleSystem>().Play();
    }
}
