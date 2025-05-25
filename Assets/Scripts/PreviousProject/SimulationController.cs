using System.Windows.Forms;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimulationController : MonoBehaviour
{   private Camera activeCamera;
    private Camera mainCamera;
    private Camera cenitalCamera;
	private Camera littleCamera;

	public TMP_Text treeNameText;
    public TMP_Text heightText;
    public TMP_Text widthText;
    public TMP_Text ageText;
	[SerializeField] private GameObject panel;

	public GameObject panelAtributos;

    void Start ()
	{
		MapLoader mapLoader;
		float wideX, wideZ, maxY, heightY;

		mapLoader=FindObjectOfType<MapLoader>();
		mainCamera =GameObject.FindWithTag ("MainCamera").GetComponent<Camera>();
		mainCamera.GetComponent<AudioListener>().enabled = true;
		cenitalCamera = GameObject.Find("Cenital Camera").GetComponent<Camera>();
		cenitalCamera.GetComponent<AudioListener>().enabled = false;
		cenitalCamera.enabled = false;
		littleCamera = GameObject.Find("Little Camera").GetComponent<Camera>();
		littleCamera.GetComponent<AudioListener>().enabled = false;
		littleCamera.enabled = false;		


		wideX = mapLoader.End_map.x - mapLoader.Origin_map.x;
		wideZ = mapLoader.End_map.z - mapLoader.Origin_map.z;
		
		if (Mathf.Abs(wideX) < Mathf.Abs(wideZ)){
			cenitalCamera.transform.Rotate(0,0,90);
			maxY = wideZ;
		}
		else
			maxY = wideX;

		
		heightY = 100 * maxY / 126; // Height of cenital camera depends on the maximum side
		cenitalCamera.transform.position = new Vector3(mapLoader.Origin_map.x+wideX/2,heightY,mapLoader.Origin_map.z+wideZ/2);
		activeCamera = mainCamera;

        //Debug.Log ("Inicializacion control");
	}
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("Menu");
            // Application.Quit();

        if (Input.GetKeyDown(KeyCode.M)) {
			activeCamera.enabled=false;
			activeCamera.GetComponent<AudioListener>().enabled = false;
			activeCamera = mainCamera;
			activeCamera.enabled=true;
			activeCamera.GetComponent<AudioListener>().enabled = true;

			//Debug.Log ("Post cambio");
		}

		if (Input.GetKeyDown(KeyCode.C)) {
			activeCamera.enabled=false;
			activeCamera.GetComponent<AudioListener>().enabled = false;
			activeCamera = cenitalCamera;
			activeCamera.enabled=true;
			activeCamera.GetComponent<AudioListener>().enabled = true;
			//Debug.Log ("C cambio");
		}

	   
		if (Input.GetKeyDown(KeyCode.L)) {
			if (littleCamera.enabled){
				littleCamera.enabled=false;
			}
			else{
				littleCamera.enabled=true;
				//Debug.Log ("L cambio");
			}
		}
	}

	public void ClosePanel(){
		panel.SetActive(false);
	}

	public void ShowTreeInfo(TreeAttributes treeAttributes)
    {
        // Actualizamos los textos con los atributos
        treeNameText.text = "Nombre: " + treeAttributes.treeName;
        heightText.text   = "Altura: " + treeAttributes.height.ToString();
        widthText.text    = "Ancho:  " + treeAttributes.width.ToString();
        ageText.text      = "Edad:   " + treeAttributes.age.ToString();

        // Mostramos el panel
        panel.SetActive(true);
    }

        
    
}
