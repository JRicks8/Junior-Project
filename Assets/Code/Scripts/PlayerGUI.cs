using TMPro;
using UnityEngine;

public class PlayerGUI : MonoBehaviour, IDataPersistence
{
    [Header("References to Set")]
    public PlayerController controller;
    // GUI Elements
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI lostSoulsText;

    private void Start()
    {
        GameState.instance.CoinCollected += OnCoinCollected;
        GameState.instance.LostSoulCollected += OnLostSoulCollected;
    }

    public void LoadData(GameData data)
    {
        coinsText.text = data.coinsAmt.ToString();
        lostSoulsText.text = data.soulsAmt.ToString();
    }

    public void SaveData(ref GameData data)
    {
        
    }

    private void OnCoinCollected()
    {
        coinsText.text = GameState.instance.coinsAmt.ToString();
    }

    private void OnLostSoulCollected()
    {
        lostSoulsText.text = GameState.instance.soulsAmt.ToString();
    }
}
