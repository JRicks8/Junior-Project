using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerGUI : MonoBehaviour
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

    private void OnCoinCollected()
    {
        coinsText.text = GameState.instance.coinsAmt.ToString();
    }

    private void OnLostSoulCollected()
    {
        // TODO: Implement delegate invoke on collecting a lost soul
        lostSoulsText.text = GameState.instance.soulsAmt.ToString();
    }
}
