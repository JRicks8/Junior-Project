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
        if (other.CompareTag(tagToCheck))
        {
            if (!audioToPlay.isPlaying) audioToPlay.Play();
        }
    }
}
