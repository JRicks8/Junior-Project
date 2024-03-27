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

    private void Start()
    {
        if (id.Equals(string.Empty))
            Debug.LogError("GUID for this object is null. Please assign a GUID for saving data.");
    }

    public void LoadData(GameData data)
    {
        if (id.Equals(string.Empty)) return;

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
        if (id.Equals(string.Empty)) return;

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

        //GetComponent<PlayEffectOnTouch>().enabled = false;
        //GetComponent<PlaySoundOnTouch>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController playerController) && !collected && !playerController.HasPackage())
        {
            Collect();
            playerController.CollectPackage();
        }
    }
}
