using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostSoul : MonoBehaviour, IDataPersistence
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
        data.lostSoulsCollected.TryGetValue(id, out collected);
        if (collected)
        {
            visual.SetActive(false);
        }
    }

    public void SaveData(ref GameData data)
    {
        if (data.lostSoulsCollected.ContainsKey(id))
        {
            data.lostSoulsCollected.Remove(id);
        }
        data.lostSoulsCollected.Add(id, collected);
    }

    private void Collect()
    {
        visual.SetActive(false);
        collected = true;

        GameState.instance.CollectLostSoul();
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
        }
    }
}
