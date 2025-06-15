using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    [Header("Dropdowns")]
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown vsyncDropdown;
    public TMP_Dropdown lodDropdown;

    [Header("Sliders")]
    public Slider brightnessSlider;
    public Slider contrastSlider;

    [Header("Post Processing")]
    public Material postProcessMaterial; // Usa un material con shader que modifique brillo/contraste

    private void Start()
    {
        SetupQualityOptions();
        SetupVSyncOptions();
        SetupLODOptions();

        brightnessSlider.onValueChanged.AddListener(SetBrightness);
        contrastSlider.onValueChanged.AddListener(SetContrast);

        LoadSettings();
    }

    // ---------------- CALIDAD ----------------
    private void SetupQualityOptions()
    {
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new System.Collections.Generic.List<string> { "Baja", "Media", "Alta", "Ultra" });
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();
        qualityDropdown.onValueChanged.AddListener(SetQuality);
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index, true);
    }

    // ---------------- VSYNC ----------------
    private void SetupVSyncOptions()
    {
        vsyncDropdown.ClearOptions();
        vsyncDropdown.AddOptions(new System.Collections.Generic.List<string> { "Off", "On" });
        vsyncDropdown.value = QualitySettings.vSyncCount > 0 ? 1 : 0;
        vsyncDropdown.RefreshShownValue();
        vsyncDropdown.onValueChanged.AddListener(SetVSync);
    }

    public void SetVSync(int index)
    {
        QualitySettings.vSyncCount = index == 0 ? 0 : 1;
    }

    // ---------------- LOD (Nivel de detalle) ----------------
    private void SetupLODOptions()
    {
        lodDropdown.ClearOptions();
        lodDropdown.AddOptions(new System.Collections.Generic.List<string> { "Automático", "Bajo", "Alto" });
        lodDropdown.value = 0; // Default: Automático
        lodDropdown.RefreshShownValue();
        lodDropdown.onValueChanged.AddListener(SetLOD);
    }

    public void SetLOD(int index)
    {
        switch (index)
        {
            case 0: QualitySettings.lodBias = 1f; break;      // Automático (default)
            case 1: QualitySettings.lodBias = 0.5f; break;    // Bajo
            case 2: QualitySettings.lodBias = 2f; break;      // Alto
        }
    }

    // ---------------- BRILLO Y CONTRASTE ----------------
    public void SetBrightness(float value)
    {
        if (postProcessMaterial != null)
            postProcessMaterial.SetFloat("_Brightness", value);
    }

    public void SetContrast(float value)
    {
        if (postProcessMaterial != null)
            postProcessMaterial.SetFloat("_Contrast", value);
    }


    public void ToggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }


    // ---------------- PERSISTENCIA OPCIONAL ----------------
    private void LoadSettings()
    {
        brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 1f);
        contrastSlider.value = PlayerPrefs.GetFloat("Contrast", 1f);
        SetBrightness(brightnessSlider.value);
        SetContrast(contrastSlider.value);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("Brightness", brightnessSlider.value);
        PlayerPrefs.SetFloat("Contrast", contrastSlider.value);
        PlayerPrefs.Save();
    }
    
}
