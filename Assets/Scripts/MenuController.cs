using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SFB;
using System.Threading.Tasks;
using System.Numerics;
using CesiumForUnity;

public class MenuController : MonoBehaviour
{
    public static MenuController Instance { get; private set; }
    public TMP_InputField ipAddress;
    public TMP_InputField commandPort;
    public TMP_InputField imagePort;
    public TMP_Dropdown resolution;

    public TMP_Dropdown attributes;
    public Toggle fullscreen;
    Resolution[] resolutions;

    ExcelReader excelReader;

    [SerializeField] public TMP_InputField urlInputField;

    public string fileExcelPath = null;

    public int excelSheetIndex = 0;

    public string fileKMLPath = null;

    [SerializeField] private TMP_Text fileExcelText;

    public TMP_Text fileKMLText;

    [SerializeField] private Button save;

    private Color normalColorText;
    private Color errorColorText;
    private Color saveColorBg;

    public bool byClick = false;

    public List<GameObject> instantiatedToggles;

    [SerializeField] public GameObject filterMenu;

    [SerializeField] public GameObject Canvas;

    public string selectedKey;

    [SerializeField] private GameObject warningMessagePrefab;
    [SerializeField] private Transform canvasTransform; // Lugar donde instanciar (Canvas)

    public TMP_Text warningText;

    public bool isCesium;

    public GameObject selectedRepresentation;

    public List<GameObject> menus = new List<GameObject>();


    /*
    private string[] ipAddress;
    private int[] commandPort;
    private int[] imagePort;
    private int[] resolutionIndex;
    private bool[] fullscreen;

    private const int CURRENT = 0;
    private const int MODIFIED = 1;

    void Awake() {
        ipAddress = new string[2];
        commandPort = new int[2];
        imagePort = new int[2];
        resolutionIndex = new int[2];
        fullscreen = new bool[2];
        PopulateFromPlayerPrefs();
    }
    */

    private void displayIfLastSelectedFileExists()
    {
        // Excel
        if (File.Exists("lastfileExcel.txt"))
        {
            fileExcelPath = File.ReadAllText("lastfileExcel.txt");
            if (!File.Exists(fileExcelPath))
            {
                fileExcelText.color = Color.red;
                fileExcelText.text = "Could not find selected Excel file " + Path.GetFileName(fileExcelPath);
            }
            else
            {
                fileExcelText.color = Color.black;
                fileExcelText.text = "Last selected Excel file: " + Path.GetFileName(fileExcelPath);
            }
        }
        else {
            fileExcelText.text = "No selected Excel file";
        }
        // KML
        if (File.Exists("lastfileKML.txt"))
        {
            fileKMLPath = File.ReadAllText("lastfileKML.txt");
            if (!File.Exists(fileKMLPath))
            {
                fileKMLText.color = Color.red;
                fileKMLText.text = "Could not find last selected KML file" + Path.GetFileName(fileKMLPath);
            }
            else
            {
                fileKMLText.color = Color.black;
                fileKMLText.text = "Last selected KML file: " + Path.GetFileName(fileKMLPath);
            }
        }
        else {
            fileKMLText.text = "No selected KML file";
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        errorColorText = Color.red;
        //normalColorText = commandPort.colors.selectedColor;
        //saveColorBg = save.image.color;
        displayIfLastSelectedFileExists();
    }

    private void Start()
    {
        resolutions = Screen.resolutions;
        PopulateResolutionDropdown();
    }

    private void Update()
    {
        //bool errors = false;
        //errors |= CheckPortText(xmppPort);
        //UpdateSaveButton(errors);
    }

    private void UpdateSaveButton(bool errors)
    {
        save.enabled = !errors;
        if (errors)
            save.image.color = Color.gray;
        else
            save.image.color = saveColorBg;
    }

    /* private bool CheckPortText(TMP_InputField control) {
        bool error = false;
        if (!AllNumbers(control.text))
            error = true;
        else {
            var value = int.Parse(control.text);
            if (value < 1 || value > 65535)
                error = true;
        } 
        ChangeColorOnError(control, error);
        return error;
    } */

    private void ChangeColorOnError(TMP_InputField control, bool error)
    {
        var colors = control.colors;
        if (error)
        {
            colors.selectedColor = errorColorText;
            colors.normalColor = errorColorText;
        }
        else
        {
            colors.selectedColor = normalColorText;
            colors.normalColor = normalColorText;
        }
        control.colors = colors;
    }

    private bool AllNumbers(string text)
    {
        bool result = true;
        for (int i = 0; i < text.Length && result; i++)
            result &= char.IsDigit(text[i]);
        return result;
    }

    private void PopulateResolutionDropdown()
    {
        resolution.ClearOptions();
        int currentResolutionIndex = 0;
        List<string> resolutionDescriptions = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            string resolution = resolutions[i].width + " x " + resolutions[i].height;
            resolutionDescriptions.Add(resolution);
            if (IsActiveResolution(resolutions[i]))
                currentResolutionIndex = i;
        }
        resolution.AddOptions(resolutionDescriptions);
        resolution.value = currentResolutionIndex;
        resolution.RefreshShownValue();
    }

