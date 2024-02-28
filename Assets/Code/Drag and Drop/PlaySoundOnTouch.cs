using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlaySoundOnTouch : MonoBehaviour
{
    [SerializeField] private string tagToCheck;

    [SerializeField] private AudioSource audioToPlay;

    private void OnTriggerEnter(Collider other)
    {
        if (!enabled) return;
        if (other.CompareTag(tagToCheck))
        {
            audioToPlay.gameObject.transform.position = transform.position;
            audioToPlay.time = 0;
            audioToPlay.Play();
        }
    }
}
