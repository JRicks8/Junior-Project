using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour, IDataPersistence
{
    public static Settings instance;

    public bool vsync;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Settings object in the scene.");
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadData(GameData data)
    {
        vsync = data.vsync;
    }

    public void SaveData(ref GameData data)
    {
        data.vsync = vsync;
    }
}