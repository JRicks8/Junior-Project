using System.Collections.Generic;
using System.Data.Common;
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

    private bool isPrefab = true;

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

        isPrefab = false;
    }

    public void LoadData(GameData data)
    {
        if (isPrefab) return;
        lostSoulsCollected = data.lostSoulsCollected;
        coinsCollected = data.coinsCollected;
        coinsAmt = data.coinsAmt;
        soulsAmt = data.soulsAmt;
        personalRecord = data.personalRecord;
        currentTime = data.currentTime;
        fastestSpeedAchieved = data.fastestSpeedAchieved;
        checkpoints = data.checkpoints;
    }

    public void SaveData(ref GameData data)
    {
        if (isPrefab) return;
        data.lostSoulsCollected = lostSoulsCollected;
        data.coinsCollected = coinsCollected;
        data.coinsAmt = coinsAmt;
        data.soulsAmt = soulsAmt;
        data.personalRecord = personalRecord;
        data.currentTime = currentTime;
        data.fastestSpeedAchieved = fastestSpeedAchieved;
        data.checkpoints = checkpoints;
    }

    public void CollectCoins(uint amt)
    {
        coinsAmt += amt;
        CoinCollected.Invoke();
    }

    public void CollectLostSoul()
    {
        soulsAmt++;
        LostSoulCollected.Invoke();
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
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = checkpointLocation;
            }
        }
        else Debug.Log("Checkpoint not found!");
    }
}