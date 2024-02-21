using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayEffectOnDestroy : MonoBehaviour
{
    [Header("Create an empty gameObject with a particle system in the hierachy\nthen drag it from the hiearchy into this slot\nThe script creates a new instance of it\nso you can use the same particleSystem multiple times.")]
    [SerializeField] private GameObject m_EffectSourcePrefab;

    private void OnDestroy()
    {
        GameObject newInst = Instantiate(m_EffectSourcePrefab);
        newInst.transform.position = transform.position;
        newInst.transform.rotation = transform.rotation;
        newInst.GetComponent<ParticleSystem>().Play();
        newInst.GetComponent<DestroyEffectOnFinish>().enabled = true;
    }
}
