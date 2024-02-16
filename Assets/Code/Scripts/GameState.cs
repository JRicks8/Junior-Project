using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour, IDataPersistence
{
    public static GameState instance;

    [Header("Game Data - Serializable")]
    public SerializableDictionary<string, bool> lostSouls;
    public uint coins;
    public float personalRecord;
    public float currentTime;
    public bool hasDoubleJump;
    public bool hasDash;
    public bool hasDive;
    public bool hasGrapple;
    public float fastestSpeedAchieved;
    public Transform spawnPoint;

    [Header("Other")]
    private static readonly uint maxCurrency = 999;

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
        hasDoubleJump = data.hasDoubleJump;
        hasDash = data.hasDash;
        hasDive = data.hasDive;
        hasGrapple = data.hasGrapple;
        fastestSpeedAchieved = data.fastestSpeedAchieved;
        spawnPoint = data.spawnPoint;
    }

    public void SaveData(ref GameData data)
    {
        data.lostSoulsCollected = lostSouls;
        data.coins = coins;
        data.personalRecord = personalRecord;
        data.currentTime = currentTime;
        data.hasDoubleJump = hasDoubleJump;
        data.hasDash = hasDash;
        data.hasDive = hasDive;
        data.hasGrapple = hasGrapple;
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
