using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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
    public Volume globalVolume;// Usa un material con shader que modifique brillo/contraste

    private ColorAdjustments colorAdjustments;

    public Toggle fullscreenToggle;

    [SerializeField] private float minExposure = -2f;
    [SerializeField] private float maxExposure = 2f;

    private void Awake()
    {
        if (globalVolume == null)
        {
            Debug.LogError("[OptionsMenu] ⚠️ No hay Volume asignado en el inspector.");
            return;
        }

        bool found = globalVolume.profile.TryGet(out colorAdjustments);
        if (!found)
        {
            Debug.LogError("[OptionsMenu] ❌ No se encontró Color Adjustments en el Volume Profile.");
        }
        else
        {
            Debug.Log("[OptionsMenu] ✅ Color Adjustments detectado correctamente.");
        }
    }

    private void Start()
    {

        // Inicializa el toggle según el estado actual de la pantalla
        fullscreenToggle.isOn = Screen.fullScreen;

        // Conecta el listener
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        
        SetupQualityOptions();
        //SetupVSyncOptions();
        //SetupLODOptions();

        brightnessSlider.onValueChanged.AddListener(SetBrightness);
        //contrastSlider.onValueChanged.AddListener(SetContrast);

        Debug.Log("[OptionsMenu] ✅ Listeners de sliders conectados.");

        LoadSettings();
    }

    private void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        Debug.Log("[OptionsMenu] Fullscreen: " + isFullscreen);
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
        if (colorAdjustments == null)
        {
            Debug.LogWarning("[OptionsMenu] ⚠️ ColorAdjustments es NULL, no se aplica brillo.");
            return;
        }

        // Normalizamos el valor del slider a 0–1
        float normalized = Mathf.InverseLerp(brightnessSlider.minValue, brightnessSlider.maxValue, value);

        // Mapeamos al rango real de exposición
        float mapped = Mathf.Lerp(minExposure, maxExposure, normalized);

        // Aplicamos el brillo
        colorAdjustments.postExposure.value = mapped;

        Debug.Log($"[OptionsMenu] Brillo Slider: {value} → Brillo aplicado: {mapped}");
    }
    

    public void SetContrast(float value)
    {
        Debug.Log($"[OptionsMenu] Contraste Slider: {value}");
        if (colorAdjustments != null)
        {
            float mapped = Mathf.Lerp(-50f, 50f, value);
            colorAdjustments.contrast.value = mapped;
            Debug.Log($"[OptionsMenu] → Contraste aplicado: {mapped}");
        }
        else
        {
            Debug.LogWarning("[OptionsMenu] ⚠️ ColorAdjustments es NULL, no se aplica contraste.");
        }
    }


    // ---------------- PERSISTENCIA OPCIONAL ----------------
    private void LoadSettings()
    {
        brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 0f);
        contrastSlider.value = PlayerPrefs.GetFloat("Contrast", 0f);
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
