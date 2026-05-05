using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Sliders")]
    public Slider sensitivitySlider;
    public Slider volumeSlider;

    void Start()
    {
        // Wczytaj zapisane (albo default)
        float sens = PlayerPrefs.GetFloat("Sensitivity", 2f);
        float vol = PlayerPrefs.GetFloat("Volume", 1f);

        // Ustaw UI
        sensitivitySlider.value = sens;
        volumeSlider.value = vol;

        // Zastosuj od razu
        ApplySensitivity(sens);
        ApplyVolume(vol);

        // Reakcja na zmiany
        sensitivitySlider.onValueChanged.AddListener(ApplySensitivity);
        volumeSlider.onValueChanged.AddListener(ApplyVolume);
    }

    public void ApplySensitivity(float value)
    {
        PlayerPrefs.SetFloat("Sensitivity", value);
        PlayerPrefs.Save();
    }

    public void ApplyVolume(float value)
    {
        AudioListener.volume = value;     // globalnie dla ca³ej gry
        PlayerPrefs.SetFloat("Volume", value);
        PlayerPrefs.Save();
    }
}