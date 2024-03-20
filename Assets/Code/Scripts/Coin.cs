using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour, IDataPersistence
{
    // Each lost soul has a unique id. This is for the save system and remembering which ones we have collected.
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
        data.coinsCollected.TryGetValue(id, out collected);
        if (collected)
        {
            visual.SetActive(false);
        }
        if (collected)
        {
            visual.SetActive(false);
        }
    }

    private void Collect()
    {
        visual.SetActive(false);
        collected = true;

        GameState.instance.CollectCoins(1);
    }

    public void SaveData(ref GameData data)
    {
        if (data.coinsCollected.ContainsKey(id))
        {
            data.coinsCollected.Remove(id);
        }
        data.coinsCollected.Add(id, collected);
    }

    private void OnValidate()
    {
        if (id == string.Empty)
        {
            GenerateGuid();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController _) && !collected)
        {
            Collect();

            GetComponent<PlayEffectOnTouch>().enabled = false;
            GetComponent<PlaySoundOnTouch>().enabled = false;
        }
    }
}
