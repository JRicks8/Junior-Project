using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayEffectOnDestroy : MonoBehaviour
{
    [Header("Required: Attach the blue cube labeled\n'EffectSourceObject' from the DragAndDrop folder.")]
    [SerializeField] private GameObject m_EffectSourceObjectPrefab;

    [Header("Required: Attach a new component of type 'Particle System'\nthen drag from the created component into here.")]
    [SerializeField] private ParticleSystem m_EffectSourceComponent;

    private void OnDestroy()
    {
        foreach (Component c in GetComponents<Component>()) if (c != m_EffectSourceComponent || c != this) Destroy(c);
        //GameObject spawnedObject = Instantiate(m_EffectSourceObjectPrefab, transform.position, transform.rotation, null);
        //spawnedObject.CopyComponent(m_EffectSourceComponent);
        //ParticleSystem spawnedComponent = spawnedObject.GetComponent<ParticleSystem>();
        //if (!spawnedComponent || spawnedComponent.isPlaying) return;
        m_EffectSourceComponent.Play();
    }
}
