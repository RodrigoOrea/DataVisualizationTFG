using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TreeClickHandler : MonoBehaviour
{
    private PanelAboveTree panel;
    private ProgressBar bar;

    private void Start()
    {
        panel = GetComponent<PanelAboveTree>();
        bar = GetComponent<ProgressBar>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Verificar si hay UI delante (incluye el panel del árbol)
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // Si el clic fue en nuestro propio panel, no hacer nada
                // Si fue en otra UI, ignorar el clic en el árbol
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                if (hitInfo.collider.gameObject == this.gameObject)
                {
                    ToggleTreeInfo();
                }
            }
        }
    }

    private void ToggleTreeInfo()
    {
        if (panel.panelInstance.activeSelf)
        {
            HideTreeInfo();
        }
        else
        {
            ShowTreeInfo();
        }
    }

    private void ShowTreeInfo()
    {
        if (panel != null) panel.ShowTreeInfo();
        Debug.Log("Clicked on " + gameObject.name + "\n" + gameObject.GetComponent<TreeAttributes>().ToString());
        
        if (UIManager.Instance.selectedAttributeBoolean)
        {
            string currentSelectedAttribute = UIManager.Instance.selectedAttributeString;
            var stats = MapManager.Instance.CalculateStats(currentSelectedAttribute);
            float value = GetComponent<TreeAttributes>().GetValue(currentSelectedAttribute);
            
            //bar.UpdateProgressBar(currentSelectedAttribute, stats);
            //bar.ShowBar();
        }
    }

    private void HideTreeInfo()
    {
        if (panel != null) panel.HidePanel();
        bar.HideBar();
    }
}