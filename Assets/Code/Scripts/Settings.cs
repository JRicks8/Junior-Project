using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour, IDataPersistence
{
    public static Settings instance;

    [SerializeField] private bool vsync;
    [SerializeField] private float globalVolume;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
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

    public void SetVSync(bool value)
    {
        vsync = value;
    }

    public void SetVolume(float value)
    {
        globalVolume = value;
    }
}