using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenuObject;
    [SerializeField] private GameObject creditsMenuObject;
    public string gameSceneName = "Movement Testing";

    public void OnContinueClicked()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnNewGameClicked()
    {
        DataPersistenceManager.instance.NewGame();
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnSettingsClicked()
    {
        settingsMenuObject.SetActive(!settingsMenuObject.activeSelf);
        creditsMenuObject.SetActive(false);
    }

    public void OnCreditsClicked()
    {
        creditsMenuObject.SetActive(!creditsMenuObject.activeSelf);
        settingsMenuObject.SetActive(false);
    }

    public void OnQuitClicked()
    {
        DataPersistenceManager.instance.SaveGame();
        Application.Quit();
    }
}
