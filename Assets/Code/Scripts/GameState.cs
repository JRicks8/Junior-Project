using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour, IDataPersistence
{
    public static GameState instance;

    [Header("Game Data - Serializable")]
    public SerializableDictionary<string, bool> lostSoulsCollected;
    public SerializableDictionary<string, bool> coinsCollected;
    public uint coinsAmt;
    public uint soulsAmt;
    public float personalRecord;
    public float currentTime;
    public bool hasDoubleJump;
    public bool hasDash;
    public bool hasDive;
    public bool hasGrapple;
    public float fastestSpeedAchieved;
    public SerializableDictionary<string, Vector3> checkpoints;

    public delegate void GenericGameStateDelegate();
    public GenericGameStateDelegate CoinCollected;
    public GenericGameStateDelegate LostSoulCollected;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
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
        checkpoints = data.checkpoints;
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
        data.checkpoints = checkpoints;
    }

    public void CollectCoins(uint amt)
    {
        coinsAmt += amt;
        CoinCollected.Invoke();
    }
    
    public void SpendCoins(uint amt)
    {
        if (amt > coinsAmt)
            coinsAmt = 0;
        else
            coinsAmt -= amt;
    }

    public void SetCheckpoint(Vector3 checkpointLocation)
    {
        if (checkpoints.TryGetValue(SceneManager.GetActiveScene().name, out Vector3 _))
        {
            checkpoints[SceneManager.GetActiveScene().name] = checkpointLocation;
        }
        else
        {
            checkpoints.Add(SceneManager.GetActiveScene().name, checkpointLocation);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (checkpoints.TryGetValue(SceneManager.GetActiveScene().name, out Vector3 checkpointLocation))
        {
            checkpoints[SceneManager.GetActiveScene().name] = checkpointLocation;
            // TODO: Set location of player to the checkpoint location
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = checkpointLocation;
            }
        }
        else Debug.Log("Checkpoint not found!");
    }
}