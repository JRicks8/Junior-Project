using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayEffectOnTouch : MonoBehaviour
{
    [Header("Set this value to 'Player' if you want the player to activate it.")]
    [SerializeField] private string tagToCheck;

    [Header("Make sure to turn off 'looping' and 'play on awake' :D")]
    [SerializeField] private ParticleSystem effectToPlay;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagToCheck))
        {
            effectToPlay.Play();
        }
    }
}
