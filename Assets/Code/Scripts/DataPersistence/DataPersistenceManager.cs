using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    public bool dataLoaded = false;

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    public static DataPersistenceManager instance { get; private set; }

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

    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.activeSceneChanged += OnSceneChange;
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    private void OnSceneChange(Scene scene1, Scene scene2)
    {
        SaveGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        this.gameData = dataHandler.Load();

        if (this.gameData == null)
        {
            Debug.Log("No save data was found. Initializing to defaults.");
            NewGame();
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }

        Debug.Log("Loaded Data");
        dataLoaded = true;
    }

    public void SaveGame()
    {
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        // Handle overflow of coin and lost soul amount (shouldn't ever happen in the shipped game)
        if (gameData.coinsAmt > gameData.coinsCollected.Count)
            gameData.coinsAmt = (uint)gameData.coinsCollected.Count;
        if (gameData.soulsAmt > gameData.lostSoulsCollected.Count)
            gameData.soulsAmt = (uint)gameData.lostSoulsCollected.Count;

        Debug.Log("Saved Data");

        dataHandler.Save(gameData);
    }

    /// <summary>
    /// ONLY CALL THIS FUNCTION IF YOU'RE ABSOLUTELY SURE YOU NEED THIS.
    /// </summary>
    public GameData GetGameData()
    {
        return gameData;
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        // Use this function because we want to find ALL objects that inherit the Data Persistence interface, including disabled ones.
        IEnumerable<IDataPersistence> dataPersistenceObjects = Resources.FindObjectsOfTypeAll<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
