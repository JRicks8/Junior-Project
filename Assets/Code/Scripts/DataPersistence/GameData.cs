using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // Game
    public SerializableDictionary<string, bool> lostSoulsCollected;
    public SerializableDictionary<string, bool> coinsCollected;
    public float personalRecord;
    public float currentTime;
    public bool hasDoubleJump;
    public bool hasDash;
    public bool hasDive;
    public bool hasGrapple;
    public float fastestSpeedAchieved;
    public Transform spawnPoint;
    // Settings
    public bool vsync;

    public GameData()
    {
        // Game
        lostSoulsCollected = new SerializableDictionary<string, bool>();
        coinsCollected = new SerializableDictionary<string, bool>();
        personalRecord = 0;
        currentTime = 0;
        hasDoubleJump = false;
        hasDash = false;
        hasDive = false;
        hasGrapple = false;
        fastestSpeedAchieved = 0;
        spawnPoint = null;
        // Settings
        vsync = false;
    }
}
