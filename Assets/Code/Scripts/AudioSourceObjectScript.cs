using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceObjectScript : MonoBehaviour
{
    private AudioSource audioSource;
    private IEnumerator dieCoroutine;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        dieCoroutine = Die();
        StartCoroutine(dieCoroutine);
    }

    private IEnumerator Die()
    {
        while (audioSource.isPlaying)
        {
            yield return new WaitForSeconds(0.2f);
            if (!audioSource.isPlaying)
            {
                Destroy(gameObject);
                yield break;
            }
        }
    }

    private void OnDestroy()
    {
        Debug.Log("Destroyed Audio Source Object");
    }
}
