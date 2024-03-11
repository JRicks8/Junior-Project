using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayEffectOnTouch : MonoBehaviour
{
    [Header("Set this value to 'Player' if you want the player to activate it.")]
    [SerializeField] private string tagToCheck;

    [Header("Make sure to turn off 'looping' and 'play on awake' :D")]
    [SerializeField] private GameObject effectToPlay;

    private void OnTriggerEnter(Collider other)
    {
        if (!enabled) return;
        if (other.CompareTag(tagToCheck))
        {
            GameObject newInst = Instantiate(effectToPlay);
            newInst.SetActive(true);
            newInst.transform.position = transform.position;
            newInst.transform.rotation = transform.rotation;
            newInst.GetComponent<ParticleSystem>().Play();
            newInst.GetComponent<DestroyEffectOnFinish>().enabled = true;
        }
    }
}
