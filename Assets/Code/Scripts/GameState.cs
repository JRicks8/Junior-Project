using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour, IDataPersistence
{
    public static GameState instance;

    [Header("Game Data - Serializable")]
    public Dictionary<string, bool> lostSouls;
    public uint coins;
    public float personalRecord;
    public float currentTime;
    public Dictionary<string, bool> abilitiesCollected;
    public float fastestSpeedAchieved;
    public Transform spawnPoint;

    [Header("Other")]
    private static uint maxCurrency;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Game State in the scene.");
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadData(GameData data)
    {
        lostSouls = data.lostSoulsCollected;
        coins = data.coins;
        personalRecord = data.personalRecord;
        currentTime = data.currentTime;
        abilitiesCollected = data.abilitiesCollected;
        fastestSpeedAchieved = data.fastestSpeedAchieved;
        spawnPoint = data.spawnPoint;
    }

    public void SaveData(ref GameData data)
    {
        data.lostSoulsCollected = lostSouls;
        data.coins = coins;
        data.personalRecord = personalRecord;
        data.currentTime = currentTime;
        data.abilitiesCollected = abilitiesCollected;
        data.fastestSpeedAchieved = fastestSpeedAchieved;
        data.spawnPoint = spawnPoint;
    }

    public void CollectCurrency(uint amt)
    {
        coins += amt;
        if (coins > maxCurrency) coins = maxCurrency;
    }
    
    public void SpendCurrency(uint amt)
    {
        if (amt > coins)
            coins = 0;
        else
            coins -= amt;
    }
}
