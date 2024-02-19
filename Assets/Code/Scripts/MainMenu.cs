using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string gameSceneName = "Movement Testing";

    public void OnContinueClicked()
    {
        DataPersistenceManager.instance.LoadGame();
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnNewGameClicked()
    {
        DataPersistenceManager.instance.NewGame();
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnSettingsClicked()
    {
        Debug.Log("NOT IMPLEMENTED :(");
    }

    public void OnQuitClicked()
    {
        DataPersistenceManager.instance.SaveGame();
        Application.Quit();
    }
}
