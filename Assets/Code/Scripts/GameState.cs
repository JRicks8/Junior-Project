using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour, IDataPersistence
{
    public static GameState instance;

    [Header("Game Data - Serializable")]
    public SerializableDictionary<string, bool> lostSoulsCollected;
    public SerializableDictionary<string, bool> coinsCollected;
    public float coinsAmt;
    public float personalRecord;
    public float currentTime;
    public bool hasDoubleJump;
    public bool hasDash;
    public bool hasDive;
    public bool hasGrapple;
    public float fastestSpeedAchieved;
    public Transform spawnPoint;

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
        lostSoulsCollected = data.lostSoulsCollected;
        coinsCollected = data.coinsCollected;
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
        data.lostSoulsCollected = lostSoulsCollected;
        data.coinsCollected = coinsCollected;
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
        coinsAmt += amt;
    }
    
    public void SpendCurrency(uint amt)
    {
        if (amt > coinsAmt)
            coinsAmt = 0;
        else
            coinsAmt -= amt;
    }

    public void SetSpawnPoint(Transform spawnPoint)
    {
        this.spawnPoint = spawnPoint;
    }
}
