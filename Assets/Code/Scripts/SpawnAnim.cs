using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAnim : MonoBehaviour
{

    private IEnumerator spawnAnimCoroutine;
    [SerializeField] private float animDuration;
    private Vector3 homePosition;
    private Vector3 animStartPosition;
    [SerializeField] private float jumpHeight;

    public void StartSpawnAnimFrom(Vector3 position)
    {
        homePosition = transform.position;
        animStartPosition = position;
        transform.position = position;
        spawnAnimCoroutine = AnimMovement();
        StartCoroutine(spawnAnimCoroutine);
    }

    private IEnumerator AnimMovement()
    {
        float elapsed = 0;
        float zChangeAmount = 0f;
        while (elapsed < animDuration)
        {
            zChangeAmount = (elapsed / animDuration) * 2;
            float tempJumpHeight;
            if (zChangeAmount < 1f) { tempJumpHeight = Mathf.Lerp(0, jumpHeight, zChangeAmount); }
            else { tempJumpHeight = Mathf.Lerp(jumpHeight, 0, zChangeAmount - 1); }
            transform.position = Vector3.Lerp(animStartPosition, homePosition + new Vector3(0f, tempJumpHeight,0f), elapsed / animDuration);
            elapsed += Time.deltaTime;

            yield return null;
        }

        Collider collision = GetComponent<Collider>();
        if (collision) collision.enabled = true;
        transform.position = homePosition;
    }
}
