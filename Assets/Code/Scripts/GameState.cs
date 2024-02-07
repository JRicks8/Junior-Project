using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameState : MonoBehaviour
{
    [Header("GameState Variables")]
    [SerializeField] private uint coins;
    [SerializeField] private Transform spawnPoint;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
