using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public Dictionary<string, bool> lostSoulsCollected;
    public uint coins;
    public float personalRecord;
    public float currentTime;
    public Dictionary<string, bool> abilitiesCollected;
    public float fastestSpeedAchieved;
    public Transform spawnPoint;

    public GameData()
    {
        lostSoulsCollected = new Dictionary<string, bool>();
        coins = 0;
        personalRecord = 0;
        currentTime = 0;
        abilitiesCollected = new Dictionary<string, bool>();
        fastestSpeedAchieved = 0;
        spawnPoint = null;
    }
}
