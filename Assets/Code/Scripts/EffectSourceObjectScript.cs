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
            yield return new WaitForSeconds(0.2f);
            elapsed += Time.deltaTime;
            if (elapsed > particleSystem.main.duration) particleSystem.Stop(); // HEHE I DONT KNOW HOW TO GET IT TO SHUT OFF
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
