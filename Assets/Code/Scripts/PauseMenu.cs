using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    // TODO: Quitting to main menu then starting a new game causes weird input callback method errors. Fix later

    public static bool paused = false;

    [SerializeField] private InputActionAsset actions;
    private InputAction escapeAction;
    [SerializeField] private GameObject menuContent;

    private void Awake()
    {
        InputActionMap actionMap = actions.FindActionMap("Gameplay");
        escapeAction = actionMap.FindAction("escape");
        escapeAction.performed += TogglePauseMenu;
    }

    private void TogglePauseMenu(InputAction.CallbackContext context)
    {
        paused = !menuContent.activeSelf;

        menuContent.SetActive(paused);
        Cursor.visible = paused;

        if (paused)
        {
            Time.timeScale = 0.0f;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1.0f;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void OnResumeButtonClicked()
    {
        TogglePauseMenu(default);
    }

    public void OnQuitButtonClicked()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("Main Menu");
    }

    private void OnDestroy()
    {
        // Remove Listeners
        escapeAction.performed -= TogglePauseMenu;
    }
}