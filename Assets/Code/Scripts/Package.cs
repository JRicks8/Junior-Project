using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Package : MonoBehaviour, IDataPersistence
{
    [SerializeField] private string id;

    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    [SerializeField] private GameObject visual;
    [SerializeField] private bool collected = false;

    public void LoadData(GameData data)
    {
        data.packagesCollected.TryGetValue(id, out collected);
        if (collected)
        {
            visual.SetActive(false);
            //GetComponent<PlayEffectOnTouch>().enabled = false;
            //GetComponent<PlaySoundOnTouch>().enabled = false;
        }
    }

    public void SaveData(ref GameData data)
    {
        if (data.packagesCollected.ContainsKey(id))
        {
            data.packagesCollected.Remove(id);
        }
        data.packagesCollected.Add(id, collected);
    }

    private void Collect()
    {
        visual.SetActive(false);
        collected = true;

        // TODO: On collect, save package state to game state and update player GUI

        //GetComponent<PlayEffectOnTouch>().enabled = false;
        //GetComponent<PlaySoundOnTouch>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController _) && !collected)
        {
            Collect();
        }
    }
}
