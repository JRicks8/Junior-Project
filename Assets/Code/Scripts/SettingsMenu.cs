using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour, IDataPersistence
{
    [SerializeField] private GameObject mainMenuObject;

    [SerializeField] private Toggle vsyncToggle;

    private void Awake()
    {
        vsyncToggle.onValueChanged.AddListener(OnToggleVSync);
    }

    public void LoadData(GameData data)
    {
        vsyncToggle.isOn = data.vsync;
        OnToggleVSync(data.vsync);
    }

    public void SaveData(ref GameData data)
    {
        data.vsync = vsyncToggle.isOn;
    }

    public void OnToggleVSync(bool value)
    {
        Settings.instance.SetVSync(value);
        if (value)
            QualitySettings.vSyncCount = 1;
        else
            QualitySettings.vSyncCount = 0;
    }

    public void OnBackButtonClicked()
    {
        mainMenuObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
