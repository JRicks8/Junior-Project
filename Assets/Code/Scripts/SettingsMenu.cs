using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour, IDataPersistence
{
    [SerializeField] private GameObject mainMenuObject;
    [SerializeField] private Toggle vsyncToggle;
    [SerializeField] private Slider volumeSlider;

    private bool isPrefab = true;

    private void Awake()
    {
        vsyncToggle.onValueChanged.AddListener(OnToggleVSync);
        volumeSlider.onValueChanged.AddListener(OnVolumeOptionChanged);

        isPrefab = false;
    }

    public void LoadData(GameData data)
    {
        vsyncToggle.isOn = data.vsync;
        OnToggleVSync(data.vsync);
        volumeSlider.value = data.volume;
        OnVolumeOptionChanged(data.volume);
    }

    public void SaveData(ref GameData data)
    {
        if (isPrefab) return;
        data.vsync = vsyncToggle.isOn;
        data.volume = volumeSlider.value;
    }

    public void OnToggleVSync(bool _)
    {
        Settings.instance.SetVSync(vsyncToggle.isOn);
        if (vsyncToggle.isOn)
            QualitySettings.vSyncCount = 1;
        else
            QualitySettings.vSyncCount = 0;
    }

    public void OnBackButtonClicked()
    {
        mainMenuObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void OnVolumeOptionChanged(float value)
    {
        Settings.instance.SetVolume(value);
        AudioListener.volume = value;
    }
}
