using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public bool[] lostSouls;
    public uint coins;
    public float personalRecord;
    public float currentTime;
    public bool[] abilitiesCollected;
    public float fastestSpeedAchieved;
    public Transform spawnPoint;

    public GameData()
    {
        lostSouls = new bool[0];
        coins = 0;
        personalRecord = 0;
        currentTime = 0;
        abilitiesCollected = new bool[0];
        fastestSpeedAchieved = 0;
        spawnPoint = null;
    }
}
