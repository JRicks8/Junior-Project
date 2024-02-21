using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void OnResumeButtonClicked()
    {
        gameObject.SetActive(false);
    }

    public void OnQuitButtonClicked()
    {
        SceneManager.LoadScene("Main Menu");
    }
}