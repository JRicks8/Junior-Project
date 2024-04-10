using UnityEngine;

[System.Serializable]
public class GameData
{
    // Game
    public SerializableDictionary<string, bool> lostSoulsCollected;
    public SerializableDictionary<string, bool> coinsCollected;
    public SerializableDictionary<string, bool> packagesCollected;
    public SerializableDictionary<string, bool> collectionZones;
    public uint coinsAmt;
    public uint soulsAmt;
    public float personalRecord;
    public float currentTime;
    public bool hasDoubleJump;
    public bool hasDash;
    public bool hasDive;
    public bool hasGrapple;
    public bool hasPackage;
    public float fastestSpeedAchieved;
    public SerializableDictionary<string, Vector3> checkpoints;
    // Settings
    public bool vsync;
    public float volume;

    public GameData()
    {
        // Game
        lostSoulsCollected = new SerializableDictionary<string, bool>();
        coinsCollected = new SerializableDictionary<string, bool>();
        packagesCollected = new SerializableDictionary<string, bool>();
        collectionZones = new SerializableDictionary<string, bool>();
        coinsAmt = 0;
        soulsAmt = 0;
        personalRecord = 0;
        currentTime = 0;
        hasDoubleJump = false;
        hasDash = false;
        hasDive = false;
        hasGrapple = false;
        hasPackage = false;
        fastestSpeedAchieved = 0;
        checkpoints = new SerializableDictionary<string, Vector3>();
        // Settings
        vsync = false;
        volume = 0.5f;
    }
}
