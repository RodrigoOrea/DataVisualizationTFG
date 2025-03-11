using System;
using TMPro;
using UnityEngine;

public class TreeClickHandler : MonoBehaviour
{
    private PanelAboveTree panel;

    private void Start()
    {
        // Supone que PanelAboveTree está en ESTE MISMO objeto
        panel = GetComponent<PanelAboveTree>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                // ¿El objeto golpeado es este?
                if (hitInfo.collider.gameObject == this.gameObject)
                {
                    // Mostrar la info
                    if (panel != null) panel.ShowTreeInfo();
                    
                    //set the bar
                    if(UIManager.Instance.selectedAttributeBoolean) {
                        String currentSelectedAttribute = UIManager.Instance.selectedAttributeString;
                        (float min, float max, float avg) Stats = MapLoader.Instance.CalculateStats(currentSelectedAttribute);
                        float value = this.gameObject.GetComponent<TreeAttributes>().GetValue(currentSelectedAttribute);
                        UIManager.Instance.currentText.GetComponent<TMP_Text>().text = "Current-" + Math.Round(value, 4).ToString();
                        BarScript.instance.UpdateIndicator(Stats.min, Stats.max, value);
                    }
                }
                else
                {
                    // Ocultar
                    if (panel != null) panel.HidePanel();
                }
            }
            else
            {
                // Clic en el vacío
                if (panel != null) panel.HidePanel();
            }
        }
    }
}


