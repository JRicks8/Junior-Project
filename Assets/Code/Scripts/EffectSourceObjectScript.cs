using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSourceObjectScript : MonoBehaviour
{
    private new ParticleSystem particleSystem;
    private IEnumerator dieCoroutine;
    private float elapsed = 0;
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        dieCoroutine = Die();
        StartCoroutine(dieCoroutine);
    }

    private IEnumerator Die()
    {
        while (particleSystem.isPlaying)
        {
            elapsed += 0.2f;
            yield return new WaitForSeconds(0.2f);
            if (elapsed > particleSystem.main.duration) particleSystem.Stop();
            if (!particleSystem) particleSystem = GetComponent<ParticleSystem>();

            if (!particleSystem.isPlaying)
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
