using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Coin : MonoBehaviour, IDataPersistence
{
    // Each coin has a unique id. This is for the save system and remembering which ones we have collected.
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

        data.coinsCollected.TryGetValue(id, out collected);
        if (collected)
        {
            visual.SetActive(false);
            GetComponent<PlayEffectOnTouch>().enabled = false;
            GetComponent<PlaySoundOnTouch>().enabled = false;
        }
    }

    public void SaveData(ref GameData data)
    {
        if (id.Equals(string.Empty)) return;

        if (data.coinsCollected.ContainsKey(id))
        {
            data.coinsCollected.Remove(id);
        }
        data.coinsCollected.Add(id, collected);
    }

    private void Collect()
    {
        visual.SetActive(false);
        collected = true;

        GameState.instance.CollectCoins(1);

        GetComponent<PlayEffectOnTouch>().enabled = false;
        GetComponent<PlaySoundOnTouch>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController _) && !collected)
        {
            Collect();
        }
    }
}
