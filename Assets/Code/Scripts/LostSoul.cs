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

    private void Start()
    {
        if (id.Equals(string.Empty))
            Debug.LogError("GUID for this object is null. Please assign a GUID for saving data.");
    }

    public void LoadData(GameData data)
    {
        if (id.Equals(string.Empty)) return;

        data.lostSoulsCollected.TryGetValue(id, out collected);
        if (collected)
        {
            visual.SetActive(false);
        }
    }

    public void SaveData(ref GameData data)
    {
        if (id.Equals(string.Empty)) return;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController _) && !collected)
        {
            Collect();
        }
    }
}