    private bool IsActiveResolution(Resolution resolution)
    {
        Resolution current = Screen.currentResolution;
        return resolution.width == current.width && resolution.height == current.height;
    }




    public void SetResolution(int resolutionIndex)
    {
        if (resolutionIndex >= resolutions.Length)
            resolutionIndex = resolutions.Length - 1;
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
    }
    public void StartSimulatorSatelital()
    {
        try
        {
            if (string.IsNullOrEmpty(fileExcelPath) || string.IsNullOrEmpty(fileKMLPath))
            {
                Warning("Para la vista satelital necesitas:\n\n- Un archivo .kml para representar los cultivos vía vista satelital.\n- Un archivo .xlsx para asociar cada árbol con sus atributos.");
                return;
            }

            Debug.Log("Processing KML file: " + fileKMLPath);
            double[] coords = KMLParser.GetCoordinatesFromSpawn(fileKMLPath);
            double[] coordsEFEC = CoordinateConverter.GeodeticToEcef(coords);
            GeneralData.coords = coordsEFEC;
            GeneralData.coordenadas = KMLParser.ParseKml(fileKMLPath);

            Debug.Log("Starting satelital view simulation");
            SceneManager.LoadScene("Mapa");
            gameObject.SetActive(false);
            isCesium = true;
        }
        catch (Exception ex)
        {
            Debug.LogError("Error during satelital simulation startup: " + ex.Message);
            Warning("Error durante el inicio de la simulación satelital: " + ex.Message);
        }
    }

    public void StartSimulatorVirtual()
    {
        try
        {
            if (string.IsNullOrEmpty(fileExcelPath))
            {
                Warning("Para la vista virtual necesitas:\n\n- Un archivo .xlsx para asociar cada árbol con sus atributos.");
                return;
            }

            Debug.Log("Starting virtual view simulation");
            SceneManager.LoadScene("SandBox");
            gameObject.SetActive(false);
            isCesium = false;
        }
        catch (Exception ex)
        {
            Debug.LogError("Error during virtual simulation startup: " + ex.Message);
            Warning("Error durante el inicio de la simulación virtual: " + ex.Message);
        }
    }
    /* private void PopulateAttributesDropdown(){
    // Limpiar las opciones existentes del Dropdown
        attributes.ClearOptions();

        ExcelReader excelReader = gameObject.AddComponent<ExcelReader>();
        // Obtenemos la lista de diccionarios desde el Excel
        List<Dictionary<string, string>> treeDataList = excelReader.ReadExcelData();

        // Lista para almacenar las claves del primer diccionario
        List<string> firstElementKeys = new List<string>();

        // Verificamos que haya al menos un elemento en la lista
        if (treeDataList != null && treeDataList.Count > 0)
        {
            // Obtenemos el primer diccionario
            Dictionary<string, string> firstDict = treeDataList[0];

            // Agregamos todas las claves del primer diccionario a la lista
            foreach (string key in firstDict.Keys)
            {
                firstElementKeys.Add(key);
            }
        }

        // Añadimos las claves al Dropdown como opciones
        attributes.AddOptions(firstElementKeys);

        // Opcionalmente, establecemos el valor por defecto en la primera opción
        attributes.value = 0;

        // Refrescamos para mostrar el valor actual
        attributes.RefreshShownValue();
    } */


