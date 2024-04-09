using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGUI : MonoBehaviour, IDataPersistence
{
    [Header("References to Set")]
    public PlayerController controller;
    // GUI Elements
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI lostSoulsText;
    public Image packageImage;

    private void Start()
    {
        GameState.instance.CoinCollected += OnCoinCollected;
        GameState.instance.LostSoulCollected += OnLostSoulCollected;
        controller.PackageCollected += OnPackageCollected;
        controller.PackageRemoved += OnPackageRemoved;
    }

    public void LoadData(GameData data)
    {
        coinsText.text = data.coinsAmt.ToString();
        lostSoulsText.text = data.soulsAmt.ToString();
        packageImage.enabled = data.hasPackage;
    }

    public void SaveData(ref GameData data)
    {
        // Don't save the coin or soul amounts data because that is handled separately inside the Game State
    }

    private void OnCoinCollected()
    {
        coinsText.text = GameState.instance.coinsAmt.ToString();
    }

    private void OnLostSoulCollected()
    {
        lostSoulsText.text = GameState.instance.soulsAmt.ToString();
    }

    private void OnPackageCollected()
    {
        packageImage.enabled = true;
    }

    private void OnPackageRemoved()
    {
        packageImage.enabled = false;
    }
}
