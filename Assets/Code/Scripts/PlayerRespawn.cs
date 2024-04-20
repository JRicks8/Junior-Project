using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRespawn : MonoBehaviour
{
    private void Start()
    {
        transform.position = GameState.instance.checkpoints[SceneManager.GetActiveScene().name];
    }
}