    public static void Warning(string message)
    {
        // if (Instance == null)
        // {
        //     Debug.LogError("MenuController not initialized.");
        //     return;
        // }

        // GameObject warningGO = Instantiate(Instance.warningMessagePrefab, Instance.canvasTransform);
        // var ui = warningGO.GetComponent<WarningMessageUI>();
        // if (ui != null)
        // {
        //     ui.SetMessage(message);
        // }

        if (Instance != null && Instance.warningText != null)
        {
            Instance.warningText.text = message;
        }
        else
        {
            Debug.LogError("WarningText or MenuController instance is not initialized.");
        }

    }

    public void SelectLocalExcelFile()
    {
        // Abre el explorador de archivos para seleccionar un archivo Excel
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Selecciona un archivo XLSX", "", "xlsx", false);

        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            fileExcelPath = paths[0];

            try
            {
                // Guardar ruta en archivo persistente
                File.WriteAllText("lastfileExcel.txt", fileExcelPath);

                // Actualizar la interfaz
                fileExcelText.color = Color.black;
                fileExcelText.text = "Selected file: " + Path.GetFileName(fileExcelPath);
                Debug.Log("Archivo Excel seleccionado: " + fileExcelPath);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error al guardar la ruta del archivo Excel: " + ex.Message);
            }
        }
        else
        {
            Debug.LogWarning("No se seleccionó ningún archivo.");
        }
    }

    public async void SelectURLExcelFile()
    {
        string url = urlInputField.text.Trim();

        if (string.IsNullOrEmpty(url))
        {
            Debug.LogWarning("La URL está vacía.");
            return;
        }

        try
        {
            string downloadedFilePath = await FileDownloader.DownloadFileWithFileNameAsync(url, "./");
            if (!string.IsNullOrEmpty(downloadedFilePath))
            {
                fileExcelPath = downloadedFilePath;
                fileExcelText.color = Color.black;
                fileExcelText.text = "Archivo descargado: " + Path.GetFileName(fileExcelPath);
                File.WriteAllText("lastfileExcel.txt", fileExcelPath);
                Debug.Log("Archivo Excel descargado desde URL: " + fileExcelPath);
            }
            else
            {
                Debug.LogWarning("La descarga no produjo un archivo válido.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al descargar el archivo Excel desde la URL: " + ex.Message);
        }
    }


    public void SelectLocalKMLFile()
    {
        // Abre el explorador de archivos para seleccionar un archivo KML
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Selecciona un archivo KML", "", "kml", false);

        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            fileKMLPath = paths[0];

            try
            {
                // Guardar la ruta del archivo para persistencia
                File.WriteAllText("lastfileKML.txt", fileKMLPath);

                // Mostrar en la interfaz
                fileKMLText.color = Color.black;
                fileKMLText.text = "Selected file: " + Path.GetFileName(fileKMLPath);
                Debug.Log("Archivo KML seleccionado: " + fileKMLPath);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error al guardar la ruta del archivo KML: " + ex.Message);
            }
        }
        else
        {
            Debug.LogWarning("No se seleccionó ningún archivo.");
        }
    }

    public async void SelectURLKMLFile()
    {
        string url = urlInputField.text.Trim();

        if (string.IsNullOrEmpty(url))
        {
            Debug.LogWarning("La URL está vacía.");
            return;
        }

        try
        {
            string downloadedFilePath = await FileDownloader.DownloadFileWithFileNameAsync(url, "./");
            if (!string.IsNullOrEmpty(downloadedFilePath))
            {
                fileKMLPath = downloadedFilePath;
                fileKMLText.color = Color.black;
                fileKMLText.text = "Archivo descargado: " + Path.GetFileName(fileKMLPath);
                File.WriteAllText("lastfileKML.txt", fileKMLPath);
                Debug.Log("Archivo KML descargado desde URL: " + fileKMLPath);
            }
            else
            {
                Debug.LogWarning("La descarga no produjo un archivo válido.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al descargar el archivo desde la URL: " + ex.Message);
        }
    }

    //assigned in the inspector
    public void GoToMenu(string menuName)
    {
        {
            DeactivateAllMenusExcept(menuName);
        }
    }
    
    private void DeactivateAllMenusExcept(string menuName)
    {
        foreach (GameObject menu in menus)
        {
            if (menu.name == menuName)
            {
                menu.SetActive(true);
            }
            else
            {
                menu.SetActive(false);
            }
        }
    }

}
