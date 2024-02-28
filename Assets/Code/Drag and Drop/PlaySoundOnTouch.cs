using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PlaySoundOnTouch : MonoBehaviour
{
    [SerializeField] private string tagToCheck;

    [SerializeField] private AudioClip audioClip;
    //[SerializeField] private AudioSource audioToPlay;

    private void OnTriggerEnter(Collider other)
    {
        if (!enabled) return;
        if (other.CompareTag(tagToCheck))
        {
            AudioSource audioToPlay = gameObject.AddComponent<AudioSource>();
            audioToPlay.clip = audioClip;
            audioToPlay.gameObject.transform.position = transform.position;
            audioToPlay.time = 0;
            audioToPlay.Play();
        }
    }
}
