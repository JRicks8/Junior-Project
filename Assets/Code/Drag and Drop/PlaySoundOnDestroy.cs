using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnDestroy : MonoBehaviour
{
    [Header("Required: Attach the blue cube labeled\n'AudioSourceObject' from the DragAndDrop folder.")]
    [SerializeField] private GameObject m_AudioSourceObjectPrefab;

    [Header("Required: Attach a new component of type 'AudioSource'\nthen drag from the created component into here.")]
    [SerializeField] private AudioSource m_AudioSourceComponent;

    private void OnDestroy()
    {
        GameObject spawnedObject = Instantiate(m_AudioSourceObjectPrefab);
        AudioSource spawnedObjAudioSource = spawnedObject.GetComponent<AudioSource>();
        spawnedObjAudioSource.clip = m_AudioSourceComponent.clip;
        spawnedObjAudioSource.volume = m_AudioSourceComponent.volume;
        spawnedObjAudioSource.pitch = m_AudioSourceComponent.pitch;
        spawnedObjAudioSource.reverbZoneMix = m_AudioSourceComponent.reverbZoneMix;
        if (!spawnedObjAudioSource || spawnedObjAudioSource.isPlaying) return;
        spawnedObjAudioSource.Play();
    }
}
